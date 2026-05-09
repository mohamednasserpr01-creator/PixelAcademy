#!/bin/bash
set -e

echo "Starting infrastructure services..."
docker-compose -f docker/docker-compose.dev.yml up -d

echo "Running API..."
cd src/PixelAcademy.API
dotnet run
