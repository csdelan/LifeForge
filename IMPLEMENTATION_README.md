# LifeForge - MongoDB Backend Implementation

This implementation adds a complete MongoDB backend to LifeForge with a Quest management system.

## ?? What's Been Added

### New Projects

1. **LifeForge.DataAccess** - Data access layer for MongoDB
   - `QuestEntity` - MongoDB entity with BSON attributes
   - `QuestRepository` - Repository pattern for CRUD operations
   - `MongoDbSettings` - Configuration model

2. **LifeForge.Api** - ASP.NET Core Web API
   - `QuestsController` - RESTful API endpoints for Quest management
   - Image upload functionality
   - CORS configuration for Blazor WebAssembly

### Enhanced Projects

3. **LifeForge.Web** - Blazor WebAssembly Client
   - `Quests.razor` - Full Quest management page with:
     - List view of all quests with thumbnails
     - Create new quest modal
     - Edit existing quest modal
     - Delete quest with confirmation
     - Image upload support
   - `QuestService` - Service to communicate with the API
   - Updated navigation menu

## ??? Architecture

```
???????????????????????????????????????????????????????????
?                   Blazor WebAssembly                     ?
?                    (LifeForge.Web)                       ?
?                     Port: 7295                           ?
???????????????????????????????????????????????????????????
                       ? HTTP/JSON
                       ?
???????????????????????????????????????????????????????????
?                   ASP.NET Core API                       ?
?                    (LifeForge.Api)                       ?
?                     Port: 7001                           ?
???????????????????????????????????????????????????????????
                       ? MongoDB Driver
                       ?
???????????????????????????????????????????????????????????
?                 MongoDB Atlas (Cloud)                    ?
?              Database: LifeForgeDb                       ?
?              Collection: Quests                          ?
???????????????????????????????????????????????????????????
```

## ?? Quick Start

### 1. Set Up MongoDB Atlas

Follow the detailed instructions in [MONGODB_SETUP.md](MONGODB_SETUP.md) to:
- Create a MongoDB Atlas account
- Set up a free cluster
- Configure database access and network settings
- Get your connection string

### 2. Configure the API

Edit `LifeForge.Api/appsettings.Development.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://YOUR_USERNAME:YOUR_PASSWORD@YOUR_CLUSTER.mongodb.net/?retryWrites=true&w=majority",
    "DatabaseName": "LifeForgeDb",
    "QuestsCollectionName": "Quests"
  }
}
```

### 3. Run the Application

**Terminal 1 - Start the API:**
```bash
cd LifeForge.Api
dotnet run
```

**Terminal 2 - Start the Blazor App:**
```bash
cd LifeForge.Web
dotnet run
```

### 4. Access the Application

- **Blazor UI:** https://localhost:7295
- **API Swagger:** https://localhost:7001/swagger
- Navigate to **"Quests"** in the menu to manage your quests

## ?? Features

### Quest Management Page (`/quests`)

- **View Quests:** Table view with quest thumbnails, names, descriptions, difficulty, and repeatability
- **Create Quest:** Modal form to create new quests with:
  - Name (required)
  - Description
  - Difficulty level (Trivial, Easy, Medium, Hard, CrazyHard)
  - Repeatability (OneTime, Unlimited, Daily, Weekly, Monthly)
  - Image upload
- **Edit Quest:** Update any quest property including changing the image
- **Delete Quest:** Remove quests with confirmation dialog
- **Image Upload:** Upload quest thumbnails (max 5MB)

### API Endpoints

- `GET /api/quests` - Get all quests
- `GET /api/quests/{id}` - Get a specific quest
- `POST /api/quests` - Create a new quest
- `PUT /api/quests/{id}` - Update a quest
- `DELETE /api/quests/{id}` - Delete a quest
- `POST /api/quests/upload-image` - Upload a quest image

## ??? Project Structure

```
LifeForge/
??? LifeForge.Domain/
?   ??? Quest.cs                          # Domain model
?
??? LifeForge.DataAccess/
?   ??? Models/
?   ?   ??? QuestEntity.cs                # MongoDB entity
?   ??? Repositories/
?   ?   ??? IQuestRepository.cs           # Repository interface
?   ?   ??? QuestRepository.cs            # MongoDB implementation
?   ??? Configuration/
?       ??? MongoDbSettings.cs            # Settings model
?
??? LifeForge.Api/
?   ??? Controllers/
?   ?   ??? QuestsController.cs           # API controller
?   ??? Models/
?   ?   ??? QuestDtos.cs                  # Data transfer objects
?   ??? wwwroot/images/quests/            # Uploaded images
?   ??? Program.cs                        # API configuration
?   ??? appsettings.json                  # Configuration
?
??? LifeForge.Web/
?   ??? Pages/
?   ?   ??? Quests.razor                  # Quest management page
?   ??? Services/
?   ?   ??? QuestService.cs               # API client service
?   ??? Models/
?   ?   ??? QuestDto.cs                   # Client-side DTO
?   ??? Layout/
?   ?   ??? NavMenu.razor                 # Updated navigation
?   ??? Program.cs                        # Client configuration
?
??? MONGODB_SETUP.md                      # Setup instructions
```

## ?? Security Notes

?? **Important for Production:**

1. **Never commit connection strings to source control**
   - Use environment variables
   - Use Azure Key Vault or similar
   - Add `appsettings.Development.json` to `.gitignore`

2. **Use .NET User Secrets for development:**
```bash
cd LifeForge.Api
dotnet user-secrets init
dotnet user-secrets set "MongoDbSettings:ConnectionString" "your-connection-string"
```

3. **Restrict MongoDB Network Access**
   - Don't use "Allow Access from Anywhere" in production
   - Whitelist specific IP addresses

4. **Add Authentication & Authorization**
   - Implement user authentication
   - Add authorization to API endpoints
   - Secure file uploads

## ?? Troubleshooting

### API won't start
- Check MongoDB connection string is correct
- Ensure MongoDB Atlas cluster is running
- Verify IP address is whitelisted in Atlas

### Blazor can't connect to API
- Verify API is running on port 7001
- Check CORS settings match Blazor port (7295)
- Look for browser console errors

### Images not displaying
- Check API wwwroot folder exists
- Verify image was uploaded successfully
- Check image URL in browser dev tools

### Build errors
```bash
# Clean and rebuild all projects
dotnet clean
dotnet build
```

## ?? NuGet Packages Added

### LifeForge.DataAccess
- MongoDB.Driver (3.5.2)
- Microsoft.Extensions.Options (10.0.1)

### LifeForge.Api
- Default ASP.NET Core packages
- Swashbuckle.AspNetCore (for Swagger)

## ?? Future Enhancements

- [ ] Add authentication (JWT tokens)
- [ ] Implement quest assignments to characters
- [ ] Track quest progress and completion
- [ ] Add quest rewards system
- [ ] Implement quest categories/tags
- [ ] Add search and filtering
- [ ] Implement pagination for large quest lists
- [ ] Store images in Azure Blob Storage or CDN
- [ ] Add quest prerequisites and dependencies
- [ ] Implement quest expiration dates

## ?? Additional Resources

- [MongoDB .NET Driver Documentation](https://www.mongodb.com/docs/drivers/csharp/)
- [Blazor WebAssembly Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

## ?? Contributing

When adding new features:
1. Follow the existing pattern (Domain ? DataAccess ? API ? Web)
2. Keep DTOs and Entities separate
3. Use dependency injection
4. Add proper error handling
5. Update this README

## ?? Notes

- The Quest domain model Name property is read-only (set in constructor)
- Images are stored locally in wwwroot/images/quests (consider cloud storage for production)
- Collections are automatically created in MongoDB when first data is inserted
- The API uses repository pattern for data access abstraction
