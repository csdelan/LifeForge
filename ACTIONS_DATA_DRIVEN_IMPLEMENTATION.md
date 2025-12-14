# Actions System - Data-Driven Implementation

## Date: January 2025

## Overview

The Actions system has been converted from hardcoded implementation to a fully data-driven system with a complete CRUD management UI. Users can now create, edit, and delete actions through a visual interface, and actions are stored in the database.

## What Was Changed

### 1. **Domain Layer Updates**
- **File**: `LifeForge.Domain\Action.cs`
- **Changes**: Added image support fields
  - `ImageName` - Original filename
  - `ImageData` - Base64-encoded image data
  - `ImageContentType` - MIME type (e.g., image/jpeg)

### 2. **Data Access Layer Updates**
- **File**: `LifeForge.DataAccess\Models\ActionEntity.cs`
- **Changes**: Added image fields with BSON attributes for MongoDB storage

### 3. **API Layer Updates**
- **File**: `LifeForge.Api\Models\ActionDtos.cs`
- **Changes**: Added image fields to `ActionDto`

- **File**: `LifeForge.Api\Controllers\ActionsController.cs`
- **Changes**:
  - Added `HttpPut` endpoint for updating actions
  - Added `upload-image` endpoint for image uploads
  - Updated all DTOs to include image fields

### 4. **Web Services Updates**
- **File**: `LifeForge.Web\Models\ActionDto.cs`
- **Changes**: Added image support fields to match API DTOs

- **File**: `LifeForge.Web\Services\ActionService.cs`
- **Changes**:
  - Added `CreateActionAsync()` method
  - Added `UpdateActionAsync()` method
  - Added `DeleteActionAsync()` method
  - Added `UploadImageAsync()` method for image uploads

### 5. **New UI Components**

#### Actions Management Page
- **File**: `LifeForge.Web\Pages\Actions.razor` (NEW)
- **Features**:
  - Gallery view of all actions
  - Create/Edit modal dialog
  - Image upload support (drag & drop or file select)
  - Buff selection interface (checkbox list)
  - Action categories (Social, Health, Recreation, Work, Other)
  - Delete confirmation dialog
  - Execute action directly from management page
  - Real-time preview of selected buffs
  - Icon/emoji support
  - Cooldown configuration

- **Styling**: Matches existing Buffs page design
  - Card-based gallery layout
  - Hover effects with action buttons
  - Responsive grid (auto-fills based on screen size)
  - Light modal with dark gallery cards

### 6. **Character Sheet Updates**
- **File**: `LifeForge.Web\Pages\CharacterSheet.razor`
- **Changes**:
  - **Removed**: Hardcoded "Drink Alcoholic Beverage" button and method
  - **Added**: Dynamic action loading from database
  - **Added**: Generic `ExecuteAction()` method that works with any action
  - **Updated**: Actions section displays all available actions
  - **Added**: Support for action images and icons
  - **Added**: Link to Actions page when no actions exist

- **File**: `LifeForge.Web\Pages\CharacterSheet.razor.css`
- **Changes**: Added `.action-image` styling for displaying action images

### 7. **Navigation Updates**
- **File**: `LifeForge.Web\Layout\NavMenu.razor`
- **Changes**: Added "Actions" menu item with lightning bolt icon

## New Features

### Action Management UI
1. **Create Actions**:
   - Name and description
   - Icon (emoji) or image upload
   - Category selection
   - Multiple buff selection
   - Cooldown configuration

2. **Edit Actions**:
   - Update any field
   - Change buff associations
   - Replace or remove image

3. **Delete Actions**:
   - Confirmation dialog
   - Removes from database

4. **Execute Actions**:
   - Can be executed from Actions page or Character Sheet
   - Activates all associated buffs
   - Shows success message with buff names

### Buff Selection Interface
- Checkbox list of all available buffs
- Visual distinction between buffs and debuffs
- Shows selected count
- Easy toggle on/off

### Image Support
- Upload images for actions (max 5MB)
- Automatic Base64 encoding
- Preview in create/edit modal
- Display in action cards
- Falls back to emoji icons if no image

## API Endpoints

### New/Updated Endpoints

```http
PUT /api/actions/{id}
Content-Type: application/json
```
Updates an existing action.

```http
POST /api/actions/upload-image
Content-Type: multipart/form-data
```
Uploads an image and returns Base64-encoded data.

## Database Schema

### Actions Collection (Updated)
```json
{
  "_id": "ObjectId",
  "name": "Drink Alcoholic Beverage",
  "description": "Drink an alcoholic drink...",
  "icon": "??",
  "imageName": "beer.jpg",
  "imageData": "base64-encoded-string...",
  "imageContentType": "image/jpeg",
  "buffIds": ["buff-id-1", "buff-id-2"],
  "category": "Social",
  "cooldownHours": 0,
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

## Migration Guide

### For Existing Actions

If you have hardcoded actions (like the "Drink Alcoholic Beverage" example), you need to:

1. **Navigate to `/actions` page**
2. **Click "Create New Action"**
3. **Fill in details**:
   - Name: "Drink Alcoholic Beverage"
   - Description: "Drink an alcoholic drink, resulting in hangover and detox effects"
   - Icon: ??
   - Category: Social
   - Buffs: Select "Hangover" and "Alcohol Detoxing"
4. **Save**

The action will now appear on the Character Sheet automatically!

### Required Buffs

Make sure these buffs exist (from the previous implementation):
- **Hangover** (Debuff)
- **Alcohol Detoxing** (Buff)

Create them in `/buffs` if they don't exist yet.

## How to Use

### Creating an Action

1. Navigate to **Actions** (/actions)
2. Click "**Create New Action**"
3. Fill in the form:
   - **Name**: What the action is called
   - **Description**: What it does
   - **Icon**: An emoji (e.g., ??, ?, ??)
   - **Image** (optional): Upload a custom image
   - **Category**: Social, Health, Recreation, Work, or Other
   - **Cooldown**: Hours before can be used again (0 = no cooldown)
   - **Buffs**: Check which buffs to activate
4. Click "**Create Action**"

### Using an Action

**From Character Sheet:**
1. Go to Character Sheet (/)
2. Scroll to "Common Actions"
3. Click "**Execute**" on any action
4. Buffs activate immediately

**From Actions Page:**
1. Go to Actions (/actions)
2. Hover over an action card
3. Click "**Execute**" button
4. Buffs activate on your character

### Editing an Action

1. Navigate to Actions (/actions)
2. Click on an action card (or click the "Edit" button)
3. Modify any fields
4. Click "**Update Action**"

### Deleting an Action

1. Navigate to Actions (/actions)
2. Hover over an action card
3. Click the "**Delete**" button (trash icon)
4. Confirm deletion

## User Workflow Example

### Example: Creating a "Drink Coffee" Action

1. **Create Buffs** (if they don't exist):
   - Navigate to `/buffs`
   - Create "Caffeine Boost" (Buff)
     - HP: +10
     - MP: +20
     - Duration: 4 hours
   - Create "Caffeine Crash" (Debuff)
     - HP: -5
     - MP: -10
     - Duration: 2 hours

2. **Create Action**:
   - Navigate to `/actions`
   - Click "Create New Action"
   - Name: "Drink Coffee"
   - Description: "Drink a cup of coffee for a temporary boost"
   - Icon: ?
   - Category: Health
   - Cooldown: 4 hours
   - Buffs: Check both "Caffeine Boost" and "Caffeine Crash"
   - Click "Create Action"

3. **Use Action**:
   - Go to Character Sheet (/)
   - Find "Drink Coffee" in Common Actions
   - Click "Execute"
   - Both buffs activate on your character!

## Benefits of Data-Driven Approach

### Before (Hardcoded)
- ? Had to modify code for every new action
- ? Required developer knowledge
- ? Needed redeployment for changes
- ? Hard to maintain and extend
- ? No user customization

### After (Data-Driven)
- ? Users create actions through UI
- ? No code changes needed
- ? Easy to add/modify/remove actions
- ? Fully customizable
- ? Extensible and maintainable
- ? Supports images and icons
- ? Buff associations managed visually

## Technical Architecture

### Data Flow

```
User Creates Action (UI)
    ?
ActionService.CreateActionAsync()
    ?
POST /api/actions
    ?
ActionsController.CreateAction()
    ?
ActionRepository.CreateActionAsync()
    ?
MongoDB (Actions Collection)
```

```
User Executes Action (UI)
    ?
ActionService.PerformActionAsync()
    ?
POST /api/actions/perform
    ?
ActionsController.PerformAction()
    ?
For each BuffId:
    BuffInstanceService.ActivateBuffAsync()
    ?
Character gets buffs applied
```

### Image Handling

```
User uploads image
    ?
ActionService.UploadImageAsync()
    ?
POST /api/actions/upload-image
    ?
File ? MemoryStream ? Base64 String
    ?
Returns { fileName, imageData, contentType }
    ?
Stored in ActionDto
    ?
Saved to MongoDB
```

## Testing Checklist

- [ ] **Create Action**
  - [ ] With icon only
  - [ ] With image only
  - [ ] With multiple buffs
  - [ ] With no buffs (should warn/prevent?)
  - [ ] With cooldown

- [ ] **Edit Action**
  - [ ] Change name
  - [ ] Change buffs
  - [ ] Add/remove image
  - [ ] Change category

- [ ] **Delete Action**
  - [ ] Confirm deletion works
  - [ ] Action removed from database
  - [ ] Action removed from Character Sheet

- [ ] **Execute Action**
  - [ ] From Character Sheet
  - [ ] From Actions page
  - [ ] Verify buffs activate
  - [ ] Verify success message

- [ ] **UI/UX**
  - [ ] Cards display properly
  - [ ] Hover effects work
  - [ ] Modal opens/closes
  - [ ] Image upload works
  - [ ] Buff selection works
  - [ ] Responsive on mobile

## Future Enhancements (Optional)

### Cooldown System
- [ ] Track when actions were last performed
- [ ] Disable actions still on cooldown
- [ ] Show cooldown timer

### Action History
- [ ] Log when actions are performed
- [ ] Show action history on Character Sheet
- [ ] Statistics (most used actions, etc.)

### Conditional Actions
- [ ] Require certain stats/buffs to use
- [ ] Only available at certain times
- [ ] Only available in certain locations

### Action Costs
- [ ] Consume currency to use
- [ ] Consume items
- [ ] Consume HP/MP

### Advanced UI
- [ ] Drag & drop to reorder
- [ ] Favorites/pins
- [ ] Search and filter
- [ ] Categories filter on Character Sheet

## Breaking Changes

**None**. This is a non-breaking enhancement. The system is backward-compatible:
- Existing hardcoded actions can be migrated to database
- API contracts remain compatible
- No database schema changes to existing collections

## Performance Considerations

- **Image Storage**: Base64 encoding increases storage size by ~33%. Consider switching to blob storage (Azure Blob Storage, AWS S3) for production.
- **Image Size Limit**: Currently set to 5MB per image
- **Query Performance**: Actions collection is small (<100 typical), no indexing needed yet
- **Client-Side**: Images are cached in browser after first load

## Security Considerations

- **Image Upload**: Validates file type and size
- **Input Validation**: All fields validated on client and server
- **Authentication**: Add authentication before production (not currently implemented)
- **Authorization**: Consider who can create/edit/delete actions

## Deployment Notes

### Before Deploying
1. Test image uploads thoroughly
2. Verify all existing actions are migrated to database
3. Remove or comment out hardcoded action examples
4. Set up image storage if using external blob storage
5. Configure file size limits if needed

### After Deploying
1. Migrate any hardcoded actions to database via UI
2. Test action execution end-to-end
3. Monitor MongoDB for action data
4. Verify image display works correctly
5. Check responsive design on different devices

## Troubleshooting

### "No actions available" on Character Sheet
- Navigate to `/actions` and create some actions
- Make sure actions have buffs associated
- Check browser console for errors

### Image not displaying
- Verify image was uploaded successfully
- Check ContentType is correct (image/jpeg, image/png, etc.)
- Verify imageData is not empty
- Check browser console for Base64 errors

### Buffs not activating
- Verify buffs exist in `/buffs`
- Check buff IDs are correctly associated with action
- Verify character exists
- Check API logs for errors

### Action not appearing after creation
- Reload the page
- Check MongoDB to verify action was saved
- Check browser console for errors
- Verify API returned success

## Files Changed Summary

| File | Type | Changes |
|------|------|---------|
| `LifeForge.Domain\Action.cs` | Modified | Added image support fields |
| `LifeForge.DataAccess\Models\ActionEntity.cs` | Modified | Added image fields with BSON attributes |
| `LifeForge.Api\Models\ActionDtos.cs` | Modified | Added image fields to DTOs |
| `LifeForge.Api\Controllers\ActionsController.cs` | Modified | Added Update and UploadImage endpoints |
| `LifeForge.Web\Models\ActionDto.cs` | Modified | Added image support fields |
| `LifeForge.Web\Services\ActionService.cs` | Modified | Added Create, Update, Delete, UploadImage methods |
| `LifeForge.Web\Pages\Actions.razor` | **NEW** | Complete Actions management UI |
| `LifeForge.Web\Pages\CharacterSheet.razor` | Modified | Dynamic action loading, removed hardcoding |
| `LifeForge.Web\Pages\CharacterSheet.razor.css` | Modified | Added action-image styling |
| `LifeForge.Web\Layout\NavMenu.razor` | Modified | Added Actions menu item |

## Build Status

? **Build Successful**
- No compilation errors
- All new files added
- All changes integrated
- Ready for testing

## Summary

The Actions system is now fully data-driven! Users can create, edit, and delete actions through an intuitive UI without touching code. Actions support images, multiple buffs, categories, and cooldowns. The Character Sheet dynamically displays all available actions and executes them with a single click.

**Key Achievement**: Converted from hardcoded implementation to database-driven system with complete CRUD UI in a single session!

