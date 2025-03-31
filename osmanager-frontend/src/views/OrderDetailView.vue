<template>
  <MainLayout>
    <div class="px-4 sm:px-0">
      <div v-if="loading" class="flex justify-center py-12">
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
            <h3 class="text-sm font-medium text-red-800">Erro ao carregar a ordem</h3>
            <div class="mt-2 text-sm text-red-700">
              <p>{{ error }}</p>
            </div>
            <div class="mt-4">
              <router-link to="/orders" class="text-sm font-medium text-red-600 hover:text-red-500">
                Voltar para a lista de ordens
              </router-link>
            </div>
          </div>
        </div>
      </div>
      <div v-else>
        <div class="sm:flex sm:items-center sm:justify-between">
          <div>
            <div class="flex items-center">
              <h1 class="text-2xl font-semibold text-gray-900">
                Ordem #{{ order.id }}
              </h1>
              <span :class="getStatusClass(order.status)" class="ml-3 px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ getStatusText(order.status) }}
              </span>
            </div>
            <p class="mt-2 text-sm text-gray-700">
              Criada por {{ order.user.fullName || order.user.username }} em {{ formatDate(order.createdAt) }}
            </p>
          </div>
          <div class="mt-4 flex gap-3 sm:mt-0">
            <router-link 
              to="/orders" 
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Voltar para Lista
            </router-link>
            <button 
              v-if="canChangeStatus && nextStatusAction"
              @click="openUpdateStatusModal"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              {{ nextStatusAction.label }}
            </button>
          </div>
        </div>
        <div class="mt-6 bg-white shadow overflow-hidden sm:rounded-lg">
          <div class="px-4 py-5 sm:px-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900">
              Informações da Ordem
            </h3>
            <p class="mt-1 max-w-2xl text-sm text-gray-500">
              Detalhes completos da ordem de serviço.
            </p>
          </div>
          <div class="border-t border-gray-200 px-4 py-5 sm:p-0">
            <dl class="sm:divide-y sm:divide-gray-200">
              <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Descrição</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ order.description }}</dd>
              </div>
              <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Status</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                  <span :class="getStatusClass(order.status)" class="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full">
                    {{ getStatusText(order.status) }}
                  </span>
                </dd>
              </div>
              <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Responsável</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ order.user.fullName || order.user.username }}</dd>
              </div>
              <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Data de Criação</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ formatDate(order.createdAt) }}</dd>
              </div>
              <div v-if="order.startedAt" class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Data de Início</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ formatDate(order.startedAt) }}</dd>
              </div>
              <div v-if="order.completedAt" class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Data de Conclusão</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ formatDate(order.completedAt) }}</dd>
              </div>
              <div v-if="order.approvedAt" class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Data de Aprovação</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ formatDate(order.approvedAt) }}</dd>
              </div>
              <div v-if="order.approver" class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Aprovado por</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ order.approver.fullName || order.approver.username }}</dd>
              </div>
              <div v-if="order.rejectionReason" class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                <dt class="text-sm font-medium text-gray-500">Motivo da Rejeição</dt>
                <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{{ order.rejectionReason }}</dd>
              </div>
            </dl>
          </div>
        </div>
        <div class="mt-8 bg-white shadow overflow-hidden sm:rounded-lg">
          <div class="px-4 py-5 sm:px-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900">
              Checklist
            </h3>
            <p class="mt-1 max-w-2xl text-sm text-gray-500">
              Verifique todos os itens antes de concluir a ordem.
            </p>
          </div>
          <div class="border-t border-gray-200 px-4 py-5 sm:px-6">
            <div v-if="order.checklistItems.length === 0" class="text-center py-4">
              <p class="text-sm text-gray-500">Nenhum item de checklist disponível.</p>
            </div>
            <div v-else>
              <div v-for="(items, category) in groupedChecklist" :key="category" class="mb-6">
                <h4 class="text-md font-medium text-gray-900 mb-3">{{ category }}</h4>
                <ul class="space-y-3">
                  <li v-for="item in items" :key="item.id" class="bg-gray-50 p-4 rounded-md">
                    <div class="flex items-start">
                      <div class="flex-shrink-0 pt-0.5">
                        <input 
                          type="checkbox" 
                          :id="`item-${item.id}`" 
                          :checked="item.isCompleted" 
                          @change="updateChecklistItem(item.id, !item.isCompleted)"
                          :disabled="!canUpdateChecklist || updatingChecklistItem === item.id" 
                          class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
                        />
                      </div>
                      <div class="ml-3 flex-1">
                        <label :for="`item-${item.id}`" class="text-sm font-medium text-gray-900" :class="{ 'line-through': item.isCompleted }">
                          {{ item.description }}
                        </label>
                        <div v-if="item.images && item.images.length > 0" class="mt-3 grid grid-cols-2 sm:grid-cols-3 gap-2">
                          <div v-for="image in item.images" :key="image.id" class="relative group">
                            <img 
                              :src="`${apiUrl}/uploads/${image.fileName}`" 
                              alt="Evidência" 
                              class="h-24 w-full object-cover rounded-md"
                              @click="openImageModal(image)"
                            />
                            <button 
                              v-if="canUpdateChecklist"
                              @click="deleteImage(image.id)"
                              class="absolute top-1 right-1 bg-red-600 text-white rounded-full p-1 opacity-0 group-hover:opacity-100 transition-opacity"
                              title="Excluir imagem"
                            >
                              <svg class="h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                              </svg>
                            </button>
                          </div>
                        </div>
                        <div v-if="canUpdateChecklist" class="mt-3">
                          <label :for="`file-${item.id}`" class="inline-flex items-center px-3 py-1.5 border border-gray-300 shadow-sm text-xs font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 cursor-pointer">
                            <svg class="-ml-0.5 mr-2 h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                              <path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd" />
                            </svg>
                            Adicionar Foto
                          </label>
                          <input 
                            type="file" 
                            :id="`file-${item.id}`" 
                            @change="uploadImage($event, item.id)" 
                            accept="image/jpeg,image/png,image/jpg"
                            class="sr-only" 
                          />
                          <span v-if="uploadingImageForItem === item.id" class="ml-2 text-xs text-indigo-600">Enviando...</span>
                        </div>
                      </div>
                    </div>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
        <div class="mt-8 bg-white shadow overflow-hidden sm:rounded-lg">
          <div class="px-4 py-5 sm:px-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900">
              Imagens da Ordem
            </h3>
            <p class="mt-1 max-w-2xl text-sm text-gray-500">
              Imagens gerais da ordem de serviço.
            </p>
          </div>
          <div class="border-t border-gray-200 px-4 py-5 sm:px-6">
            <div v-if="order.images.length === 0 && !canUpdateChecklist" class="text-center py-4">
              <p class="text-sm text-gray-500">Nenhuma imagem disponível.</p>
            </div>
            <div v-if="canUpdateChecklist" class="mb-4">
              <label for="file-order" class="inline-flex items-center px-4 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 cursor-pointer">
                <svg class="-ml-1 mr-2 h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd" />
                </svg>
                Adicionar Imagem
              </label>
              <input 
                type="file" 
                id="file-order" 
                @change="uploadImage($event)" 
                accept="image/jpeg,image/png,image/jpg" 
                class="sr-only" 
              />
              <span v-if="uploadingImageForOrder" class="ml-2 text-sm text-indigo-600">Enviando...</span>
            </div>
            <div v-if="order.images.length > 0" class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
              <div v-for="image in order.images" :key="image.id" class="relative group">
                <img 
                  :src="`${apiUrl}/uploads/${image.fileName}`" 
                  alt="Imagem da Ordem" 
                  class="h-40 w-full object-cover rounded-md"
                  @click="openImageModal(image)"
                />
                <button 
                  v-if="canUpdateChecklist"
                  @click="deleteImage(image.id)"
                  class="absolute top-2 right-2 bg-red-600 text-white rounded-full p-1 opacity-0 group-hover:opacity-100 transition-opacity"
                  title="Excluir imagem"
                >
                  <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div v-if="showStatusModal" class="fixed z-10 inset-0 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
        <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
          <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" aria-hidden="true" @click="showStatusModal = false"></div>
          <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
          <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <div class="sm:flex sm:items-start">
                <div class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-indigo-100 sm:mx-0 sm:h-10 sm:w-10">
                  <svg class="h-6 w-6 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" />
                  </svg>
                </div>
                <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
                  <h3 class="text-lg leading-6 font-medium text-gray-900" id="modal-title">
                    {{ nextStatusAction.modalTitle }}
                  </h3>
                  <div class="mt-2">
                    <p class="text-sm text-gray-500">
                      {{ nextStatusAction.modalDescription }}
                    </p>
                    <div v-if="nextStatusAction.status === 'Rejected'" class="mt-4">
                      <label for="rejection-reason" class="block text-sm font-medium text-gray-700">
                        Motivo da Rejeição <span class="text-red-500">*</span>
                      </label>
                      <textarea
                        id="rejection-reason"
                        v-model="rejectionReason"
                        rows="3"
                        class="mt-1 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 block w-full sm:text-sm border-gray-300 rounded-md"
                        placeholder="Informe o motivo da rejeição..."
                        required
                      ></textarea>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
              <button 
                type="button" 
                :disabled="updatingOrderStatus"
                @click="updateOrderStatus" 
                class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-indigo-600 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:ml-3 sm:w-auto sm:text-sm"
              >
                <svg v-if="updatingOrderStatus" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                {{ updatingOrderStatus ? 'Processando...' : 'Confirmar' }}
              </button>
              <button 
                type="button" 
                @click="showStatusModal = false" 
                class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      </div>
      <div v-if="selectedImage" class="fixed z-10 inset-0 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
        <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
          <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" aria-hidden="true" @click="selectedImage = null"></div>
          <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
          <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <div class="flex justify-between items-center mb-4">
                <h3 class="text-lg font-medium text-gray-900">
                  {{ selectedImage.fileName }}
                </h3>
                <button 
                  @click="selectedImage = null" 
                  class="rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <span class="sr-only">Fechar</span>
                  <svg class="h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
              <div class="flex justify-center">
                <img 
                  :src="`${apiUrl}/uploads/${selectedImage.fileName}`" 
                  :alt="selectedImage.fileName" 
                  class="max-h-[70vh] max-w-full object-contain"
                />
              </div>
              <div class="mt-4 text-sm text-gray-500">
                <p>Enviado em: {{ formatDate(selectedImage.uploadedAt) }}</p>
                <p>Tamanho: {{ formatFileSize(selectedImage.fileSize) }}</p>
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
import OrderService from '../services/order.service.js';
import AuthService from '../services/auth.service.js';
import { jwtDecode } from 'jwt-decode';

export default {
  name: 'OrderDetailView',
  components: {
    MainLayout
  },
  data() {
    return {
      apiUrl: import.meta.env.VITE_API_URL,
      loading: true,
      error: null,
      order: null,
      currentUser: {
        id: null,
        role: ''
      },
      showStatusModal: false,
      updatingOrderStatus: false,
      rejectionReason: '',
      updatingChecklistItem: null,
      uploadingImageForItem: null,
      uploadingImageForOrder: false,
      selectedImage: null
    };
  },
  computed: {
    canUpdateChecklist() {
      if (!this.order) return false;
      const isOrderOwner = this.order.user.id === this.currentUser.id;
      const isAdminOrSupervisor = this.currentUser.role === 'Admin' || this.currentUser.role === 'Supervisor';
      const isUpdateableStatus = ['Created', 'InProgress'].includes(this.order.status);
      
      return (isOrderOwner || isAdminOrSupervisor) && isUpdateableStatus;
    },
    
    canChangeStatus() {
      if (!this.order) return false;
      if (this.currentUser.role === 'Technician' && this.order.user.id !== this.currentUser.id) {
        return false;
      }
      if ((this.currentUser.role === 'Admin' || this.currentUser.role === 'Supervisor') && 
          this.order.status === 'Completed') {
        return true;
      }
      if (this.currentUser.role === 'Technician') {
        if (this.order.status === 'Created') {
          return true;
        }
        
        if (this.order.status === 'InProgress' && this.order.allChecklistItemsCompleted) {
          return true;
        }
      }
      
      return false;
    },
    
    nextStatusAction() {
      if (!this.order || !this.canChangeStatus) return null;
      const statusActions = {
        'Created': {
          status: 'InProgress',
          label: 'Iniciar Trabalho',
          modalTitle: 'Iniciar Trabalho',
          modalDescription: 'Ao iniciar o trabalho, você confirma que está começando a execução desta ordem de serviço.'
        },
        'InProgress': {
          status: 'Completed',
          label: 'Concluir Ordem',
          modalTitle: 'Concluir Ordem',
          modalDescription: 'Ao concluir, você confirma que todos os itens do checklist foram verificados e o trabalho foi realizado conforme especificado.'
        },
        'Completed': [
          {
            status: 'Approved',
            label: 'Aprovar Ordem',
            modalTitle: 'Aprovar Ordem',
            modalDescription: 'Ao aprovar, você confirma que o trabalho foi realizado corretamente e atende aos requisitos.'
          },
          {
            status: 'Rejected',
            label: 'Rejeitar Ordem',
            modalTitle: 'Rejeitar Ordem',
            modalDescription: 'Ao rejeitar, você indica que o trabalho precisa ser revisto ou corrigido. Por favor, forneça um motivo para a rejeição.'
          }
        ]
      };
      
      if (this.order.status === 'Completed' && 
          (this.currentUser.role === 'Admin' || this.currentUser.role === 'Supervisor')) {
        return statusActions[this.order.status][0];
      }
      
      return statusActions[this.order.status];
    },
    
    groupedChecklist() {
      if (!this.order || !this.order.checklistItems) return {};
      
      return this.order.checklistItems.reduce((grouped, item) => {
        const category = item.category || 'Geral';
        if (!grouped[category]) {
          grouped[category] = [];
        }
        grouped[category].push(item);
        return grouped;
      }, {});
    }
  },
  created() {
    this.loadUserInfo();
    this.fetchOrder();
  },
  methods: {
    loadUserInfo() {
      const user = AuthService.getCurrentUser();
      if (user?.token) {
        try {
          const decodedToken = jwtDecode(user.token);
          this.currentUser.id = Number.parseInt(decodedToken.nameid || '0');
          this.currentUser.role = decodedToken.role || '';
        } catch (error) {
          console.error('Error decoding token:', error);
          this.$router.push('/login');
        }
      } else {
        this.$router.push('/login');
      }
    },
    
    fetchOrder() {
      const id = this.$route.params.id;
      this.loading = true;
      this.error = null;
      
      OrderService.getOrderById(id)
        .then(response => {
          this.order = response.data;
          this.loading = false;
        })
        .catch(error => {
          console.error('Error fetching order:', error);
          this.error = error.response?.data?.message || 'Erro ao carregar a ordem. Tente novamente mais tarde.';
          this.loading = false;
        });
    },
    
    formatDate(dateString) {
      if (!dateString) return 'N/A';
      
      const date = new Date(dateString);
      return new Intl.DateTimeFormat('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }).format(date);
    },
    
    formatFileSize(bytes) {
      if (bytes < 1024) {
        return `${bytes} bytes`;
      }
      if (bytes < 1048576) {
        return `${(bytes / 1024).toFixed(2)} KB`;
      } 
      return `${(bytes / 1048576).toFixed(2)} MB`;
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
    },
    
    openUpdateStatusModal() {
      if (this.nextStatusAction.status === 'Rejected') {
        this.rejectionReason = '';
      }
      this.showStatusModal = true;
    },
    
    updateOrderStatus() {
      if (!this.nextStatusAction) return;
      
      const status = this.nextStatusAction.status;
      let rejectionReason = null;
      if (status === 'Rejected') {
        if (!this.rejectionReason.trim()) {
          alert('É necessário informar o motivo da rejeição.');
          return;
        }
        rejectionReason = this.rejectionReason.trim();
      }
      
      this.updatingOrderStatus = true;
      
      OrderService.updateOrderStatus(this.order.id, status, rejectionReason)
        .then(() => {
          this.showStatusModal = false;
          this.updatingOrderStatus = false;
          this.fetchOrder();
        })
        .catch(error => {
          console.error('Error updating order status:', error);
          alert(error.response?.data?.message || 'Erro ao atualizar o status da ordem.');
          this.updatingOrderStatus = false;
        });
    },
    
    updateChecklistItem(itemId, isCompleted) {
      this.updatingChecklistItem = itemId;
      
      OrderService.updateChecklistItem(itemId, isCompleted)
        .then(() => {
          const item = this.order.checklistItems.find(i => i.id === itemId);
          if (item) {
            item.isCompleted = isCompleted;
          }
          this.order.allChecklistItemsCompleted = this.order.checklistItems.every(item => item.isCompleted);
          
          this.updatingChecklistItem = null;
        })
        .catch(error => {
          console.error('Error updating checklist item:', error);
          alert(error.response?.data?.message || 'Erro ao atualizar o item do checklist.');
          this.updatingChecklistItem = null;
          this.fetchOrder();
        });
    },
    
    uploadImage(event, checklistItemId = null) {
      const file = event.target.files[0];
      if (!file) return;
      const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];
      if (!validTypes.includes(file.type)) {
        alert('Apenas imagens JPG e PNG são permitidas.');
        return;
      }
      const maxSize = 5 * 1024 * 1024;
      if (file.size > maxSize) {
        alert('O tamanho máximo permitido é 5MB.');
        return;
      }
      if (checklistItemId) {
        this.uploadingImageForItem = checklistItemId;
      } else {
        this.uploadingImageForOrder = true;
      }
      
      OrderService.uploadImage(this.order.id, file, checklistItemId)
        .then(() => {
          event.target.value = '';
          this.fetchOrder();
          if (checklistItemId) {
            this.uploadingImageForItem = null;
          } else {
            this.uploadingImageForOrder = false;
          }
        })
        .catch(error => {
          console.error('Error uploading image:', error);
          alert(error.response?.data?.message || 'Erro ao enviar a imagem.');
          if (checklistItemId) {
            this.uploadingImageForItem = null;
          } else {
            this.uploadingImageForOrder = false;
          }
        });
    },
    
    deleteImage(imageId) {
      if (!confirm('Tem certeza que deseja excluir esta imagem?')) {
        return;
      }
      
      OrderService.deleteImage(imageId)
        .then(() => {
          this.fetchOrder();
        })
        .catch(error => {
          console.error('Error deleting image:', error);
          alert(error.response?.data?.message || 'Erro ao excluir a imagem.');
        });
    }, 
    openImageModal(image) {
      this.selectedImage = image;
    }
  }
};
</script>