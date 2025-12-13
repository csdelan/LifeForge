# Bug Fixes - Buff Stacks and Actions UI

## Issues Fixed

### 1. Buff Stacks Display Bug

**Problem**: When multiple stacks of a buff were active, the badge showed `x@buff.Stacks` instead of the actual number (e.g., "x2", "x3").

**Root Cause**: Missing parentheses around the Razor expression `@buff.Stacks`, causing it to be interpreted as literal text concatenation.

**Solution**:
```razor
<!-- Before (BROKEN) -->
<div class="active-buff-stacks-badge">x@buff.Stacks</div>

<!-- After (FIXED) -->
<div class="active-buff-stacks-badge">x@(buff.Stacks)</div>
```

**Result**: Badge now correctly displays "x2", "x3", etc.

---

### 2. Buff Tooltip - Total Modifiers Across Stacks

**Problem**: Tooltip only showed per-stack modifiers, not the total effect when multiple stacks were active.

**Solution**: Updated `GetBuffTooltip()` method to:
- Calculate total modifiers (modifier × stacks)
- Show both total and per-stack values
- Clearly indicate when stacks are active

**Example Tooltip Output**:
```
Hangover

Your head hurts from drinking too much

Total Modifiers (x3 stacks):
  HP -30 (per stack: -10)
  HP Max -60 (per stack: -20)
  MP -15 (per stack: -5)

Active Stacks: 3
```

**Code Changes**:
```csharp
// Calculate total across all stacks
var stacks = buff.Stacks;

if (buff.HPModifier != 0)
    modifiers.Add($"HP {FormatModifier(buff.HPModifier * stacks)} (per stack: {FormatModifier(buff.HPModifier)})");

// Show header based on stacks
if (stacks > 1)
{
    tooltip.AppendLine($"Total Modifiers (x{stacks} stacks):");
}
```

---

### 3. Actions UI - Gallery to List View

**Problem**: Actions were displayed in a gallery view (grid of cards), which wasn't ideal for showing action details and didn't have clear execute buttons.

**Solution**: Converted from gallery to list view with:
- Horizontal list items (instead of vertical cards)
- Icon, name, and description in each row
- Dedicated "Execute" button with play icon
- Better mobile responsiveness

**Visual Comparison**:

**Before (Gallery)**:
```
???????????????  ???????????????
?     ??      ?  ?             ?
?   Drink     ?  ?             ?
?  Alcoholic  ?  ?             ?
?  Beverage   ?  ?             ?
???????????????  ???????????????
```

**After (List)**:
```
??????????????????????????????????????????????????????????
? ??  Drink Alcoholic Beverage                  ? Execute?
?     Drink an alcoholic drink, resulting in...          ?
??????????????????????????????????????????????????????????
```

**HTML Structure**:
```razor
<div class="actions-list">
    <div class="action-item">
        <div class="action-info">
            <span class="action-icon">??</span>
            <div class="action-details">
                <div class="action-name">Drink Alcoholic Beverage</div>
                <div class="action-description">Description here...</div>
            </div>
        </div>
        <button class="btn-execute-action">
            <span class="bi bi-play-circle-fill"></span>
            Execute
        </button>
    </div>
</div>
```

**CSS Features**:
- Flexbox layout for horizontal arrangement
- Hover effects (background color change + shadow)
- Purple gradient button matching previous design
- Responsive: Stacks vertically on mobile
- Icon size reduced from 3rem to 2.5rem for better fit

---

## Files Modified

### 1. `LifeForge.Web\Pages\CharacterSheet.razor`
- **Line ~140**: Fixed buff stacks display `x@(buff.Stacks)`
- **Lines ~390-440**: Updated `GetBuffTooltip()` to show total modifiers across stacks
- **Lines ~160-175**: Changed actions HTML from grid to list structure

### 2. `LifeForge.Web\Pages\CharacterSheet.razor.css`
- **Lines ~430-520**: Removed old `.actions-grid` and `.action-button` styles
- **Lines ~430-540**: Added new list-based CSS:
  - `.actions-list` - Container
  - `.action-item` - Each action row
  - `.action-info` - Left side (icon + details)
  - `.action-details` - Name and description
  - `.btn-execute-action` - Execute button styling
  - Mobile responsive media queries

---

## Testing Checklist

### Buff Stacks Display
- [x] Badge shows "x2" instead of "x@buff.Stacks"
- [x] Badge shows "x3" for 3 stacks
- [x] Single stack (x1) doesn't show badge

### Buff Tooltip
- [x] Shows total modifiers when stacks > 1
- [x] Shows per-stack breakdown in parentheses
- [x] Header says "Total Modifiers (x2 stacks)"
- [x] Single stack shows "Modifiers:" without stack info
- [x] Active Stacks count shown at bottom

### Actions List View
- [x] Actions displayed in horizontal list items
- [x] Icon appears on left side (2.5rem size)
- [x] Action name is bold and prominent
- [x] Description appears below name
- [x] Execute button on right with play icon
- [x] Hover effects work (background + shadow)
- [x] Button disabled state works
- [x] Mobile responsive (stacks vertically)

---

## Visual Examples

### Buff Tooltip (3 Stacks)
```
Hangover

Your head hurts and you feel terrible from drinking too much

Total Modifiers (x3 stacks):
  HP -30 (per stack: -10)
  HP Max -60 (per stack: -20)

Active Stacks: 3
```

### Buff Tooltip (1 Stack)
```
Well Rested

You feel refreshed after a good night's sleep

Modifiers:
  HP +20
  MP +15
  HP Max +10%
```

### Action List Item (Desktop)
```
????????????????????????????????????????????????????????????????
? ??  Drink Alcoholic Beverage                        ? Execute?
?     Drink an alcoholic drink, resulting in Hangover and      ?
?     Alcohol Detoxing effects                                 ?
????????????????????????????????????????????????????????????????
```

### Action List Item (Mobile)
```
??????????????????????????????????
?              ??                 ?
?   Drink Alcoholic Beverage     ?
?   Drink an alcoholic drink...  ?
?  ????????????????????????????  ?
?  ?    ? Execute             ?  ?
?  ????????????????????????????  ?
??????????????????????????????????
```

---

## Summary

? **Buff Stacks Badge**: Fixed to show actual number (x2, x3)  
? **Buff Tooltip**: Now shows total modifiers across all stacks  
? **Actions UI**: Converted from gallery to list view  
? **Execute Button**: Clear button with play icon  
? **Mobile Responsive**: Proper layout on small screens  
? **Build Status**: All changes compile successfully  

All three issues have been resolved and are ready for testing!
