# Aplicação de teste

### Requisitos para rodar o sistema
- **Docker Engine**: 20.10.0 or later
- **Docker Compose**: 2.0.0 or later (Compose file format 3.8 compatible)

### Diagrama simples da solução
- InsuranceSystem.jpg

### Iniciar o sistema usando PowerShell (Windows)
```powershell
.\start-services.ps1
```

### Iniciar o sistema usando Bash (Linux/Mac/WSL)
```bash
chmod +x start-services.sh
./start-services.sh
```

### Acesse o sistema
- Acesse http://localhost:5155/

### Observações sobre a arquitetura

 - As regras de negócio estão todas no projeto InsuranceCoreBusiness. Este projeto é puro C# e não tem nenhuma dependência externa. Ele define as entidades, os use cases e as interfaces que deverão ser implementadas pelas camadas externas (plug-ins)
 - Os microserviços InsurancePropostaService e InsuranceContratacaoService implementam as Outbound interfaces. Ambos dependem (ou conhecem) o core business, mas são independentes um do outro. Eles se comunicam através do RabbitMQ em duas operações: solicitação de status de proposta e solicitação de contratação de proposta
 - O projeto InsuranceWeb é uma interface simples para operação do sistema. Este projeto é totalmente independente, e se comunica com os microserviços para realizar as operações via HTTP
