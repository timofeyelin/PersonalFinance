import apiClient from './axios';

export default {
    create(data) {
        return apiClient.post('/Categories', data);
    },
    getById(id) {
        return apiClient.get(`/Categories/${id}`);
    },
    getAll(page = 1, size = 10) {
        return apiClient.get('/Categories', { params: { page, size } });
    },
    getTotalBudget() {
        return apiClient.get('/Categories/total-budget');
    },
    update(id, data) {
        return apiClient.put(`/Categories/${id}`, data);
    },
    delete(id) {
        return apiClient.delete(`/Categories/${id}`);
    }
};