import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5244/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 400) {
      console.error('Ошибка валидации:', error.response.data);
    }
    return Promise.reject(error);
  }
);

export default apiClient;