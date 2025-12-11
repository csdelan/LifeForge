using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.DataAccess.Models;
using LifeForge.Domain;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestRunsController : ControllerBase
    {
        private readonly IQuestRunRepository _questRunRepository;
        private readonly IQuestRepository _questRepository;
        private readonly ILogger<QuestRunsController> _logger;

        public QuestRunsController(
            IQuestRunRepository questRunRepository,
            IQuestRepository questRepository,
            ILogger<QuestRunsController> logger)
        {
            _questRunRepository = questRunRepository;
            _questRepository = questRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestRunDto>>> GetAllQuestRuns()
        {
            try
            {
                var questRuns = await _questRunRepository.GetAllQuestRunsAsync();
                var questRunDtos = questRuns.Select(MapToDto).ToList();
                return Ok(questRunDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quest runs");
                return StatusCode(500, "An error occurred while retrieving quest runs");
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<QuestRunDto>>> GetActiveQuestRuns()
        {
            try
            {
                var questRuns = await _questRunRepository.GetActiveQuestRunsAsync();
                var questRunDtos = questRuns.Select(MapToDto).ToList();
                return Ok(questRunDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active quest runs");
                return StatusCode(500, "An error occurred while retrieving active quest runs");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestRunDto>> GetQuestRun(string id)
        {
            try
            {
                var questRun = await _questRunRepository.GetQuestRunByIdAsync(id);
                if (questRun == null)
                {
                    return NotFound();
                }

                return Ok(MapToDto(questRun));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quest run {QuestRunId}", id);
                return StatusCode(500, "An error occurred while retrieving the quest run");
            }
        }

        [HttpPost("start")]
        public async Task<ActionResult<QuestRunDto>> StartQuestRun([FromBody] StartQuestRunDto startQuestRunDto)
        {
            try
            {
                // Validate quest exists
                var quest = await _questRepository.GetQuestByIdAsync(startQuestRunDto.QuestId);
                if (quest == null)
                {
                    return NotFound($"Quest with ID {startQuestRunDto.QuestId} not found");
                }

                // Create quest run entity
                var questRunEntity = new QuestRunEntity
                {
                    QuestId = startQuestRunDto.QuestId,
                    QuestName = quest.Name,
                    Status = QuestStatus.InProgress,
                    StartTime = DateTime.UtcNow,
                    EndTime = null,
                    Rewards = new List<RewardEntity>()
                };

                var createdQuestRun = await _questRunRepository.CreateQuestRunAsync(questRunEntity);

                return CreatedAtAction(
                    nameof(GetQuestRun),
                    new { id = createdQuestRun.Id },
                    MapToDto(createdQuestRun));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quest run");
                return StatusCode(500, "An error occurred while starting the quest run");
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<QuestRunDto>> CompleteQuestRun(string id)
        {
            try
            {
                var questRun = await _questRunRepository.GetQuestRunByIdAsync(id);
                if (questRun == null)
                {
                    return NotFound();
                }

                if (questRun.Status != QuestStatus.InProgress)
                {
                    return BadRequest($"Cannot complete a quest run that is {questRun.Status}");
                }

                // Get the quest to retrieve configured rewards
                var quest = await _questRepository.GetQuestByIdAsync(questRun.QuestId);
                if (quest == null)
                {
                    return NotFound($"Quest with ID {questRun.QuestId} not found");
                }

                // Use quest-defined rewards if available, otherwise fallback to default calculation
                List<RewardEntity> rewards;
                if (quest.Rewards != null && quest.Rewards.Any())
                {
                    rewards = quest.Rewards.Select(r => new RewardEntity
                    {
                        Type = r.Type,
                        RewardClass = r.RewardClass,
                        Amount = r.Amount,
                        Icon = r.Icon
                    }).ToList();
                }
                else
                {
                    // Fallback to auto-calculated rewards if quest has no rewards configured
                    var calculatedRewards = CalculateRewards(quest);
                    rewards = calculatedRewards.Select(r => new RewardEntity
                    {
                        Type = r.Type,
                        RewardClass = r.RewardClass,
                        Amount = r.Amount,
                        Icon = r.Icon
                    }).ToList();
                }

                // Update quest run
                questRun.Status = QuestStatus.Completed;
                questRun.EndTime = DateTime.UtcNow;
                questRun.Rewards = rewards;

                var success = await _questRunRepository.UpdateQuestRunAsync(id, questRun);
                if (!success)
                {
                    return StatusCode(500, "Failed to complete quest run");
                }

                return Ok(MapToDto(questRun));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing quest run {QuestRunId}", id);
                return StatusCode(500, "An error occurred while completing the quest run");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestRun(string id)
        {
            try
            {
                var success = await _questRunRepository.DeleteQuestRunAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quest run {QuestRunId}", id);
                return StatusCode(500, "An error occurred while deleting the quest run");
            }
        }

        private QuestRunDto MapToDto(QuestRunEntity entity)
        {
            return new QuestRunDto
            {
                Id = entity.Id,
                QuestId = entity.QuestId,
                QuestName = entity.QuestName,
                Status = entity.Status,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Rewards = entity.Rewards.Select(r => new RewardDto
                {
                    Type = r.Type,
                    RewardClass = r.RewardClass,
                    Amount = r.Amount,
                    Icon = r.Icon
                }).ToList()
            };
        }

        private List<RewardDto> CalculateRewards(QuestEntity quest)
        {
            var rewards = new List<RewardDto>();

            // Calculate XP based on difficulty
            int xpAmount = quest.Difficulty switch
            {
                DifficultyLevel.Trivial => 10,
                DifficultyLevel.Easy => 25,
                DifficultyLevel.Medium => 50,
                DifficultyLevel.Hard => 100,
                DifficultyLevel.CrazyHard => 200,
                _ => 50
            };

            rewards.Add(new RewardDto
            {
                Type = RewardType.Experience,
                RewardClass = "General",
                Amount = xpAmount
            });

            // Calculate Gold based on difficulty
            int goldAmount = quest.Difficulty switch
            {
                DifficultyLevel.Trivial => 5,
                DifficultyLevel.Easy => 15,
                DifficultyLevel.Medium => 30,
                DifficultyLevel.Hard => 60,
                DifficultyLevel.CrazyHard => 120,
                _ => 30
            };

            rewards.Add(new RewardDto
            {
                Type = RewardType.Currency,
                RewardClass = "Gold",
                Amount = goldAmount
            });

            return rewards;
        }
    }
}
