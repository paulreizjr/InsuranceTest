@echo off
echo Insurance RabbitMQ Messaging Test Script
echo =======================================

echo.
echo Checking Docker status...
docker ps >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: Docker is not running. Please start Docker Desktop.
    pause
    exit /b 1
) else (
    echo ✓ Docker is running
)

echo.
echo Starting RabbitMQ and PostgreSQL...
docker-compose up -d

echo.
echo Waiting for services to start...
timeout /t 10 /nobreak >nul

echo.
echo Building the solution...
dotnet build
if %errorlevel% neq 0 (
    echo Error: Build failed
    pause
    exit /b 1
) else (
    echo ✓ Solution built successfully
)

echo.
echo Setup complete! Next steps:
echo 1. Start InsurancePropostaService:
echo    cd InsurancePropostaService ^&^& dotnet run
echo 2. Start InsuranceContratacaoService:
echo    cd InsuranceContratacaoService ^&^& dotnet run
echo 3. Test the messaging endpoint:
echo    curl -X GET "https://localhost:7001/api/Contrato/proposta/{propostaId}/status"
echo.
echo RabbitMQ Management UI: http://localhost:15672
echo PostgreSQL: localhost:5432 (postgres/postgres)

pause
