using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.DataAccess.Models;
using LifeForge.Application.Services;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        private readonly IActionRepository _actionRepository;
        private readonly IBuffInstanceApplicationService _buffInstanceService;
        private readonly ILogger<ActionsController> _logger;

        public ActionsController(
            IActionRepository actionRepository,
            IBuffInstanceApplicationService buffInstanceService,
            ILogger<ActionsController> logger)
        {
            _actionRepository = actionRepository;
            _buffInstanceService = buffInstanceService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActionDto>>> GetAllActions()
        {
            try
            {
                var actions = await _actionRepository.GetAllActionsAsync();
                var dtos = actions.Select(a => new ActionDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.Icon,
                    ImageName = a.ImageName,
                    ImageData = a.ImageData,
                    ImageContentType = a.ImageContentType,
                    BuffIds = a.BuffIds,
                    Category = a.Category,
                    CooldownHours = a.CooldownHours
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving actions");
                return StatusCode(500, "An error occurred while retrieving actions");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActionDto>> GetAction(string id)
        {
            try
            {
                var action = await _actionRepository.GetActionByIdAsync(id);
                if (action == null)
                {
                    return NotFound();
                }

                var dto = new ActionDto
                {
                    Id = action.Id,
                    Name = action.Name,
                    Description = action.Description,
                    Icon = action.Icon,
                    ImageName = action.ImageName,
                    ImageData = action.ImageData,
                    ImageContentType = action.ImageContentType,
                    BuffIds = action.BuffIds,
                    Category = action.Category,
                    CooldownHours = action.CooldownHours
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action {ActionId}", id);
                return StatusCode(500, "An error occurred while retrieving the action");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ActionDto>> CreateAction([FromBody] ActionDto actionDto)
        {
            try
            {
                var actionEntity = new ActionEntity
                {
                    Name = actionDto.Name,
                    Description = actionDto.Description,
                    Icon = actionDto.Icon,
                    ImageName = actionDto.ImageName,
                    ImageData = actionDto.ImageData,
                    ImageContentType = actionDto.ImageContentType,
                    BuffIds = actionDto.BuffIds,
                    Category = actionDto.Category,
                    CooldownHours = actionDto.CooldownHours
                };

                var created = await _actionRepository.CreateActionAsync(actionEntity);

                var resultDto = new ActionDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Description = created.Description,
                    Icon = created.Icon,
                    ImageName = created.ImageName,
                    ImageData = created.ImageData,
                    ImageContentType = created.ImageContentType,
                    BuffIds = created.BuffIds,
                    Category = created.Category,
                    CooldownHours = created.CooldownHours
                };

                return CreatedAtAction(nameof(GetAction), new { id = resultDto.Id }, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating action");
                return StatusCode(500, "An error occurred while creating the action");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAction(string id, [FromBody] ActionDto actionDto)
        {
            try
            {
                var existingAction = await _actionRepository.GetActionByIdAsync(id);
                if (existingAction == null)
                {
                    return NotFound();
                }

                existingAction.Name = actionDto.Name;
                existingAction.Description = actionDto.Description;
                existingAction.Icon = actionDto.Icon;
                existingAction.ImageName = actionDto.ImageName;
                existingAction.ImageData = actionDto.ImageData;
                existingAction.ImageContentType = actionDto.ImageContentType;
                existingAction.BuffIds = actionDto.BuffIds;
                existingAction.Category = actionDto.Category;
                existingAction.CooldownHours = actionDto.CooldownHours;

                var success = await _actionRepository.UpdateActionAsync(id, existingAction);
                if (!success)
                {
                    return StatusCode(500, "Failed to update action");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating action {ActionId}", id);
                return StatusCode(500, "An error occurred while updating the action");
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

        [HttpPost("perform")]
        public async Task<ActionResult<ActionResultDto>> PerformAction([FromBody] PerformActionDto performDto)
        {
            try
            {
                var action = await _actionRepository.GetActionByIdAsync(performDto.ActionId);
                if (action == null)
                {
                    return NotFound("Action not found");
                }

                var result = new ActionResultDto { Success = true };
                var activatedBuffs = new List<string>();

                // Activate each buff associated with the action
                foreach (var buffId in action.BuffIds)
                {
                    var buffResult = await _buffInstanceService.ActivateBuffAsync(
                        performDto.CharacterId,
                        buffId);

                    if (buffResult.Success)
                    {
                        activatedBuffs.Add(buffId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to activate buff {BuffId} for action {ActionId}: {Error}",
                            buffId, performDto.ActionId, buffResult.ErrorMessage);
                    }
                }

                result.ActivatedBuffs = activatedBuffs;
                result.Success = activatedBuffs.Count > 0;

                if (!result.Success)
                {
                    result.ErrorMessage = "Failed to activate any buffs";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing action");
                return StatusCode(500, "An error occurred while performing the action");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAction(string id)
        {
            try
            {
                var success = await _actionRepository.DeleteActionAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting action {ActionId}", id);
                return StatusCode(500, "An error occurred while deleting the action");
            }
        }
    }
}
