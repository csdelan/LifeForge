# Quick Start Guide - MongoDB Backend for LifeForge

## ? What Has Been Created

Your LifeForge application now has a complete MongoDB-backed Quest management system with:

1. **3 New Projects:**
   - `LifeForge.DataAccess` - MongoDB data access layer
   - `LifeForge.Api` - RESTful Web API
   - Blazor page for Quest management

2. **Full CRUD Operations:**
   - Create, Read, Update, Delete quests
   - Upload and store quest thumbnail images
   - Display quests in a responsive table

## ?? Getting Started in 5 Minutes

### Step 1: Set Up MongoDB (5-10 minutes)

1. **Go to MongoDB Atlas:** https://www.mongodb.com/cloud/atlas/register
2. **Create a free account** (M0 tier is free forever)
3. **Create a cluster** (takes 3-5 minutes to provision)
4. **Create a database user:**
   - Security ? Database Access ? Add New Database User
   - Username: `lifeforge_admin`
   - Generate a secure password (save it!)
   - Grant "Read and write to any database"
5. **Allow network access:**
   - Security ? Network Access ? Add IP Address
   - For testing: "Allow Access from Anywhere" (0.0.0.0/0)
6. **Get connection string:**
   - Database ? Connect ? Drivers ? C#/.NET
   - Copy the connection string
   - It looks like: `mongodb+srv://username:password@cluster0.xxxxx.mongodb.net/...`

### Step 2: Configure Your API (1 minute)

Edit `LifeForge.Api/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://lifeforge_admin:YOUR_PASSWORD@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority",
    "DatabaseName": "LifeForgeDb",
    "QuestsCollectionName": "Quests"
  }
}
```

**Replace:**
- `YOUR_PASSWORD` with your actual MongoDB password
- `cluster0.xxxxx` with your actual cluster address

### Step 3: Run the Application (30 seconds)

Open **TWO terminal windows**:

**Terminal 1 - API:**
```bash
cd LifeForge.Api
dotnet run
```
Wait until you see: `Now listening on: https://localhost:7001`

**Terminal 2 - Blazor App:**
```bash
cd LifeForge.Web
dotnet run
```
Wait until you see: `Now listening on: https://localhost:7295`

### Step 4: Test It Out! (2 minutes)

1. **Open your browser:** https://localhost:7295
2. **Click "Quests"** in the navigation menu
3. **Click "Create New Quest"**
4. **Fill in the form:**
   - Name: "Learn MongoDB"
   - Description: "Complete MongoDB setup for LifeForge"
   - Difficulty: Medium
   - Repeatability: OneTime
   - (Optional) Upload an image
5. **Click "Create Quest"**
6. **See your quest appear in the table!**

## ?? You're Done!

Your quest is now stored in MongoDB Atlas. You can:
- ? View all quests
- ? Edit quests
- ? Delete quests
- ? Upload images
- ? Filter by difficulty and repeatability

## ?? Testing the API Directly

Visit the Swagger UI: https://localhost:7001/swagger

You can test all API endpoints:
- GET `/api/quests` - List all quests
- POST `/api/quests` - Create a quest
- PUT `/api/quests/{id}` - Update a quest
- DELETE `/api/quests/{id}` - Delete a quest
- POST `/api/quests/upload-image` - Upload an image

## ?? View Your Data in MongoDB

1. Go to MongoDB Atlas
2. Click "Browse Collections"
3. You'll see:
   - Database: `LifeForgeDb`
   - Collection: `Quests`
   - Your quest documents with all properties

## ??? Project Structure

```
LifeForge/
??? LifeForge.Domain/          # Your existing domain models
??? LifeForge.DataAccess/      # NEW - MongoDB access
?   ??? Models/QuestEntity.cs
?   ??? Repositories/QuestRepository.cs
?   ??? Configuration/MongoDbSettings.cs
??? LifeForge.Api/             # NEW - Web API
?   ??? Controllers/QuestsController.cs
?   ??? Models/QuestDtos.cs
?   ??? appsettings.json
??? LifeForge.Web/             # Enhanced Blazor app
    ??? Pages/Quests.razor     # NEW - Quest management page
    ??? Services/QuestService.cs  # NEW - API client
    ??? Models/QuestDto.cs

```

## ?? Common Issues & Solutions

### "Connection timeout" error
- ? Check your MongoDB connection string is correct
- ? Verify IP address is whitelisted (Network Access in Atlas)
- ? Make sure cluster is running (not paused)

### "Authentication failed" error
- ? Double-check username and password
- ? URL-encode special characters in password
- ? Verify user has correct permissions

### Blazor app can't connect to API
- ? Make sure API is running on port 7001
- ? Check for CORS errors in browser console
- ? Verify both projects are running

### Images not showing
- ? Check that wwwroot/images/quests folder exists in LifeForge.Api
- ? Verify image uploaded successfully (check API logs)
- ? Try accessing image URL directly in browser

## ?? Next Steps

Now that you have a working backend, you can:

1. **Add more entities:** Follow the same pattern for Character, Items, etc.
2. **Add authentication:** Implement JWT tokens for user login
3. **Add quest assignments:** Link quests to characters
4. **Track progress:** Add completion tracking for quests
5. **Add rewards:** Implement reward system for quest completion

## ?? Documentation Files

- **MONGODB_SETUP.md** - Detailed MongoDB setup instructions
- **IMPLEMENTATION_README.md** - Technical documentation and architecture
- This file - Quick start guide

## ?? Need Help?

If something isn't working:
1. Check the terminal output for error messages
2. Look at browser console (F12) for JavaScript errors
3. Test API endpoints in Swagger UI
4. Verify MongoDB connection in Atlas

## ?? What You Can Do Now

Try creating different types of quests:
- Daily habits (Difficulty: Easy, Repeatability: Daily)
- Learning goals (Difficulty: Hard, Repeatability: OneTime)
- Weekly routines (Difficulty: Medium, Repeatability: Weekly)
- Major challenges (Difficulty: CrazyHard, Repeatability: Unlimited)

**Congratulations! You now have a fully functional MongoDB-backed quest system!** ??
