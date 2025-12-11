using Microsoft.AspNetCore.Mvc;
using LifeForge.Api.Models;
using LifeForge.DataAccess.Repositories;

namespace LifeForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<CharactersController> _logger;

        public CharactersController(
            ICharacterRepository characterRepository,
            ILogger<CharactersController> logger)
        {
            _characterRepository = characterRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<CharacterDto>> GetCharacter()
        {
            try
            {
                var character = await _characterRepository.GetCharacterAsync();
                if (character == null)
                {
                    return NotFound("No character found. Complete a quest to create your character!");
                }

                var characterDto = new CharacterDto
                {
                    Id = character.Id,
                    Name = character.Name,
                    HP = character.HP,
                    HPMax = character.HPMax,
                    MP = character.MP,
                    MPMax = character.MPMax,
                    Strength = character.Strength,
                    Discipline = character.Discipline,
                    Focus = character.Focus,
                    Currencies = character.Currencies,
                    ClassProfiles = character.ClassProfiles.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new CharacterClassDto
                        {
                            ClassName = kvp.Value.ClassName,
                            Level = kvp.Value.Level,
                            CurrentXp = kvp.Value.CurrentXp,
                            XpToNextLevel = kvp.Value.XpToNextLevel
                        }
                    )
                };

                return Ok(characterDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving character");
                return StatusCode(500, "An error occurred while retrieving the character");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter([FromBody] UpdateCharacterDto updateDto)
        {
            try
            {
                var character = await _characterRepository.GetCharacterAsync();
                if (character == null)
                {
                    return NotFound("No character found");
                }

                character.Name = updateDto.Name;
                character.HP = updateDto.HP;
                character.HPMax = updateDto.HPMax;
                character.MP = updateDto.MP;
                character.MPMax = updateDto.MPMax;
                character.Strength = updateDto.Strength;
                character.Discipline = updateDto.Discipline;
                character.Focus = updateDto.Focus;

                var success = await _characterRepository.UpdateCharacterAsync(character);
                if (!success)
                {
                    return StatusCode(500, "Failed to update character");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating character");
                return StatusCode(500, "An error occurred while updating the character");
            }
        }
    }
}
