# MongoDB and API Configuration - DO NOT COMMIT SECRETS!

## Important Security Notes

Your MongoDB connection string contains sensitive credentials. Add these to .gitignore:

```gitignore
# Sensitive configuration files
**/appsettings.Development.json
**/appsettings.Production.json

# User secrets
**/secrets.json

# Uploaded images (optional - depends on your needs)
**/wwwroot/images/quests/*
!**/wwwroot/images/quests/.gitkeep
```

## Using .NET User Secrets (Recommended)

Instead of storing the connection string in appsettings.json, use User Secrets:

### Setup User Secrets:
```bash
cd LifeForge.Api
dotnet user-secrets init
dotnet user-secrets set "MongoDbSettings:ConnectionString" "your-actual-connection-string-here"
```

### Benefits:
- ? Secrets stored outside of project directory
- ? Not committed to source control
- ? Easy to manage per developer
- ? Works automatically in development

### Update appsettings.Development.json:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "MongoDbSettings": {
    "ConnectionString": "PLACEHOLDER - Set via User Secrets",
    "DatabaseName": "LifeForgeDb",
    "QuestsCollectionName": "Quests"
  }
}
```

## Environment Variables for Production

For production deployments (Azure, AWS, etc.), use environment variables:

### Azure App Service:
1. Go to Configuration ? Application settings
2. Add:
   - Name: `MongoDbSettings__ConnectionString`
   - Value: Your connection string

### Docker:
```dockerfile
docker run -e MongoDbSettings__ConnectionString="your-connection-string" your-app
```

### appsettings.json for Production:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "PLACEHOLDER - Set via Environment Variables or Azure Key Vault",
    "DatabaseName": "LifeForgeDb",
    "QuestsCollectionName": "Quests"
  }
}
```

## What to Commit

? **DO commit:**
- appsettings.json (with placeholders)
- Code files
- Project files (.csproj)
- README and documentation

? **DO NOT commit:**
- Actual connection strings
- Passwords or API keys
- appsettings.Development.json (with real values)
- User secrets

## Checking for Secrets Before Commit

Before committing, search for:
```bash
# Search for MongoDB connection strings
git grep "mongodb+srv://"

# Search for potential passwords
git grep -i "password"
```

If found in staged files, remove them!

## Azure Key Vault (For Production)

For enterprise/production use, store secrets in Azure Key Vault:

1. Create an Azure Key Vault
2. Add your connection string as a secret
3. Configure your app to use Key Vault:

```csharp
// In Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

## Summary

?? **Security Checklist:**
- [ ] Add appsettings.Development.json to .gitignore
- [ ] Use User Secrets for local development
- [ ] Use Environment Variables or Key Vault for production
- [ ] Never commit connection strings to Git
- [ ] Review commits before pushing
- [ ] Use different credentials for dev/staging/production

**Remember:** Once a secret is committed to Git, it's in the history forever. If this happens, rotate your credentials immediately!
