Write-Host "Restoring dependencies..." -ForegroundColor Cyan
dotnet restore PixelAcademy.sln

Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build PixelAcademy.sln --no-restore

Write-Host "Build completed successfully!" -ForegroundColor Green
