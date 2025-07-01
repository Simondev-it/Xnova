# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["Xnova.API/Xnova.API.csproj", "Xnova.API/"]
RUN dotnet restore "Xnova.API/Xnova.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Xnova.API"
RUN dotnet publish "Xnova.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Xnova.API.dll"]
