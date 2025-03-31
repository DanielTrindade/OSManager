<template>
  <MainLayout>
    <div class="px-4 sm:px-0">
      <h1 class="text-2xl font-semibold text-gray-900">Dashboard</h1>
      <div v-if="loading" class="mt-6 flex justify-center">
        <svg class="animate-spin h-10 w-10 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      </div>
      <div v-else-if="error" class="mt-6 bg-red-50 p-4 rounded-md">
        <div class="flex">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-red-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Erro ao carregar estatísticas</h3>
            <div class="mt-2 text-sm text-red-700">
              <p>{{ error }}</p>
            </div>
          </div>
        </div>
      </div>   
      <div v-else class="mt-6">
        <dl class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
          <div class="bg-white overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <dt class="text-sm font-medium text-gray-500 truncate">
                Total de Ordens
              </dt>
              <dd class="mt-1 text-3xl font-semibold text-gray-900">
                {{ stats.totalOrders }}
              </dd>
            </div>
          </div>
          <div class="bg-white overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <dt class="text-sm font-medium text-gray-500 truncate">
                Em Progresso
              </dt>
              <dd class="mt-1 text-3xl font-semibold text-indigo-600">
                {{ stats.inProgressOrders }}
              </dd>
            </div>
          </div>
          <div class="bg-white overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <dt class="text-sm font-medium text-gray-500 truncate">
                Concluídas
              </dt>
              <dd class="mt-1 text-3xl font-semibold text-green-600">
                {{ stats.completedOrders }}
              </dd>
            </div>
          </div>
          <div class="bg-white overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <dt class="text-sm font-medium text-gray-500 truncate">
                Criadas
              </dt>
              <dd class="mt-1 text-3xl font-semibold text-yellow-600">
                {{ stats.createdOrders }}
              </dd>
            </div>
          </div>
          <div class="bg-white overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <dt class="text-sm font-medium text-gray-500 truncate">
                Aprovadas
              </dt>
              <dd class="mt-1 text-3xl font-semibold text-green-600">
                {{ stats.approvedOrders }}
              </dd>
            </div>
          </div>
          <div class="bg-white overflow-hidden shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <dt class="text-sm font-medium text-gray-500 truncate">
                Rejeitadas
              </dt>
              <dd class="mt-1 text-3xl font-semibold text-red-600">
                {{ stats.rejectedOrders }}
              </dd>
            </div>
          </div>
        </dl>
        
        <div class="mt-8">
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:px-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900">
                Ações Rápidas
              </h3>
            </div>
            <div class="border-t border-gray-200 px-4 py-5 sm:p-6">
              <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <router-link 
                  to="/orders/new" 
                  class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  <svg class="-ml-1 mr-2 h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clip-rule="evenodd" />
                  </svg>
                  Nova Ordem de Serviço
                </router-link>
                <router-link 
                  to="/orders" 
                  class="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  <svg class="-ml-1 mr-2 h-5 w-5 text-gray-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clip-rule="evenodd" />
                  </svg>
                  Ver Todas as Ordens
                </router-link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script>
import MainLayout from '../components/layouts/MainLayout.vue';
import axios from 'axios';
import AuthService from '../services/auth.service.js';

export default {
  name: 'DashboardView',
  components: {
    MainLayout
  },
  data() {
    return {
      loading: true,
      error: null,
      stats: {
        totalOrders: 0,
        createdOrders: 0,
        inProgressOrders: 0,
        completedOrders: 0,
        approvedOrders: 0,
        rejectedOrders: 0
      }
    };
  },
  created() {
    this.fetchOrderStats();
  },
  methods: {
    fetchOrderStats() {
      const user = AuthService.getCurrentUser();
      if (!user) {
        this.$router.push('/login');
        return;
      }

      const API_URL = import.meta.env.VITE_API_URL;
      axios.get(`${API_URL}/orders/stats`, {
        headers: {
          'Authorization': `Bearer ${user.token}`
        }
      })
      .then(response => {
        this.stats = response.data;
        this.loading = false;
      })
      .catch(error => {
        console.error('Error fetching order stats:', error);
        this.error = error.response?.data?.message || 'Erro ao carregar estatísticas. Tente novamente mais tarde.';
        this.loading = false;
      });
    }
  }
};
</script>