# LifeForge Sound Effects

This directory contains sound effect files for the LifeForge application.

## Required Sound Files

Place the following audio files in this directory:

### buff-applied.mp3
A positive, uplifting sound effect that plays when a buff is activated.
- Suggested style: Light chime, power-up sound, or gentle bell
- Duration: 0.5-1.5 seconds
- Format: MP3

### debuff-applied.mp3
A negative or warning sound effect that plays when a debuff is activated.
- Suggested style: Dark tone, warning beep, or ominous sound
- Duration: 0.5-1.5 seconds
- Format: MP3

## Finding Sound Effects

You can find free sound effects from:
- **Freesound.org** (https://freesound.org) - Creative Commons licensed sounds
- **Zapsplat.com** (https://www.zapsplat.com) - Free sound effects for personal use
- **Mixkit.co** (https://mixkit.co/free-sound-effects) - Free sound effects
- Create your own using tools like Audacity

## Adding New Sound Effects

To add new sound effects:

1. Add the MP3 file to this directory
2. Update `LifeForge.Web/wwwroot/js/audio.js` - Add entry to `soundFiles` object
3. Update `LifeForge.Web/Services/AudioService.cs` - Add to `SoundEffect` enum
4. Use `AudioService.PlaySoundAsync()` to play the sound in your code

## Format Recommendations

- **Format**: MP3 (best browser compatibility)
- **Sample Rate**: 44.1 kHz
- **Bit Rate**: 128-192 kbps
- **Duration**: Keep under 2 seconds for UI feedback sounds
- **Volume**: Normalize audio to prevent clipping

## License Notes

Ensure any sound effects you add are properly licensed for your use case. 
Keep attribution information if required by the license.
