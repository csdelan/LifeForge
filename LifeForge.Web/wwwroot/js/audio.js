// Audio playback module for LifeForge
// Handles loading and playing sound effects

// Map of sound effect names to their file paths
const soundFiles = {
    'BuffApplied': '/sounds/buff-applied.mp3',
    'DebuffApplied': '/sounds/debuff-applied.mp3',
    // Add more sound effects here as they are added to the enum
};

// Cache for preloaded audio elements
const audioCache = {};

/**
 * Preload a sound effect into the cache
 * @param {string} soundName - Name of the sound effect
 */
function preloadSound(soundName) {
    if (!audioCache[soundName] && soundFiles[soundName]) {
        const audio = new Audio(soundFiles[soundName]);
        audio.preload = 'auto';
        audioCache[soundName] = audio;
    }
}

/**
 * Preload all sound effects
 */
export function preloadAllSounds() {
    Object.keys(soundFiles).forEach(soundName => {
        preloadSound(soundName);
    });
}

/**
 * Play a sound effect
 * @param {string} soundName - Name of the sound effect to play
 * @param {number} volume - Volume level (0.0 to 1.0)
 */
export function playSound(soundName, volume = 0.5) {
    // Ensure volume is within valid range
    volume = Math.max(0, Math.min(1, volume));

    // Check if sound file exists
    if (!soundFiles[soundName]) {
        console.warn(`Sound effect '${soundName}' not found in sound files map`);
        return;
    }

    // Get or create audio element
    let audio;
    if (audioCache[soundName]) {
        // Clone the cached audio to allow multiple simultaneous plays
        audio = audioCache[soundName].cloneNode();
    } else {
        audio = new Audio(soundFiles[soundName]);
        // Cache for future use
        preloadSound(soundName);
    }

    // Set volume and play
    audio.volume = volume;
    audio.play().catch(err => {
        console.warn(`Could not play sound '${soundName}'. This is normal if audio files haven't been added yet. See /sounds/README.md for instructions.`);
    });
}

/**
 * Stop all currently playing sounds
 */
export function stopAllSounds() {
    Object.values(audioCache).forEach(audio => {
        if (!audio.paused) {
            audio.pause();
            audio.currentTime = 0;
        }
    });
}

// Preload sounds when the module loads
preloadAllSounds();
