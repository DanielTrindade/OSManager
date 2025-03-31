# OSManager Frontend

## Visão Geral

Este é o frontend para o sistema OSManager, uma aplicação de gerenciamento de ordens de serviço que permite que técnicos registrem suas atividades, preencham checklists e enviem evidências fotográficas do trabalho realizado. Desenvolvido com Vue.js, Vite e Tailwind CSS, o frontend oferece uma interface moderna e responsiva que se integra perfeitamente com a API backend.

## Tecnologias

- **Vue.js 3**: Framework JavaScript progressivo
- **Vite**: Build tool para desenvolvimento rápido
- **Tailwind CSS**: Framework CSS utility-first
- **Axios**: Cliente HTTP para comunicação com a API
- **Vue Router**: Gerenciamento de rotas
- **JWT**: Autenticação baseada em tokens

## Instalação

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/osmanager-frontend.git
cd osmanager-frontend
```

2. Instale as dependências:
```bash
npm install
```

3. Configure a URL da API no arquivo `.env.development`:
```
VITE_API_URL=http://localhost:5000/api
```

4. Execute o servidor de desenvolvimento:
```bash
npm run dev
```

## Estrutura da Aplicação

### Páginas Principais

- **Login (`LoginView.vue`)**: Autenticação de usuários
- **Dashboard (`DashboardView.vue`)**: Visão geral das estatísticas das ordens de serviço
- **Lista de Ordens (`OrdersView.vue`)**: Visualização e filtro de todas as ordens
- **Detalhes da Ordem (`OrderDetailView.vue`)**: Gestão de uma ordem específica
- **Criar Ordem (`CreateOrderView.vue`)**: Formulário para criação de novas ordens

### Fluxo de Trabalho

1. **Autenticação**: Usuários fazem login para acessar o sistema
2. **Dashboard**: Visualização das estatísticas de ordens
3. **Gestão de Ordens**:
   - Listagem e filtro de ordens
   - Criação de novas ordens
   - Visualização detalhada
4. **Ciclo de Vida da Ordem**:
   - Criada → Em Progresso → Concluída → Aprovada/Rejeitada
5. **Checklist e Evidências**:
   - Preenchimento dos itens de checklist
   - Upload de imagens como evidência

### Controle de Acesso

O sistema implementa três níveis de acesso:
- **Técnico**: Pode criar e gerenciar suas próprias ordens
- **Supervisor**: Pode visualizar todas as ordens e aprovar/rejeitar
- **Administrador**: Acesso completo, incluindo gerenciamento de usuários

## Componentes Principais

### MainLayout

O `MainLayout.vue` fornece a estrutura básica da aplicação após o login, incluindo:
- Barra de navegação responsiva
- Menu lateral
- Perfil do usuário
- Área de conteúdo principal

### Visualização de Ordem

A página de detalhes da ordem (`OrderDetailView.vue`) permite:
- Visualizar informações detalhadas da ordem
- Gerenciar status da ordem
- Preencher itens de checklist
- Fazer upload de imagens
- Visualizar evidências fotográficas

### Upload de Imagens

O sistema permite:
- Upload de imagens para a ordem geral
- Upload de imagens para itens específicos do checklist
- Visualização em galeria
- Visualização em modal para detalhes

## Comunicação com a API

A comunicação com o backend é gerenciada pelos serviços:
- `auth.service.js`: Autenticação e gerenciamento de tokens
- `order.service.js`: Operações relacionadas às ordens de serviço
- `api.js`: Configuração do Axios com interceptores

## Desenvolvimento

### Executando com Docker

1. Crie um arquivo `Dockerfile.dev` na raiz do projeto:
```dockerfile
FROM node:16-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
EXPOSE 8080
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
```

2. Execute com Docker:
```bash
docker build -f Dockerfile.dev -t osmanager-frontend .
docker run -p 8080:8080 -v $(pwd):/app -v /app/node_modules osmanager-frontend
```

### Integração com Backend

Para funcionar corretamente, o backend deve ter o CORS configurado para permitir requisições do frontend. A URL da API é configurada através da variável de ambiente `VITE_API_URL`.

## Build para Produção

```bash
npm run build
```

Os arquivos compilados serão gerados na pasta `dist/` e podem ser servidos por qualquer servidor web estático.

## Usuários Padrão

O sistema é inicializado com os seguintes usuários:

| Usuário     | Senha         | Papel      |
|-------------|---------------|------------|
| admin       | admin123      | Admin      |
| supervisor  | supervisor123 | Supervisor |
| tecnico     | tecnico123    | Technician |

---

Este frontend foi desenvolvido como parte do desafio técnico da Kodigos 2025.