# Function to compare versions
function Compare-Version {
    param(
        [string]$Version1,
        [string]$Version2
    )
    
    $v1 = [Version]$Version1
    $v2 = [Version]$Version2
    
    return $v1.CompareTo($v2)
}

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Cyan
try {
    $dockerInfo = docker info 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Docker is not running. Please start Docker Desktop and try again." -ForegroundColor Red
        exit 1
    }
    Write-Host " Docker is running" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Docker is not installed or not accessible. Please install Docker and try again." -ForegroundColor Red
    exit 1
}

# Check Docker version
Write-Host "Checking Docker version..." -ForegroundColor Cyan
try {
    $dockerVersionOutput = docker --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Unable to get Docker version." -ForegroundColor Red
        exit 1
    }
    
    $dockerVersion = ($dockerVersionOutput -split ' ')[2] -replace ',', ''
    $minDockerVersion = "20.10.0"
    
    if ((Compare-Version $dockerVersion $minDockerVersion) -lt 0) {
        Write-Host "ERROR: Docker version $dockerVersion is too old. Minimum required version is $minDockerVersion" -ForegroundColor Red
        exit 1
    }
    Write-Host " Docker version $dockerVersion is compatible" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Unable to parse Docker version." -ForegroundColor Red
    exit 1
}

# Check Docker Compose version
Write-Host "Checking Docker Compose version..." -ForegroundColor Cyan
try {
    # Try docker compose version first (newer syntax)
    $composeVersionOutput = docker compose version 2>$null
    if ($LASTEXITCODE -ne 0) {
        # Fall back to docker-compose version (older syntax)
        $composeVersionOutput = docker-compose --version 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERROR: Docker Compose is not installed or not accessible." -ForegroundColor Red
            exit 1
        }
    }
    
    # Parse version from output (handles both formats)
    if ($composeVersionOutput -match 'v?(\d+\.\d+\.\d+)') {
        $composeVersion = $matches[1]
    } else {
        Write-Host "ERROR: Unable to parse Docker Compose version from: $composeVersionOutput" -ForegroundColor Red
        exit 1
    }
    
    $minComposeVersion = "2.0.0"
    
    if ((Compare-Version $composeVersion $minComposeVersion) -lt 0) {
        Write-Host "ERROR: Docker Compose version $composeVersion is too old. Minimum required version is $minComposeVersion" -ForegroundColor Red
        exit 1
    }
    Write-Host " Docker Compose version $composeVersion is compatible" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Unable to check Docker Compose version." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "All prerequisites satisfied. Starting services..." -ForegroundColor Green
Write-Host ""

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
