# Currency Rewards System Implementation

## Overview
This implementation adds the ability to apply quest rewards (currencies and experience) to the character when completing quests. The system follows a clean architecture pattern with a new Application layer for business logic.

## What Was Implemented

### 1. **Domain Layer Updates** (`LifeForge.Domain`)

#### `Character.cs` - New Methods Added:
- `AddCurrency(CurrencyType type, int amount)` - Adds currency to character's wallet
- `AddExperience(string className, int amount)` - Adds XP to character classes with automatic leveling
- `ApplyReward(Reward reward)` - Applies any reward type to the character

**Key Features:**
- Automatic level-up when XP threshold is reached
- Creates new character classes if they don't exist when XP is awarded
- Validation to ensure amounts are non-negative
- Immutable CharacterClassSnapshot properly recreated with updated values

---

### 2. **Data Access Layer** (`LifeForge.DataAccess`)

#### New Files Created:

**`Models/CharacterEntity.cs`**
- MongoDB entity representation of Character
- Includes nested `CharacterClassEntity` for class profiles
- Methods: `FromDomain()` and `ToDomain()` for entity/domain conversion
- Stores currencies as `Dictionary<string, int>` for MongoDB compatibility

**`Repositories/ICharacterRepository.cs`**
- Interface for character data access
- Methods:
  - `GetCharacterAsync()` - Get the single character
  - `CreateCharacterAsync(CharacterEntity)` - Create initial character
  - `UpdateCharacterAsync(CharacterEntity)` - Update character
  - `CharacterExistsAsync()` - Check if character exists

**`Repositories/CharacterRepository.cs`**
- MongoDB implementation of ICharacterRepository
- Single character system (always returns/updates the one character)
- Auto-creates character if none exists on first update

**`Configuration/MongoDbSettings.cs` - Updated**
- Added `CharactersCollectionName` property (default: "Characters")

---

### 3. **Application Layer** (`LifeForge.Application`) **[NEW PROJECT]**

This new project contains business logic for applying rewards.

#### Files Created:

**`Models/RewardApplicationResult.cs`**
```csharp
public class RewardApplicationResult
{
    public bool Success { get; set; }
    public List<string> AppliedRewards { get; set; }
    public Dictionary<string, int> CurrenciesGained { get; set; }
    public Dictionary<string, int> ExperienceGained { get; set; }
    public string? ErrorMessage { get; set; }
}
```

**`Services/IRewardApplicationService.cs`**
- Interface for reward application logic
- Method: `ApplyQuestRewardsAsync(string questRunId)`

**`Services/RewardApplicationService.cs`**
- Orchestrates reward application process
- Steps:
  1. Retrieves quest run from database
  2. Validates quest is completed
  3. Gets or creates character
  4. Applies each reward to character
  5. Saves updated character
- Auto-creates default character if none exists
- Comprehensive logging for debugging

**`LifeForge.Application.csproj`**
- References: Domain, DataAccess
- Package: Microsoft.Extensions.Logging.Abstractions

---

### 4. **API Layer** (`LifeForge.Api`)

#### New Files:

**`Models/CharacterDtos.cs`**
- `CharacterDto` - Full character data for API responses
- `CharacterClassDto` - Class profile data
- `UpdateCharacterDto` - For updating character stats
- `RewardApplicationResultDto` - Result of applying rewards

**`Controllers/CharactersController.cs`**
- `GET /api/characters` - Get the character
- `PUT /api/characters` - Update character stats

#### Updated Files:

**`Controllers/QuestRunsController.cs`**
- Added `IRewardApplicationService` dependency
- New endpoint: `POST /api/questruns/{id}/apply-rewards`
  - Applies rewards to character after quest completion
  - Returns detailed result of what was applied

**`Program.cs`**
- Registered `ICharacterRepository` as Singleton
- Registered `IRewardApplicationService` as Scoped
- Added reference to Application project

**`LifeForge.Api.csproj`**
- Added project reference to `LifeForge.Application`

---

### 5. **Web Layer** (`LifeForge.Web`)

#### New Files:

**`Models/CharacterDto.cs`**
- Client-side DTOs for character data
- `CharacterDto`, `CharacterClassDto`, `RewardApplicationResultDto`

**`Services/CharacterService.cs`**
- `GetCharacterAsync()` - Retrieves character from API
- Handles 404 gracefully (character doesn't exist yet)

#### Updated Files:

**`Services/QuestRunService.cs`**
- Added `ApplyRewardsAsync(string questRunId)` method
- Calls the new `/api/questruns/{id}/apply-rewards` endpoint

**`Pages/ActiveQuests.razor`**
- Updated `CompleteQuestRun()` method:
  1. Completes the quest run
  2. Calls `ApplyRewardsAsync()` to apply rewards
  3. Shows detailed success message with currencies and XP gained
- New helper method: `FormatRewardMessage()` - Formats reward details for display

**`Pages/CharacterSheet.razor`**
- Completely rewritten to load real character data
- Injects `CharacterService`
- Loads character from API on initialization
- Shows "No Character Yet" message if character doesn't exist
- Displays real currencies and class XP from database
- Loading state while fetching data

**`Program.cs`**
- Registered `CharacterService` in DI

---

## Data Flow

### When User Completes a Quest:

```
1. User clicks "Finish" button
   ?
2. ActiveQuests.razor calls CompleteQuestRunAsync()
   ?
3. API marks quest run as Completed and assigns rewards
   ?
4. ActiveQuests.razor calls ApplyRewardsAsync()
   ?
5. API ? RewardApplicationService.ApplyQuestRewardsAsync()
   ?
6. Service retrieves quest run rewards
   ?
7. Service gets/creates character from database
   ?
8. Service applies each reward:
      - Currency ? Character.AddCurrency()
      - Experience ? Character.AddExperience() (with auto-leveling)
   ?
9. Service saves updated character to MongoDB
   ?
10. Success result returned to UI with details
   ?
11. UI displays: "Quest 'X' completed! +50 Gold, +100 Developer XP"
```

---

## Architecture Diagram

```
???????????????????????????????????????????????????????????
?                  Blazor WebAssembly                     ?
?                   (LifeForge.Web)                       ?
?  • CharacterSheet.razor (displays real character)      ?
?  • ActiveQuests.razor (completes & applies rewards)    ?
?  • CharacterService, QuestRunService                   ?
???????????????????????????????????????????????????????????
                     ? HTTP/JSON
                     ?
???????????????????????????????????????????????????????????
?                  ASP.NET Core API                       ?
?                   (LifeForge.Api)                       ?
?  • CharactersController (GET character)                ?
?  • QuestRunsController (apply-rewards endpoint)        ?
???????????????????????????????????????????????????????????
                     ?
                     ?
???????????????????????????????????????????????????????????
?                Application Layer                        ?
?              (LifeForge.Application)                    ?
?  • RewardApplicationService                            ?
?     - Orchestrates reward application                  ?
?     - Business logic for currency & XP                 ?
???????????????????????????????????????????????????????????
                     ?
                     ?
???????????????????????????????????????????????????????????
?                Data Access Layer                        ?
?              (LifeForge.DataAccess)                     ?
?  • CharacterRepository (MongoDB CRUD)                  ?
?  • QuestRunRepository (get quest rewards)              ?
???????????????????????????????????????????????????????????
                     ?
                     ?
???????????????????????????????????????????????????????????
?                Domain Layer                             ?
?                (LifeForge.Domain)                       ?
?  • Character.ApplyReward()                             ?
?  • Character.AddCurrency()                             ?
?  • Character.AddExperience() (with leveling)           ?
???????????????????????????????????????????????????????????
                     ?
                     ?
???????????????????????????????????????????????????????????
?                MongoDB Atlas                            ?
?  • Characters Collection (stores character data)       ?
?  • QuestRuns Collection (stores completed quests)      ?
???????????????????????????????????????????????????????????
```

---

## Testing the Feature

### Prerequisites:
1. MongoDB Atlas configured
2. API running on https://localhost:7001
3. Web app running on https://localhost:7295

### Test Steps:

1. **Create a Quest with Rewards**
   - Go to `/quests`
   - Create a quest with rewards: 50 Gold, 100 Developer XP

2. **Start the Quest**
   - Click "Start" button on the quest

3. **View Active Quest**
   - Go to `/active-quests`
   - See the quest in progress

4. **Complete the Quest**
   - Click "Finish" button
   - See success message: "Quest 'X' completed! Rewards: +50 Gold, +100 Developer XP"

5. **View Character Sheet**
   - Go to `/` or `/character`
   - See updated currency: Gold = 50
   - See new class: Developer (Level 1, 100/100 XP)

6. **Complete Another Quest**
   - Repeat steps 2-4 with a quest giving 200 Developer XP
   - Character sheet should show: Developer (Level 2, with remaining XP)

---

## Database Collections

### Characters Collection
```json
{
  "_id": "ObjectId",
  "name": "Hero of LifeForge",
  "hp": 100,
  "hpMax": 100,
  "mp": 100,
  "mpMax": 100,
  "strength": 10,
  "discipline": 10,
  "focus": 10,
  "currencies": {
    "Gold": 150,
    "Karma": 50,
    "DesignWorkslot": 3
  },
  "classProfiles": {
    "Developer": {
      "className": "Developer",
      "level": 2,
      "currentXp": 50,
      "xpToNextLevel": 100,
      "baseXp": 100,
      "xpMultiplier": 1.1
    },
    "Trader": {
      "className": "Trader",
      "level": 1,
      "currentXp": 75,
      "xpToNextLevel": 100,
      "baseXp": 100,
      "xpMultiplier": 1.1
    }
  },
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

---

## Key Features

? **Automatic Character Creation**
   - First quest completion creates default character
   - No manual setup required

? **Currency Management**
   - Supports Gold, Karma, DesignWorkslot
   - Easily extensible for new currency types

? **Experience & Leveling**
   - Automatic level-up when XP threshold reached
   - Creates character classes dynamically
   - XP requirements scale with level (BaseXP * Multiplier^Level)

? **Separation of Concerns**
   - Domain: Pure business logic
   - Application: Orchestration & workflows
   - DataAccess: Database operations
   - API: HTTP endpoints
   - Web: UI components

? **Error Handling**
   - Comprehensive error messages
   - Graceful handling of missing character
   - Validation of quest completion status

? **Logging**
   - Detailed logs for debugging
   - Tracks each reward application
   - Logs errors with context

---

## Future Enhancements

### Short Term:
- [ ] Add character initialization UI (set name, starting stats)
- [ ] Show level-up notifications in UI
- [ ] Add character stat increases on level-up
- [ ] Track reward history/transaction log

### Medium Term:
- [ ] Implement Item rewards (inventory system)
- [ ] Implement Badge rewards (achievements)
- [ ] Add character portraits/avatars
- [ ] Multi-currency wallet UI

### Long Term:
- [ ] Character skills/perks system
- [ ] Prestige/rebirth system
- [ ] Multi-character support
- [ ] Character import/export

---

## Files Created/Modified Summary

### Created:
- `LifeForge.Application/` (entire new project)
  - `LifeForge.Application.csproj`
  - `Models/RewardApplicationResult.cs`
  - `Services/IRewardApplicationService.cs`
  - `Services/RewardApplicationService.cs`
- `LifeForge.DataAccess/Models/CharacterEntity.cs`
- `LifeForge.DataAccess/Repositories/ICharacterRepository.cs`
- `LifeForge.DataAccess/Repositories/CharacterRepository.cs`
- `LifeForge.Api/Models/CharacterDtos.cs`
- `LifeForge.Api/Controllers/CharactersController.cs`
- `LifeForge.Web/Models/CharacterDto.cs`
- `LifeForge.Web/Services/CharacterService.cs`
- `CURRENCY_REWARDS_IMPLEMENTATION.md` (this file)

### Modified:
- `LifeForge.Domain/Character.cs` - Added reward application methods
- `LifeForge.DataAccess/Configuration/MongoDbSettings.cs` - Added CharactersCollectionName
- `LifeForge.Api/Controllers/QuestRunsController.cs` - Added apply-rewards endpoint
- `LifeForge.Api/Program.cs` - Registered new services
- `LifeForge.Api/LifeForge.Api.csproj` - Added Application reference
- `LifeForge.Web/Services/QuestRunService.cs` - Added ApplyRewardsAsync
- `LifeForge.Web/Pages/ActiveQuests.razor` - Updated CompleteQuestRun
- `LifeForge.Web/Pages/CharacterSheet.razor` - Load real character data
- `LifeForge.Web/Program.cs` - Registered CharacterService
- `LifeForge.sln` - Added Application project

---

## Build Status
? **All projects compile successfully**

## Conclusion

The Currency Rewards System is now fully functional! When users complete quests, rewards are automatically applied to their character, currencies are added, and XP is awarded with automatic leveling. The character sheet displays real-time data from the database, creating a complete reward loop for the gamification system.

The clean architecture ensures the system is maintainable, testable, and ready for future enhancements like items, badges, and achievements.
