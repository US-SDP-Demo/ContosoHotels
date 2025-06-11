# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install SQL Server tools and dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        apt-transport-https \
        gnupg \
        lsb-release \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/msprod.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev \
    && echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Install Entity Framework Core tools globally for .NET 8
RUN dotnet tool install --global dotnet-ef --version 8.0.0

# Ensure dotnet tools are in PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Set the working directory
WORKDIR /workspace

# Generate dev certificate for HTTPS without trust prompt (non-interactive)
RUN dotnet dev-certs https

# Expose ports for the application
EXPOSE 5000 5001

# Set the entry point - use sleep infinity to keep container running
CMD ["sleep", "infinity"]
