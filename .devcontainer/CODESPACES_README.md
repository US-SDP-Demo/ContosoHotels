# Contoso Hotels - GitHub Codespaces Setup

This document provides information about running the Contoso Hotels project in GitHub Codespaces.

## Automated Setup

The devcontainer configuration automatically sets up:

1. .NET 8 development environment
2. SQL Server 2019 database
3. Required VS Code extensions
4. Database initialization with sample data

## Environment Variables

The following environment variables are automatically set in the Codespaces environment:

- `SQL_PASSWORD`: Set to a default value for development
- `ASPNETCORE_ENVIRONMENT`: Set to "Development"
- `ASPNETCORE_URLS`: Configured to use ports 5000 (HTTP) and 5001 (HTTPS)

## Ports

The following ports are forwarded in Codespaces:

- 5000: HTTP application endpoint
- 5001: HTTPS application endpoint
- 1433: SQL Server database

## Getting Started

After the Codespace is created:

1. The database is automatically initialized with sample data
2. You can run the application using the "run-contoso-hotels" task in VS Code

## Troubleshooting

If you encounter any issues:

1. Check the terminal output for any error messages
2. Verify that the SQL Server container is running with `docker ps`
3. You can manually run the database initialization script with `./.devcontainer/init-db.sh`

## Important Notes

- The default database password is used for development only and should be changed in production
- All data in the Codespaces environment is temporary and will be lost when the Codespace is deleted
