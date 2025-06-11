# Contoso Hotels Dev Container Guide (.NET 8)

This project uses a Docker-based development environment with .NET 8 to ensure consistent development across different machines. It replaces the need for LocalDB with a containerized SQL Server instance and implements security best practices for handling sensitive information.

## Why SQL Server in Docker instead of LocalDB?

LocalDB is a lightweight version of SQL Server that's designed for development on Windows machines. However, it has several limitations:

1. **Platform Limitation**: LocalDB only works on Windows and cannot be used in Linux-based containers
2. **Containerization Issues**: LocalDB cannot be properly containerized in a Docker environment
3. **Cross-Platform Development**: Using SQL Server in a container provides a consistent experience across all platforms
4. **Production Similarity**: The containerized SQL Server is more similar to production environments

By using SQL Server in a container, we ensure that all developers have the same database experience regardless of their operating system or development environment.

## Security-First Approach

This project follows a security-first approach:

1. **No hardcoded credentials**: All sensitive information is stored in environment variables
2. **Environment-specific configuration**: Different environments use different configuration files
3. **Auto-generated secure passwords**: For development, secure random passwords are generated automatically
4. **Proper secret management**: For Codespaces, GitHub Secrets can be used for shared development

### Secure Password Management

The SQL Server password is managed securely:

- **During Initialization**: A secure random password is automatically generated when the container starts
- **No Committed Secrets**: The password is never stored in files that would be committed to source control
- **Team Development**: GitHub Codespaces can use repository or organization secrets for team-wide consistency

See [CODESPACES_SECURE_PASSWORD.md](.devcontainer/CODESPACES_SECURE_PASSWORD.md) for detailed information.
3. **Template files for guidance**: Example files show the structure without exposing actual credentials
4. **Gitignore protection**: Sensitive files are excluded from version control

This approach ensures that no sensitive information is accidentally committed to the repository while maintaining a convenient developer experience.

## Requirements

- Docker Desktop
- Visual Studio Code with Remote - Containers extension
- Git (for cloning the repository)
- Basic understanding of environment variables
- Access to create and modify local files

## Getting Started

1. Clone the repository
2. Copy `.env.example` to `.env` in the project root and set a secure SQL password
3. Copy `.devcontainer/.env.example` to `.devcontainer/.env` and set the same password
4. Open the project folder in VS Code
5. When prompted, click "Reopen in Container" or run the "Remote-Containers: Reopen in Container" command from the Command Palette
6. The container will build and start up (this may take a few minutes the first time)
7. After the container is built, the database will be initialized automatically

## Database Connection

Inside the container, the database connection string is configured to use environment variables:
```
Server=db;Database=ContosoHotelsDb;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=True;MultipleActiveResultSets=true
```

The `${SQL_PASSWORD}` placeholder is replaced at runtime with the value from the SQL_PASSWORD environment variable.

You can connect to the database using SQL Server Management Studio or the VS Code SQL Server extension with:
- Server: localhost,1433
- Authentication: SQL Login
- User: sa
- Password: *Use the password you set in your .env file*

## Running the Application

To run the application:

1. In VS Code, run the "run-contoso-hotels" task from the Command Palette or Terminal
2. Navigate to http://localhost:5000 in your browser

## Common Tasks

- **Update Database Schema**: `dotnet ef database update`
- **Add a Migration**: `dotnet ef migrations add MigrationName`
- **Reset Database**: `dotnet ef database drop` followed by `dotnet ef database update`

## Helper Scripts

The repository includes several helper scripts to make development easier:

### For Windows Development (Outside Container)

- **docker-start.ps1**: Starts a SQL Server container for local development
  ```powershell
  ./docker-start.ps1
  ```
  This script automatically loads environment variables from your `.env` file.

- **load-env.ps1**: Loads environment variables from `.env` into your current PowerShell session
  ```powershell
  ./load-env.ps1
  ```
  This is useful when running commands that need access to environment variables.

### Inside the Container

- **init-db.sh**: Initializes the database (automatically run during container setup)
  This script checks if the database exists and creates it if needed, then runs migrations.

## Troubleshooting

- If the SQL Server container doesn't start correctly, try restarting Docker Desktop
- If you see connection errors, make sure port 1433 is not in use by another application
- Check the database initialization script logs for any errors during container setup

## Security and Environment Variables

This project uses environment variables to manage sensitive information like database credentials:

### Environment Files
- `.env` in the project root - Contains environment variables for local development
- `.devcontainer/.env` - Contains environment variables for container development
- Both files are excluded from version control via `.gitignore`

### Setting Up Environment Variables
1. Copy the example files to create your own environment files:
   ```
   cp .env.example .env
   cp .devcontainer/.env.example .devcontainer/.env
   ```
2. Edit the files and set a secure password for `SQL_PASSWORD`
3. Use the same password in both files for consistency

### How Environment Variables Are Used
- In `docker-compose.yml`, `${SQL_PASSWORD}` pulls the value from environment
- In `appsettings.Development.json`, placeholders are replaced at runtime
- The `Startup.cs` file handles substituting the placeholders with actual values

For more information on security practices, see [SECURITY.md](SECURITY.md) in the project root.
