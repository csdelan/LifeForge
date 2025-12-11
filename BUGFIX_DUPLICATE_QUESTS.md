# Bug Fix: Prevent Duplicate Active Quest Instances

## Issue
Users could start the same quest multiple times, creating duplicate active quest runs. This caused:
- Multiple instances of the same quest in the Active Quests list
- Confusion about which instance to complete
- Potential for exploiting reward systems

## Root Cause
The `StartQuestRun` endpoint in `QuestRunsController` did not validate whether a quest was already in progress before creating a new quest run.

## Solution

### 1. Data Access Layer - Added Repository Method

**File: `LifeForge.DataAccess/Repositories/IQuestRunRepository.cs`**
- Added method: `GetActiveQuestRunByQuestIdAsync(string questId)`

**File: `LifeForge.DataAccess/Repositories/QuestRunRepository.cs`**
- Implemented the method to query MongoDB for active quest runs matching the questId

```csharp
public async Task<QuestRunEntity?> GetActiveQuestRunByQuestIdAsync(string questId)
{
    return await _questRunsCollection
        .Find(qr => qr.QuestId == questId && qr.Status == QuestStatus.InProgress)
        .FirstOrDefaultAsync();
}
```

### 2. API Layer - Added Validation

**File: `LifeForge.Api/Controllers/QuestRunsController.cs`**

Updated `StartQuestRun` method to:
1. Check if quest exists (already done)
2. **NEW:** Check if quest is already in progress
3. Return 409 Conflict if duplicate found
4. Otherwise, create new quest run

```csharp
// Check if this quest is already in progress
var existingActiveQuestRun = await _questRunRepository.GetActiveQuestRunByQuestIdAsync(startQuestRunDto.QuestId);
if (existingActiveQuestRun != null)
{
    return Conflict(new 
    { 
        message = $"Quest '{quest.Name}' is already in progress. Complete or cancel it before starting a new instance.",
        existingQuestRunId = existingActiveQuestRun.Id
    });
}
```

**Response Codes:**
- **409 Conflict** - Quest already in progress (with helpful message)
- **404 Not Found** - Quest doesn't exist
- **201 Created** - Quest run started successfully
- **500 Internal Server Error** - Unexpected error

### 3. Web Layer - Updated Error Handling

**File: `LifeForge.Web/Pages/Quests.razor`**

Updated `StartQuest` method to handle the 409 Conflict response:

```csharp
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
{
    errorMessage = $"Quest '{quest.Name}' is already in progress. Complete or cancel it before starting a new instance.";
}
```

**User Experience:**
- Friendly error message appears as alert banner
- Message clearly explains the issue
- Suggests user complete or cancel existing quest

---

## Testing

### Test Case 1: Start Quest (First Time)
1. Navigate to Quests page
2. Click "Start" on any quest
3. **Expected:** ? Success message, quest appears in Active Quests

### Test Case 2: Start Same Quest Again (Duplicate Attempt)
1. Navigate to Quests page
2. Click "Start" on the **same quest** that's already active
3. **Expected:** ? Error message: "Quest 'X' is already in progress..."
4. **Verify:** Only one instance in Active Quests

### Test Case 3: Complete Quest, Then Restart
1. Complete a quest from Active Quests
2. Go back to Quests page
3. Click "Start" on the same quest
4. **Expected:** ? Success (quest can be restarted after completion)

### Test Case 4: Cancel Quest, Then Restart
1. Cancel a quest from Active Quests
2. Go back to Quests page
3. Click "Start" on the same quest
4. **Expected:** ? Success (quest can be restarted after cancellation)

### Test Case 5: Different Quests
1. Start Quest A
2. Start Quest B
3. **Expected:** ? Both succeed (different quests can run simultaneously)

---

## API Response Examples

### Success Response (201 Created)
```json
{
  "id": "507f1f77bcf86cd799439011",
  "questId": "507f191e810c19729de860ea",
  "questName": "Complete Documentation",
  "status": "InProgress",
  "startTime": "2025-01-15T10:30:00Z",
  "rewards": []
}
```

### Conflict Response (409 Conflict)
```json
{
  "message": "Quest 'Complete Documentation' is already in progress. Complete or cancel it before starting a new instance.",
  "existingQuestRunId": "507f1f77bcf86cd799439011"
}
```

---

## Database Query

The new repository method uses this MongoDB query:

```csharp
_questRunsCollection.Find(qr => 
    qr.QuestId == questId && 
    qr.Status == QuestStatus.InProgress
).FirstOrDefaultAsync();
```

**Performance:**
- Efficient single-document lookup
- Uses compound query (questId + status)
- Consider adding index: `{ questId: 1, status: 1 }`

---

## Edge Cases Handled

### ? Quest Repeatability
- **OneTime quests:** Can be started again after completion
- **Daily/Weekly/Monthly:** Can be started again after completion (future: add cooldown logic)
- **Unlimited:** Can be started again after completion

### ? Multiple Users (Future)
- Current implementation: Single character system
- Future: When multi-user support is added, query should filter by userId too

### ? Race Conditions
- Potential race condition: Two simultaneous requests to start same quest
- **Mitigation:** MongoDB's unique index constraint (future enhancement)
- **Current:** Last-write-wins (acceptable for single-user system)

---

## Future Enhancements

### 1. Add Database Index
```javascript
db.QuestRuns.createIndex({ "questId": 1, "status": 1 })
```
Benefits: Faster lookups for active quests

### 2. Enforce Quest Repeatability Rules
```csharp
// Check repeatability rules before allowing restart
if (quest.Repeatability == QuestRepeatability.OneTime)
{
    var anyCompletedRuns = await _questRunRepository.HasCompletedQuestRunAsync(questId);
    if (anyCompletedRuns)
    {
        return Conflict("This is a one-time quest that has already been completed.");
    }
}
```

### 3. Add Cooldown Periods for Repeatable Quests
```csharp
// Check cooldown for daily/weekly/monthly quests
if (quest.Repeatability == QuestRepeatability.Daily)
{
    var lastCompleted = await _questRunRepository.GetLastCompletedTimeAsync(questId);
    if (lastCompleted.HasValue && (DateTime.UtcNow - lastCompleted.Value).TotalHours < 24)
    {
        return Conflict("This daily quest is on cooldown. Try again tomorrow.");
    }
}
```

### 4. UI Improvements
- Disable "Start" button for quests already in progress
- Show "In Progress" badge on quest cards
- Add "View Active Quest" button that navigates to Active Quests page

---

## Files Modified

### Created:
- `BUGFIX_DUPLICATE_QUESTS.md` (this document)

### Modified:
- `LifeForge.DataAccess/Repositories/IQuestRunRepository.cs`
- `LifeForge.DataAccess/Repositories/QuestRunRepository.cs`
- `LifeForge.Api/Controllers/QuestRunsController.cs`
- `LifeForge.Web/Pages/Quests.razor`

---

## Build Status
? All projects compile successfully

---

## Summary

**Bug Fixed:** ? Users can no longer start duplicate instances of the same quest

**Implementation:**
- Added repository method to check for active quest runs
- Added validation in API controller
- Return 409 Conflict with user-friendly message
- Updated UI to handle conflict gracefully

**User Impact:**
- Prevents confusion from duplicate active quests
- Clear error messages guide users
- Maintains data integrity

**Next Steps:**
- Consider adding UI indicators for quests already in progress
- Implement repeatability cooldown logic
- Add database indexes for performance
