<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          OSManager
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Sistema de Gerenciamento de Ordens de Serviço
        </p>
      </div>

      <div class="bg-white p-8 rounded-lg shadow-md">
        <form class="space-y-6" @submit.prevent="handleLogin">
          <div class="alert alert-danger" v-if="message">
            {{ message }}
          </div>

          <div>
            <label for="username" class="block text-sm font-medium text-gray-700">Usuário</label>
            <div class="mt-1">
              <input id="username" v-model="user.username" name="username" type="text" required
                class="appearance-none rounded-md relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                placeholder="Nome de usuário" />
            </div>
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">Senha</label>
            <div class="mt-1">
              <input id="password" v-model="user.password" name="password" type="password" required
                class="appearance-none rounded-md relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                placeholder="Sua senha" />
            </div>
          </div>

          <div>
            <button type="submit" :disabled="loading"
              class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
              <span class="absolute left-0 inset-y-0 flex items-center pl-3">
                <svg v-if="!loading" class="h-5 w-5 text-indigo-500 group-hover:text-indigo-400"
                  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                  <path fill-rule="evenodd"
                    d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                    clip-rule="evenodd" />
                </svg>
                <svg v-if="loading" class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg"
                  fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                  </path>
                </svg>
              </span>
              {{ loading ? 'Entrando...' : 'Entrar' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import AuthService from '../services/auth.service.js';

export default {
  name: 'LoginView',
  data() {
    return {
      user: {
        username: '',
        password: ''
      },
      loading: false,
      message: ''
    };
  },
  methods: {
    handleLogin() {
      this.loading = true;
      this.message = '';
      if (this.user.username && this.user.password) {
        AuthService.login(this.user.username, this.user.password)
          .then(() => {
            this.$router.push('/dashboard');
          })
          .catch(error => {
            this.loading = false;
            this.message = (error.response?.data?.message) ||
              'Erro ao fazer login. Verifique suas credenciais.';
          });
      }
    }
  }
};
</script>