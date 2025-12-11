# Buff Management System - Implementation Summary

## Overview
A complete full-stack solution for managing Buffs/Debuffs in the LifeForge application has been implemented, following the same patterns as the Quest system.

## Components Created

### 1. Data Access Layer (LifeForge.DataAccess)

#### BuffEntity.cs
- MongoDB entity representation of a Buff
- Contains all properties from the domain model including:
  - Image data (base64 encoded)
  - Buff/Debuff type
  - All stat modifiers (HP, MP, XP)
  - Duration in days (integer) and stacking information
- Includes `FromDomain()` and `ToDomain()` conversion methods

#### IBuffRepository.cs & BuffRepository.cs
- Standard CRUD operations for Buffs
- MongoDB integration using the Buffs collection
- Methods: GetAllBuffsAsync, GetBuffByIdAsync, CreateBuffAsync, UpdateBuffAsync, DeleteBuffAsync

#### MongoDbSettings.cs (Updated)
- Added `BuffsCollectionName` property set to "Buffs"

### 2. API Layer (LifeForge.Api)

#### BuffDtos.cs
- `BuffDto`: Complete buff data transfer object
- `CreateBuffDto`: DTO for creating new buffs
- `UpdateBuffDto`: DTO for updating existing buffs
- All include full modifier support and DurationDays (integer)

#### BuffsController.cs
- RESTful API endpoints:
  - `GET /api/buffs` - Get all buffs
  - `GET /api/buffs/{id}` - Get specific buff
  - `POST /api/buffs` - Create new buff
  - `PUT /api/buffs/{id}` - Update buff
  - `DELETE /api/buffs/{id}` - Delete buff
  - `POST /api/buffs/upload-image` - Upload buff image
- Comprehensive error handling and logging

#### Program.cs (Updated)
- Registered `IBuffRepository` and `BuffRepository` as singleton services

### 3. Web Layer (LifeForge.Web)

#### BuffDto.cs
- Web-specific DTO for Buffs
- Includes default values for common properties (DurationDays defaults to 7)

#### BuffService.cs
- HTTP client service for communicating with the API
- Methods mirror the API controller endpoints
- Image upload support for buff icons/images

#### Buffs.razor
- Complete UI page for buff management
- **Gallery View Features:**
  - Grid layout similar to Quest gallery
  - Card-based design with image thumbnails
  - Visual distinction between Buffs (green border) and Debuffs (red border)
  - Hover effects with action buttons (Edit/Delete)
  - Modifier badges showing active stat changes
  - Duration display in days and stack count display

- **Modal Form Features:**
  - Create/Edit functionality in a modal dialog
  - Image upload with preview
  - Radio buttons for Buff/Debuff selection
  - Trigger type dropdown (Manual/Action)
  - All 9 stat modifiers editable:
    - HP, HP Max, HP %, HP Max %
    - MP, MP Max, MP %, MP Max %
    - XP Gains %
  - Duration in days (integer input)
  - Max stacks configuration
  - Full validation support

#### NavMenu.razor (Updated)
- Added navigation link to Buffs page with star icon

#### Program.cs (Updated)
- Registered `BuffService` as scoped service

### 4. Domain Layer (LifeForge.Domain)

#### Buff.cs (Updated)
- Changed `Duration` property from `TimeSpan` to `DurationDays` (int)
- Duration is now measured in whole days only
- Buffs are processed on a daily basis

#### BuffInstance.cs (Updated)
- Updated `EndTime` calculation to use `StartTime.AddDays(Buff.DurationDays)`
- Properly handles day-based duration for active buff instances

## Features Implemented

### Visual Design
- **Card Gallery**: Responsive grid layout matching Quest page style
- **Color Coding**: 
  - Green border for Buffs (positive effects)
  - Red border for Debuffs (negative effects)
- **Modifier Display**: Color-coded badges (green for positive, red for negative)
- **Image Support**: Upload and display custom buff icons
- **Placeholder Icons**: Default icons when no image is provided

### Functionality
- **CRUD Operations**: Full Create, Read, Update, Delete support
- **Image Management**: Upload images up to 5MB
- **Modifier System**: Support for all 9 stat modifiers defined in domain
- **Stacking**: Configure max stack count per buff
- **Duration**: Set buff duration in whole days (displayed as "X days")
- **Triggers**: Manual or Action-based activation
- **Validation**: Client and server-side validation

### User Experience
- **Responsive Design**: Works on desktop and mobile
- **Modal Dialogs**: Clean edit/create experience
- **Success/Error Messages**: User feedback for all operations
- **Confirmation Dialogs**: Confirm before deleting
- **Loading States**: Visual feedback during operations

## Database Schema

The Buffs collection in MongoDB will store documents with this structure:
```json
{
  "_id": ObjectId,
  "imageName": string,
  "imageData": string (base64),
  "imageContentType": string,
  "isDebuff": boolean,
  "name": string,
  "trigger": string (enum),
  "maxStacks": int,
  "description": string,
  "hpModifier": int,
  "hpMaxModifier": int,
  "hpPercentModifier": int,
  "hpMaxPercentModifier": int,
  "mpModifier": int,
  "mpMaxModifier": int,
  "mpPercentModifier": int,
  "mpMaxPercentModifier": int,
  "xpGainsPercentModifier": int,
  "durationDays": int,
  "createdAt": DateTime,
  "updatedAt": DateTime
}
```

## Duration Changes

**Important:** The buff duration system has been simplified to use whole days only:
- `Duration` property changed from `TimeSpan` to `DurationDays` (int)
- UI input changed from minutes to days
- Buffs are designed to be processed on a daily basis
- `BuffInstance.EndTime` calculates expiration using `StartTime.AddDays(DurationDays)`
- Display format shows "1 day" or "X days" (e.g., "7 days", "30 days")

This design decision reflects that buffs are intended for longer-term effects and will be processed during daily game loops rather than minute-by-minute updates.

## Usage

1. **Navigate to Buffs**: Click "Buffs" in the navigation menu
2. **Create a Buff**: Click "Create New Buff" button
3. **Configure**:
   - Set name and description
   - Choose Buff or Debuff
   - Upload an optional image
   - Set trigger type and stacking
   - Configure any stat modifiers (positive or negative numbers)
   - Set duration in days (e.g., 1, 7, 30)
4. **Edit**: Click on a buff card or the edit button
5. **Delete**: Click the trash icon and confirm

## Next Steps (Optional Enhancements)

- Add buff application to characters
- Create a buff effect calculator
- Add expiration tracking for active buffs
- Implement buff categories/tags
- Add search and filter functionality
- Create buff templates/presets
- Add buff history/logs
- Implement daily buff processing system
