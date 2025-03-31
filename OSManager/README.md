# OSManager - Sistema de Gestão de Ordens de Serviço

Uma API robusta para gerenciamento completo de Ordens de Serviço (OS), desenvolvida com .NET 8 utilizando Minimal APIs e seguindo as melhores práticas de desenvolvimento.

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Funcionalidades](#funcionalidades)
- [Arquitetura](#arquitetura)
- [Modelo de Dados](#modelo-de-dados)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Requisitos](#requisitos)
- [Instalação e Execução](#instalação-e-execução)
  - [Usando Docker (Recomendado)](#usando-docker-recomendado)
  - [Usando .NET CLI](#usando-net-cli)
- [Documentação da API](#documentação-da-api)
- [Fluxo de Trabalho](#fluxo-de-trabalho)
- [Usuários Padrão](#usuários-padrão)
- [Contribuição](#contribuição)
- [Licença](#licença)

## 📝 Visão Geral

OSManager é um sistema completo para gerenciamento de ordens de serviço que permite que técnicos de campo registrem suas atividades, preencham checklists e enviem evidências fotográficas do trabalho realizado. O sistema inclui níveis de aprovação, garantindo qualidade e conformidade em todo o processo de serviço.

## ✨ Funcionalidades

- **Autenticação e Autorização**
  - Sistema de login seguro com tokens JWT
  - Níveis de acesso: Administrador, Supervisor e Técnico
  - Gestão de senhas com hash seguro

- **Gestão de Ordens de Serviço**
  - Criação, visualização, filtragem e monitoramento de OS
  - Fluxo de trabalho completo: Criação → Em Progresso → Concluído → Aprovado/Rejeitado
  - Estatísticas e relatórios de ordens de serviço

- **Sistema de Checklist**
  - Templates de checklist configuráveis e categorizados
  - Itens de checklist obrigatórios antes da conclusão da OS
  - Organização por categorias personalizáveis

- **Documentação Fotográfica**
  - Upload de imagens como evidência do serviço realizado
  - Associação de imagens a itens específicos do checklist
  - Validação de formato e tamanho de arquivos

- **Monitoramento e Logs**
  - Registro detalhado de requisições HTTP
  - Métricas de desempenho para cada solicitação

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas limpa e moderna:

```
OSManager/
├── Data/            # Acesso a dados e configurações do EF Core
├── DTOs/            # Objetos de transferência de dados
├── Extensions/      # Métodos de extensão para mapeamento e utilidades
├── Middleware/      # Middlewares personalizados
├── Models/          # Entidades de domínio
├── Services/        # Lógica de negócios 
├── Utils/           # Classes utilitárias
└── Program.cs       # Configuração da aplicação e definição de endpoints
```

### Princípios SOLID Aplicados

- **S (Single Responsibility)**: Cada classe tem uma única responsabilidade bem definida
- **O (Open/Closed)**: Entidades são abertas para extensão, fechadas para modificação
- **L (Liskov Substitution)**: Subtipos podem ser substituídos por seus tipos base
- **I (Interface Segregation)**: Interfaces específicas para clientes específicos
- **D (Dependency Inversion)**: Dependência de abstrações, não de implementações concretas

## 📊 Modelo de Dados

O sistema utiliza as seguintes entidades principais:

### User
- Representa usuários do sistema com diferentes papéis (Admin, Supervisor, Technician)
- Armazena informações de autenticação e perfil

### Order
- Ordem de serviço com descrição, datas, status e relacionamentos
- Status possíveis: Created, InProgress, Completed, Approved, Rejected

### ChecklistItem
- Itens a serem verificados e completados em uma ordem de serviço
- Pode ser um template (modelo) ou um item específico de uma OS

### Image
- Evidências fotográficas de trabalho realizado
- Pode estar associada a uma OS ou a um item específico de checklist

### Diagrama de Relacionamentos

```
┌─────────┐       ┌─────────┐       ┌──────────────┐
│  User   │       │  Order  │       │ ChecklistItem│
├─────────┤       ├─────────┤       ├──────────────┤
│ Id      │◄──┐   │ Id      │       │ Id           │
│ Username│   │   │ Desc    │       │ Description  │
│ Password│   ├───┤ UserId  │◄──┐   │ IsCompleted  │
│ Email   │   │   │ Status  │   │   │ OrderId      │◄─┐
│ Role    │   │   │ Created │   └───┤ Category     │  │
└─────────┘   │   │ Started │       │ DisplayOrder │  │
              │   │ Approved│       └──────────────┘  │
              │   └─────────┘              ▲          │
              │        │                   │          │
              │        ▼                   │          │
              │   ┌─────────┐              │          │
              └───┤ Approver│              │          │
                  │ (User)  │              │          │
                  └─────────┘              │          │
                                           │          │
                  ┌─────────┐              │          │
                  │  Image  │              │          │
                  ├─────────┤              │          │
                  │ Id      │              │          │
                  │ FileName│              │          │
                  │ Path    │              │          │
                  │ OrderId │──────────────┘          │
                  │ ChkItmId│─────────────────────────┘
                  └─────────┘
```

## 🛠️ Tecnologias Utilizadas

- **.NET 8**: Framework moderno para desenvolvimento de aplicações
- **Entity Framework Core**: ORM para acesso a dados com abordagem Code First
- **SQL Server**: Sistema de gerenciamento de banco de dados relacional
- **JWT Authentication**: Autenticação baseada em tokens
- **BCrypt**: Algoritmo seguro para hash de senhas
- **Docker**: Contêinerização para implantação simplificada
- **Swagger/OpenAPI**: Documentação automatizada da API

## 📋 Requisitos

- .NET 8 SDK
- Docker e Docker Compose (para execução em contêineres)
- SQL Server (necessário apenas para execução sem Docker)
- Visual Studio 2022, VS Code ou outro editor compatível

## 🚀 Instalação e Execução

### Usando Docker (Recomendado)

1. Clone o repositório:
   ```bash
   git clone https://github.com/DanielTrindade/OSManager.git
   cd OSManager
   ```

2. Execute com Docker Compose:
   ```bash
   docker-compose up -d
   ```

3. Acesse a API em:
   - Documentação Swagger: http://localhost:50780/swagger
   - API: http://localhost:50780/api

### Usando .NET CLI

1. Clone o repositório:
   ```bash
   git clone https://github.com/DanielTrindade/OSManager.git
   cd os-manager
   ```

2. Configure a connection string no arquivo `appsettings.json` para apontar para seu servidor SQL:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=OSManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
     }
   }
   ```

3. Restaure os pacotes e execute as migrações:
   ```bash
   dotnet restore
   dotnet ef database update
   ```

4. Execute a aplicação:
   ```bash
   dotnet run
   ```

5. Acesse:
   - Documentação Swagger: https://localhost:50780/swagger
   - API: https://localhost:50780'''''''''''/api

## 📚 Documentação da API

A documentação completa da API está disponível através do Swagger UI após a execução do projeto. Principais endpoints:

### Autenticação
- **POST /api/auth/login**: Autenticação de usuário e obtenção de token JWT

### Usuários
- **GET /api/users**: Listar todos os usuários (Admin)
- **GET /api/users/{id}**: Obter usuário por ID (Admin)
- **POST /api/users**: Criar novo usuário (Admin)
- **PUT /api/users/{id}**: Atualizar usuário (Admin)
- **POST /api/users/change-password**: Alterar senha

### Ordens de Serviço
- **GET /api/orders**: Listar ordens de serviço
- **GET /api/orders/{id}**: Obter ordem por ID
- **POST /api/orders**: Criar nova ordem
- **PUT /api/orders/status**: Atualizar status de uma ordem
- **GET /api/orders/filter**: Filtrar ordens por status/data
- **GET /api/orders/stats**: Obter estatísticas de ordens

### Checklist
- **PUT /api/checklist/items**: Atualizar item de checklist
- **GET /api/checklist/templates**: Obter templates de checklist
- **POST /api/checklist/templates**: Criar template de checklist
- **PUT /api/checklist/templates/{id}**: Atualizar template
- **DELETE /api/checklist/templates/{id}**: Remover template

### Imagens
- **POST /api/orders/{orderId}/images**: Enviar imagem para uma OS
- **DELETE /api/images/{id}**: Remover imagem

## 🔄 Fluxo de Trabalho

O sistema implementa o seguinte fluxo de trabalho para ordens de serviço:

1. **Criação da OS** (Status: Created)
   - Técnico cria uma nova OS com descrição
   - O sistema adiciona automaticamente itens de checklist baseados nos templates

2. **Início do Trabalho** (Status: InProgress)
   - Técnico atualiza o status para "Em Progresso"
   - A data de início é registrada

3. **Execução e Documentação**
   - Técnico preenche os itens do checklist
   - Upload de imagens como evidência
   - Cada item do checklist pode ter suas próprias imagens

4. **Finalização do Trabalho** (Status: Completed)
   - Técnico marca a OS como concluída
   - Só é possível concluir quando todos os itens do checklist estiverem completos
   - A data de conclusão é registrada

5. **Aprovação/Rejeição** (Status: Approved/Rejected)
   - Supervisor ou Administrador revisa a OS
   - Pode aprovar (Approved) ou rejeitar (Rejected)
   - Em caso de rejeição, um motivo deve ser fornecido
   - A data de aprovação é registrada

## 👥 Usuários Padrão

O sistema é inicializado com os seguintes usuários:

| Usuário     | Senha         | Papel      | Descrição                            |
|-------------|---------------|------------|--------------------------------------|
| admin       | admin123      | Admin      | Acesso total ao sistema              |
| supervisor  | supervisor123 | Supervisor | Pode aprovar/rejeitar OS             |
| tecnico     | tecnico123    | Technician | Cria e executa ordens de serviço     |

---

Desenvolvido como parte do desafio técnico da Kodigos 2025.
