# First stage: Node.js build for the frontend
FROM node:latest as build-node
WORKDIR /app/frontend

# Copy package.json and package-lock.json (if available)
COPY ClientApp/package*.json ./

# Install dependencies
RUN npm install

# Copy the frontend source code
COPY ClientApp/ .

# Build the frontend
RUN npm run build

#-------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore any dependencies (via NuGet)
COPY *.csproj ./
RUN dotnet restore

# Copy the backend source code and frontend build
COPY . ./
COPY --from=build-node /app/frontend/build ./wwwroot

# Copy the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port the app runs on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "ConfigStream.Api.dll"]
