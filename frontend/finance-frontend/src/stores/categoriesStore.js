import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import api from '../api/categories';

export const useCategoriesStore = defineStore('categories', () => {
    const categories = ref([]);
    const totalBudget = ref(0);
    const isLoading = ref(false);
    const error = ref(null);

    const activeCategories = computed(() =>
        categories.value.filter(c => c.isActive)
    );

    async function fetchCategories(page = 1, size = 100) {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await api.getAll(page, size);
            categories.value = response.data.items || response.data;
        } catch (e) {
            error.value = e;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchTotalBudget() {
        try {
            const response = await api.getTotalBudget();
            totalBudget.value = response.data.totalBudget;
        } catch (e) {
            console.error(e);
        }
    }

    async function createCategory(data) {
        const response = await api.create(data);
        categories.value.push(response.data);
        return response.data;
    }

    async function updateCategory(id, data) {
        const response = await api.update(id, data);
        const index = categories.value.findIndex(c => c.id === id);
        if (index !== -1) {
            categories.value[index] = response.data;
        }
        return response.data;
    }

    async function deleteCategory(id) {
        await api.delete(id);
        categories.value = categories.value.filter(c => c.id !== id);
    }

    return {
        categories,
        totalBudget,
        isLoading,
        error,
        activeCategories,
        fetchCategories,
        fetchTotalBudget,
        createCategory,
        updateCategory,
        deleteCategory
    };
});