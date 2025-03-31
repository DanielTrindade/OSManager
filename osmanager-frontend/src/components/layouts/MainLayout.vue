<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-indigo-600">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <span class="text-white font-bold text-xl">OSManager</span>
            </div>
            <div class="hidden md:block">
              <div class="ml-10 flex items-baseline space-x-4">
                <router-link 
                  to="/dashboard" 
                  class="text-white hover:bg-indigo-500 px-3 py-2 rounded-md text-sm font-medium"
                  :class="{'bg-indigo-700': $route.path === '/dashboard'}"
                >
                  Dashboard
                </router-link>
                <router-link 
                  to="/orders" 
                  class="text-white hover:bg-indigo-500 px-3 py-2 rounded-md text-sm font-medium"
                  :class="{'bg-indigo-700': $route.path.includes('/orders')}"
                >
                  Ordens de Serviço
                </router-link>
                <router-link 
                  v-if="isAdmin || isSupervisor"
                  to="/users" 
                  class="text-white hover:bg-indigo-500 px-3 py-2 rounded-md text-sm font-medium"
                  :class="{'bg-indigo-700': $route.path.includes('/users')}"
                >
                  Usuários
                </router-link>
                <router-link 
                  v-if="isAdmin"
                  to="/checklist-templates" 
                  class="text-white hover:bg-indigo-500 px-3 py-2 rounded-md text-sm font-medium"
                  :class="{'bg-indigo-700': $route.path.includes('/checklist-templates')}"
                >
                  Templates de Checklist
                </router-link>
              </div>
            </div>
          </div>
          <div class="hidden md:block">
            <div class="ml-4 flex items-center md:ml-6">
              <div class="ml-3 relative">
                <div>
                  <button 
                    @click="profileDropdownOpen = !profileDropdownOpen" 
                    class="max-w-xs bg-indigo-600 rounded-full flex items-center text-sm focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-indigo-600 focus:ring-white"
                  >
                    <span class="sr-only">Abrir menu do usuário</span>
                    <span class="inline-flex items-center justify-center h-8 w-8 rounded-full bg-indigo-500">
                      <span class="text-lg font-medium leading-none text-white">{{ userInitials }}</span>
                    </span>
                  </button>
                </div>
                <div 
                  v-if="profileDropdownOpen" 
                  class="origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-white ring-1 ring-black ring-opacity-5 focus:outline-none"
                >
                  <div class="px-4 py-2 text-xs text-gray-500">
                    Conectado como <span class="font-medium">{{ currentUser.username }}</span>
                  </div>
                  <router-link 
                    to="/profile" 
                    class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                    @click="profileDropdownOpen = false"
                  >
                    Seu Perfil
                  </router-link>
                  <a 
                    href="#" 
                    class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                    @click="logout"
                  >
                    Sair
                  </a>
                </div>
              </div>
            </div>
          </div>
          <div class="-mr-2 flex md:hidden">
            <button 
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="bg-indigo-600 inline-flex items-center justify-center p-2 rounded-md text-indigo-200 hover:text-white hover:bg-indigo-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-indigo-600 focus:ring-white"
            >
              <span class="sr-only">Abrir menu principal</span>
              <svg v-if="!mobileMenuOpen" class="block h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
              <svg v-else class="block h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>
      </div>
      <div v-if="mobileMenuOpen" class="md:hidden">
        <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3">
          <router-link 
            to="/dashboard" 
            class="text-white hover:bg-indigo-500 block px-3 py-2 rounded-md text-base font-medium"
            :class="{'bg-indigo-700': $route.path === '/dashboard'}"
            @click="mobileMenuOpen = false"
          >
            Dashboard
          </router-link>
          <router-link 
            to="/orders" 
            class="text-white hover:bg-indigo-500 block px-3 py-2 rounded-md text-base font-medium"
            :class="{'bg-indigo-700': $route.path.includes('/orders')}"
            @click="mobileMenuOpen = false"
          >
            Ordens de Serviço
          </router-link>
          <router-link 
            v-if="isAdmin || isSupervisor"
            to="/users" 
            class="text-white hover:bg-indigo-500 block px-3 py-2 rounded-md text-base font-medium"
            :class="{'bg-indigo-700': $route.path.includes('/users')}"
            @click="mobileMenuOpen = false"
          >
            Usuários
          </router-link>
          <router-link 
            v-if="isAdmin"
            to="/checklist-templates" 
            class="text-white hover:bg-indigo-500 block px-3 py-2 rounded-md text-base font-medium"
            :class="{'bg-indigo-700': $route.path.includes('/checklist-templates')}"
            @click="mobileMenuOpen = false"
          >
            Templates de Checklist
          </router-link>
        </div>
        <div class="pt-4 pb-3 border-t border-indigo-700">
          <div class="flex items-center px-5">
            <div class="flex-shrink-0">
              <span class="inline-flex items-center justify-center h-10 w-10 rounded-full bg-indigo-500">
                <span class="text-lg font-medium leading-none text-white">{{ userInitials }}</span>
              </span>
            </div>
            <div class="ml-3">
              <div class="text-base font-medium leading-none text-white">{{ currentUser.username }}</div>
            </div>
          </div>
          <div class="mt-3 px-2 space-y-1">
            <router-link 
              to="/profile" 
              class="block px-3 py-2 rounded-md text-base font-medium text-white hover:bg-indigo-500"
              @click="mobileMenuOpen = false"
            >
              Seu Perfil
            </router-link>
            <a 
              href="#" 
              class="block px-3 py-2 rounded-md text-base font-medium text-white hover:bg-indigo-500"
              @click="logout"
            >
              Sair
            </a>
          </div>
        </div>
      </div>
    </nav>
    <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <slot></slot>
    </main>
  </div>
</template>

<script>
import AuthService from '../../services/auth.service.js';
import { useRouter } from 'vue-router';
import { jwtDecode } from 'jwt-decode';

export default {
  name: 'MainLayout',
  
  setup() {
    const router = useRouter();
    return { router };
  },
  
  data() {
    return {
      mobileMenuOpen: false,
      profileDropdownOpen: false,
      currentUser: {
        username: '',
        token: '',
        role: ''
      }
    };
  },
  
  computed: {
    userInitials() {
      return this.currentUser.username ? this.currentUser.username.charAt(0).toUpperCase() : '?';
    },
    
    isAdmin() {
      return this.currentUser.role === 'Admin';
    },
    
    isSupervisor() {
      return this.currentUser.role === 'Supervisor';
    }
  },
  
  created() {
    const user = AuthService.getCurrentUser();
    if (user?.token) {
      this.currentUser.token = user.token;
      this.currentUser.username = user.username;
      
      try {
        const decodedToken = jwtDecode(user.token);
        this.currentUser.role = decodedToken.role || '';
      } catch (error) {
        console.error('Error decoding token:', error);
      }
    } else {
      this.router.push('/login');
    }
  },
  
  methods: {
    logout() {
      AuthService.logout();
      this.router.push('/login');
    }
  }
};
</script>