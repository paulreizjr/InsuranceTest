#!/bin/bash

# Stop and remove existing containers
echo "Stopping and removing existing containers..."
docker-compose down

# Remove existing images to force rebuild
echo "Removing existing images..."
docker rmi insurance-insurance-web insurance-insurance-proposta-service insurance-insurance-contratacao-service 2>/dev/null || true

# Build and start all services
echo "Building and starting all services..."
docker-compose up --build -d

# Show running containers
echo "Running containers:"
docker ps

echo ""
echo "Services are starting up. Please wait a moment for all services to be ready."
echo ""
echo "Access points:"
echo "- Insurance Web App: http://localhost:5155"
echo "- Insurance Proposta Service API: http://localhost:5298/swagger"
echo "- Insurance Contratacao Service API: http://localhost:5088/swagger"
echo "- RabbitMQ Management: http://localhost:15672 (admin/admin)"
echo "- PostgreSQL: localhost:5432 (postgres/postgres)"
echo ""
echo "To check logs, use: docker-compose logs -f [service-name]"
echo "To stop all services, use: docker-compose down"
