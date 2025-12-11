# UI Improvements - Quest Rewards & Active Quests

## Overview
Improved the quest rewards configuration UI and fixed readability issues in the Active Quests page.

## Changes Made

### 1. Fixed Active Quests Table Readability

**Problem**: White text on white background made the table unreadable.

**Solution**: Updated CSS with proper dark theme colors:
- Table headers: `#ffffff` text on `#1a1a1a` background
- Table body: `#e8e8e8` text on `#2d2d2d` background
- Strong text: `#ffffff` for emphasis
- Badges: Explicit white text color
- Buttons: Proper contrast colors
- Hover states: `#353535` background

**File Modified**: `LifeForge.Web\Pages\ActiveQuests.razor`

### 2. Simplified Reward Configuration UI

**Problem**: 
- Redundant "Type" dropdown (Experience/Currency)
- Had to manually enter reward class names
- Had to copy/paste emoji icons
- Confusing multi-step process

**Solution**: Created unified reward system with fixed icons:

#### New RewardDefinition System
Created `LifeForge.Web\Models\RewardDefinition.cs`:
- Central registry of available rewards
- Each reward has: Name, Icon, Type, RewardClass
- Fixed icons: ?? Gold, ?? Karma, ?? Design Workslot
- Easy to extend with character classes later

#### Simplified UI
- **Single dropdown** with all reward options
- Shows icon + name in dropdown: "?? Gold"
- Icon automatically set when reward selected
- Icon displayed in read-only field
- Removed manual emoji input
- Grid layout: `[Dropdown] [Icon] [Amount] [Delete]`

**Files Modified**:
- `LifeForge.Web\Pages\Quests.razor` - UI and logic
- `LifeForge.Domain\Reward.cs` - Icon type changed to string

### 3. Domain Model Updates

**Change**: `Reward.Icon` changed from `char` to `string`

**Reason**:
- Emoji characters can be compound (multi-byte)
- String is more flexible for future extensions
- Consistent with database storage

**Files Modified**:
- `LifeForge.Domain\Reward.cs`
- `LifeForge.DataAccess\Models\QuestEntity.cs`

## New UI Flow

### Adding Rewards to Quest

**Before**:
```
1. Click "Add Reward"
2. Select "Currency" from Type dropdown
3. Type "Gold" in Class field
4. Copy/paste ?? in Icon field
5. Enter amount
```

**After**:
```
1. Click "Add Reward"
2. Select "?? Gold" from dropdown (icon auto-set)
3. Enter amount
4. Done!
```

### Reward Configuration Layout

```
???????????????????????????????????????????????????????
? Rewards                                             ?
? ?????????????????????????????????????????????????  ?
? ? [?? Gold ?]      [??]    [50]    [???]        ?  ?
? ? [?? Karma ?]     [??]    [25]    [???]        ?  ?
? ? [?? Design... ?]  [??]    [1]     [???]        ?  ?
? ?                                                ?  ?
? ? [+ Add Reward]                                 ?  ?
? ?????????????????????????????????????????????????  ?
???????????????????????????????????????????????????????
```

## Available Rewards

Current rewards with fixed icons:

| Reward | Icon | Type | RewardClass |
|--------|------|------|-------------|
| Gold | ?? | Currency | Gold |
| Karma | ?? | Currency | Karma |
| Design Workslot | ?? | Currency | DesignWorkslot |

## Future Extensibility

### Adding Character Classes

When character classes are implemented, extend `RewardDefinition.GetAvailableRewards()`:

```csharp
public static List<RewardDefinition> GetAvailableRewards()
{
    var rewards = new List<RewardDefinition>
    {
        // Currencies
        new RewardDefinition { Name = "Gold", Icon = "??", Type = RewardType.Currency, RewardClass = "Gold" },
        new RewardDefinition { Name = "Karma", Icon = "??", Type = RewardType.Currency, RewardClass = "Karma" },
        new RewardDefinition { Name = "Design Workslot", Icon = "??", Type = RewardType.Currency, RewardClass = "DesignWorkslot" }
    };

    // TODO: Add character classes dynamically
    // var characterClasses = GetCharacterClasses();
    // foreach (var charClass in characterClasses)
    // {
    //     rewards.Add(new RewardDefinition 
    //     { 
    //         Name = $"{charClass.Name} XP", 
    //         Icon = "?", 
    //         Type = RewardType.Experience, 
    //         RewardClass = charClass.Name 
    //     });
    // }

    return rewards;
}
```

### Adding New Currency

1. Add to `CurrencyType` enum
2. Add entry to `RewardDefinition.GetAvailableRewards()`
3. Choose appropriate emoji icon
4. Done! Automatically appears in dropdown

## Technical Details

### Icon Storage
- **Database**: Stored as `string` in MongoDB
- **Domain**: `string Icon` property
- **UI**: Displayed as emoji characters (UTF-8)

### Reward Class Mapping
```csharp
private void UpdateRewardFromClass(RewardDto reward)
{
    var rewardDef = RewardDefinition.GetByName(reward.RewardClass);
    if (rewardDef != null)
    {
        reward.Type = rewardDef.Type;
        reward.Icon = rewardDef.Icon;
        reward.RewardClass = rewardDef.RewardClass;
    }
}
```

When user selects from dropdown, `@bind:after` triggers this method to:
1. Look up reward definition
2. Set Type automatically
3. Set Icon automatically
4. Ensure RewardClass is correct

## CSS Grid Layout

```css
.reward-item-config {
    display: grid;
    grid-template-columns: 2fr 60px 1fr 40px;
    gap: 0.5rem;
    margin-bottom: 0.5rem;
    align-items: center;
}
```

**Columns**:
1. Dropdown (2fr) - Takes most space
2. Icon display (60px) - Fixed width for emoji
3. Amount input (1fr) - Flexible
4. Delete button (40px) - Fixed width

## Color Scheme - Active Quests Table

### Dark Theme
- **Background**: `#2d2d2d` (table body), `#1a1a1a` (header)
- **Text**: `#e8e8e8` (normal), `#ffffff` (emphasis)
- **Borders**: `#3a3a3a`
- **Hover**: `#353535`

### Badges
- **Primary** (In Progress): `#0d6efd`
- **Success** (Completed): `#198754`
- **Danger** (Failed): `#dc3545`
- **Secondary** (Not Started): `#6c757d`

### Reward Badges
- **XP**: Orange gradient `#f39c12` ? `#e67e22`
- **Currency**: Yellow gradient `#f1c40f` ? `#f39c12`
- **Items**: Purple gradient `#9b59b6` ? `#8e44ad`

## Testing Checklist

### Reward Configuration
- [ ] Click "Add Reward" creates empty reward
- [ ] Dropdown shows all rewards with icons
- [ ] Selecting reward automatically sets icon
- [ ] Icon displays in read-only field
- [ ] Amount can be changed
- [ ] Delete button removes reward
- [ ] Quest saves with correct reward data

### Active Quests Display
- [ ] Table header text is readable (white on dark)
- [ ] Table body text is readable (light gray on dark)
- [ ] Quest names are bold and bright white
- [ ] Badges have proper colors and contrast
- [ ] Reward badges show emoji icons
- [ ] Buttons have good contrast
- [ ] Hover states are visible

## Files Modified

1. `LifeForge.Web\Models\RewardDefinition.cs` - **Created**
2. `LifeForge.Web\Pages\Quests.razor` - Simplified reward UI
3. `LifeForge.Web\Pages\ActiveQuests.razor` - Fixed colors
4. `LifeForge.Domain\Reward.cs` - Icon type changed to string
5. `LifeForge.DataAccess\Models\QuestEntity.cs` - Updated Icon conversion

## Build Status
? All projects compile successfully

## Summary

### What Was Fixed
1. ? Active Quests table now readable with proper dark theme
2. ? Reward configuration simplified to single dropdown
3. ? Icons automatically assigned based on reward selection
4. ? No manual emoji copy/paste needed
5. ? Cleaner, more intuitive UI

### Benefits
- **Faster**: 3 steps reduced to 2
- **Easier**: No manual icon entry
- **Consistent**: Fixed icons prevent mistakes
- **Extensible**: Easy to add new rewards
- **Professional**: Better UX and visual polish
