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
        /// Get all characters (useful for buff activation to find character ID)
        /// </summary>
        Task<List<CharacterEntity>> GetAllCharactersAsync();

        /// <summary>
        /// Get character by ID
        /// </summary>
        Task<CharacterEntity?> GetCharacterByIdAsync(string id);

        /// <summary>
        /// Create the initial character
        /// </summary>
        Task<CharacterEntity> CreateCharacterAsync(CharacterEntity character);

        /// <summary>
        /// Update the character
        /// </summary>
        Task<bool> UpdateCharacterAsync(CharacterEntity character);

        /// <summary>
        /// Update the character by ID
        /// </summary>
        Task<bool> UpdateCharacterAsync(string id, CharacterEntity character);

        /// <summary>
        /// Check if a character exists
        /// </summary>
        Task<bool> CharacterExistsAsync();
    }
}
