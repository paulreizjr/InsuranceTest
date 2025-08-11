# Aplicação de teste

- Esta aplicação roda em 5 containers Docker

- **Insurance Web App**: http://localhost:5155
- **Insurance Proposta Service API**: http://localhost:5298/swagger
- **Insurance Contratacao Service API**: http://localhost:5088/swagger
- **RabbitMQ Management**: http://localhost:15672 (username: admin, password: admin)
- **PostgreSQL**: localhost:5432 (username: postgres, password: postgres, database: insurance_db)

- Veja o diagrama simples da solução: InsuranceSystem.jpg

- Para inicar utilize o comando abaixo

### Using PowerShell (Windows)
```powershell
.\start-services.ps1
```

### Using Bash (Linux/Mac/WSL)
```bash
chmod +x start-services.sh
./start-services.sh
```

- Veja mais detalhes e requisitos no README_Docker.md