using LifeForge.DataAccess.Models;

namespace LifeForge.DataAccess.Repositories
{
    /// <summary>
    /// Repository interface for Character data access
    /// Single character system - always returns/updates the one character
    /// </summary>
    public interface ICharacterRepository
    {
        /// <summary>
        /// Get the character (single character system)
        /// </summary>
        Task<CharacterEntity?> GetCharacterAsync();

        /// <summary>
        /// Create the initial character
        /// </summary>
        Task<CharacterEntity> CreateCharacterAsync(CharacterEntity character);

        /// <summary>
        /// Update the character
        /// </summary>
        Task<bool> UpdateCharacterAsync(CharacterEntity character);

        /// <summary>
        /// Check if a character exists
        /// </summary>
        Task<bool> CharacterExistsAsync();
    }
}
