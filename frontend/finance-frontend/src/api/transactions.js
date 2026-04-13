import apiClient from './axios';

export default {
    create(data) {
        return apiClient.post('/Transactions', data);
    },
    getById(id) {
        return apiClient.get(`/Transactions/${id}`);
    },
    getAll(page = 1, size = 10) {
        return apiClient.get('/Transactions', { params: { page, size } });
    },
    getByMonth(year, month, page = 1, size = 10) {
        return apiClient.get('/Transactions/by-month', { params: { year, month, page, size } });
    },
    getDayStatus(date) {
        return apiClient.get('/Transactions/day-status', { params: { date } });
    },
    getByDate(date, page = 1, size = 10) {
        return apiClient.get('/Transactions/by-date', { params: { date, page, size } });
    },
    update(id, data) {
        return apiClient.put(`/Transactions/${id}`, data);
    },
    delete(id) {
        return apiClient.delete(`/Transactions/${id}`);
    }
};