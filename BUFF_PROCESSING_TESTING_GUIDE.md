# Testing Guide: Daily Buff Processing System

## Quick Start Testing

### Prerequisites
- MongoDB running locally or connection string configured
- LifeForge.Api running (https://localhost:7001)
- LifeForge.Web running (https://localhost:7295)

## Test Scenarios

### Scenario 1: Basic Buff Activation & Processing

**Steps:**
1. Navigate to `/buffs` page
2. Create a test buff:
   - Name: "Test Buff"
   - Type: Buff (Positive)
   - Duration: 1 day
   - HP Max Modifier: +10
   - Click "Create"

3. Activate the buff:
   - Click "Activate" button on the buff card
   - Should show success message

4. Go to Character Sheet (`/`)
   - Buff should appear in "Active Buffs/Debuffs" section
   - Should have **"Pending"** badge
   - Stats should NOT be buffed yet

5. Click **"? Manually Trigger Buff Processing"** button
   - Should show "Processing..." message
   - After completion, page reloads

6. Verify results:
   - "Pending" badge should be gone
   - HP Max should be increased by 10
   - Progress bar should show buffed values
   - Hover over HP label to see base value tooltip

### Scenario 2: Multiple Stacks (Independent Instances)

**Setup: Create stackable buff**
- Name: "Strength Boost"
- Max Stacks: 3
- Duration: 2 days
- HP Max Modifier: +5

**Steps:**
1. Activate the buff 5 times (clicking "Activate" 5 times)
2. Go to Character Sheet
   - Should see 5 pending buff instances (UI may consolidate display)

3. Trigger buff processing
4. Verify:
   - Only 3 instances contribute (MaxStacks=3)
   - HP Max increased by +15 (3 stacks × 5 HP Max)
   - Extra 2 instances are ignored (but still exist in DB)

5. Wait (or manually expire one)
6. Trigger processing again
7. Verify:
   - One stack expired ? only 2 active
   - Next excess stack becomes active
   - Total still 3 active stacks

### Scenario 3: Percentage Modifiers

**Setup: Create percentage buff**
- Name: "HP Boost %"
- Duration: 1 day
- HP Max Percent Modifier: 20

**Assuming character has 100 HP Max:**

**Steps:**
1. Activate buff
2. Trigger processing
3. Verify:
   - Effective HP Max = 100 + (100 × 20%) = 120
   - Character sheet shows 120 as max

### Scenario 4: Combined Flat + Percentage Modifiers

**Setup: Create combined buff**
- Name: "Mixed Boost"
- HP Max Modifier: +10 (flat)
- HP Max Percent Modifier: 10 (percentage)

**Assuming character has 100 HP Max:**

**Steps:**
1. Activate buff
2. Trigger processing
3. Calculation:
   ```
   Base HP Max: 100
   After flat modifier: 100 + 10 = 110
   After percentage: 110 + (110 × 10%) = 121
   ```
4. Verify: Effective HP Max = 121

### Scenario 5: XP Gains Modifier

**Setup: Create XP buff**
- Name: "XP Boost"
- Duration: 1 day
- XP Gains Percent Modifier: 50

**Steps:**
1. Activate buff
2. Trigger processing
3. Complete a quest that gives 10 XP
4. Verify:
   - Character gains 15 XP total (10 base + 50% bonus)
   - Check class XP in Character Sheet

### Scenario 6: Debuff (Negative Modifiers)

**Setup: Create debuff**
- Name: "Hangover"
- Type: Debuff
- Duration: 1 day
- HP Max Modifier: -20
- MP Max Modifier: -10

**Steps:**
1. Activate debuff
2. Trigger processing
3. Verify:
   - HP Max decreased by 20
   - MP Max decreased by 10
   - Current HP/MP capped at new max
   - Red border on debuff card

### Scenario 7: Manual Deactivation (Immediate)

**Steps:**
1. Activate any buff
2. Trigger processing (buff becomes active)
3. Note current HP Max (should be buffed)
4. Click "X" button on buff card to end it
5. Confirm deletion
6. **Immediate effect** (no processing needed):
   - Buff disappears immediately
   - HP Max returns to base value immediately
   - No need to trigger processing manually

### Scenario 8: Buff Expiration

**Steps:**
1. Activate a 1-day buff
2. Trigger processing (becomes active)
3. Manually set EndTime in MongoDB to past:
   ```javascript
   db.buffInstances.updateOne(
     { buffName: "Test Buff" },
     { $set: { endTime: new Date("2024-01-01") } }
   )
   ```
4. Trigger processing
5. Verify:
   - Buff changes to Expired status
   - Buff modifiers removed from character
   - Buff still visible in UI (will be deleted after 7 days)

### Scenario 9: Multiple Buffs (Aggregate)

**Setup: Create multiple buffs**
- Buff A: HP Max +10
- Buff B: HP Max +20
- Buff C: MP Max +15

**Steps:**
1. Activate all three buffs
2. Trigger processing
3. Verify aggregate:
   - HP Max increased by 30 total
   - MP Max increased by 15 total
   - All buffs show as active

### Scenario 10: Stress Test (Many Buffs)

**Steps:**
1. Create 10 different buffs
2. Activate each one 5 times
3. Trigger processing
4. Check performance:
   - Processing should complete in < 5 seconds
   - Check API logs for timing
   - Verify all buffs processed correctly

## Manual MongoDB Queries for Testing

### View All Buff Instances
```javascript
db.buffInstances.find().pretty()
```

### View Character Aggregate Modifiers
```javascript
db.characters.find({}, { activeBuffModifiers: 1 }).pretty()
```

### Manually Set Buff to Expired
```javascript
db.buffInstances.updateOne(
  { buffName: "Test Buff" },
  { 
    $set: { 
      endTime: new Date("2020-01-01"),
      status: "Active"
    } 
  }
)
```

### Count Buffs by Status
```javascript
db.buffInstances.aggregate([
  { $group: { _id: "$status", count: { $sum: 1 } } }
])
```

### View Oldest Expired Buffs (for cleanup testing)
```javascript
db.buffInstances.find(
  { 
    status: "Expired",
    endTime: { $lt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000) }
  }
).pretty()
```

## Automated Testing (Future)

### Unit Tests to Write

1. **AggregateModifier Tests**
   ```csharp
   [Test]
   public void ApplyModifier_ShouldAddValues()
   {
       var aggregate = new AggregateModifier { HPModifier = 10 };
       var other = new AggregateModifier { HPModifier = 5 };
       aggregate.ApplyModifier(other);
       Assert.AreEqual(15, aggregate.HPModifier);
   }
   ```

2. **Character Effective Stats Tests**
   ```csharp
   [Test]
   public void EffectiveHPMax_ShouldApplyModifiers()
   {
       var character = new Character("Test") 
       { 
           HPMax = 100,
           ActiveBuffModifiers = new AggregateModifier 
           { 
               HPMaxModifier = 10,
               HPMaxPercentModifier = 20 
           }
       };
       Assert.AreEqual(132, character.EffectiveHPMax); // 100 + 10 + (110 * 20%)
   }
   ```

3. **Buff Aggregation Service Tests**
   ```csharp
   [Test]
   public async Task CalculateAggregateModifiers_ShouldRespectMaxStacks()
   {
       // Setup: 5 instances of buff with MaxStacks=3
       // Assert: Only 3 instances contribute
   }
   ```

## Common Issues & Troubleshooting

### Issue: Buffs not activating
**Check:**
- Is the midnight job running? Check API logs
- Did you trigger processing manually?
- Check buff status in MongoDB

### Issue: Stats not updating after processing
**Check:**
- Reload the page (F5)
- Check character.activeBuffModifiers in MongoDB
- Verify buff instances have Status="Active"

### Issue: Manual trigger button not visible
**Check:**
- Are you running in Debug mode?
- Button is wrapped in `#if DEBUG` directive
- Run LifeForge.Api in Development environment

### Issue: Too many stacks contributing
**Check:**
- Verify MaxStacks setting on buff definition
- Check aggregation logic respects MaxStacks
- Look at API logs during processing

### Issue: Percentage modifiers not working
**Check:**
- Calculation order: Flat modifiers applied first, then percentage
- Percentage is based on modified value, not base

## API Endpoints for Testing

### Trigger Buff Processing (Development Only)
```http
POST https://localhost:7001/api/buffprocessing/trigger
```

Expected response:
```json
{
  "success": true,
  "message": "Buff processing completed successfully"
}
```

### Get Character with Effective Stats
```http
GET https://localhost:7001/api/characters
```

Response includes:
```json
{
  "hp": 50,           // Base value
  "hpMax": 100,       // Base value
  "effectiveHP": 50,  // Computed with buffs
  "effectiveHPMax": 120, // Computed with buffs
  "activeBuffModifiers": {
    "hpMaxModifier": 10,
    "hpMaxPercentModifier": 10
  }
}
```

### Get Active Buff Instances
```http
GET https://localhost:7001/api/buffinstances/character/{characterId}
```

Response includes status field:
```json
[
  {
    "id": "...",
    "buffName": "Test Buff",
    "status": "Active",    // or "Pending" or "Expired"
    "startTime": "...",
    "endTime": "...",
    "hpMaxModifier": 10
  }
]
```

## Logging

### Check API Logs for Processing

Look for these log entries:

```
[Information] Midnight Buff Processing Service started
[Information] Next buff processing scheduled for 2024-01-02 00:00:00 UTC
[Information] Starting buff processing at 2024-01-01 23:00:00
[Information] Processing buffs for 1 characters
[Information] Activated 2 pending buffs for character 67abc123...
[Information] Expired 1 buffs for character 67abc123...
[Information] Updated aggregate modifiers for character 67abc123: HP+10, HPMax+20, MP+0, MPMax+15, XP+50%
[Information] Cleaned up 0 old expired buffs for character 67abc123
[Information] Buff processing completed successfully
```

## Performance Benchmarks

Expected performance (approximate):

| Characters | Avg Buffs Each | Processing Time |
|-----------|----------------|-----------------|
| 1         | 10             | < 1 second      |
| 10        | 10             | < 2 seconds     |
| 100       | 10             | < 10 seconds    |
| 1000      | 10             | < 60 seconds    |

If performance is slower, check:
- MongoDB indexes on `characterId`, `status`, `endTime`
- Network latency to MongoDB
- Logging verbosity (reduce in production)

## Success Criteria

? **Basic Flow**
- Buff activates with Pending status
- Manual trigger processes buffs correctly
- Stats update immediately after processing
- UI reflects changes without refresh needed

? **Edge Cases**
- MaxStacks respected
- Percentage modifiers calculate correctly
- Manual deactivation works immediately
- Expired buffs clean up properly

? **Performance**
- Processing completes in reasonable time
- No memory leaks during processing
- Midnight job runs reliably

? **User Experience**
- Clear feedback when buffs are pending
- Stat tooltips show base values
- Buff cards show status badges
- Manual trigger works smoothly (dev mode)
