import apiClient from './axios';

export default {
    create(data) {
        return apiClient.post('/ExpenseArticles', data);
    },
    getById(id) {
        return apiClient.get(`/ExpenseArticles/${id}`);
    },
    getAll(page = 1, size = 10) {
        return apiClient.get('/ExpenseArticles', { params: { page, size } });
    },
    getByCategoryId(categoryId, page = 1, size = 10) {
        return apiClient.get(`/ExpenseArticles/by-category/${categoryId}`, { params: { page, size } });
    },
    update(id, data) {
        return apiClient.put(`/ExpenseArticles/${id}`, data);
    },
    delete(id) {
        return apiClient.delete(`/ExpenseArticles/${id}`);
    }
};