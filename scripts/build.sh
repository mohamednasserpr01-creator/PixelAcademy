#!/bin/bash
set -e

echo "Restoring dependencies..."
dotnet restore PixelAcademy.sln

echo "Building solution..."
dotnet build PixelAcademy.sln --no-restore

echo "Build completed successfully!"
