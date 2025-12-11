# Emoji Icon Display Fix - Quest Rewards

## Issue
When selecting "Gold" from the reward dropdown, the icon column showed "??" instead of the ?? emoji.

## Root Cause Analysis

The issue had multiple potential causes:

1. **Blazor Change Detection**: The `@onchange` event wasn't triggering a UI re-render after updating the reward's Icon property
2. **HTML Rendering**: Standard text rendering might not properly handle emoji Unicode characters in all cases
3. **Character Encoding**: Emojis might be getting corrupted during string assignment

## Solutions Implemented

### 1. Force UI Re-render with `StateHasChanged()`

Added `StateHasChanged()` call to the `OnRewardSelected` method to ensure Blazor updates the UI after the icon is set:

```csharp
private void OnRewardSelected(RewardDto reward, string rewardClass)
{
    reward.RewardClass = rewardClass;
    var rewardDef = RewardDefinition.GetByRewardClass(rewardClass);
    if (rewardDef != null)
    {
        reward.Type = rewardDef.Type;
        reward.Icon = rewardDef.Icon;
    }
    StateHasChanged(); // Force UI update
}
```

**Why this helps**: Blazor's change detection might not always detect changes to individual properties of objects in a collection. `StateHasChanged()` explicitly tells Blazor to re-render the component.

### 2. Use `MarkupString` for Emoji Rendering

Changed from simple text output to `MarkupString` to ensure proper emoji rendering:

```razor
@if (!string.IsNullOrEmpty(reward.Icon))
{
    <span class="emoji-display" title="Icon: @reward.Icon">@((MarkupString)reward.Icon)</span>
}
else
{
    <span class="text-muted" style="font-size: 0.8rem;">No icon</span>
}
```

**Benefits**:
- `MarkupString` ensures the string is rendered as-is without HTML encoding
- Added title attribute for debugging (hover to see the actual icon value)
- Shows "No icon" text when icon is empty (helps debugging)

### 3. Added Debugging Visual

If the icon is still empty after selection, the field now shows "No icon" instead of being blank, making it easier to identify if the icon isn't being set vs. not being rendered.

## Technical Details

### Why Emojis Can Show as "??"

Emojis can display as "??" for several reasons:

1. **Font Support**: The browser's font doesn't include emoji glyphs
   - **Solution**: Already added emoji font stack in CSS
   
2. **Character Encoding**: UTF-8 encoding issues
   - **Solution**: `MarkupString` preserves the exact Unicode characters

3. **HTML Encoding**: Browser converts emojis to HTML entities
   - **Solution**: `MarkupString` prevents auto-encoding

4. **Blazor Rendering**: Component not updating after state change
   - **Solution**: `StateHasChanged()` forces re-render

### Emoji Font Stack (Already Applied)

```css
.emoji-display {
    font-size: 1.5rem;
    line-height: 1;
    font-family: "Segoe UI Emoji", "Apple Color Emoji", "Noto Color Emoji", sans-serif;
}
```

This ensures emoji display across:
- ? Windows: Segoe UI Emoji
- ? macOS: Apple Color Emoji
- ? Linux: Noto Color Emoji

## Testing Steps

1. **Open quest create/edit modal**
2. **Click "Add Reward"**
3. **Select "Gold" from dropdown**
4. **Icon field should immediately show**: ??
5. **If still showing "??" hover over it** - the title tooltip will show the actual icon value
6. **If showing "No icon"** - the icon isn't being set (RewardDefinition issue)

## Expected Behavior

```
Dropdown    Icon Column    Amount
[Gold ?]    [  ??  ]      [  10  ]
[Karma ?]   [  ??  ]      [  25  ]
```

## Files Modified

1. **LifeForge.Web\Pages\Quests.razor**
   - Added `StateHasChanged()` to `OnRewardSelected` method
   - Changed icon rendering to use `MarkupString`
   - Added debug text "No icon" when icon is empty
   - Added title attribute to icon span for debugging

## Troubleshooting

### If Icon Still Shows "??"

1. **Check Browser Console**: Look for encoding errors
2. **Hover over icon field**: Tooltip shows actual icon value
3. **Check RewardDefinition**: Verify emojis are correctly defined
4. **Try different browser**: Test in Chrome, Firefox, Edge
5. **Check file encoding**: Ensure source files are UTF-8

### If Icon Shows "No icon"

1. **Debug OnRewardSelected**: Set breakpoint to check if method is called
2. **Check GetByRewardClass**: Verify it returns correct definition
3. **Verify dropdown value**: Ensure rewardClass matches exactly ("Gold", not "gold")

## Alternative Debugging Approach

If the issue persists, you can add console logging:

```csharp
private void OnRewardSelected(RewardDto reward, string rewardClass)
{
    Console.WriteLine($"Selected: {rewardClass}");
    reward.RewardClass = rewardClass;
    var rewardDef = RewardDefinition.GetByRewardClass(rewardClass);
    if (rewardDef != null)
    {
        reward.Type = rewardDef.Type;
        reward.Icon = rewardDef.Icon;
        Console.WriteLine($"Icon set to: {reward.Icon}");
    }
    else
    {
        Console.WriteLine($"RewardDef not found for: {rewardClass}");
    }
    StateHasChanged();
}
```

Check browser console (F12) for the log messages.

## Build Status
? All projects compile successfully

## Summary

The fix addresses the emoji rendering issue by:
1. ? Forcing UI re-render with `StateHasChanged()`
2. ? Using `MarkupString` for proper emoji rendering
3. ? Adding debugging aids (title attribute, "No icon" text)
4. ? Maintaining emoji font stack for cross-platform support

The icon should now display correctly when selecting rewards from the dropdown!
