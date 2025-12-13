# Actions System Implementation

## Overview

The Actions system provides a way to define common actions that characters can perform, which automatically activate one or more buffs. This creates a streamlined way to handle complex interactions without requiring manual buff activation.

## What Was Implemented

### 1. Domain Layer
- **`Action.cs`**: Core domain model representing an action with:
  - Name and Description
  - Icon (emoji or image)
  - List of BuffIds to activate
  - Category (Social, Health, Recreation, Work, Other)
  - Cooldown (in hours)

### 2. Data Access Layer
- **`ActionEntity.cs`**: MongoDB entity with BSON mapping
- **`IActionRepository.cs`** & **`ActionRepository.cs`**: Full CRUD operations
- Updated `MongoDbSettings` to include `ActionsCollectionName`

### 3. API Layer
- **`ActionDtos.cs`**: API DTOs for actions
- **`ActionsController.cs`**: RESTful API with endpoints:
  - `GET /api/actions` - Get all actions
  - `GET /api/actions/{id}` - Get specific action
  - `POST /api/actions` - Create new action
  - `POST /api/actions/perform` - Perform an action (activates its buffs)
  - `DELETE /api/actions/{id}` - Delete an action

### 4. Web Layer
- **`ActionDto.cs`**: Client DTOs
- **`ActionService.cs`**: Service for API calls
- Updated `CharacterSheet.razor` with:
  - **Common Actions section** at the bottom of the character sheet
  - **Drink Alcoholic Beverage button** that activates Hangover and Alcohol Detoxing buffs
- Added CSS styling for the actions section

### 5. Configuration
- Updated `appsettings.json` with ActionsCollectionName
- Registered repositories and services in DI containers

## Required Buffs

For the "Drink Alcoholic Beverage" action to work, you need to create two buffs:

### 1. Hangover (Debuff)
```
Name: Hangover
Type: Debuff
Description: Your head hurts and you feel terrible from drinking too much
Duration: 1 day
Modifiers:
- HP: -10
- HP Max: -20
- Focus: -5 (if implemented)
```

### 2. Alcohol Detoxing (Buff)
```
Name: Alcohol Detoxing
Type: Buff
Description: Your body is processing and recovering from alcohol
Duration: 2 days
Modifiers:
- HP: +5 (gradual recovery)
- MP: -10 (energy spent on recovery)
```

## How to Use

### 1. Create the Required Buffs

1. Navigate to `/buffs`
2. Click "Create New Buff"
3. Create the "Hangover" buff with negative modifiers
4. Create the "Alcohol Detoxing" buff with recovery modifiers

### 2. Use the Action

1. Navigate to `/character` (Character Sheet)
2. Scroll to the "Common Actions" section at the bottom
3. Click the "?? Drink Alcoholic Beverage" button
4. Both buffs will be activated on your character
5. Check your active buffs section to see the effects

## API Endpoints

### Perform an Action
```http
POST https://localhost:7001/api/actions/perform
Content-Type: application/json

{
  "characterId": "your-character-id",
  "actionId": "action-id"
}
```

### Create a New Action
```http
POST https://localhost:7001/api/actions
Content-Type: application/json

{
  "name": "Drink Alcoholic Beverage",
  "description": "Drink an alcoholic drink, resulting in hangover and detox effects",
  "icon": "??",
  "buffIds": ["hangover-buff-id", "detox-buff-id"],
  "category": "Social",
  "cooldownHours": 0
}
```

## Future Enhancements

### Planned Features
- [ ] **Action Cooldowns**: Track when actions were last performed
- [ ] **Action History**: Log of performed actions
- [ ] **Conditional Actions**: Actions that require certain conditions
- [ ] **Custom Actions**: Let users create their own actions
- [ ] **Action Categories**: Filter actions by category
- [ ] **Action Prerequisites**: Require certain stats or buffs
- [ ] **Visual Improvements**: Action cards with images
- [ ] **Action Management UI**: Full CRUD interface for actions

### Additional Action Ideas
- **? Drink Coffee** - Activates "Caffeine Boost" and "Caffeine Crash" buffs
- **?? Go for a Run** - Activates "Exercise High" and "Muscle Fatigue" buffs
- **?? Pull an All-Nighter** - Activates "Sleep Deprived" debuff
- **?? Gaming Session** - Activates "Entertainment Boost" and "Eye Strain" buffs
- **?? Study Session** - Activates "Knowledge Gain" and "Mental Fatigue" buffs
- **?? Eat Junk Food** - Activates "Comfort Food" and "Unhealthy Eating" buffs

## Architecture

```
Character Sheet
    ?
Common Actions Section
    ?
[Action Button] ? ActionService.PerformActionAsync()
    ?
API: POST /api/actions/perform
    ?
ActionsController.PerformAction()
    ?
BuffInstanceApplicationService.ActivateBuffAsync() (for each buff)
    ?
Multiple Buffs Activated on Character
```

## Implementation Notes

1. **Current Approach**: The drinking action currently looks up buffs by name at runtime
2. **Better Approach** (for production): 
   - Create Actions in the database with specific BuffIds
   - Use the `POST /api/actions/perform` endpoint
   - This provides better flexibility and configurability

3. **Buff Dependencies**: Actions depend on buffs existing in the system
4. **Error Handling**: If a buff doesn't exist, the action will fail gracefully with an error message

## Testing

### Test the Drinking Action

1. **Setup**:
   ```bash
   # Start API
   cd LifeForge.Api
   dotnet run
   
   # Start Web (in another terminal)
   cd LifeForge.Web
   dotnet run
   ```

2. **Create Buffs**:
   - Navigate to https://localhost:7295/buffs
   - Create "Hangover" buff (debuff)
   - Create "Alcohol Detoxing" buff (buff)

3. **Perform Action**:
   - Navigate to https://localhost:7295/character
   - Click "?? Drink Alcoholic Beverage"
   - Verify both buffs appear in Active Buffs section

4. **Check Effects**:
   - Verify HP and MP changes
   - Check buff timers
   - Test stacking behavior

## Database Collections

### Actions Collection
```json
{
  "_id": "ObjectId",
  "name": "Drink Alcoholic Beverage",
  "description": "Drink an alcoholic drink...",
  "icon": "??",
  "buffIds": ["buff-id-1", "buff-id-2"],
  "category": "Social",
  "cooldownHours": 0,
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

## Summary

? **Action System**: Complete domain model with MongoDB persistence  
? **API Endpoints**: Full CRUD + perform action endpoint  
? **Web UI**: Common Actions section on Character Sheet  
? **Drink Action**: Button that activates Hangover and Alcohol Detoxing buffs  
? **Error Handling**: Graceful failures with user feedback  
? **Styling**: Attractive gradient button with hover effects  
? **Documentation**: Complete guide for using and extending the system  

The Actions system is now ready to use and can be easily extended with new actions!
