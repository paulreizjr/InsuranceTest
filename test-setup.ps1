# PowerShell script to test the RabbitMQ messaging implementation

Write-Host "Insurance RabbitMQ Messaging Test Script" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# Check if Docker is running
Write-Host "`nChecking Docker status..." -ForegroundColor Yellow
try {
    docker ps | Out-Null
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Start infrastructure services
Write-Host "`nStarting RabbitMQ and PostgreSQL..." -ForegroundColor Yellow
docker-compose up -d

# Wait for services to be ready
Write-Host "`nWaiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check if RabbitMQ is accessible
Write-Host "`nChecking RabbitMQ connectivity..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:15672" -Method GET -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ RabbitMQ Management UI is accessible at http://localhost:15672" -ForegroundColor Green
        Write-Host "  Username: admin, Password: admin" -ForegroundColor Cyan
    }
}
catch {
    Write-Host "✗ RabbitMQ is not accessible yet. Please wait a few more seconds." -ForegroundColor Red
}

# Build the solution
Write-Host "`nBuilding the solution..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Solution built successfully" -ForegroundColor Green
}
else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host "`nSetup complete! Next steps:" -ForegroundColor Green
Write-Host "1. Start InsurancePropostaService:" -ForegroundColor Cyan
Write-Host "   cd InsurancePropostaService; dotnet run" -ForegroundColor White
Write-Host "2. Start InsuranceContratacaoService:" -ForegroundColor Cyan  
Write-Host "   cd InsuranceContratacaoService; dotnet run" -ForegroundColor White
Write-Host "3. Test the messaging endpoint:" -ForegroundColor Cyan
Write-Host '   curl -X GET "https://localhost:7001/api/Contrato/proposta/{propostaId}/status"' -ForegroundColor White

Write-Host "`nRabbitMQ Management UI: http://localhost:15672" -ForegroundColor Yellow
Write-Host "PostgreSQL: localhost:5432 (postgres/postgres)" -ForegroundColor Yellow
