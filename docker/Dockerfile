FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for optimal layer caching
COPY PixelAcademy.sln .
COPY src/PixelAcademy.Domain/PixelAcademy.Domain.csproj src/PixelAcademy.Domain/
COPY src/PixelAcademy.Application/PixelAcademy.Application.csproj src/PixelAcademy.Application/
COPY src/PixelAcademy.Infrastructure/PixelAcademy.Infrastructure.csproj src/PixelAcademy.Infrastructure/
COPY src/PixelAcademy.API/PixelAcademy.API.csproj src/PixelAcademy.API/

# Restore dependencies
RUN dotnet restore src/PixelAcademy.API/PixelAcademy.API.csproj

# Copy source code and build
COPY . .
RUN dotnet publish src/PixelAcademy.API/PixelAcademy.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=http://+:8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health/ready || exit 1

ENTRYPOINT ["dotnet", "PixelAcademy.API.dll"]
