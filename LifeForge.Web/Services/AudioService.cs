using Microsoft.JSInterop;

namespace LifeForge.Web.Services
{
    /// <summary>
    /// Service for playing audio sound effects throughout the application.
    /// </summary>
    public class AudioService
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _audioModule;

        public AudioService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Initialize the audio module. Should be called after the app has loaded.
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_audioModule == null)
            {
                _audioModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./js/audio.js");
            }
        }

        /// <summary>
        /// Play a sound effect by its identifier.
        /// </summary>
        /// <param name="soundEffect">The sound effect to play</param>
        /// <param name="volume">Volume level (0.0 to 1.0)</param>
        public async Task PlaySoundAsync(SoundEffect soundEffect, float volume = 0.5f)
        {
            if (_audioModule != null)
            {
                try
                {
                    await _audioModule.InvokeVoidAsync("playSound", soundEffect.ToString(), volume);
                }
                catch (Exception ex)
                {
                    // Log error but don't throw - audio failures shouldn't break the app
                    Console.WriteLine($"Error playing sound {soundEffect}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Dispose of the audio module.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_audioModule != null)
            {
                await _audioModule.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Available sound effects in the application.
    /// </summary>
    public enum SoundEffect
    {
        BuffApplied,
        DebuffApplied,
        // Add more sound effects here as needed
        // Examples for future expansion:
        // QuestCompleted,
        // LevelUp,
        // ActionTriggered,
        // CoinEarned,
        // Error,
        // Success,
        // Click
    }
}
