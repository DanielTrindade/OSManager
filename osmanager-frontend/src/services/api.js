import axios from 'axios';
import router from '../../router/index';

const API_URL = import.meta.env.VITE_API_URL;

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

api.interceptors.request.use(
  config => {
    const user = JSON.parse(localStorage.getItem('user'));
    
    if (user?.token) {
      config.headers.Authorization = `Bearer ${user.token}`;
    }
    
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  response => response,
  error => {
    if (error.response && (error.response.status === 401 || error.response.status === 403)) {
      localStorage.removeItem('user');
      router.push('/login');
    }
    
    return Promise.reject(error);
  }
);

export default api;