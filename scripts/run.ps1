Write-Host "Starting infrastructure services..." -ForegroundColor Cyan
docker-compose -f docker/docker-compose.dev.yml up -d

Write-Host "Running API..." -ForegroundColor Cyan
cd src/PixelAcademy.API
dotnet run
