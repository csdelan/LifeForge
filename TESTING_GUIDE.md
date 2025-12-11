# Quick Start: Testing Currency Rewards System

## ?? Goal
Test the complete flow of earning and applying rewards when completing quests.

## ? Prerequisites

1. **MongoDB Atlas** configured with connection string in `LifeForge.Api/appsettings.Development.json`
2. **Visual Studio** or **VS Code** with .NET 9 SDK

## ?? Step-by-Step Testing

### Step 1: Start the API (Terminal 1)

```bash
cd LifeForge.Api
dotnet run
```

Wait for: `Now listening on: https://localhost:7001`

### Step 2: Start the Web App (Terminal 2)

```bash
cd LifeForge.Web
dotnet run
```

Wait for: `Now listening on: https://localhost:7295`

### Step 3: Create a Quest with Rewards

1. Open browser: **https://localhost:7295**
2. Click **"Quests"** in navigation
3. Click **"Create New Quest"**
4. Fill in:
   - **Name:** "Complete Documentation"
   - **Description:** "Write project documentation"
   - **Difficulty:** Medium
   - **Repeatability:** OneTime
5. Scroll to **Rewards** section
6. Click **"Add Reward"**
   - Select: **Gold**
   - Amount: **50**
7. Click **"Add Reward"** again
   - Select: **Trader XP** (or any XP type)
   - Amount: **100**
8. Click **"Create Quest"**

? **Expected:** Quest appears in the table

### Step 4: Check Character Sheet (Before Quest)

1. Click **"Character Sheet"** in navigation (or go to `/`)

? **Expected:** "No Character Yet" message with link to quests

### Step 5: Start the Quest

1. Go back to **"Quests"** page
2. Find your quest "Complete Documentation"
3. Click the **green "Start" button** (play icon)

? **Expected:** Success message "Quest started successfully!"

### Step 6: View Active Quest

1. Click **"Active Quests"** button at top, or navigate to `/active-quests`

? **Expected:** 
- Quest appears in table
- Status: **In Progress** (blue badge)
- Rewards column shows: "Complete to see rewards"

### Step 7: Complete the Quest

1. On Active Quests page
2. Click the green **"Finish"** button next to your quest
3. Wait a moment for processing

? **Expected:** Success message like:
```
Quest 'Complete Documentation' completed! 
Rewards: +50 Gold, +100 Trader XP
```

### Step 8: Verify Character Sheet (After Quest)

1. Navigate to **"Character Sheet"** (`/` or `/character`)

? **Expected:** Character sheet now shows:
- **Name:** Hero of LifeForge
- **HP:** 100/100
- **MP:** 100/100
- **Strength:** 10
- **Discipline:** 10
- **Focus:** 10
- **Wealth:**
  - **Gold:** 50 ?
  - **Karma:** 0
  - **DesignWorkslot:** 0
- **Classes:**
  - **Trader** (or whichever XP type you chose)
  - **Level:** 1
  - **XP:** 100 / 100 (or similar)

### Step 9: Complete Another Quest (Test Leveling)

1. Create another quest with **250 Trader XP** and **100 Gold**
2. Start and complete it
3. Check Character Sheet

? **Expected:**
- **Gold:** 150 (50 + 100)
- **Trader Level:** 2 (leveled up!)
- **Trader XP:** 50/110 (or similar - depends on level formula)

### Step 10: Verify in MongoDB Atlas

1. Log in to **MongoDB Atlas**
2. Click **"Browse Collections"**
3. Select database: **LifeForgeDb**
4. Select collection: **Characters**

? **Expected:** See your character document with:
```json
{
  "_id": "...",
  "name": "Hero of LifeForge",
  "currencies": {
    "Gold": 150,
    "Karma": 0,
    "DesignWorkslot": 0
  },
  "classProfiles": {
    "Trader": {
      "className": "Trader",
      "level": 2,
      "currentXp": 50,
      "xpToNextLevel": 110
    }
  }
}
```

---

## ?? Advanced Testing

### Test Different Reward Types

Create quests with:
- **Multiple currencies:** Gold + Karma + Design Workslot
- **Multiple XP types:** Trader XP + Developer XP
- **Mix:** Gold + Developer XP

### Test Error Handling

1. **Complete quest twice:**
   - Try to click "Finish" on an already completed quest
   - Expected: Error message "Cannot complete a quest that is Completed"

2. **Cancel quest:**
   - Start a quest
   - Click red X button
   - Confirm cancellation
   - Expected: Quest removed, no rewards applied

### Test API Directly via Swagger

1. Open **https://localhost:7001/swagger**
2. Test endpoints:
   - `POST /api/questruns/start` - Start a quest
   - `POST /api/questruns/{id}/complete` - Complete quest
   - `POST /api/questruns/{id}/apply-rewards` - Apply rewards
   - `GET /api/characters` - View character

---

## ?? Troubleshooting

### "No character found" error
- **Solution:** Complete a quest first to auto-create character

### Rewards not appearing
- **Solution:** Check browser console (F12) for errors
- **Solution:** Check API terminal for error logs
- **Solution:** Verify MongoDB connection in API settings

### Build errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### API won't start
- **Check:** MongoDB connection string in `appsettings.Development.json`
- **Check:** Port 7001 is not in use
- **Check:** All NuGet packages restored

---

## ?? What You Should See

### Before Any Quests:
- Character Sheet: "No Character Yet"
- Active Quests: Empty

### After First Quest:
- Character exists with starting stats
- Currencies appear (whatever was rewarded)
- Classes appear with level 1

### After Multiple Quests:
- Currencies accumulate
- XP accumulates
- Characters level up automatically
- XP requirements increase per level

---

## ? Success Criteria

You've successfully implemented the rewards system if:

? Completing a quest shows detailed reward message  
? Character sheet displays real data from database  
? Currencies accumulate across multiple quests  
? XP accumulates and characters level up  
? MongoDB shows updated character data  
? No errors in browser console or API logs  

---

## ?? Next Steps

Now that rewards work, you can:

1. **Create more reward types** (Items, Badges)
2. **Add character customization** (name, avatar)
3. **Implement skill trees** per character class
4. **Add stat bonuses** on level up
5. **Create achievement system**

Congratulations! You have a working gamification rewards system! ??
