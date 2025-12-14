using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.BackgroundServices;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuffProcessingController : ControllerBase
    {
        private readonly MidnightBuffProcessingService _buffProcessingService;
        private readonly ILogger<BuffProcessingController> _logger;

        public BuffProcessingController(
            MidnightBuffProcessingService buffProcessingService,
            ILogger<BuffProcessingController> logger)
        {
            _buffProcessingService = buffProcessingService;
            _logger = logger;
        }

        /// <summary>
        /// Manually trigger buff processing (for development/testing)
        /// </summary>
        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerBuffProcessing()
        {
            try
            {
                _logger.LogInformation("Manual buff processing triggered via API");
                await _buffProcessingService.ProcessBuffsAsync();
                return Ok(new { success = true, message = "Buff processing completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual buff processing");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
