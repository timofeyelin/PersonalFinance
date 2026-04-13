import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5244/api',
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

apiClient.interceptors.response.use(
    response => response,
    error => {
        return Promise.reject(error);
    }
);

export default apiClient;