<template>
  <MainLayout>
    <div class="px-4 sm:px-0">
      <div class="sm:flex sm:items-center">
        <div class="sm:flex-auto">
          <h1 class="text-2xl font-semibold text-gray-900">Nova Ordem de Serviço</h1>
          <p class="mt-2 text-sm text-gray-700">
            Crie uma nova ordem de serviço para registrar atividades.
          </p>
        </div>
        <div class="mt-4 sm:mt-0 sm:ml-16 sm:flex-none">
          <router-link 
            to="/orders" 
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Voltar para Lista
          </router-link>
        </div>
      </div>
      <div class="mt-6 bg-white shadow px-4 py-5 sm:rounded-lg sm:p-6">
        <div class="md:grid md:grid-cols-3 md:gap-6">
          <div class="md:col-span-1">
            <h3 class="text-lg font-medium leading-6 text-gray-900">Detalhes da OS</h3>
            <p class="mt-1 text-sm text-gray-500">
              Forneça uma descrição detalhada do serviço a ser realizado.
            </p>
          </div>
          <div class="mt-5 md:mt-0 md:col-span-2">
            <form @submit.prevent="createOrder">
              <div>
                <label for="description" class="block text-sm font-medium text-gray-700">
                  Descrição <span class="text-red-500">*</span>
                </label>
                <div class="mt-1">
                  <textarea
                    id="description"
                    v-model="description"
                    rows="5"
                    class="shadow-sm focus:ring-indigo-500 focus:border-indigo-500 block w-full sm:text-sm border-gray-300 rounded-md"
                    placeholder="Descreva em detalhes o serviço a ser realizado..."
                    required
                  ></textarea>
                </div>
                <p v-if="errors.description" class="mt-2 text-sm text-red-600">{{ errors.description }}</p>
                <p class="mt-2 text-sm text-gray-500">
                  Inclua informações relevantes como localização, equipamento, problema identificado, etc.
                </p>
              </div>
              <div class="mt-8 flex justify-end">
                <router-link
                  to="/orders"
                  class="ml-3 py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Cancelar
                </router-link>
                <button
                  type="submit"
                  :disabled="loading"
                  class="ml-3 inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  {{ loading ? 'Criando...' : 'Criar Ordem' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      <div v-if="successMessage" class="mt-6 rounded-md bg-green-50 p-4">
        <div class="flex">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-green-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-medium text-green-800">{{ successMessage }}</h3>
            <div class="mt-2 text-sm text-green-700">
              <p>A ordem foi criada com sucesso.</p>
            </div>
            <div class="mt-4">
              <div class="-mx-2 -my-1.5 flex">
                <router-link 
                  :to="`/orders/${createdOrderId}`"
                  class="bg-green-50 px-2 py-1.5 rounded-md text-sm font-medium text-green-800 hover:bg-green-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-600"
                >
                  Ver ordem
                </router-link>
                <button
                  type="button"
                  class="ml-3 bg-green-50 px-2 py-1.5 rounded-md text-sm font-medium text-green-800 hover:bg-green-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-600"
                  @click="clearForm"
                >
                  Criar outra
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div v-if="errorMessage" class="mt-6 rounded-md bg-red-50 p-4">
        <div class="flex">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-red-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Erro ao criar ordem</h3>
            <div class="mt-2 text-sm text-red-700">
              <p>{{ errorMessage }}</p>
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
  name: 'CreateOrderView',
  components: {
    MainLayout
  },
  data() {
    return {
      description: '',
      loading: false,
      errors: {
        description: ''
      },
      successMessage: '',
      errorMessage: '',
      createdOrderId: null
    };
  },
  methods: {
    validateForm() {
      this.errors = {
        description: ''
      };
      
      let isValid = true;
      
      if (!this.description.trim()) {
        this.errors.description = 'A descrição é obrigatória';
        isValid = false;
      } else if (this.description.trim().length < 10) {
        this.errors.description = 'A descrição deve ter pelo menos 10 caracteres';
        isValid = false;
      }
      
      return isValid;
    },
    
    createOrder() {
      if (!this.validateForm()) {
        return;
      }
      
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';
      
      OrderService.createOrder(this.description.trim())
        .then(response => {
          this.successMessage = 'Ordem de serviço criada com sucesso!';
          this.createdOrderId = response.data.id;
          this.loading = false;
        })
        .catch(error => {
          console.error('Error creating order:', error);
          this.errorMessage = error.response?.data?.message || 'Erro ao criar a ordem. Tente novamente mais tarde.';
          this.loading = false;
        });
    },
    
    clearForm() {
      this.description = '';
      this.errors = {
        description: ''
      };
      this.successMessage = '';
      this.errorMessage = '';
      this.createdOrderId = null;
    }
  }
};
</script>