import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import api from '../api/articles';

export const useArticlesStore = defineStore('articles', () => {
    const articles = ref([]);
    const isLoading = ref(false);
    const error = ref(null);

    const activeArticles = computed(() =>
        articles.value.filter(a => a.isActive)
    );

    async function fetchArticles(page = 1, size = 100) {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await api.getAll(page, size);
            articles.value = response.data.items || response.data;
        } catch (e) {
            error.value = e;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchByCategoryId(categoryId, page = 1, size = 100) {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await api.getByCategoryId(categoryId, page, size);
            articles.value = response.data.items || response.data;
        } catch (e) {
            error.value = e;
        } finally {
            isLoading.value = false;
        }
    }

    async function createArticle(data) {
        const response = await api.create(data);
        articles.value.push(response.data);
        return response.data;
    }

    async function updateArticle(id, data) {
        const response = await api.update(id, data);
        const index = articles.value.findIndex(a => a.id === id);
        if (index !== -1) {
            articles.value[index] = response.data;
        }
        return response.data;
    }

    async function deleteArticle(id) {
        await api.delete(id);
        articles.value = articles.value.filter(a => a.id !== id);
    }

    return {
        articles,
        isLoading,
        error,
        activeArticles,
        fetchArticles,
        fetchByCategoryId,
        createArticle,
        updateArticle,
        deleteArticle
    };
});