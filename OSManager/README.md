# Sistema de Ordem de Serviço (OS)

Uma API simples para gerenciamento de Ordens de Serviço utilizando .NET 8 com Minimal APIs.

## Funcionalidades

- Autenticação de usuários com token JWT
- Criação e consulta de ordens de serviço
- Preenchimento de checklist para cada OS
- Upload de fotos comprobatórias
- Documentação com Swagger

## Tecnologias Utilizadas

- .NET 8
- Entity Framework Core (Code First)
- SQL Server
- JWT para autenticação
- Swagger para documentação

## Requisitos

- .NET 8 SDK
- SQL Server (ou SQL Server LocalDB)

## Como Executar

### Opção 1: .NET CLI

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/os-manager.git
   cd os-manager
   ```

2. Configure a connection string no arquivo `appsettings.json`

3. Execute as migrações do banco de dados:
   ```
   dotnet ef database update
   ```

4. Execute a aplicação:
   ```
   dotnet run
   ```

5. Acesse a API em `https://localhost:5001` ou a documentação Swagger em `https://localhost:5001/swagger`

### Opção 2: Docker

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/os-manager.git
   cd os-manager
   ```

2. Construa a imagem Docker:
   ```
   docker build -t os-manager .
   ```

3. Execute o contêiner:
   ```
   docker run -p 8080:80 os-manager
   ```

4. Acesse a API em `http://localhost:8080` ou a documentação Swagger em `http://localhost:8080/swagger`

## Uso da API

### Autenticação

```
POST /api/auth/login
```
Corpo da requisição:
```json
{
  "username": "admin",
  "password": "admin"
}
```

### Criar Ordem de Serviço

```
POST /api/orders
Authorization: Bearer {seu-token}
```
Corpo da requisição:
```json
{
  "description": "Manutenção de equipamento XYZ"
}
```

### Listar Ordens de Serviço

```
GET /api/orders
Authorization: Bearer {seu-token}
```

### Ver Detalhes da Ordem de Serviço

```
GET /api/orders/{id}
Authorization: Bearer {seu-token}
```

### Atualizar Item de Checklist

```
PUT /api/checklist/items
Authorization: Bearer {seu-token}
```
Corpo da requisição:
```json
{
  "id": 1,
  "isCompleted": true
}
```

### Enviar Imagem

```
POST /api/orders/{orderId}/images
Authorization: Bearer {seu-token}
Content-Type: multipart/form-data
```
Formulário:
- `file`: Arquivo de imagem (JPG, JPEG ou PNG)

## Princípios SOLID Aplicados

- **S (Single Responsibility)**: Cada classe tem uma única responsabilidade
- **O (Open/Closed)**: As entidades são abertas para extensão, fechadas para modificação
- **D (Dependency Inversion)**: Dependemos de abstrações (interfaces), não de implementações concretas

## Usuário Padrão

- Username: admin
- Password: admin