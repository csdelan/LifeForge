# BuffInstance Implementation Summary

## Overview
A complete full-stack solution for managing BuffInstances (active buffs) in the LifeForge application has been implemented. This allows users to activate manual buffs, track them as active instances, apply/remove modifiers to characters, and end active buffs.

## Components Created

### 1. Data Access Layer (LifeForge.DataAccess)

#### BuffInstanceEntity.cs
- MongoDB entity representation of an active buff instance
- Stores:
  - BuffId and CharacterId references
  - Buff metadata (name, isDebuff, stacks)
  - Start and end times
  - All buff modifiers (copied from buff definition for consistency)
  - IsActive flag
- Includes creation and update timestamps

#### IBuffInstanceRepository.cs & BuffInstanceRepository.cs
- Repository for buff instance management
- Methods:
  - `GetAllBuffInstancesAsync()` - Get all buff instances
  - `GetActiveBuffInstancesByCharacterIdAsync(characterId)` - Get active buffs for a character
  - `GetBuffInstanceByIdAsync(id)` - Get specific buff instance
  - `CreateBuffInstanceAsync(buffInstance)` - Create new buff instance
  - `UpdateBuffInstanceAsync(id, buffInstance)` - Update buff instance
  - `DeleteBuffInstanceAsync(id)` - Delete buff instance
  - `DeactivateBuffInstanceAsync(id)` - Mark buff as inactive

#### MongoDbSettings.cs (Updated)
- Added `BuffInstancesCollectionName` property set to "BuffInstances"

#### ICharacterRepository.cs & CharacterRepository.cs (Updated)
- Added `GetAllCharactersAsync()` - Get all characters
- Added `GetCharacterByIdAsync(id)` - Get character by ID
- Added `UpdateCharacterAsync(id, character)` - Update character by ID overload

### 2. Application Layer (LifeForge.Application)

#### BuffInstanceApplicationResult.cs
- Result model for buff activation/deactivation operations
- Contains:
  - Success flag
  - Error message
  - BuffInstanceId
  - Dictionary of applied modifiers

#### IBuffInstanceApplicationService.cs & BuffInstanceApplicationService.cs
- Core buff instance business logic
- **ActivateBuffAsync(characterId, buffId)**:
  1. Validates buff and character exist
  2. Checks for existing active buffs and handles stacking
  3. Creates new buff instance if not stacked
  4. Applies buff modifiers to character stats
  5. Handles both flat modifiers and percentage modifiers
  6. Returns result with applied modifiers
- **DeactivateBuffInstanceAsync(characterId, buffInstanceId)**:
  1. Validates buff instance and character
  2. Removes buff modifiers from character stats
  3. Deactivates the buff instance
  4. Returns result with removed modifiers

#### Modifier Application Logic
- **Flat Modifiers**: Applied directly, multiplied by stacks
  - HP, HPMax, MP, MPMax modifiers
- **Percentage Modifiers**: Calculated based on current/max values, multiplied by stacks
  - HP%, HPMax%, MP%, MPMax% modifiers
- **Ensures Bounds**: HP/MP clamped between 0 and max values
- **Reversible**: Same logic used for both application and removal

### 3. API Layer (LifeForge.Api)

#### BuffInstanceDtos.cs
- `BuffInstanceDto` - Complete buff instance data transfer object
- `ActivateBuffDto` - DTO for activating a buff (characterId + buffId)
- `DeactivateBuffInstanceDto` - DTO for deactivating a buff (characterId + buffInstanceId)
- `BuffInstanceApplicationResultDto` - Result DTO with success/error info

#### BuffInstancesController.cs
- RESTful API endpoints:
  - `GET /api/buffinstances` - Get all buff instances
  - `GET /api/buffinstances/character/{characterId}` - Get active buffs for character
  - `GET /api/buffinstances/{id}` - Get specific buff instance
  - `POST /api/buffinstances/activate` - Activate a buff
  - `POST /api/buffinstances/deactivate` - Deactivate a buff
  - `DELETE /api/buffinstances/{id}` - Delete buff instance
- Comprehensive error handling and logging

#### CharactersController.cs (Updated)
- Added `GET /api/characters/all` - Get all characters endpoint

#### Program.cs (Updated)
- Registered `IBuffInstanceRepository` and `BuffInstanceRepository` as singleton
- Registered `IBuffInstanceApplicationService` and `BuffInstanceApplicationService` as scoped

### 4. Web Layer (LifeForge.Web)

#### BuffInstanceDto.cs
- Web-specific DTOs for BuffInstance and result models
- Matches API DTOs for serialization

#### BuffInstanceService.cs
- HTTP client service for buff instance operations
- Methods:
  - `GetAllBuffInstancesAsync()` - Get all buff instances
  - `GetActiveBuffInstancesByCharacterIdAsync(characterId)` - Get active buffs
  - `GetBuffInstanceAsync(id)` - Get specific buff instance
  - `ActivateBuffAsync(characterId, buffId)` - Activate a buff
  - `DeactivateBuffInstanceAsync(characterId, buffInstanceId)` - Deactivate a buff
  - `DeleteBuffInstanceAsync(id)` - Delete buff instance

#### CharacterService.cs (Updated)
- Added `GetAllCharactersAsync()` - Get all characters

#### Buffs.razor (Updated)
- Added **Activate button** for manual buffs in the gallery view
- Shows play icon button for buffs with `Trigger = Manual`
- Clicking activate:
  1. Finds the character
  2. Calls `ActivateBuffAsync`
  3. Shows success message with applied modifiers
  4. Shows error message if activation fails

#### CharacterSheet.razor (Updated)
- Added **Active Buffs/Debuffs section**
- Displays all active buff instances for the character
- Shows for each buff:
  - Buff name
  - Stack count (if > 1)
  - Time remaining (formatted as days/hours/minutes)
  - End button (X icon)
- Visual styling:
  - Green gradient for buffs
  - Red gradient for debuffs
  - Translucent background for stacks/timer
- **End Buff functionality**:
  1. Confirms with user
  2. Calls `DeactivateBuffInstanceAsync`
  3. Shows success message with removed modifiers
  4. Reloads character and buff lists

#### CharacterSheet.razor.css (Updated)
- Enhanced `.buff-item` styling
- Added `.buff-info`, `.buff-name`, `.buff-timer` styles
- Hover effects for end button
- Responsive design

#### Program.cs (Updated)
- Registered `BuffInstanceService` as scoped service

## Features Implemented

### Buff Activation
- ? Activate manual buffs from the Buffs page
- ? Automatic character lookup
- ? Buff stacking support (up to MaxStacks)
- ? Apply modifiers to character stats
- ? Success/error feedback with modifier details

### Buff Deactivation
- ? View active buffs on Character Sheet
- ? End buffs manually
- ? Remove modifiers from character stats
- ? Confirmation dialog before ending
- ? Success/error feedback

### Modifier System
- ? Flat modifiers (HP, HPMax, MP, MPMax)
- ? Percentage modifiers (HP%, HPMax%, MP%, MPMax%)
- ? XP Gains percentage modifier (stored but not yet fully implemented)
- ? Stack multiplication
- ? Bounds checking (HP/MP stay within 0 - max)
- ? Reversible application/removal

### User Experience
- ? Visual distinction (green for buffs, red for debuffs)
- ? Time remaining display
- ? Stack count display
- ? Hover effects
- ? Responsive design
- ? Success/error messages
- ? Confirmation dialogs

## Database Schema

The BuffInstances collection in MongoDB stores documents with this structure:
```json
{
  "_id": ObjectId,
  "buffId": ObjectId (reference to Buff),
  "characterId": ObjectId (reference to Character),
  "buffName": string,
  "isDebuff": boolean,
  "startTime": DateTime,
  "endTime": DateTime,
  "stacks": int,
  "isActive": boolean,
  "hpModifier": int,
  "hpMaxModifier": int,
  "hpPercentModifier": int,
  "hpMaxPercentModifier": int,
  "mpModifier": int,
  "mpMaxModifier": int,
  "mpPercentModifier": int,
  "mpMaxPercentModifier": int,
  "xpGainsPercentModifier": int,
  "createdAt": DateTime,
  "updatedAt": DateTime
}
```

## Usage Workflow

### Activating a Buff
1. Navigate to `/buffs` page
2. Find a manual buff (has play button)
3. Click the **play button** to activate
4. System:
   - Finds your character
   - Checks if buff can stack
   - Creates buff instance
   - Applies modifiers to character
   - Shows success message
5. View active buff on Character Sheet

### Ending a Buff
1. Navigate to `/character` (Character Sheet)
2. See active buffs in the **Active Buffs/Debuffs** section
3. Click the **X button** on a buff
4. Confirm in dialog
5. System:
   - Removes modifiers from character
   - Deactivates buff instance
   - Shows success message
   - Refreshes displays

## Implementation Notes

### Modifier Calculation
- **Flat modifiers** are straightforward: add/subtract the value
- **Percentage modifiers** are calculated on the base value before modification
- **Stacking** multiplies the modifier by the stack count
- **Order of operations**:
  1. Calculate base values
  2. Apply flat modifiers × stacks
  3. Apply percentage modifiers × stacks
  4. Clamp to valid range

### Future Enhancements
- ?? **Automatic expiration**: Daily job to check and expire buffs
- ?? **Buff history**: Track when buffs were applied/removed
- ?? **Buff refresh**: Option to refresh/extend buff duration
- ?? **Action-triggered buffs**: Auto-apply buffs based on actions
- ?? **Buff analytics**: Track most-used buffs, effectiveness
- ?? **Expiration notifications**: Alert before buff expires
- ?? **Custom buff icons**: Per-buff custom images
- ?? **Filter active buffs**: By type, duration, etc.

## Testing

To test the implementation:
1. **Stop the running application** (hot reload won't work with these changes)
2. **Rebuild the solution**
3. **Start the application**
4. Create a character (complete a quest if needed)
5. Create a manual buff with some modifiers
6. Activate the buff from the Buffs page
7. Check Character Sheet to see active buff
8. Verify character stats were modified
9. End the buff
10. Verify character stats returned to normal

## Known Issues to Fix
- Application must be restarted for changes to take effect (hot reload limitation)
- XP Gains modifier is stored but not yet applied to XP calculations
- No automatic buff expiration (requires background job)
- No validation that character has enough HP/MP if debuffs would reduce below 0
