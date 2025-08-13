#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to compare versions
compare_version() {
    if [[ $1 == $2 ]]; then
        return 0
    fi
    local IFS=.
    local i ver1=($1) ver2=($2)
    # fill empty fields in ver1 with zeros
    for ((i=${#ver1[@]}; i<${#ver2[@]}; i++)); do
        ver1[i]=0
    done
    for ((i=0; i<${#ver1[@]}; i++)); do
        if [[ -z ${ver2[i]} ]]; then
            # fill empty fields in ver2 with zeros
            ver2[i]=0
        fi
        if ((10#${ver1[i]} > 10#${ver2[i]})); then
            return 1
        fi
        if ((10#${ver1[i]} < 10#${ver2[i]})); then
            return 2
        fi
    done
    return 0
}

# Check if Docker is running
echo -e "${CYAN}Checking Docker status...${NC}"
if ! docker info >/dev/null 2>&1; then
    echo -e "${RED}ERROR: Docker is not running. Please start Docker and try again.${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Docker is running${NC}"

# Check Docker version
echo -e "${CYAN}Checking Docker version...${NC}"
DOCKER_VERSION_OUTPUT=$(docker --version 2>/dev/null)
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Unable to get Docker version.${NC}"
    exit 1
fi

DOCKER_VERSION=$(echo "$DOCKER_VERSION_OUTPUT" | sed -n 's/Docker version \([0-9]*\.[0-9]*\.[0-9]*\).*/\1/p')
MIN_DOCKER_VERSION="20.10.0"

compare_version "$DOCKER_VERSION" "$MIN_DOCKER_VERSION"
case $? in
    2)
        echo -e "${RED}ERROR: Docker version $DOCKER_VERSION is too old. Minimum required version is $MIN_DOCKER_VERSION${NC}"
        exit 1
        ;;
    *)
        echo -e "${GREEN}✓ Docker version $DOCKER_VERSION is compatible${NC}"
        ;;
esac

# Check Docker Compose version
echo -e "${CYAN}Checking Docker Compose version...${NC}"
# Try docker compose version first (newer syntax)
COMPOSE_VERSION_OUTPUT=$(docker compose version 2>/dev/null)
if [ $? -ne 0 ]; then
    # Fall back to docker-compose version (older syntax)
    COMPOSE_VERSION_OUTPUT=$(docker-compose --version 2>/dev/null)
    if [ $? -ne 0 ]; then
        echo -e "${RED}ERROR: Docker Compose is not installed or not accessible.${NC}"
        exit 1
    fi
fi

# Parse version from output (handles both formats)
if [[ $COMPOSE_VERSION_OUTPUT =~ v?([0-9]+\.[0-9]+\.[0-9]+) ]]; then
    COMPOSE_VERSION=${BASH_REMATCH[1]}
else
    echo -e "${RED}ERROR: Unable to parse Docker Compose version from: $COMPOSE_VERSION_OUTPUT${NC}"
    exit 1
fi

MIN_COMPOSE_VERSION="2.0.0"

compare_version "$COMPOSE_VERSION" "$MIN_COMPOSE_VERSION"
case $? in
    2)
        echo -e "${RED}ERROR: Docker Compose version $COMPOSE_VERSION is too old. Minimum required version is $MIN_COMPOSE_VERSION${NC}"
        exit 1
        ;;
    *)
        echo -e "${GREEN}✓ Docker Compose version $COMPOSE_VERSION is compatible${NC}"
        ;;
esac

echo ""
echo -e "${GREEN}All prerequisites satisfied. Starting services...${NC}"
echo ""

# Stop and remove existing containers
echo -e "${YELLOW}Stopping and removing existing containers...${NC}"
docker-compose down

# Remove existing images to force rebuild
echo -e "${YELLOW}Removing existing images...${NC}"
docker rmi insurance-insurance-web insurance-insurance-proposta-service insurance-insurance-contratacao-service 2>/dev/null || true

# Build and start all services
echo -e "${GREEN}Building and starting all services...${NC}"
docker-compose up --build -d

# Show running containers
echo -e "${CYAN}Running containers:${NC}"
docker ps

echo ""
echo -e "${GREEN}Services are starting up. Please wait a moment for all services to be ready.${NC}"
echo ""
echo -e "${CYAN}Access points:${NC}"
echo "- Insurance Web App: http://localhost:5155"
echo "- Insurance Proposta Service API: http://localhost:5298/swagger"
echo "- Insurance Contratacao Service API: http://localhost:5088/swagger"
echo "- RabbitMQ Management: http://localhost:15672 (admin/admin)"
echo "- PostgreSQL: localhost:5432 (postgres/postgres)"
echo ""
echo -e "${YELLOW}To check logs, use: docker-compose logs -f [service-name]${NC}"
echo -e "${YELLOW}To stop all services, use: docker-compose down${NC}"
