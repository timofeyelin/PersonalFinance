import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '../api/transactions';

export const useTransactionsStore = defineStore('transactions', () => {
    const transactions = ref([]);
    const currentDayStatus = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    async function fetchTransactions(page = 1, size = 100) {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await api.getAll(page, size);
            transactions.value = response.data.items || response.data;
        } catch (e) {
            error.value = e;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchByMonth(year, month, page = 1, size = 100) {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await api.getByMonth(year, month, page, size);
            transactions.value = response.data.items || response.data;
        } catch (e) {
            error.value = e;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchByDate(date, page = 1, size = 100) {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await api.getByDate(date, page, size);
            transactions.value = response.data.items || response.data;
        } catch (e) {
            error.value = e;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchDayStatus(date) {
        try {
            const response = await api.getDayStatus(date);
            currentDayStatus.value = response.data;
        } catch (e) {
            console.error(e);
        }
    }

    async function createTransaction(data) {
        const response = await api.create(data);
        transactions.value.push(response.data);
        return response.data;
    }

    async function updateTransaction(id, data) {
        const response = await api.update(id, data);
        const index = transactions.value.findIndex(t => t.id === id);
        if (index !== -1) {
            transactions.value[index] = response.data;
        }
        return response.data;
    }

    async function deleteTransaction(id) {
        await api.delete(id);
        transactions.value = transactions.value.filter(t => t.id !== id);
    }

    return {
        transactions,
        currentDayStatus,
        isLoading,
        error,
        fetchTransactions,
        fetchByMonth,
        fetchByDate,
        fetchDayStatus,
        createTransaction,
        updateTransaction,
        deleteTransaction
    };
});