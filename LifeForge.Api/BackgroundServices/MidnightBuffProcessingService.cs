using LifeForge.Application.Services;
using LifeForge.DataAccess.Repositories;
using LifeForge.Domain;

namespace LifeForge.Api.BackgroundServices
{
    /// <summary>
    /// Background service that processes buff lifecycle changes once per day at midnight UTC
    /// </summary>
    public class MidnightBuffProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MidnightBuffProcessingService> _logger;

        public MidnightBuffProcessingService(
            IServiceProvider serviceProvider,
            ILogger<MidnightBuffProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Midnight Buff Processing Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextMidnight = now.Date.AddDays(1); // Next midnight UTC
                var delay = nextMidnight - now;

                _logger.LogInformation("Next buff processing scheduled for {NextMidnight} UTC (in {Hours} hours)",
                    nextMidnight, delay.TotalHours);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                    await ProcessBuffsAsync();
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Midnight Buff Processing Service is stopping");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Midnight Buff Processing Service main loop");
                }
            }
        }

        /// <summary>
        /// Public method to manually trigger buff processing (for testing/development)
        /// </summary>
        public async Task ProcessBuffsAsync()
        {
            _logger.LogInformation("Starting buff processing at {Time}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            var characterRepository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
            var buffInstanceRepository = scope.ServiceProvider.GetRequiredService<IBuffInstanceRepository>();
            var buffAggregationService = scope.ServiceProvider.GetRequiredService<IBuffAggregationService>();

            try
            {
                // Get all characters
                var allCharacters = await characterRepository.GetAllCharactersAsync();
                _logger.LogInformation("Processing buffs for {Count} characters", allCharacters.Count);

                foreach (var character in allCharacters)
                {
                    await ProcessBuffsForCharacterAsync(
                        character.Id!,
                        buffInstanceRepository,
                        buffAggregationService);
                }

                _logger.LogInformation("Buff processing completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during buff processing");
            }
        }

        private async Task ProcessBuffsForCharacterAsync(
            string characterId,
            IBuffInstanceRepository buffInstanceRepository,
            IBuffAggregationService buffAggregationService)
        {
            _logger.LogDebug("Processing buffs for character {CharacterId}", characterId);

            // Step 1: Activate pending buffs (Pending ? Active)
            var pendingBuffs = await buffInstanceRepository.GetPendingBuffInstancesAsync();
            var characterPendingBuffs = pendingBuffs.Where(b => b.CharacterId == characterId).ToList();

            if (characterPendingBuffs.Any())
            {
                var pendingIds = characterPendingBuffs.Select(b => b.Id!).ToList();
                await buffInstanceRepository.BulkUpdateStatusAsync(pendingIds, BuffInstanceStatus.Active);
                _logger.LogInformation("Activated {Count} pending buffs for character {CharacterId}",
                    characterPendingBuffs.Count, characterId);
            }

            // Step 2: Expire old buffs (Active ? Expired if past EndTime)
            var expiredBuffs = await buffInstanceRepository.GetExpiredBuffInstancesAsync();
            var characterExpiredBuffs = expiredBuffs.Where(b => b.CharacterId == characterId).ToList();

            if (characterExpiredBuffs.Any())
            {
                var expiredIds = characterExpiredBuffs.Select(b => b.Id!).ToList();
                await buffInstanceRepository.BulkUpdateStatusAsync(expiredIds, BuffInstanceStatus.Expired);
                _logger.LogInformation("Expired {Count} buffs for character {CharacterId}",
                    characterExpiredBuffs.Count, characterId);
            }

            // Step 3: Recalculate aggregate modifiers
            await buffAggregationService.UpdateCharacterAggregateModifiersAsync(characterId);

            // Step 4: Clean up expired buffs (optional - delete buffs that have been expired for more than 7 days)
            await CleanupOldExpiredBuffsAsync(characterId, buffInstanceRepository);
        }

        private async Task CleanupOldExpiredBuffsAsync(string characterId, IBuffInstanceRepository buffInstanceRepository)
        {
            var allBuffs = await buffInstanceRepository.GetActiveBuffInstancesByCharacterIdAsync(characterId);
            var oldExpiredBuffs = allBuffs
                .Where(b => b.Status == BuffInstanceStatus.Expired && b.EndTime < DateTime.UtcNow.AddDays(-7))
                .ToList();

            foreach (var buff in oldExpiredBuffs)
            {
                await buffInstanceRepository.DeleteBuffInstanceAsync(buff.Id!);
            }

            if (oldExpiredBuffs.Any())
            {
                _logger.LogInformation("Cleaned up {Count} old expired buffs for character {CharacterId}",
                    oldExpiredBuffs.Count, characterId);
            }
        }
    }
}
