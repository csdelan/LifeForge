# Configurable Quest Rewards Feature

## Overview
Quest rewards are now fully configurable! Each quest can have custom rewards defined with emoji icons for visual display.

## What's Been Added

### 1. Domain Layer Updates
- **`Reward.cs`**: Made properties settable and added constructor with Icon parameter
  - `Icon` property now uses `char` to store emoji characters
  - Added constructors for easy reward creation

- **`Quest.cs`**: Added `Rewards` property
  - Type: `List<Reward>`
  - Stores configured rewards for each quest

### 2. Data Access Layer Updates
- **`QuestEntity.cs`**: 
  - Added `Rewards` property mapped to MongoDB
  - Updated `FromDomain()` and `ToDomain()` methods to handle rewards
  - Icon stored as string in database for emoji support

- **`QuestRunEntity.cs`**: 
  - Added `Icon` property to `RewardEntity`
  - Stores emoji characters for visual display

### 3. API Layer Updates
- **`QuestDtos.cs`**: 
  - Added `Rewards` to all Quest DTOs (QuestDto, CreateQuestDto, UpdateQuestDto)
  - Created shared `RewardDto` with Icon property

- **`QuestsController.cs`**: 
  - All CRUD operations now handle rewards
  - Rewards serialized/deserialized properly

- **`QuestRunsController.cs`**: 
  - Updated `CompleteQuestRun()` to use quest-defined rewards
  - Falls back to auto-calculation if no rewards configured
  - Copies icon from quest rewards to quest run

### 4. Web Layer Updates
- **`QuestDto.cs` & `QuestRunDto.cs`**: Added Rewards property with Icon support

- **`Quests.razor`**: 
  - **Rewards Configuration UI** in create/edit modal:
    - Select reward type (Experience, Currency, Item, Badge)
    - Enter reward class/currency name
    - Set reward amount
    - Add emoji icon (??????? etc.)
    - Add/Remove multiple rewards per quest
  - **Quest Cards** now show rewards preview with icons
  - Proper handling of reward data when creating/editing quests

- **`ActiveQuests.razor`**: 
  - Rewards column displays emoji icons when available
  - Falls back to Bootstrap icons if no emoji set

## Usage Guide

### Creating a Quest with Rewards

1. Navigate to **Quests** page
2. Click **Create New Quest**
3. Fill in quest details (name, description, difficulty, etc.)
4. **Configure Rewards:**
   - Click "Add Reward" button
   - Select reward type from dropdown
   - Enter class/currency (e.g., "Gold", "General", "Developer")
   - Set amount (e.g., 50)
   - Add emoji icon (e.g., ?? for gold, ? for XP)
   - Add multiple rewards as needed
   - Remove rewards with trash icon button
5. Click **Create Quest**

### Common Emoji Icons for Rewards

| Reward Type | Suggested Emoji | Example |
|------------|----------------|---------|
| Gold | ?? ?? | 50 ?? Gold |
| XP | ? ? | 100 ? XP |
| Karma | ?? ?? | 25 ?? Karma |
| Items | ?? ?? | 1 ?? Item |
| Badge | ?? ?? | 1 ?? Badge |
| Workslots | ?? ?? | 3 ?? Workslots |

### Editing Quest Rewards

1. On Quests page, click **Edit** button on a quest card
2. Scroll to **Rewards** section
3. Modify existing rewards or add new ones
4. Click **Update Quest**

### How Rewards Work

1. **Quest Creation**: Rewards are configured and saved with the quest
2. **Starting a Quest**: Quest run created with empty rewards list
3. **Completing a Quest**: 
   - System retrieves quest's configured rewards
   - Copies rewards to the completed quest run
   - Displays rewards in success message and Active Quests page
4. **Future**: Rewards will be applied to character stats (not yet implemented)

## Database Schema

### Quests Collection
```json
{
  "_id": "ObjectId",
  "name": "Complete Project Documentation",
  "difficulty": "Medium",
  "repeatability": "OneTime",
  "rewards": [
    {
      "type": "Experience",
      "rewardClass": "Developer",
      "amount": 75,
      "icon": "?"
    },
    {
      "type": "Currency",
      "rewardClass": "Gold",
      "amount": 50,
      "icon": "??"
    }
  ]
}
```

### QuestRuns Collection
```json
{
  "_id": "ObjectId",
  "questId": "reference-to-quest",
  "status": "Completed",
  "rewards": [
    {
      "type": "Experience",
      "rewardClass": "Developer",
      "amount": 75,
      "icon": "?"
    },
    {
      "type": "Currency",
      "rewardClass": "Gold",
      "amount": 50,
      "icon": "??"
    }
  ]
}
```

## UI Components

### Rewards Configuration (Quest Modal)
- Grid layout with 5 columns:
  1. Reward Type dropdown
  2. Class/Currency text input
  3. Amount number input
  4. Emoji icon input (max 2 chars for combo emojis)
  5. Delete button
- "Add Reward" button to add more rewards
- Help text explaining emoji usage

### Rewards Preview (Quest Cards)
- Shows configured rewards with emoji icons
- Displays as badges: `?? 50` or `? 100`
- Hidden if no rewards configured

### Rewards Display (Active Quests Table)
- Shows earned rewards upon completion
- Uses emoji icons if configured
- Falls back to Bootstrap icons for backward compatibility
- Color-coded badges by reward type

## Example Reward Configurations

### Simple Quest (Easy)
```
Rewards:
- ? 25 Experience (General)
- ?? 15 Gold
```

### Complex Quest (Hard)
```
Rewards:
- ? 100 Experience (Developer)
- ? 50 Experience (Designer)
- ?? 75 Gold
- ?? 30 Karma
- ?? 1 DesignWorkslot
```

### Daily Habit
```
Rewards:
- ? 10 Experience (General)
- ?? 5 Gold
```

## Backward Compatibility

If a quest has **no rewards configured**:
- QuestRunsController automatically calculates rewards based on difficulty
- Uses default XP and Gold amounts
- No icons are assigned (falls back to Bootstrap icons in UI)

## Future Enhancements

### Character Integration
- [ ] Apply rewards to character upon quest completion
- [ ] Update character XP for specific classes
- [ ] Add currency to character's wallet
- [ ] Track reward history

### Reward Types Expansion
- [ ] Implement Item rewards (inventory system)
- [ ] Implement Badge rewards (achievements)
- [ ] Custom reward types
- [ ] Percentage-based rewards

### UI Improvements
- [ ] Reward templates/presets
- [ ] Emoji picker for icons
- [ ] Reward preview in quest details view
- [ ] Reward history view
- [ ] Leaderboard showing total rewards earned

### Reward Calculation
- [ ] Difficulty multipliers
- [ ] Time-based bonuses
- [ ] Streak bonuses for repeatable quests
- [ ] Bonus rewards for perfect completion

## Technical Notes

### Icon Storage
- Icons stored as strings (not char) in database for emoji support
- Unicode emojis work natively in modern browsers
- Single emoji (like ??) = 1-2 bytes
- Compound emojis (like ?????) may use more bytes

### Empty Icon Handling
- If Icon is empty/null, UI falls back to Bootstrap icons
- Icon property is optional in DTOs
- Default constructor initializes Icon as `\0` (null character)

## Testing Scenarios

### Test Case 1: Create Quest with Rewards
1. Create quest with 2 rewards (XP + Gold)
2. Add emoji icons
3. Save and verify rewards appear on quest card
4. Start quest and complete it
5. Verify rewards display correctly in Active Quests

### Test Case 2: Edit Existing Quest Rewards
1. Edit a quest without rewards
2. Add rewards with emojis
3. Update quest
4. Verify changes saved
5. Start and complete quest to verify new rewards

### Test Case 3: Quest Without Rewards (Backward Compatibility)
1. Create quest without adding any rewards
2. Start quest
3. Complete quest
4. Verify auto-calculated rewards appear
5. Verify Bootstrap icons used instead of emojis

### Test Case 4: Multiple Reward Types
1. Create quest with 4 different reward types
2. Use different emojis for each
3. Complete quest
4. Verify all rewards display correctly

## Files Modified

### Created
- `QUEST_REWARDS_FEATURE.md` (this document)

### Modified
- Domain:
  - `Reward.cs` - Made properties settable, added Icon support
  - `Quest.cs` - Added Rewards property

- DataAccess:
  - `QuestEntity.cs` - Added Rewards with Icon support
  - `QuestRunEntity.cs` - Added Icon to RewardEntity

- API:
  - `QuestDtos.cs` - Added Rewards and Icon
  - `QuestRunDtos.cs` - Removed duplicate RewardDto
  - `QuestsController.cs` - Handle rewards in CRUD operations
  - `QuestRunsController.cs` - Use quest-defined rewards

- Web:
  - `QuestDto.cs` - Added Rewards property
  - `QuestRunDto.cs` - Added Icon to RewardDto
  - `Quests.razor` - Added rewards configuration UI and preview
  - `ActiveQuests.razor` - Display emoji icons in rewards

## Conclusion

? Quest rewards are now fully configurable
? Emoji icons provide visual appeal
? Backward compatible with auto-calculated rewards
? Full CRUD support for reward management
? Rewards properly transferred from quests to quest runs
? UI displays rewards throughout the quest lifecycle

Ready for character integration to actually apply rewards!
