# Sky-Link Airlines Docker Setup Script for Windows

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Sky-Link Airlines Docker Setup" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Create backup directory
New-Item -ItemType Directory -Force -Path "backup"

# Check if Docker is installed
$dockerCheck = docker --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker is not installed. Please install Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Build and run containers
Write-Host "Building and starting containers..." -ForegroundColor Yellow
docker-compose up -d --build

# Wait for MongoDB to be ready
Write-Host "Waiting for MongoDB to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check container status
Write-Host "`nContainer Status:" -ForegroundColor Green
docker-compose ps

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "MongoDB: localhost:27017" -ForegroundColor White
Write-Host "MongoDB Express: http://localhost:8081" -ForegroundColor White
Write-Host "  Username: admin" -ForegroundColor White
Write-Host "  Password: admin123" -ForegroundColor White
Write-Host "`nTo view logs: docker-compose logs -f" -ForegroundColor Yellow
Write-Host "To stop: docker-compose down" -ForegroundColor Yellow
Write-Host "To stop and remove volumes: docker-compose down -v" -ForegroundColor Yellow