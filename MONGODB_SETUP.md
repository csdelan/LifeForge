# MongoDB Atlas Setup Instructions for LifeForge

This guide will walk you through setting up MongoDB Atlas (cloud-hosted MongoDB) for your LifeForge application.

## Step 1: Create a MongoDB Atlas Account

1. Go to [MongoDB Atlas](https://www.mongodb.com/cloud/atlas/register)
2. Sign up for a free account (or log in if you already have one)
3. Complete the registration process

## Step 2: Create a New Cluster

1. After logging in, click **"Build a Database"** or **"Create"**
2. Choose the **FREE** tier (M0 Sandbox)
3. Select a cloud provider (AWS, Google Cloud, or Azure)
4. Choose a region closest to your location for better performance
5. Name your cluster (default name is fine, e.g., "Cluster0")
6. Click **"Create Cluster"** (this may take 3-5 minutes)

## Step 3: Create a Database User

1. Click **"Database Access"** in the left sidebar under Security
2. Click **"Add New Database User"**
3. Choose **"Password"** as the authentication method
4. Enter a username (e.g., `lifeforge_admin`)
5. Click **"Autogenerate Secure Password"** or create your own strong password
6. **IMPORTANT:** Copy and save this password securely - you'll need it for the connection string
7. Under "Database User Privileges", select **"Read and write to any database"**
8. Click **"Add User"**

## Step 4: Configure Network Access

1. Click **"Network Access"** in the left sidebar under Security
2. Click **"Add IP Address"**
3. For development, you can click **"Allow Access from Anywhere"** (0.0.0.0/0)
   - **Note:** For production, restrict this to specific IP addresses
4. Click **"Confirm"**

## Step 5: Get Your Connection String

1. Click **"Database"** in the left sidebar
2. Click **"Connect"** on your cluster
3. Choose **"Drivers"** (for application connection)
4. Select **"C# / .NET"** as the driver
5. Copy the connection string that looks like:
   ```
   mongodb+srv://<username>:<password>@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority
   ```

## Step 6: Update Your Application Configuration

1. Open `LifeForge.Api/appsettings.Development.json`
2. Replace the connection string with your actual values:
   ```json
   {
     "MongoDbSettings": {
       "ConnectionString": "mongodb+srv://<username>:<password>@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority",
       "DatabaseName": "LifeForgeDb",
       "QuestsCollectionName": "Quests"
     }
   }
   ```
3. Replace `<username>` with your database username
4. Replace `<password>` with your database password
5. Replace `cluster0.xxxxx` with your actual cluster address

**Example:**
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://lifeforge_admin:MyS3cur3P@ssw0rd@cluster0.abc123.mongodb.net/?retryWrites=true&w=majority",
    "DatabaseName": "LifeForgeDb",
    "QuestsCollectionName": "Quests"
  }
}
```

## Step 7: Verify Connection

The database and collection will be automatically created when you first insert data. To verify:

1. Start your API project:
   ```bash
   cd LifeForge.Api
   dotnet run
   ```

2. The API should start without errors (default at https://localhost:7001)

3. You can test the API using Swagger UI at: `https://localhost:7001/swagger`

## Step 8: Running the Full Application

### Terminal 1 - Start the API:
```bash
cd LifeForge.Api
dotnet run
```

### Terminal 2 - Start the Blazor WebAssembly App:
```bash
cd LifeForge.Web
dotnet run
```

The Blazor app will be available at the URL shown in the terminal (typically https://localhost:7169).

## Step 9: Create Your First Quest

1. Open the Blazor app in your browser
2. Navigate to the **"Quests"** page from the menu
3. Click **"Create New Quest"**
4. Fill in the quest details
5. Optionally upload a thumbnail image
6. Click **"Create Quest"**

You can verify in MongoDB Atlas:
1. Go to your cluster in Atlas
2. Click **"Browse Collections"**
3. You should see your `LifeForgeDb` database with a `Quests` collection

## Security Best Practices

### For Production:

1. **Never commit your connection string to source control**
   - Add `appsettings.Development.json` to `.gitignore`
   - Use environment variables or Azure Key Vault for production secrets

2. **Use User Secrets for local development:**
   ```bash
   cd LifeForge.Api
   dotnet user-secrets init
   dotnet user-secrets set "MongoDbSettings:ConnectionString" "your-connection-string-here"
   ```

3. **Restrict Network Access**
   - In production, only allow specific IP addresses
   - Use VPC peering for cloud-hosted applications

4. **Create separate users for different environments**
   - Development user
   - Production user with appropriate permissions

## Troubleshooting

### Connection Timeout
- Check that your IP address is whitelisted in Network Access
- Verify your connection string is correct
- Ensure firewall allows outbound connections on port 27017

### Authentication Failed
- Double-check username and password in connection string
- Ensure password doesn't contain special characters that need URL encoding
- Verify user has correct permissions

### Collection Not Found
- Collections are created automatically when data is first inserted
- Try creating a quest through the Blazor UI

## Additional Configuration

### Connection String URL Encoding
If your password contains special characters, they must be URL-encoded:
- `@` becomes `%40`
- `:` becomes `%3A`
- `/` becomes `%2F`
- `?` becomes `%3F`
- `#` becomes `%23`

Example: Password `P@ss:word` becomes `P%40ss%3Aword`

## Project Structure

```
LifeForge/
??? LifeForge.Domain/          # Domain models (Quest, etc.)
??? LifeForge.DataAccess/      # MongoDB repositories and entities
??? LifeForge.Api/             # ASP.NET Core Web API
?   ??? Controllers/           # QuestsController
?   ??? Models/                # DTOs
?   ??? appsettings.json       # Configuration
??? LifeForge.Web/             # Blazor WebAssembly
    ??? Pages/                 # Quests.razor
    ??? Services/              # QuestService
    ??? Models/                # Client-side DTOs
```

## Next Steps

- Implement authentication and authorization
- Add more quest-related features (quest assignments, progress tracking)
- Implement image optimization and CDN storage
- Add data validation and error handling
- Create indexes for better query performance
