# Finance — Sistema de Transferências Financeiras

API REST para gerenciamento de contas e realização de transferências financeiras imediatas e agendadas.

## Tecnologias utilizadas

**Backend**
- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI

**Frontend**
- Vue.js 3
- JavaScript
- Vite
- HTML5 e CSS3

**Testes**
- xUnit
- Moq
- ASP.NET Core WebApplicationFactory
- Testes unitários e de integração

## Arquitetura

O backend utiliza uma abordagem baseada em Clean Architecture, com separação de responsabilidades entre as seguintes camadas:

```text
Finance/
├── Finance.Api/
├── Finance.Application/
├── Finance.Domain/
├── Finance.Infrastructure/
├── Finance.IntegrationTests/
├── Finance.UnitTests/
├── finance-frontend/
├── Finance.sln
├── docker-compose.yml
├── .dockerignore
├── .env.example
├── .gitignore
└── README.md

finance-frontend/
└── src/
    ├── services/
    ├── App.vue
    ├── main.js
    └── style.css
```

- **Domain:** entidades, enums e regras de negócio.
- **Application:** casos de uso, DTOs e interfaces.
- **Infrastructure:** persistência de dados, Entity Framework Core e repositórios.
- **Api:** endpoints REST, configuração da aplicação, middleware e processamento em segundo plano.
- **UnitTests:** testes unitários das regras de negócio.
- **IntegrationTests:** testes de integração dos endpoints da API.

## Funcionalidades

### Gerenciamento de contas
- Criação e consulta de contas.
- Controle de saldo e limite de cheque especial.
- Controle de status das contas: Active, Inactive e Blocked.

### Transferências financeiras
- Transferências imediatas.
- Transferências agendadas para datas futuras.
- Cancelamento de transferências agendadas.
- Histórico de transferências.
- Débito e crédito entre contas.
- Utilização do limite de cheque especial quando necessário.

### Regras e segurança operacional
- Limites de valor e quantidade de tentativas por hora.
- Regras diferenciadas para períodos diurno e noturno.
- Registro de tentativas de transferência, inclusive rejeitadas por limites.
- Validação das contas de origem e destino.
- Controle de concorrência utilizando RowVersion.
- Tratamento centralizado de exceções.

### Processamento de agendamentos
As transferências agendadas são processadas automaticamente por um BackgroundService, que verifica periodicamente as transferências pendentes e realiza as validações necessárias antes da execução.

## Execução com Docker

A aplicação pode ser executada integralmente utilizando Docker Compose, sem a necessidade de instalar o SQL Server, .NET SDK ou Node.js na máquina.

### Pré-requisitos

- Docker Desktop instalado e em execução.

### Configuração

Na raiz do projeto, crie um arquivo `.env` com a senha do usuário `sa` do SQL Server:

```env
MSSQL_SA_PASSWORD=SuaSenhaForteAqui
```

> O arquivo `.env` não deve ser versionado, pois contém credenciais.

### Inicialização

Execute na raiz do projeto:

```bash
docker compose up -d --build
```

O Docker Compose irá construir e iniciar os seguintes serviços:

- **SQL Server:** banco de dados da aplicação.
- **API:** aplicação backend em .NET 8.
- **Frontend:** aplicação Vue 3 servida pelo Nginx.

As migrations e os dados iniciais dos limites de transferência são aplicados automaticamente pela API.

### Acessos

| Serviço | Endereço |
|---|---|
| Frontend | http://localhost:5173 |
| API / Swagger | http://localhost:5007/swagger |
| SQL Server | localhost:1434 |

### Gerenciamento dos containers

Visualizar o status dos serviços:

```bash
docker compose ps
```

Visualizar os logs:

```bash
docker compose logs -f
```

Encerrar os serviços:

```bash
docker compose down
```

Os dados do banco são armazenados em um volume Docker persistente e permanecem disponíveis após a recriação dos containers.

Para remover também os dados persistidos, utilize:

```bash
docker compose down -v
```

## Execução manual

### Pré-requisitos

- .NET 8 SDK
- SQL Server
- Node.js
- npm

### 1. Configuração do banco de dados

Crie um banco de dados chamado `FinanceDb` no SQL Server.

Configure a connection string no `appsettings.json` do projeto `Finance.Api`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FinanceDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Ajuste os parâmetros conforme a configuração do seu ambiente.

### 2. Aplicação das migrations

Na raiz da solução, execute:

```bash
dotnet ef database update --project Finance.Infrastructure --startup-project Finance.Api
```

Caso necessário, instale a ferramenta EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

### 3. Configuração dos limites de transferência

Os limites iniciais podem ser cadastrados no banco de dados:

```sql
INSERT INTO TransferLimits (Id, Period, MaxAmountPerHour, MaxAttemptsPerHour)
VALUES
    (NEWID(), 'Day', 5000.00, 10),
    (NEWID(), 'Night', 2000.00, 5);
```

Os valores representam uma configuração inicial e podem ser ajustados conforme necessário.

### 4. Execução do backend

Execute o projeto `Finance.Api` pelo Visual Studio ou pelo terminal:

```bash
dotnet run --project Finance.Api
```

A documentação da API estará disponível em:

`http://localhost:5007/swagger`

### 5. Execução do frontend

Entre na pasta `finance-frontend`:

```bash
cd finance-frontend
```

Instale as dependências:

```bash
npm install
```

Configure o arquivo `.env` na raiz do frontend:

```env
VITE_API_URL=http://localhost:5007
```

Inicie a aplicação:

```bash
npm run dev
```

O frontend estará disponível em:

`http://localhost:5173`

## Execução dos testes

Na raiz da solução, execute:

```bash
dotnet test
```

Os testes de integração utilizam um banco separado, chamado `FinanceDb_IntegrationTests`, evitando interferência nos dados do ambiente principal.

Certifique-se de que o SQL Server esteja disponível e que o usuário da conexão tenha permissão para criar e modificar esse banco.

## Decisões técnicas

- **Clean Architecture:** separação entre domínio, aplicação, infraestrutura e apresentação, facilitando manutenção e evolução.
- **Entity Framework Core:** utilizado para persistência e gerenciamento de migrations.
- **Unit of Work:** centraliza a persistência das alterações realizadas durante os casos de uso.
- **RowVersion:** utilizado para detectar alterações concorrentes nas contas.
- **BackgroundService:** responsável pelo processamento automático das transferências agendadas.
- **Testes automatizados:** utilizados para validar regras de negócio e integração entre as camadas.

## Autor

Desenvolvido como parte de um teste técnico para Desenvolvedor .NET.
