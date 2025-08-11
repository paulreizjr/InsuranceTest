# Insurance Application Docker Setup

This Docker Compose configuration sets up the complete Insurance application stack with all dependencies.

## Requirements

### Docker Requirements
- **Docker Engine**: 20.10.0 or later
- **Docker Compose**: 2.0.0 or later (Compose file format 3.8 compatible)

### Tested Versions
- Docker Engine: 28.3.0
- Docker Compose: 2.38.1

### System Requirements
- **Windows**: Docker Desktop for Windows
- **Linux**: Docker Engine + Docker Compose plugin
- **macOS**: Docker Desktop for Mac

## Services

- **PostgreSQL**: Database server (port 5432)
- **RabbitMQ**: Message broker with management interface (ports 5672, 15672)
- **Insurance Proposta Service**: API service for proposals (ports 5298, 7244)
- **Insurance Contratacao Service**: API service for contracts (ports 5088, 7217)
- **Insurance Web**: Web application (ports 5155, 7130)

## Quick Start

### Using PowerShell (Windows)
```powershell
.\start-services.ps1
```

### Using Bash (Linux/Mac/WSL)
```bash
chmod +x start-services.sh
./start-services.sh
```

### Manual Start
```bash
# Build and start all services
docker-compose up --build -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

## Access Points

- **Insurance Web App**: http://localhost:5155
- **Insurance Proposta Service API**: http://localhost:5298/swagger
- **Insurance Contratacao Service API**: http://localhost:5088/swagger
- **RabbitMQ Management**: http://localhost:15672 (username: admin, password: admin)
- **PostgreSQL**: localhost:5432 (username: postgres, password: postgres, database: insurance_db)

## Service Dependencies

The services start in the following order:
1. PostgreSQL and RabbitMQ (with health checks)
2. Insurance Proposta Service (depends on PostgreSQL and RabbitMQ)
3. Insurance Contratacao Service (depends on RabbitMQ)
4. Insurance Web (depends on both API services)

## Useful Commands

```bash
# View running containers
docker ps

# View logs for a specific service
docker-compose logs -f insurance-web
docker-compose logs -f insurance-proposta-service
docker-compose logs -f insurance-contratacao-service

# Restart a specific service
docker-compose restart insurance-web

# Rebuild a specific service
docker-compose up --build insurance-web

# Stop all services
docker-compose down

# Stop all services and remove volumes (WARNING: This will delete database data)
docker-compose down -v

# View service status
docker-compose ps
```

## Environment Variables

The services are configured with the following environment variables:

### Database Connection
- Connection string points to the PostgreSQL container
- Database: insurance_db
- Username: postgres
- Password: postgres

### RabbitMQ Configuration
- Hostname: rabbitmq (container name)
- Username: admin
- Password: admin

### Service URLs
- Services communicate using internal Docker network
- Web app connects to API services using container names

## Troubleshooting

### Services not starting
1. Check if ports are already in use: `netstat -an | findstr "5155\|5298\|5088\|5432\|5672\|15672"`
2. View logs: `docker-compose logs [service-name]`
3. Ensure Docker is running and has sufficient resources

### Database connection issues
1. Wait for PostgreSQL to be fully ready (health check ensures this)
2. Check if migrations need to be run
3. Verify connection string in appsettings.json

### RabbitMQ connection issues
1. Wait for RabbitMQ to be fully ready (health check ensures this)
2. Check RabbitMQ management interface at http://localhost:15672
3. Verify RabbitMQ configuration in services

## Configuration Files

The project uses environment-specific configuration files:

### appsettings.json (Default - Local Development)
- Database: localhost:5432
- RabbitMQ: localhost:5672
- API URLs: localhost with HTTPS ports

### appsettings.Development.json (Container Environment)
- Database: postgres:5432 (container name)
- RabbitMQ: rabbitmq:5672 (container name)  
- API URLs: container names with HTTP port 8080

The Development environment configuration is automatically used when running in containers, allowing seamless service-to-service communication using Docker's internal networking.

## Database Setup

- **Automatic Migrations**: The InsurancePropostaService automatically runs Entity Framework migrations on startup in Development environment
- **Database Schema**: Tables are created automatically when the service starts
- **Connection**: Services connect to PostgreSQL using container networking
