# Stop and remove existing containers
Write-Host "Stopping and removing existing containers..." -ForegroundColor Yellow
docker-compose down

# Remove existing images to force rebuild
Write-Host "Removing existing images..." -ForegroundColor Yellow
docker rmi insurance-insurance-web insurance-insurance-proposta-service insurance-insurance-contratacao-service 2>$null

# Build and start all services
Write-Host "Building and starting all services..." -ForegroundColor Green
docker-compose up --build -d

# Show running containers
Write-Host "Running containers:" -ForegroundColor Cyan
docker ps

Write-Host ""
Write-Host "Services are starting up. Please wait a moment for all services to be ready." -ForegroundColor Green
Write-Host ""
Write-Host "Access points:" -ForegroundColor Cyan
Write-Host "- Insurance Web App: http://localhost:5155"
Write-Host "- Insurance Proposta Service API: http://localhost:5298/swagger"
Write-Host "- Insurance Contratacao Service API: http://localhost:5088/swagger"
Write-Host "- RabbitMQ Management: http://localhost:15672 (admin/admin)"
Write-Host "- PostgreSQL: localhost:5432 (postgres/postgres)"
Write-Host ""
Write-Host "To check logs, use: docker-compose logs -f [service-name]" -ForegroundColor Yellow
Write-Host "To stop all services, use: docker-compose down" -ForegroundColor Yellow
