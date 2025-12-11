using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.DataAccess.Models;
using LifeForge.Domain;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestRepository _questRepository;
        private readonly ILogger<QuestsController> _logger;

        public QuestsController(IQuestRepository questRepository, ILogger<QuestsController> logger)
        {
            _questRepository = questRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestDto>>> GetAllQuests()
        {
            try
            {
                var quests = await _questRepository.GetAllQuestsAsync();
                var questDtos = quests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Name = q.Name,
                    ImageName = q.ImageName,
                    ImageData = q.ImageData,
                    ImageContentType = q.ImageContentType,
                    Description = q.Description,
                    Difficulty = q.Difficulty,
                    Repeatability = q.Repeatability
                }).ToList();

                return Ok(questDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quests");
                return StatusCode(500, "An error occurred while retrieving quests");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestDto>> GetQuest(string id)
        {
            try
            {
                var quest = await _questRepository.GetQuestByIdAsync(id);
                if (quest == null)
                {
                    return NotFound();
                }

                var questDto = new QuestDto
                {
                    Id = quest.Id,
                    Name = quest.Name,
                    ImageName = quest.ImageName,
                    ImageData = quest.ImageData,
                    ImageContentType = quest.ImageContentType,
                    Description = quest.Description,
                    Difficulty = quest.Difficulty,
                    Repeatability = quest.Repeatability
                };

                return Ok(questDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quest {QuestId}", id);
                return StatusCode(500, "An error occurred while retrieving the quest");
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuestDto>> CreateQuest([FromBody] CreateQuestDto createQuestDto)
        {
            try
            {
                var questEntity = new QuestEntity
                {
                    Name = createQuestDto.Name,
                    ImageName = createQuestDto.ImageName,
                    ImageData = createQuestDto.ImageData,
                    ImageContentType = createQuestDto.ImageContentType,
                    Description = createQuestDto.Description,
                    Difficulty = createQuestDto.Difficulty,
                    Repeatability = createQuestDto.Repeatability
                };

                var createdQuest = await _questRepository.CreateQuestAsync(questEntity);

                var questDto = new QuestDto
                {
                    Id = createdQuest.Id,
                    Name = createdQuest.Name,
                    ImageName = createdQuest.ImageName,
                    ImageData = createdQuest.ImageData,
                    ImageContentType = createdQuest.ImageContentType,
                    Description = createdQuest.Description,
                    Difficulty = createdQuest.Difficulty,
                    Repeatability = createdQuest.Repeatability
                };

                return CreatedAtAction(nameof(GetQuest), new { id = questDto.Id }, questDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quest");
                return StatusCode(500, "An error occurred while creating the quest");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuest(string id, [FromBody] UpdateQuestDto updateQuestDto)
        {
            try
            {
                var existingQuest = await _questRepository.GetQuestByIdAsync(id);
                if (existingQuest == null)
                {
                    return NotFound();
                }

                existingQuest.Name = updateQuestDto.Name;
                existingQuest.ImageName = updateQuestDto.ImageName;
                existingQuest.ImageData = updateQuestDto.ImageData;
                existingQuest.ImageContentType = updateQuestDto.ImageContentType;
                existingQuest.Description = updateQuestDto.Description;
                existingQuest.Difficulty = updateQuestDto.Difficulty;
                existingQuest.Repeatability = updateQuestDto.Repeatability;

                var success = await _questRepository.UpdateQuestAsync(id, existingQuest);
                if (!success)
                {
                    return StatusCode(500, "Failed to update quest");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quest {QuestId}", id);
                return StatusCode(500, "An error occurred while updating the quest");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuest(string id)
        {
            try
            {
                var success = await _questRepository.DeleteQuestAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quest {QuestId}", id);
                return StatusCode(500, "An error occurred while deleting the quest");
            }
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<object>> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("File size exceeds 5MB limit");
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);

                return Ok(new 
                { 
                    fileName = file.FileName,
                    imageData = base64String,
                    contentType = file.ContentType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, "An error occurred while uploading the image");
            }
        }
    }
}
