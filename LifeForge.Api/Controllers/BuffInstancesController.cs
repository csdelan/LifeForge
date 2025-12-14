using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.Application.Services;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuffInstancesController : ControllerBase
    {
        private readonly IBuffInstanceRepository _buffInstanceRepository;
        private readonly IBuffInstanceApplicationService _buffInstanceApplicationService;
        private readonly ILogger<BuffInstancesController> _logger;

        public BuffInstancesController(
            IBuffInstanceRepository buffInstanceRepository,
            IBuffInstanceApplicationService buffInstanceApplicationService,
            ILogger<BuffInstancesController> logger)
        {
            _buffInstanceRepository = buffInstanceRepository;
            _buffInstanceApplicationService = buffInstanceApplicationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<BuffInstanceDto>>> GetAllBuffInstances()
        {
            try
            {
                var buffInstances = await _buffInstanceRepository.GetAllBuffInstancesAsync();
                var dtos = buffInstances.Select(bi => new BuffInstanceDto
                {
                    Id = bi.Id,
                    BuffId = bi.BuffId,
                    CharacterId = bi.CharacterId,
                    BuffName = bi.BuffName,
                    Description = bi.Description,
                    IsDebuff = bi.IsDebuff,
                    StartTime = bi.StartTime,
                    EndTime = bi.EndTime,
                    Stacks = bi.Stacks,
                    IsActive = bi.IsActive,
                    Status = bi.Status,
                    HPModifier = bi.HPModifier,
                    HPMaxModifier = bi.HPMaxModifier,
                    HPPercentModifier = bi.HPPercentModifier,
                    HPMaxPercentModifier = bi.HPMaxPercentModifier,
                    MPModifier = bi.MPModifier,
                    MPMaxModifier = bi.MPMaxModifier,
                    MPPercentModifier = bi.MPPercentModifier,
                    MPMaxPercentModifier = bi.MPMaxPercentModifier,
                    XpGainsPercentModifier = bi.XpGainsPercentModifier,
                    ImageData = bi.ImageData,
                    ImageContentType = bi.ImageContentType
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buff instances");
                return StatusCode(500, "An error occurred while retrieving buff instances");
            }
        }

        [HttpGet("character/{characterId}")]
        public async Task<ActionResult<List<BuffInstanceDto>>> GetActiveBuffInstancesByCharacterId(string characterId)
        {
            try
            {
                var buffInstances = await _buffInstanceRepository.GetActiveBuffInstancesByCharacterIdAsync(characterId);
                var dtos = buffInstances.Select(bi => new BuffInstanceDto
                {
                    Id = bi.Id,
                    BuffId = bi.BuffId,
                    CharacterId = bi.CharacterId,
                    BuffName = bi.BuffName,
                    Description = bi.Description,
                    IsDebuff = bi.IsDebuff,
                    StartTime = bi.StartTime,
                    EndTime = bi.EndTime,
                    Stacks = bi.Stacks,
                    IsActive = bi.IsActive,
                    Status = bi.Status,
                    HPModifier = bi.HPModifier,
                    HPMaxModifier = bi.HPMaxModifier,
                    HPPercentModifier = bi.HPPercentModifier,
                    HPMaxPercentModifier = bi.HPMaxPercentModifier,
                    MPModifier = bi.MPModifier,
                    MPMaxModifier = bi.MPMaxModifier,
                    MPPercentModifier = bi.MPPercentModifier,
                    MPMaxPercentModifier = bi.MPMaxPercentModifier,
                    XpGainsPercentModifier = bi.XpGainsPercentModifier,
                    ImageData = bi.ImageData,
                    ImageContentType = bi.ImageContentType
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buff instances for character {CharacterId}", characterId);
                return StatusCode(500, "An error occurred while retrieving buff instances");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuffInstanceDto>> GetBuffInstance(string id)
        {
            try
            {
                var buffInstance = await _buffInstanceRepository.GetBuffInstanceByIdAsync(id);
                if (buffInstance == null)
                {
                    return NotFound();
                }

                var dto = new BuffInstanceDto
                {
                    Id = buffInstance.Id,
                    BuffId = buffInstance.BuffId,
                    CharacterId = buffInstance.CharacterId,
                    BuffName = buffInstance.BuffName,
                    Description = buffInstance.Description,
                    IsDebuff = buffInstance.IsDebuff,
                    StartTime = buffInstance.StartTime,
                    EndTime = buffInstance.EndTime,
                    Stacks = buffInstance.Stacks,
                    IsActive = buffInstance.IsActive,
                    Status = buffInstance.Status,
                    HPModifier = buffInstance.HPModifier,
                    HPMaxModifier = buffInstance.HPMaxModifier,
                    HPPercentModifier = buffInstance.HPPercentModifier,
                    HPMaxPercentModifier = buffInstance.HPMaxPercentModifier,
                    MPModifier = buffInstance.MPModifier,
                    MPMaxModifier = buffInstance.MPMaxModifier,
                    MPPercentModifier = buffInstance.MPPercentModifier,
                    MPMaxPercentModifier = buffInstance.MPMaxPercentModifier,
                    XpGainsPercentModifier = buffInstance.XpGainsPercentModifier,
                    ImageData = buffInstance.ImageData,
                    ImageContentType = buffInstance.ImageContentType
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buff instance {BuffInstanceId}", id);
                return StatusCode(500, "An error occurred while retrieving the buff instance");
            }
        }

        [HttpPost("activate")]
        public async Task<ActionResult<BuffInstanceApplicationResultDto>> ActivateBuff([FromBody] ActivateBuffDto activateBuffDto)
        {
            try
            {
                var result = await _buffInstanceApplicationService.ActivateBuffAsync(
                    activateBuffDto.CharacterId,
                    activateBuffDto.BuffId);

                if (!result.Success)
                {
                    return BadRequest(new BuffInstanceApplicationResultDto
                    {
                        Success = false,
                        ErrorMessage = result.ErrorMessage
                    });
                }

                return Ok(new BuffInstanceApplicationResultDto
                {
                    Success = true,
                    BuffInstanceId = result.BuffInstanceId,
                    ModifiersApplied = result.ModifiersApplied
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating buff");
                return StatusCode(500, "An error occurred while activating the buff");
            }
        }

        [HttpPost("deactivate")]
        public async Task<ActionResult<BuffInstanceApplicationResultDto>> DeactivateBuff([FromBody] DeactivateBuffInstanceDto deactivateDto)
        {
            try
            {
                var result = await _buffInstanceApplicationService.DeactivateBuffInstanceAsync(
                    deactivateDto.CharacterId,
                    deactivateDto.BuffInstanceId);

                if (!result.Success)
                {
                    return BadRequest(new BuffInstanceApplicationResultDto
                    {
                        Success = false,
                        ErrorMessage = result.ErrorMessage
                    });
                }

                return Ok(new BuffInstanceApplicationResultDto
                {
                    Success = true,
                    BuffInstanceId = result.BuffInstanceId,
                    ModifiersApplied = result.ModifiersApplied
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating buff");
                return StatusCode(500, "An error occurred while deactivating the buff");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuffInstance(string id)
        {
            try
            {
                var success = await _buffInstanceRepository.DeleteBuffInstanceAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting buff instance {BuffInstanceId}", id);
                return StatusCode(500, "An error occurred while deleting the buff instance");
            }
        }
    }
}
