# OSManager - Sistema de Gestão de Ordens de Serviço

## Visão Geral

O OSManager é um sistema completo para gerenciamento de ordens de serviço, desenvolvido para facilitar o controle de atividades de campo, inspeções e manutenções. O sistema permite que técnicos registrem suas atividades, preencham checklists e enviem evidências fotográficas, com fluxo de aprovação por supervisores.

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## Arquitetura da Solução

O sistema segue uma arquitetura de duas camadas principais, com uma clara separação entre frontend e backend:

### Backend (API)

- Desenvolvido em **.NET 8** utilizando **Minimal APIs**
- Banco de dados **SQL Server**
- Autenticação baseada em **JWT**
- Padrões **REST** para comunicação

### Frontend

- Desenvolvido com **Vue.js 3** e **Vite**
- Interface responsiva com **Tailwind CSS**
- Comunicação com o backend via **Axios**

### Containerização

- **Docker** para criação de ambientes isolados
- **Docker Compose** para orquestração dos serviços

## Modelo de Dados

O sistema é composto pelas seguintes entidades principais:

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

## Fluxo de Trabalho

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

## Controle de Acesso e Permissões

O sistema implementa três níveis de acesso:

| Perfil     | Descrição                                   | Permissões                                           |
|------------|---------------------------------------------|------------------------------------------------------|
| Admin      | Administrador com acesso total ao sistema   | Acesso completo a todas as funcionalidades           |
| Supervisor | Responsável pela aprovação de OS            | Pode visualizar todas as OS e aprovar/rejeitar       |
| Technician | Executa os trabalhos de campo               | Cria e gerencia suas próprias OS                     |

## Características do Sistema

### Backend (API)

- **Segurança**: Autenticação JWT, senhas com hash seguro (BCrypt)
- **Logs e Monitoramento**: Registro detalhado de requisições HTTP
- **Validação**: Verificação robusta de entrada de dados
- **Escalabilidade**: Arquitetura em camadas para fácil manutenção e expansão

### Frontend

- **Interface Responsiva**: Adaptável a dispositivos móveis e desktop
- **Experiência de Usuário**: Feedback visual claro, validações em tempo real
- **Dashboard**: Visualização rápida de estatísticas e métricas
- **Organização**: Checklists categorizados para melhor visualização

## Estrutura do Projeto

```
OSManager/
├── OSManager/             # Projeto Backend (.NET)
│   ├── Data/              # Acesso a dados e configurações
│   ├── DTOs/              # Objetos de transferência de dados
│   ├── Models/            # Entidades de domínio
│   ├── Services/          # Lógica de negócios
│   ├── Middleware/        # Middlewares personalizados
│   └── Program.cs         # Configuração da aplicação
│
├── osmanager-frontend/    # Projeto Frontend (Vue.js)
│   ├── src/               # Código fonte
│   │   ├── components/    # Componentes Vue
│   │   ├── views/         # Páginas da aplicação
│   │   ├── services/      # Serviços para API
│   │   └── router/        # Configuração de rotas
│   └── public/            # Recursos estáticos
│
└── docker-compose.yml     # Configuração de contêineres
```

## Configuração e Execução

### Requisitos

- Docker e Docker Compose
- Ou instalação local de:
  - .NET 8 SDK
  - Node.js 16+
  - SQL Server

### Instalação com Docker

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/OSManager.git
cd OSManager
```

2. Execute o Docker Compose:
```bash
docker-compose up -d
```

3. Acesse a aplicação:
   - Frontend: http://localhost:8080
   - API: http://localhost:5000/api
   - Swagger: http://localhost:5000/swagger

### Instalação Local

Consulte os READMEs específicos do [backend](./OSManager/README.md) e [frontend](./osmanager-frontend/README.md) para instruções detalhadas.

## Usuários Padrão

O sistema é inicializado com os seguintes usuários:

| Usuário     | Senha         | Papel      | Descrição                            |
|-------------|---------------|------------|--------------------------------------|
| admin       | admin123      | Admin      | Acesso total ao sistema              |
| supervisor  | supervisor123 | Supervisor | Pode aprovar/rejeitar OS             |
| tecnico     | tecnico123    | Technician | Cria e executa ordens de serviço     |

## Princípios de Design

O projeto foi desenvolvido seguindo os princípios SOLID:

- **S (Single Responsibility)**: Cada classe tem uma única responsabilidade
- **O (Open/Closed)**: Entidades abertas para extensão, fechadas para modificação
- **L (Liskov Substitution)**: Subtipos podem ser substituídos por seus tipos base
- **I (Interface Segregation)**: Interfaces específicas para clientes específicos
- **D (Dependency Inversion)**: Dependência de abstrações, não de implementações

## Evolução e Expansão

O sistema foi projetado pensando em escalabilidade e pode ser expandido com:

- **Sistema de Notificações**: Alertas e notificações em tempo real
- **Integração com Calendário**: Agendamento de ordens de serviço
- **Aplicativo Mobile**: Versão nativa para dispositivos móveis
- **Relatórios Avançados**: Exportação e análise de dados
- **Controle de Estoque**: Integração para gerenciamento de peças e materiais

---

Desenvolvido como parte do desafio técnico da Kodigos 2025.