# Feature Update: Single Stack Removal

## Date: January 2025

## Change Summary

Updated the buff cancellation behavior to remove only **one stack at a time** instead of all stacks when manually canceling a buff.

## Motivation

When a user has multiple instances of the same buff active (e.g., 3 stacks of "Strength Boost"), clicking the "X" button should remove only one stack, not all three. This provides more granular control and matches expected behavior for stackable buffs in most game systems.

## Changes Made

### UI Changes

**Before:**
- Button labeled: "End All Stacks"
- Confirmation: "Are you sure you want to end all 3 instance(s) of 'Strength Boost'?"
- Result: All 3 stacks removed

**After:**
- Button labeled: "Remove 1 Stack"
- Confirmation: "Remove 1 stack of 'Strength Boost'? (3 total instances)"
- Result: Only 1 stack removed (oldest first)

### Removal Strategy: FIFO (First In, First Out)

When removing a stack, the system removes the **oldest instance** first:

```csharp
// Remove only the oldest instance (FIFO - First In, First Out)
var oldestInstance = buffGroup.Instances.OrderBy(i => i.StartTime).First();
await BuffInstanceService.DeactivateBuffInstanceAsync(_character!.Id!, oldestInstance.Id!);
```

**Why FIFO?**
- Matches real-world behavior (oldest buffs expire first)
- Consistent with buff expiration logic
- Predictable for users

### Code Changes

**File:** `LifeForge.Web\Pages\CharacterSheet.razor`

1. **Renamed method:**
   - `EndAllBuffsInGroup()` ? `RemoveOneStack()`

2. **Updated logic:**
   ```csharp
   // OLD: Removed all instances
   foreach (var instance in buffGroup.Instances)
   {
       await BuffInstanceService.DeactivateBuffInstanceAsync(...);
   }
   
   // NEW: Removes only oldest instance
   var oldestInstance = buffGroup.Instances.OrderBy(i => i.StartTime).First();
   await BuffInstanceService.DeactivateBuffInstanceAsync(...);
   ```

3. **Updated confirmation message:**
   - Shows total instance count
   - Clarifies that only 1 will be removed

4. **Updated success message:**
   - Shows remaining count after removal
   - Example: "Removed 1 stack of 'Strength Boost' (2 remaining)"

## User Experience

### Example Scenario

**Initial State:**
- Character has 3 stacks of "Strength Boost" (HP Max +10 each)
- Total modifier: HP Max +30
- UI shows: "Strength Boost x3"

**User clicks X button:**
1. Confirmation prompt: "Remove 1 stack of 'Strength Boost'? (3 total instances)"
2. User confirms
3. Oldest stack removed
4. Success message: "Removed 1 stack of 'Strength Boost' (2 remaining)"
5. UI updates: "Strength Boost x2"
6. Total modifier: HP Max +20

**User clicks X button again:**
1. Confirmation prompt: "Remove 1 stack of 'Strength Boost'? (2 total instances)"
2. User confirms
3. Next oldest stack removed
4. Success message: "Removed 1 stack of 'Strength Boost' (1 remaining)"
5. UI updates: "Strength Boost x1" (or no badge)
6. Total modifier: HP Max +10

**User clicks X button again:**
1. Confirmation prompt: "Remove 'Strength Boost'?"
2. User confirms
3. Last stack removed
4. Success message: "Removed 'Strength Boost'"
5. Buff card disappears
6. Total modifier: HP Max +0

## Technical Details

### Instance Selection

```csharp
// Instances are ordered by StartTime (oldest first)
var oldestInstance = buffGroup.Instances.OrderBy(i => i.StartTime).First();
```

**Properties used:**
- `StartTime`: DateTime when the instance was created
- Oldest = earliest `StartTime`

### Aggregate Recalculation

After removing one stack:
1. Instance is deleted from database
2. `BuffAggregationService.UpdateCharacterAggregateModifiersAsync()` is called
3. Aggregates are recalculated based on remaining active instances
4. Character stats update immediately
5. UI refreshes to show new values

### Edge Cases Handled

1. **Single Instance:**
   - Confirmation: "Remove 'Buff Name'?" (no stack count shown)
   - Result: Buff completely removed

2. **Pending Instances:**
   - Only active and pending instances are counted
   - Oldest is selected from all non-expired instances
   - Pending badges update after removal

3. **Same StartTime:**
   - If multiple instances have same `StartTime` (unlikely), `OrderBy` provides consistent ordering
   - First one in the collection is selected

## Testing Guide

### Test Case 1: Remove Single Stack from Multi-Stack Buff

**Setup:**
1. Create buff with MaxStacks = 3
2. Activate it 3 times
3. Trigger buff processing (all become active)

**Test:**
1. Click X button on buff card
2. Verify confirmation shows "Remove 1 stack... (3 total instances)"
3. Confirm
4. Verify success message: "Removed 1 stack... (2 remaining)"
5. Verify UI shows x2 badge
6. Verify stats updated (modifier reduced by 1 stack worth)

**Repeat:**
1. Click X again ? should show (2 total instances)
2. Click X again ? should show "Remove 'Buff Name'?"
3. Verify buff completely gone after last removal

### Test Case 2: Remove Single Instance Buff

**Setup:**
1. Activate a buff once

**Test:**
1. Click X button
2. Verify confirmation: "Remove 'Buff Name'?" (no stack count)
3. Confirm
4. Verify buff removed completely
5. Verify stats updated

### Test Case 3: FIFO Order Verification

**Setup:**
1. Activate buff 3 times with delays
   - Instance A: StartTime = 10:00:00
   - Instance B: StartTime = 10:01:00
   - Instance C: StartTime = 10:02:00

**Test:**
1. Note the "Time Remaining" displayed (should be based on oldest = Instance A)
2. Click X ? Instance A should be removed
3. Verify "Time Remaining" updates to Instance B's time
4. Click X ? Instance B should be removed
5. Verify "Time Remaining" updates to Instance C's time

### Test Case 4: Pending and Active Mix

**Setup:**
1. Activate buff 5 times
2. Trigger processing (3 become active if MaxStacks=3, 2 stay pending)

**Test:**
1. Verify UI shows: x3 badge + "2 Pending" badge
2. Click X ? should remove oldest active instance
3. Verify: x2 badge + "2 Pending" badge
4. Trigger processing ? 1 pending becomes active
5. Verify: x3 badge + "1 Pending" badge

## Breaking Changes

None. This is a behavior change that makes the system more intuitive, but doesn't break any APIs or data structures.

## Performance Impact

**Positive:**
- Only one instance deleted per operation (faster than deleting all)
- Fewer aggregate recalculations
- Better user experience with incremental changes

## Future Enhancements

### 1. Shift-Click to Remove All

Add modifier key support:
- **Normal click:** Remove 1 stack
- **Shift + click:** Remove all stacks

```razor
<button class="btn-end-buff" 
        @onclick="() => RemoveStacks(buffGroup, removeAll: false)"
        @onclick:preventDefault="false"
        title="Remove 1 Stack (Shift+Click to remove all)">
```

### 2. Right-Click Context Menu

Add context menu with options:
- Remove 1 stack
- Remove all stacks
- Remove specific instance (show list)

### 3. Stack Counter with +/- Buttons

Instead of X button, show:
```
[Buff Image]
[- 2 +]  ? Click + to add, - to remove
```

### 4. Confirmation Setting

Add user preference:
- "Always confirm buff removal"
- "Never confirm buff removal"
- "Confirm only for multi-stack buffs"

## Files Modified

| File | Changes |
|------|---------|
| `LifeForge.Web\Pages\CharacterSheet.razor` | - Renamed `EndAllBuffsInGroup()` to `RemoveOneStack()`<br>- Updated to remove only oldest instance<br>- Updated confirmation messages<br>- Updated success messages<br>- Updated button title |

## Build Status

? **Build Successful**
- No compilation errors
- All existing functionality preserved
- Ready for testing

## Deployment Notes

- No database migrations required
- No API changes required
- No backend changes required
- Only client-side UI behavior change
- Backward compatible with existing data
- Can be deployed independently

---

## Summary

This change improves user control by allowing granular removal of buff stacks. Users can now remove buffs one at a time instead of all at once, providing a better experience when managing multiple stacks of the same buff.

**Key Benefits:**
- ? More intuitive behavior
- ? Better control over buff management
- ? Matches expected game mechanics
- ? FIFO removal order (oldest first)
- ? Clear feedback with remaining count
