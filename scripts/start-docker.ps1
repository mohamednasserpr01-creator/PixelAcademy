Write-Host "Building and starting PixelAcademy with Docker..." -ForegroundColor Cyan
docker-compose -f docker/docker-compose.yml up --build
