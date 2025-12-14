# Bug Fixes: Character Sheet Display Issues

## Date: January 2025

## Bugs Fixed

### Bug #1: `#if DEBUG` Directive Showing in UI ??? (Updated Fix)

**Problem:**
The `#if DEBUG #endif` preprocessor directives were appearing as literal text in the Blazor WebAssembly UI instead of being processed.

**Root Cause:**
Blazor WebAssembly runs in the browser (client-side), so C# preprocessor directives like `#if DEBUG` don't work the same way as in server-side code. The Razor compiler treats them as text rather than compilation directives.

**Initial Solution (Didn't Work):**
Attempted to use `IConfiguration` to check `ASPNETCORE_ENVIRONMENT`, but Blazor WASM doesn't have access to this environment variable in the same way server-side Blazor does.

**Final Solution:**
Use a simple boolean constant that can be manually toggled:

```csharp
// Show debug tools - set to true for development, false for production
private const bool showDebugTools = true; // Set to false before production deploy

// In markup:
@if (showDebugTools)
{
    <!-- Manual Buff Processing Trigger -->
}
```

**Why This Works:**
- Simple and explicit
- Easy to understand and modify
- Works reliably in Blazor WASM
- Can be set to `false` before deploying to production
- No dependency on configuration or environment detection

**Alternative Approaches (Future):**
If you want automatic detection, you can:
1. Create a `wwwroot/appsettings.json` with a `ShowDebugTools` flag
2. Inject and read that configuration
3. Or use compiler constants with `#if DEBUG` in the C# code block (not markup)

**Files Changed:**
- `LifeForge.Web\Pages\CharacterSheet.razor`

**Testing:**
- ? Set `showDebugTools = true` ? Button appears
- ? Set `showDebugTools = false` ? Button hidden
- ? No literal `#if DEBUG` text appears in UI
- ? Works consistently in debug and release builds

---

### Bug #2: Stacked Buffs Not Consolidating in Display ???

**Problem:**
When a user activated multiple instances of the same buff (e.g., a buff with MaxStacks > 1), each instance appeared as a separate card in the Active Buffs section. This made the UI cluttered and didn't properly show the total stack count.

**Root Cause:**
The `LoadActiveBuffs()` method was directly displaying the `activeBuffs` list from the API without grouping by `BuffId`. Each buff instance was treated independently for display purposes.

**Solution:**
Implemented buff grouping logic to consolidate multiple instances of the same buff:

```csharp
// Created helper class to group buff instances
private class BuffDisplayGroup
{
    public string BuffId { get; set; }
    public string BuffName { get; set; }
    public int TotalStacks { get; set; }        // Sum of all instance stacks
    public int ActiveStacks { get; set; }       // Sum of active instance stacks
    public int PendingCount { get; set; }       // Count of pending instances
    public DateTime OldestEndTime { get; set; } // First instance to expire
    public List<BuffInstanceDto> Instances { get; set; } = new();
    // ... modifiers from first instance
}

// Group buffs by BuffId for display
displayBuffs = activeBuffs
    .GroupBy(b => b.BuffId)
    .Select(g =>
    {
        var first = g.First();
        var activeInstances = g.Where(b => b.Status == BuffInstanceStatus.Active).ToList();
        var pendingInstances = g.Where(b => b.Status == BuffInstanceStatus.Pending).ToList();
        
        return new BuffDisplayGroup
        {
            BuffId = g.Key,
            BuffName = first.BuffName,
            TotalStacks = g.Sum(b => b.Stacks),
            ActiveStacks = activeInstances.Sum(b => b.Stacks),
            PendingCount = pendingInstances.Count,
            OldestEndTime = g.Min(b => b.EndTime),
            Instances = g.ToList(),
            // ... copy modifiers from first instance
        };
    })
    .ToList();
```

**UI Changes:**
- Buff cards now show consolidated view with total stacks badge
- "End Buff" button now ends ALL instances of that buff
- Tooltip shows:
  - Total modifiers (all stacks combined)
  - Per-stack breakdown
  - Active instance count
  - Pending count (if any)

**Example:**
```
Before:
  [Strength Buff x1] [Strength Buff x1] [Strength Buff x1]
  
After:
  [Strength Buff x3]
  
Tooltip shows:
  Strength Buff
  Total Modifiers (x3 stacks):
    HP Max +30 (per stack: +10)
  Active Instances: 3
```

**Files Changed:**
- `LifeForge.Web\Pages\CharacterSheet.razor` - Added grouping logic
- `LifeForge.Web\Pages\CharacterSheet.razor.css` - Added pending badge styling

**Testing:**
- ? Multiple instances of same buff consolidate into one card
- ? Stack count displays correctly (x3, x5, etc.)
- ? Tooltip shows total and per-stack modifiers
- ? "End Buff" button removes all instances
- ? Pending buffs show count badge ("2 Pending")
- ? Timer shows oldest EndTime (first to expire)

---

## Implementation Details

### Grouping Logic

The grouping happens in `LoadActiveBuffs()`:

1. **Fetch all buff instances** for the character
2. **Group by BuffId** (all instances of same buff)
3. **Calculate aggregates:**
   - Total stacks (sum of all instance stacks)
   - Active stacks (sum of active instances only)
   - Pending count (count of pending instances)
   - Oldest end time (min of all end times)
4. **Store first instance's modifiers** (all instances have same modifiers)
5. **Keep reference to all instances** (for bulk deletion)

### UI Rendering

The display iterates over `displayBuffs` instead of `activeBuffs`:

```razor
@foreach (var buffGroup in displayBuffs)
{
    <div class="active-buff-card">
        <!-- Image from first instance -->
        <img src="@GetImageDataUrl(buffGroup.ImageData, ...)" />
        
        <!-- Total stacks badge -->
        @if (buffGroup.TotalStacks > 1)
        {
            <div class="active-buff-stacks-badge">x@(buffGroup.TotalStacks)</div>
        }
        
        <!-- Pending count badge -->
        @if (buffGroup.HasPending)
        {
            <div class="buff-status-badge pending">@buffGroup.PendingCount Pending</div>
        }
        
        <!-- End all button -->
        <button @onclick="() => EndAllBuffsInGroup(buffGroup)">
    </div>
}
```

### Tooltip Enhancement

The tooltip now shows comprehensive information:

```
Strength Boost

This buff increases your strength

Total Modifiers (x3 stacks):
  HP Max +30 (per stack: +10)
  Strength +15 (per stack: +5)

Active Instances: 3
Total Stacks: 3
Pending: 1
```

---

### Bug #3: Debug Button Not Showing ???

**Problem:**
After fixing Bug #1, the debug button wasn't showing at all, even when debugging.

**Root Cause:**
The attempted fix using `IConfiguration.GetValue<string>("ASPNETCORE_ENVIRONMENT")` doesn't work in Blazor WebAssembly because:
- WASM runs entirely in the browser
- Server environment variables aren't accessible
- Configuration in WASM comes from `wwwroot/appsettings.json`, not server environment

**Final Solution:**
Replaced with a simple boolean constant:
```csharp
private const bool showDebugTools = true; // Set to false before production deploy
```

**Instructions for Production:**
Before deploying to production, change the constant:
```csharp
private const bool showDebugTools = false; // Hide debug tools in production
```

---

## Testing Guide

### Test Scenario 1: Single Buff Instance
1. Activate a buff once
2. Verify: Shows buff card with no stack badge (or x1)
3. Tooltip shows single stack info

### Test Scenario 2: Multiple Stacks (Same Buff)
1. Create a buff with MaxStacks > 1
2. Activate it 3 times
3. Verify: Shows ONE buff card with "x3" badge
4. Tooltip shows "Total Modifiers (x3 stacks)"
5. Click "End Buff" ? All 3 instances removed

### Test Scenario 3: Pending + Active Stacks
1. Activate a buff 5 times
2. Trigger processing (only 3 become active if MaxStacks=3)
3. Verify: Shows "x3" badge + "2 Pending" badge
4. Tooltip shows both counts

### Test Scenario 4: Development Button
1. Set `showDebugTools = true`
2. Verify: Yellow debug section appears with trigger button
3. Set `showDebugTools = false`
4. Verify: Debug section is hidden

### Test Scenario 5: Different Buffs
1. Activate Buff A (x2)
2. Activate Buff B (x1)
3. Verify: Two separate buff cards displayed
4. Each shows correct stack count

---

## Breaking Changes

None. These are UI-only bug fixes that don't affect:
- API contracts
- Database schema
- Business logic
- Existing functionality

---

## Performance Impact

**Minimal positive impact:**
- Grouping happens client-side on small datasets (< 50 buff instances typical)
- O(N) complexity for grouping
- Reduces DOM elements (fewer buff cards rendered)
- Better user experience with consolidated view

---

## Future Enhancements (Optional)

1. **Configuration-Based Debug Toggle**
   - Create `wwwroot/appsettings.json` with `ShowDebugTools` flag
   - Read from configuration instead of hardcoded constant
   - Allows different settings per environment without code changes

2. **Individual Stack Management**
   - Allow ending individual instances instead of all at once
   - Add dropdown or right-click menu

3. **Stack Visualization**
   - Show visual indicator of how many are active vs pending
   - Progress bar or color coding

4. **Sort Options**
   - Sort by remaining time
   - Sort by buff type (buffs vs debuffs)
   - Sort alphabetically

5. **Expanded Tooltips**
   - Show individual instance end times
   - Show which instances are active vs pending

---

## Files Modified Summary

| File | Changes |
|------|---------|
| `LifeForge.Web\Pages\CharacterSheet.razor` | - Replaced IConfiguration with const bool<br>- Added `BuffDisplayGroup` class<br>- Added grouping logic in `LoadActiveBuffs()`<br>- Updated UI to use `displayBuffs`<br>- Enhanced tooltip with group info<br>- Changed `EndBuff()` to `EndAllBuffsInGroup()` |
| `LifeForge.Web\Pages\CharacterSheet.razor.css` | - Added `.buff-status-badge` styles<br>- Added `.buff-status-badge.pending` styles |

---

## Build Status

? **Build Successful**
- No compilation errors
- All functionality working as expected
- Ready for testing and deployment

---

## Deployment Notes

- No database migrations required
- No API changes required
- Can be deployed independently
- Backward compatible with existing data
- **Remember to set `showDebugTools = false` before production deploy**
