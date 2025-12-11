# Security Configuration Setup

## Overview
The `appsettings.json` and `appsettings.Development.json` files have been removed from Git tracking to protect sensitive connection strings and other credentials.

## Setup Instructions

### 1. Create Your Configuration Files

1. Navigate to the `LifeForge.Api` folder
2. Copy `appsettings.Template.json` to `appsettings.json`
3. Copy `appsettings.Template.json` to `appsettings.Development.json`

### 2. Update Connection Strings

Edit both files and replace `YOUR_MONGODB_CONNECTION_STRING_HERE` with your actual MongoDB connection string.

**Example:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/?appName=YourApp",
    "DatabaseName": "LifeForgeDb",
    "QuestsCollectionName": "Quests"
  }
}
```

### 3. Security Notes

- **Never commit** `appsettings.json` or `appsettings.Development.json` files
- The `.gitignore` file is configured to exclude these files automatically
- Only commit `appsettings.Template.json` with placeholder values
- For production deployments, use environment variables or Azure Key Vault instead of configuration files

## Image Storage

Images are now stored as Base64-encoded strings directly in MongoDB instead of the file system:
- **ImageData**: Base64-encoded image string
- **ImageContentType**: MIME type (e.g., `image/jpeg`, `image/png`)
- **ImageName**: Original filename for reference

This approach:
- ? Keeps all data in one place (MongoDB)
- ? Simplifies deployment (no need to manage separate file storage)
- ? Works well for small to medium-sized images
- ?? Consider using external storage (Azure Blob Storage, AWS S3) for very large images or high-volume applications
