#!/bin/bash
set -e

echo "Building and starting PixelAcademy with Docker..."
docker-compose -f docker/docker-compose.yml up --build
