using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.DataAccess.Models;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuffsController : ControllerBase
    {
        private readonly IBuffRepository _buffRepository;
        private readonly ILogger<BuffsController> _logger;

        public BuffsController(IBuffRepository buffRepository, ILogger<BuffsController> logger)
        {
            _buffRepository = buffRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<BuffDto>>> GetAllBuffs()
        {
            try
            {
                var buffs = await _buffRepository.GetAllBuffsAsync();
                var buffDtos = buffs.Select(b => new BuffDto
                {
                    Id = b.Id,
                    ImageName = b.ImageName,
                    ImageData = b.ImageData,
                    ImageContentType = b.ImageContentType,
                    IsDebuff = b.IsDebuff,
                    Name = b.Name,
                    Trigger = b.Trigger,
                    MaxStacks = b.MaxStacks,
                    Description = b.Description,
                    HPModifier = b.HPModifier,
                    HPMaxModifier = b.HPMaxModifier,
                    HPPercentModifier = b.HPPercentModifier,
                    HPMaxPercentModifier = b.HPMaxPercentModifier,
                    MPModifier = b.MPModifier,
                    MPMaxModifier = b.MPMaxModifier,
                    MPPercentModifier = b.MPPercentModifier,
                    MPMaxPercentModifier = b.MPMaxPercentModifier,
                    XpGainsPercentModifier = b.XpGainsPercentModifier,
                    DurationDays = b.DurationDays
                }).ToList();

                return Ok(buffDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buffs");
                return StatusCode(500, "An error occurred while retrieving buffs");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuffDto>> GetBuff(string id)
        {
            try
            {
                var buff = await _buffRepository.GetBuffByIdAsync(id);
                if (buff == null)
                {
                    return NotFound();
                }

                var buffDto = new BuffDto
                {
                    Id = buff.Id,
                    ImageName = buff.ImageName,
                    ImageData = buff.ImageData,
                    ImageContentType = buff.ImageContentType,
                    IsDebuff = buff.IsDebuff,
                    Name = buff.Name,
                    Trigger = buff.Trigger,
                    MaxStacks = buff.MaxStacks,
                    Description = buff.Description,
                    HPModifier = buff.HPModifier,
                    HPMaxModifier = buff.HPMaxModifier,
                    HPPercentModifier = buff.HPPercentModifier,
                    HPMaxPercentModifier = buff.HPMaxPercentModifier,
                    MPModifier = buff.MPModifier,
                    MPMaxModifier = buff.MPMaxModifier,
                    MPPercentModifier = buff.MPPercentModifier,
                    MPMaxPercentModifier = buff.MPMaxPercentModifier,
                    XpGainsPercentModifier = buff.XpGainsPercentModifier,
                    DurationDays = buff.DurationDays
                };

                return Ok(buffDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buff {BuffId}", id);
                return StatusCode(500, "An error occurred while retrieving the buff");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BuffDto>> CreateBuff([FromBody] CreateBuffDto createBuffDto)
        {
            try
            {
                var buffEntity = new BuffEntity
                {
                    ImageName = createBuffDto.ImageName,
                    ImageData = createBuffDto.ImageData,
                    ImageContentType = createBuffDto.ImageContentType,
                    IsDebuff = createBuffDto.IsDebuff,
                    Name = createBuffDto.Name,
                    Trigger = createBuffDto.Trigger,
                    MaxStacks = createBuffDto.MaxStacks,
                    Description = createBuffDto.Description,
                    HPModifier = createBuffDto.HPModifier,
                    HPMaxModifier = createBuffDto.HPMaxModifier,
                    HPPercentModifier = createBuffDto.HPPercentModifier,
                    HPMaxPercentModifier = createBuffDto.HPMaxPercentModifier,
                    MPModifier = createBuffDto.MPModifier,
                    MPMaxModifier = createBuffDto.MPMaxModifier,
                    MPPercentModifier = createBuffDto.MPPercentModifier,
                    MPMaxPercentModifier = createBuffDto.MPMaxPercentModifier,
                    XpGainsPercentModifier = createBuffDto.XpGainsPercentModifier,
                    DurationDays = createBuffDto.DurationDays
                };

                var createdBuff = await _buffRepository.CreateBuffAsync(buffEntity);

                var buffDto = new BuffDto
                {
                    Id = createdBuff.Id,
                    ImageName = createdBuff.ImageName,
                    ImageData = createdBuff.ImageData,
                    ImageContentType = createdBuff.ImageContentType,
                    IsDebuff = createdBuff.IsDebuff,
                    Name = createdBuff.Name,
                    Trigger = createdBuff.Trigger,
                    MaxStacks = createdBuff.MaxStacks,
                    Description = createdBuff.Description,
                    HPModifier = createdBuff.HPModifier,
                    HPMaxModifier = createdBuff.HPMaxModifier,
                    HPPercentModifier = createdBuff.HPPercentModifier,
                    HPMaxPercentModifier = createdBuff.HPMaxPercentModifier,
                    MPModifier = createdBuff.MPModifier,
                    MPMaxModifier = createdBuff.MPMaxModifier,
                    MPPercentModifier = createdBuff.MPPercentModifier,
                    MPMaxPercentModifier = createdBuff.MPMaxPercentModifier,
                    XpGainsPercentModifier = createdBuff.XpGainsPercentModifier,
                    DurationDays = createdBuff.DurationDays
                };

                return CreatedAtAction(nameof(GetBuff), new { id = buffDto.Id }, buffDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating buff");
                return StatusCode(500, "An error occurred while creating the buff");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuff(string id, [FromBody] UpdateBuffDto updateBuffDto)
        {
            try
            {
                var existingBuff = await _buffRepository.GetBuffByIdAsync(id);
                if (existingBuff == null)
                {
                    return NotFound();
                }

                existingBuff.ImageName = updateBuffDto.ImageName;
                existingBuff.ImageData = updateBuffDto.ImageData;
                existingBuff.ImageContentType = updateBuffDto.ImageContentType;
                existingBuff.IsDebuff = updateBuffDto.IsDebuff;
                existingBuff.Name = updateBuffDto.Name;
                existingBuff.Trigger = updateBuffDto.Trigger;
                existingBuff.MaxStacks = updateBuffDto.MaxStacks;
                existingBuff.Description = updateBuffDto.Description;
                existingBuff.HPModifier = updateBuffDto.HPModifier;
                existingBuff.HPMaxModifier = updateBuffDto.HPMaxModifier;
                existingBuff.HPPercentModifier = updateBuffDto.HPPercentModifier;
                existingBuff.HPMaxPercentModifier = updateBuffDto.HPMaxPercentModifier;
                existingBuff.MPModifier = updateBuffDto.MPModifier;
                existingBuff.MPMaxModifier = updateBuffDto.MPMaxModifier;
                existingBuff.MPPercentModifier = updateBuffDto.MPPercentModifier;
                existingBuff.MPMaxPercentModifier = updateBuffDto.MPMaxPercentModifier;
                existingBuff.XpGainsPercentModifier = updateBuffDto.XpGainsPercentModifier;
                existingBuff.DurationDays = updateBuffDto.DurationDays;

                var success = await _buffRepository.UpdateBuffAsync(id, existingBuff);
                if (!success)
                {
                    return StatusCode(500, "Failed to update buff");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating buff {BuffId}", id);
                return StatusCode(500, "An error occurred while updating the buff");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuff(string id)
        {
            try
            {
                var success = await _buffRepository.DeleteBuffAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting buff {BuffId}", id);
                return StatusCode(500, "An error occurred while deleting the buff");
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
