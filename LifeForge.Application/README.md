# LifeForge.Application

Application layer containing business logic and orchestration services for LifeForge.

## Overview

This project sits between the API and Data Access layers, providing:
- Business logic implementation
- Workflow orchestration
- Cross-cutting concerns (logging, validation)
- Service interfaces and implementations

## Architecture

```
API Layer (Controllers)
    ?
Application Layer (Services) ? YOU ARE HERE
    ?
Data Access Layer (Repositories)
    ?
Domain Layer (Models)
```

## Services

### RewardApplicationService

Orchestrates the application of quest rewards to characters.

**Responsibilities:**
- Retrieve quest run data
- Validate quest completion
- Get or create character
- Apply rewards to character domain model
- Persist updated character
- Return detailed results

**Usage:**
```csharp
public class QuestRunsController : ControllerBase
{
    private readonly IRewardApplicationService _rewardService;
    
    [HttpPost("{id}/apply-rewards")]
    public async Task<ActionResult<RewardApplicationResultDto>> ApplyRewards(string id)
    {
        var result = await _rewardService.ApplyQuestRewardsAsync(id);
        return Ok(result);
    }
}
```

**Process Flow:**
1. Retrieve quest run from repository
2. Validate quest status is "Completed"
3. Get character (or create default if none exists)
4. Convert character entity to domain model
5. Apply each reward via domain methods
6. Convert back to entity and save
7. Return success result with details

## Models

### RewardApplicationResult

Result DTO returned by reward application service.

**Properties:**
- `Success` - Whether operation succeeded
- `AppliedRewards` - List of human-readable reward descriptions
- `CurrenciesGained` - Dictionary of currency types and amounts
- `ExperienceGained` - Dictionary of class names and XP amounts
- `ErrorMessage` - Error details if operation failed

**Example:**
```json
{
  "success": true,
  "appliedRewards": ["+50 Gold", "+100 Developer XP"],
  "currenciesGained": { "Gold": 50 },
  "experienceGained": { "Developer": 100 },
  "errorMessage": null
}
```

## Dependencies

### Projects:
- **LifeForge.Domain** - Domain models (Character, Reward, etc.)
- **LifeForge.DataAccess** - Repositories and entities

### NuGet Packages:
- **Microsoft.Extensions.Logging.Abstractions** - Logging interfaces

## Dependency Injection

Register services in `Program.cs`:

```csharp
// Add application services
builder.Services.AddScoped<IRewardApplicationService, RewardApplicationService>();
```

## Error Handling

Services handle errors gracefully:
- Return `RewardApplicationResult` with `Success = false`
- Populate `ErrorMessage` with details
- Log errors using `ILogger`
- Never throw exceptions to callers

**Example Error Cases:**
- Quest run not found
- Quest not completed
- Character creation failure
- Database update failure

## Logging

Services use structured logging:

```csharp
_logger.LogInformation("Successfully applied {RewardCount} rewards", count);
_logger.LogError(ex, "Error applying rewards for quest {QuestRunId}", id);
_logger.LogDebug("Applied reward: {Type} {Amount} {Class}", type, amount, rewardClass);
```

## Testing

### Manual Testing:
1. Start API: `dotnet run` (from LifeForge.Api)
2. Complete a quest via Swagger: `POST /api/questruns/{id}/complete`
3. Apply rewards: `POST /api/questruns/{id}/apply-rewards`
4. Check character: `GET /api/characters`

### Future Unit Tests:
```csharp
[Fact]
public async Task ApplyQuestRewards_ValidQuestRun_ReturnsSuccess()
{
    // Arrange
    var questRunId = "test-id";
    var service = CreateService();
    
    // Act
    var result = await service.ApplyQuestRewardsAsync(questRunId);
    
    // Assert
    Assert.True(result.Success);
    Assert.Contains("Gold", result.CurrenciesGained.Keys);
}
```

## Extension Points

### Adding New Reward Types:

1. **Add to Domain Enum:**
```csharp
public enum RewardType
{
    Experience,
    Currency,
    Item,        // ? New
    Badge        // ? New
}
```

2. **Update Character Domain:**
```csharp
public void ApplyReward(Reward reward)
{
    switch (reward.Type)
    {
        case RewardType.Item:
            AddItem(reward.RewardClass, reward.Amount);
            break;
    }
}
```

3. **Update Service:**
```csharp
private void ApplyReward(Character character, Reward reward, RewardApplicationResult result)
{
    character.ApplyReward(reward);
    
    // Track applied item
    if (reward.Type == RewardType.Item)
    {
        result.AppliedRewards.Add($"+{reward.Amount} {reward.RewardClass}");
    }
}
```

### Adding New Services:

```csharp
// 1. Create interface
public interface ICharacterLevelingService
{
    Task<LevelUpResult> LevelUpAsync(string characterId);
}

// 2. Implement service
public class CharacterLevelingService : ICharacterLevelingService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ILogger<CharacterLevelingService> _logger;
    
    // Implementation...
}

// 3. Register in DI
builder.Services.AddScoped<ICharacterLevelingService, CharacterLevelingService>();
```

## Design Patterns

### Patterns Used:

1. **Service Layer Pattern** - Encapsulates business logic
2. **Repository Pattern** - Data access abstraction (from DataAccess layer)
3. **Dependency Injection** - Loose coupling
4. **Result Object Pattern** - Return rich result objects instead of exceptions

### Benefits:

- ? **Testability** - Easy to mock dependencies
- ? **Maintainability** - Business logic in one place
- ? **Separation of Concerns** - Clear layer boundaries
- ? **Reusability** - Services can be used by multiple controllers
- ? **Flexibility** - Easy to add new features

## Future Enhancements

### Planned Services:

- [ ] **ICharacterProgressionService** - Handle leveling, stats, perks
- [ ] **IInventoryService** - Manage character items
- [ ] **IAchievementService** - Track and award achievements
- [ ] **IQuestRecommendationService** - Suggest quests based on character

### Planned Features:

- [ ] Transaction logging for reward application
- [ ] Rollback support for failed operations
- [ ] Bulk reward application
- [ ] Scheduled rewards (daily login bonuses)
- [ ] Reward multipliers and bonuses

## Contributing

When adding new features to the Application layer:

1. **Create Interface First** - Define the contract
2. **Implement Service** - Focus on single responsibility
3. **Add Logging** - Use structured logging
4. **Handle Errors** - Return result objects, don't throw
5. **Update DI** - Register service in API Program.cs
6. **Document** - Update this README

## Questions?

See also:
- [CURRENCY_REWARDS_IMPLEMENTATION.md](../CURRENCY_REWARDS_IMPLEMENTATION.md) - Full implementation guide
- [LifeForge.Domain/README.md](../LifeForge.Domain/README.md) - Domain models
- [LifeForge.DataAccess/README.md](../LifeForge.DataAccess/README.md) - Data access layer
