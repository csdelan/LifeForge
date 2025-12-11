# QuestRun Feature Implementation

## Overview
The QuestRun feature allows users to start quest instances from the quest library, track active quests, and complete them to earn rewards.

## What's Been Added

### 1. Domain Layer (`LifeForge.Domain`)
- **Updated `QuestRun.cs`**: Implemented `StartTime`, `EndTime`, and `Rewards` properties
  - Added methods: `Start()`, `Complete()`, `Fail()`
  - Proper state management for quest lifecycle

### 2. Data Access Layer (`LifeForge.DataAccess`)
- **`QuestRunEntity.cs`**: MongoDB entity for quest runs with rewards
- **`IQuestRunRepository.cs`**: Repository interface
- **`QuestRunRepository.cs`**: MongoDB implementation with active quest filtering
- **Updated `MongoDbSettings.cs`**: Added `QuestRunsCollectionName` property

### 3. API Layer (`LifeForge.Api`)
- **`QuestRunDtos.cs`**: DTOs for quest runs and rewards
- **`QuestRunsController.cs`**: Full CRUD API with endpoints:
  - `GET /api/questruns` - Get all quest runs
  - `GET /api/questruns/active` - Get active quest runs only
  - `GET /api/questruns/{id}` - Get specific quest run
  - `POST /api/questruns/start` - Start a new quest run
  - `POST /api/questruns/{id}/complete` - Complete a quest run (auto-calculates rewards)
  - `DELETE /api/questruns/{id}` - Cancel/delete a quest run
- **Updated `Program.cs`**: Registered `IQuestRunRepository`
- **Updated `appsettings.json`**: Added QuestRunsCollectionName configuration

### 4. Web Layer (`LifeForge.Web`)
- **`QuestRunDto.cs`**: Client-side DTOs
- **`QuestRunService.cs`**: Service for API calls
- **`ActiveQuests.razor`**: New page displaying active quests in a table with:
  - Quest name, status, start time, duration
  - Rewards display (XP and currency)
  - Finish button to complete quests
  - Cancel button to abandon quests
- **Updated `Quests.razor`**: Added "Start Quest" button to each quest card
- **Updated `NavMenu.razor`**: Added "Active Quests" navigation link
- **Updated `Program.cs`**: Registered `QuestRunService`

## Features

### Starting a Quest
1. Navigate to **Quests** page
2. Click the **Start** button (play icon) on any quest card
3. Quest run is created with `InProgress` status
4. User sees success message

### Viewing Active Quests
1. Navigate to **Active Quests** page (or click "Active Quests" button from Quests page)
2. See table with all in-progress quest runs
3. View quest details, status, duration, and pending rewards

### Completing a Quest
1. On **Active Quests** page, click **Finish** button next to a quest
2. Quest status changes to `Completed`
3. Rewards are calculated based on quest difficulty:
   - **Trivial**: 10 XP, 5 Gold
   - **Easy**: 25 XP, 15 Gold
   - **Medium**: 50 XP, 30 Gold
   - **Hard**: 100 XP, 60 Gold
   - **CrazyHard**: 200 XP, 120 Gold
4. Quest run moves out of active list
5. Success message shows rewards earned

### Canceling a Quest
1. On **Active Quests** page, click the **X** button next to a quest
2. Confirm cancellation in dialog
3. Quest run is deleted (not marked as failed, just removed)

## Reward System

Currently, the reward system is automatic and based on quest difficulty:
- **Experience (XP)**: General class XP
- **Currency**: Gold

### Future Enhancements
- Make rewards configurable per quest
- Add character database integration to actually apply rewards
- Support for multiple currency types (Karma, Design Workslots)
- Support for items and badges
- Class-specific XP rewards
- Achievement tracking

## Database Collections

### QuestRuns Collection
```json
{
  "_id": "ObjectId",
  "questId": "ObjectId",  // Reference to Quest
  "questName": "string",
  "status": "InProgress|Completed|Failed",
  "startTime": "ISODate",
  "endTime": "ISODate?",
  "rewards": [
    {
      "type": "Experience|Currency|Item|Badge",
      "rewardClass": "string",  // e.g., "General", "Gold"
      "amount": 50,
      "imagePath": "string?"
    }
  ],
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

## API Endpoints

### Start a Quest Run
```http
POST /api/questruns/start
Content-Type: application/json

{
  "questId": "quest-id-here"
}
```

### Get Active Quest Runs
```http
GET /api/questruns/active
```

### Complete a Quest Run
```http
POST /api/questruns/{id}/complete
```

### Cancel a Quest Run
```http
DELETE /api/questruns/{id}
```

## UI Screenshots Description

### Active Quests Page
- Table layout with columns: Quest Name, Status, Started, Duration, Rewards, Actions
- Status badges with color coding (InProgress = blue)
- Reward badges showing XP (orange) and Gold (yellow)
- Duration display (e.g., "2h 15m", "3d 5h")
- Green "Finish" button and red "Cancel" button per row

### Quests Page Updates
- Green "Start" button (play icon) on each quest card
- "Active Quests" button in header to navigate to active quests
- Maintains existing Edit and Delete functionality

## Testing the Feature

### Prerequisites
1. MongoDB connection configured
2. At least one quest created in the Quest Manager

### Test Flow
1. **Start API**: `cd LifeForge.Api && dotnet run`
2. **Start Web**: `cd LifeForge.Web && dotnet run`
3. Navigate to **Quests** page (https://localhost:7295/quests)
4. Click **Start** on a quest
5. Navigate to **Active Quests** page
6. Verify quest appears in table
7. Click **Finish** to complete
8. Verify success message and rewards display
9. Confirm quest removed from active list

### Testing via Swagger
1. Navigate to https://localhost:7001/swagger
2. Test endpoints:
   - POST `/api/questruns/start` with a valid quest ID
   - GET `/api/questruns/active` to see active runs
   - POST `/api/questruns/{id}/complete` to finish
   - GET `/api/questruns` to see all (including completed)

## Known Limitations

1. **Character Integration**: Rewards are calculated but not yet applied to character stats
   - Character data is currently hardcoded in CharacterSheet.razor
   - Need to implement Character database storage and API
   
2. **Reward Configuration**: Rewards are auto-calculated, not configurable per quest
   - Future: Add rewards configuration to Quest creation/editing
   
3. **Quest Repeatability**: Not enforced yet
   - OneTime quests can be started multiple times
   - Future: Check repeatability rules before starting

4. **Quest Progress**: No progress tracking for multi-step quests
   - Currently just start/complete
   - Future: Add progress percentage and milestones

5. **Time Tracking**: Duration is calculated but not used for gameplay
   - Future: Time-based bonuses or penalties

## Next Steps

### Immediate Enhancements
1. Implement Character database and API
2. Apply rewards to character upon quest completion
3. Enforce quest repeatability rules
4. Add quest run history view

### Future Features
1. Quest progress tracking (0-100%)
2. Multi-step quest objectives
3. Quest chains and prerequisites
4. Time-based quest mechanics (daily quests reset)
5. Failure conditions and penalties
6. Quest categories and filtering
7. Quest search functionality
8. Leaderboards and statistics

## Files Modified/Created

### Created
- `LifeForge.DataAccess/Models/QuestRunEntity.cs`
- `LifeForge.DataAccess/Repositories/IQuestRunRepository.cs`
- `LifeForge.DataAccess/Repositories/QuestRunRepository.cs`
- `LifeForge.Api/Models/QuestRunDtos.cs`
- `LifeForge.Api/Controllers/QuestRunsController.cs`
- `LifeForge.Web/Models/QuestRunDto.cs`
- `LifeForge.Web/Services/QuestRunService.cs`
- `LifeForge.Web/Pages/ActiveQuests.razor`
- `QUESTRUN_FEATURE.md` (this file)

### Modified
- `LifeForge.Domain/QuestRun.cs` - Implemented properties and methods
- `LifeForge.DataAccess/Configuration/MongoDbSettings.cs` - Added collection name
- `LifeForge.Api/Program.cs` - Registered repository
- `LifeForge.Api/appsettings.json` - Added configuration
- `LifeForge.Web/Program.cs` - Registered service
- `LifeForge.Web/Pages/Quests.razor` - Added Start button
- `LifeForge.Web/Layout/NavMenu.razor` - Added navigation link

## Conclusion

The QuestRun feature is now fully implemented with:
? Full stack implementation (Domain ? DataAccess ? API ? Web)
? Start quest instances from quest library
? View active quests in a dedicated page
? Complete quests and earn rewards
? Automatic reward calculation based on difficulty
? Clean UI with status badges and reward display
? All CRUD operations via API
? MongoDB persistence

The foundation is solid and ready for the enhancements listed above!
