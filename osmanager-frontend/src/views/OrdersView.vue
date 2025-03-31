<template>
  <MainLayout>
    <div class="px-4 sm:px-0">
      <div class="sm:flex sm:items-center">
        <div class="sm:flex-auto">
          <h1 class="text-2xl font-semibold text-gray-900">Ordens de Serviço</h1>
          <p class="mt-2 text-sm text-gray-700">
            Lista de todas as ordens de serviço do sistema.
          </p>
        </div>
        <div class="mt-4 sm:mt-0 sm:ml-16 sm:flex-none">
          <router-link 
            to="/orders/new" 
            class="inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Nova Ordem
          </router-link>
        </div>
      </div>
      
      <div class="bg-white shadow px-4 py-5 sm:rounded-lg sm:p-6 mt-6">
        <div class="md:grid md:grid-cols-3 md:gap-6">
          <div class="md:col-span-1">
            <h3 class="text-lg font-medium leading-6 text-gray-900">Filtros</h3>
            <p class="mt-1 text-sm text-gray-500">
              Filtre as ordens de serviço por status e data.
            </p>
          </div>
          <div class="mt-5 md:mt-0 md:col-span-2">
            <form @submit.prevent="applyFilters">
              <div class="grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
                <div class="sm:col-span-2">
                  <label for="status" class="block text-sm font-medium text-gray-700">Status</label>
                  <select
                    id="status"
                    v-model="filters.status"
                    class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                  >
                    <option value="">Todos</option>
                    <option value="Created">Criada</option>
                    <option value="InProgress">Em Progresso</option>
                    <option value="Completed">Concluída</option>
                    <option value="Approved">Aprovada</option>
                    <option value="Rejected">Rejeitada</option>
                  </select>
                </div>
                
                <div class="sm:col-span-2">
                  <label for="from-date" class="block text-sm font-medium text-gray-700">De</label>
                  <input
                    type="date"
                    id="from-date"
                    v-model="filters.fromDate"
                    class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
                
                <div class="sm:col-span-2">
                  <label for="to-date" class="block text-sm font-medium text-gray-700">Até</label>
                  <input
                    type="date"
                    id="to-date"
                    v-model="filters.toDate"
                    class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>
              </div>
              
              <div class="flex justify-end mt-6 space-x-3">
                <button
                  type="button"
                  class="py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                  @click="resetFilters"
                >
                  Limpar
                </button>
                <button
                  type="submit"
                  class="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Aplicar Filtros
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      
      <div v-if="loading" class="mt-8 flex justify-center">
        <svg class="animate-spin h-10 w-10 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      </div>
      
      <div v-else-if="error" class="mt-8 bg-red-50 p-4 rounded-md">
        <div class="flex">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-red-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Erro ao carregar ordens</h3>
            <div class="mt-2 text-sm text-red-700">
              <p>{{ error }}</p>
            </div>
          </div>
        </div>
      </div>
      
      <div v-else-if="orders.length === 0" class="mt-8 bg-white shadow sm:rounded-lg">
        <div class="px-4 py-5 sm:p-6 text-center">
          <svg class="mx-auto h-12 w-12 text-gray-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">Nenhuma ordem encontrada</h3>
          <p class="mt-1 text-sm text-gray-500">
            Comece criando uma nova ordem de serviço.
          </p>
          <div class="mt-6">
            <router-link
              to="/orders/new"
              class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clip-rule="evenodd" />
              </svg>
              Nova Ordem
            </router-link>
          </div>
        </div>
      </div>
      
      <div v-else class="mt-8 flex flex-col">
        <div class="-my-2 -mx-4 overflow-x-auto sm:-mx-6 lg:-mx-8">
          <div class="inline-block min-w-full py-2 align-middle md:px-6 lg:px-8">
            <div class="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
              <table class="min-w-full divide-y divide-gray-300">
                <thead class="bg-gray-50">
                  <tr>
                    <th scope="col" class="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">
                      ID
                    </th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Descrição
                    </th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Status
                    </th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Responsável
                    </th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                      Data de Criação
                    </th>
                    <th scope="col" class="relative py-3.5 pl-3 pr-4 sm:pr-6">
                      <span class="sr-only">Ações</span>
                    </th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 bg-white">
                  <tr v-for="order in orders" :key="order.id">
                    <td class="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900 sm:pl-6">
                      {{ order.id }}
                    </td>
                    <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                      {{ truncateText(order.description, 30) }}
                    </td>
                    <td class="whitespace-nowrap px-3 py-4 text-sm">
                      <span :class="getStatusClass(order.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                        {{ getStatusText(order.status) }}
                      </span>
                    </td>
                    <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                      {{ order.user.fullName || order.user.username }}
                    </td>
                    <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                      {{ formatDate(order.createdAt) }}
                    </td>
                    <td class="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
                      <router-link :to="`/orders/${order.id}`" class="text-indigo-600 hover:text-indigo-900">
                        Ver<span class="sr-only">, ordem {{ order.id }}</span>
                      </router-link>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script>
import MainLayout from '../components/layouts/MainLayout.vue';
import OrderService from '../services/order.service.js';

export default {
  name: 'OrdersView',
  components: {
    MainLayout
  },
  data() {
    return {
      loading: true,
      error: null,
      orders: [],
      filters: {
        status: '',
        fromDate: '',
        toDate: ''
      },
      originalOrders: []
    };
  },
  created() {
    this.fetchOrders();
  },
  methods: {
    fetchOrders() {
      this.loading = true;
      this.error = null;
      
      OrderService.getOrders()
        .then(response => {
          this.orders = response.data;
          this.originalOrders = [...response.data];
          this.loading = false;
        })
        .catch(error => {
          console.error('Error fetching orders:', error);
          this.error = error.response?.data?.message || 'Erro ao carregar as ordens. Tente novamente mais tarde.';
          this.loading = false;
        });
    },
    
    applyFilters() {
      let filteredOrders = [...this.originalOrders];
      if (this.filters.status) {
        filteredOrders = filteredOrders.filter(order => order.status === this.filters.status);
      }
      if (this.filters.fromDate) {
        const fromDate = new Date(this.filters.fromDate);
        filteredOrders = filteredOrders.filter(order => new Date(order.createdAt) >= fromDate);
      }
      
      if (this.filters.toDate) {
        const toDate = new Date(this.filters.toDate);
        toDate.setHours(23, 59, 59, 999);
        filteredOrders = filteredOrders.filter(order => new Date(order.createdAt) <= toDate);
      }
      
      this.orders = filteredOrders;
    },
    
    resetFilters() {
      this.filters = {
        status: '',
        fromDate: '',
        toDate: ''
      };
      
      this.orders = [...this.originalOrders];
    },
    
    truncateText(text, maxLength) {
      if (text.length <= maxLength) return text;
      return `${text.slice(0, maxLength)}...`;
    },
    
    formatDate(dateString) {
      const date = new Date(dateString);
      return new Intl.DateTimeFormat('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      }).format(date);
    },
    
    getStatusText(status) {
      const statusMap = {
        'Created': 'Criada',
        'InProgress': 'Em Progresso',
        'Completed': 'Concluída',
        'Approved': 'Aprovada',
        'Rejected': 'Rejeitada'
      };
      
      return statusMap[status] || status;
    },
    
    getStatusClass(status) {
      const statusClasses = {
        'Created': 'bg-yellow-100 text-yellow-800',
        'InProgress': 'bg-blue-100 text-blue-800',
        'Completed': 'bg-green-100 text-green-800',
        'Approved': 'bg-green-100 text-green-800',
        'Rejected': 'bg-red-100 text-red-800'
      };
      
      return statusClasses[status] || 'bg-gray-100 text-gray-800';
    }
  }
};
</script>