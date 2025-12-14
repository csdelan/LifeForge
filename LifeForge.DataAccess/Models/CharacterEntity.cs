using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of a Character
    /// </summary>
    public class CharacterEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("hp")]
        public decimal HP { get; set; }

        [BsonElement("hpMax")]
        public decimal HPMax { get; set; }

        [BsonElement("mp")]
        public decimal MP { get; set; }

        [BsonElement("mpMax")]
        public decimal MPMax { get; set; }

        [BsonElement("strength")]
        public int Strength { get; set; }

        [BsonElement("discipline")]
        public int Discipline { get; set; }

        [BsonElement("focus")]
        public int Focus { get; set; }

        [BsonElement("currencies")]
        public Dictionary<string, int> Currencies { get; set; } = new();

        [BsonElement("classProfiles")]
        public Dictionary<string, CharacterClassEntity> ClassProfiles { get; set; } = new();

        [BsonElement("activeBuffModifiers")]
        public AggregateModifierEntity ActiveBuffModifiers { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Convert domain Character to CharacterEntity
        /// </summary>
        public static CharacterEntity FromDomain(Character character)
        {
            var entity = new CharacterEntity
            {
                Name = character.Name,
                HP = character.HP,
                HPMax = character.HPMax,
                MP = character.MP,
                MPMax = character.MPMax,
                Strength = character.Strength,
                Discipline = character.Discipline,
                Focus = character.Focus,
                Currencies = character.Currencies.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => kvp.Value
                ),
                ClassProfiles = character.ClassProfiles.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new CharacterClassEntity
                    {
                        ClassName = kvp.Value.Class.Name,
                        Level = kvp.Value.Level,
                        CurrentXp = kvp.Value.CurrentXp,
                        XpToNextLevel = kvp.Value.XpToNextLevel,
                        BaseXp = kvp.Value.Class.BaseXp,
                        XpMultiplier = kvp.Value.Class.XpMultiplier
                    }
                ),
                ActiveBuffModifiers = AggregateModifierEntity.FromDomain(character.ActiveBuffModifiers)
            };

            return entity;
        }

        /// <summary>
        /// Convert CharacterEntity to domain Character
        /// </summary>
        public Character ToDomain()
        {
            var character = new Character(Name)
            {
                HP = HP,
                HPMax = HPMax,
                MP = MP,
                MPMax = MPMax,
                Strength = Strength,
                Discipline = Discipline,
                Focus = Focus,
                ActiveBuffModifiers = ActiveBuffModifiers?.ToDomain() ?? new AggregateModifier()
            };

            // Convert currencies
            foreach (var currency in Currencies)
            {
                if (Enum.TryParse<CurrencyType>(currency.Key, out var currencyType))
                {
                    character.Currencies[currencyType] = currency.Value;
                }
            }

            // Convert class profiles
            foreach (var classProfile in ClassProfiles)
            {
                var characterClass = new CharacterClass(classProfile.Value.ClassName)
                {
                    BaseXp = classProfile.Value.BaseXp,
                    XpMultiplier = classProfile.Value.XpMultiplier
                };

                character.ClassProfiles[classProfile.Key] = new CharacterClassSnapshot(
                    characterClass,
                    classProfile.Value.Level,
                    classProfile.Value.CurrentXp
                );
            }

            return character;
        }
    }

    /// <summary>
    /// MongoDB representation of a CharacterClassSnapshot
    /// </summary>
    public class CharacterClassEntity
    {
        [BsonElement("className")]
        public string ClassName { get; set; } = string.Empty;

        [BsonElement("level")]
        public int Level { get; set; }

        [BsonElement("currentXp")]
        public int CurrentXp { get; set; }

        [BsonElement("xpToNextLevel")]
        public int XpToNextLevel { get; set; }

        [BsonElement("baseXp")]
        public uint BaseXp { get; set; } = 100;

        [BsonElement("xpMultiplier")]
        public double XpMultiplier { get; set; } = 1.1;
    }
}
