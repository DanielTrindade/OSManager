import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../src/views/LoginView.vue'
import DashboardView from '../src/views/DashboardView.vue'
import OrdersView from '../src/views/OrdersView.vue'
import CreateOrderView from '../src/views/CreateOrderView.vue'
import OrderDetailView from '../src/views/OrderDetailView.vue'

const routes = [
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { requiresAuth: false }
  },
  {
    path: '/dashboard',
    name: 'dashboard',
    component: DashboardView,
    meta: { requiresAuth: true }
  },
  {
    path: '/orders',
    name: 'orders',
    component: OrdersView,
    meta: { requiresAuth: true }
  },
  {
    path: '/orders/new',
    name: 'create-order',
    component: CreateOrderView,
    meta: { requiresAuth: true }
  },
  {
    path: '/orders/:id',
    name: 'order-detail',
    component: OrderDetailView,
    meta: { requiresAuth: true }
  },
  // Redirecionamento para página não encontrada
  {
    path: '/:pathMatch(.*)*',
    redirect: '/dashboard'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navegação guard para proteger rotas
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth !== false);
  const loggedIn = localStorage.getItem('user');

  // Redirecionar para login se a rota requer autenticação e o usuário não está logado
  if (requiresAuth && !loggedIn) {
    next('/login');
  }
  // Se o usuário já está logado e tenta acessar o login, redirecionar para dashboard
  else if (to.path === '/login' && loggedIn) {
    next('/dashboard');
  }
  // Permitir a navegação
  else {
    next();
  }
})

export default router