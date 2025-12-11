namespace LifeForge.DataAccess.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string QuestsCollectionName { get; set; } = "Quests";
        public string QuestRunsCollectionName { get; set; } = "QuestRuns";
        public string CharactersCollectionName { get; set; } = "Characters";
        public string BuffsCollectionName { get; set; } = "Buffs";
    }
}
