# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY UnitGenius-GenerationService.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY . ./

# Build the application
RUN dotnet publish UnitGenius-GenerationService.csproj -c Release -o out

# Create a runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

# Set the working directory to /app
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build-env /app/out .

# Define the entry point for the container
ENTRYPOINT ["dotnet", "UnitGenius-GenerationService.dll"]