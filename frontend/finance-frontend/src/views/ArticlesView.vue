<template>
  <div class="pb-24">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-stone-800">Статьи</h1>
      <button 
        @click="openModal()" 
        class="bg-emerald-100 text-emerald-700 px-4 py-2 rounded-xl font-bold text-sm"
      >
        + Добавить
      </button>
    </div>

    <div v-if="articlesStore.isLoading || categoriesStore.isLoading" class="text-center text-stone-400 mt-10">
      Загрузка...
    </div>

    <div v-else-if="enrichedArticles.length === 0" class="text-center text-stone-400 mt-10">
      Список статей пуст. Сначала создайте категорию.
    </div>

    <div v-else class="space-y-3">
      <div 
        v-for="article in enrichedArticles" 
        :key="article.id"
        class="bg-white p-4 rounded-[20px] shadow-sm border border-stone-100 flex items-center justify-between"
      >
        <div>
          <div class="flex items-center gap-2 mb-1">
            <span class="font-bold text-stone-800">{{ article.name }}</span>
            <span 
              v-if="!article.isActive" 
              class="bg-stone-100 text-stone-500 text-[10px] px-2 py-0.5 rounded-full font-bold uppercase"
            >
              Неактивна
            </span>
          </div>
          <div class="text-xs font-medium text-stone-400 uppercase tracking-wide">
            Категория: <span class="text-stone-500">{{ article.categoryName }}</span>
          </div>
        </div>

        <div class="flex gap-2 shrink-0">
          <button @click="openModal(article)" class="p-2 text-stone-400 hover:text-emerald-600 bg-stone-50 rounded-lg">
            ✎
          </button>
          <button @click="handleDelete(article.id)" class="p-2 text-stone-400 hover:text-rose-500 bg-stone-50 rounded-lg">
            ✕
          </button>
        </div>
      </div>
    </div>

    <div v-if="isModalOpen" class="fixed inset-0 bg-stone-900/50 z-50 flex items-end sm:items-center justify-center p-4 pb-24 sm:pb-4">
      <div class="bg-white w-full max-w-md rounded-[32px] p-6 shadow-2xl animate-slide-up sm:animate-fade-in">
        <h2 class="text-xl font-bold text-stone-800 mb-6">
          {{ editingId ? 'Редактировать статью' : 'Новая статья' }}
        </h2>

        <form @submit.prevent="handleSubmit" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-stone-700 mb-1">Название</label>
            <input
              v-model="form.name"
              type="text"
              required
              class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-stone-700 mb-1">Категория</label>
            <select
              v-model="form.categoryId"
              required
              class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white"
            >
              <option value="" disabled>Выберите категорию</option>
              <option v-for="cat in categoriesStore.categories" :key="cat.id" :value="cat.id">
                {{ cat.name }} {{ !cat.isActive ? '(Неактивна)' : '' }}
              </option>
            </select>
          </div>

          <div class="flex items-center gap-3 pt-2">
            <input
              v-model="form.isActive"
              type="checkbox"
              id="isActiveArticle"
              class="w-5 h-5 text-emerald-500 rounded border-stone-300 focus:ring-emerald-500"
            />
            <label for="isActiveArticle" class="text-sm font-medium text-stone-700">Активная статья</label>
          </div>

          <div class="pt-6 flex gap-3">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 py-4 bg-stone-100 text-stone-700 font-bold rounded-xl"
            >
              Отмена
            </button>
            <button
              type="submit"
              :disabled="isSubmitting"
              class="flex-1 py-4 bg-emerald-500 text-white font-bold rounded-xl disabled:bg-stone-300"
            >
              Сохранить
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useArticlesStore } from '../stores/articlesStore';
import { useCategoriesStore } from '../stores/categoriesStore';

const articlesStore = useArticlesStore();
const categoriesStore = useCategoriesStore();

const isModalOpen = ref(false);
const isSubmitting = ref(false);
const editingId = ref(null);

const form = ref({
  name: '',
  categoryId: '',
  isActive: true
});

onMounted(async () => {
  await Promise.all([
    articlesStore.fetchArticles(),
    categoriesStore.fetchCategories()
  ]);
});

const enrichedArticles = computed(() => articlesStore.articles);

const openModal = (article = null) => {
  if (article) {
    editingId.value = article.id;
    form.value = { 
      name: article.name, 
      categoryId: article.categoryId, 
      isActive: article.isActive 
    };
  } else {
    editingId.value = null;
    form.value = { name: '', categoryId: '', isActive: true };
  }
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
  editingId.value = null;
};

const handleSubmit = async () => {
  isSubmitting.value = true;
  try {
    const payload = {
      name: form.value.name,
      categoryId: form.value.categoryId
    };

    if (editingId.value) {
      await articlesStore.updateArticle(editingId.value, { 
        ...payload, 
        isActive: form.value.isActive 
      });
    } else {
      await articlesStore.createArticle(payload);
    }
    closeModal();
  } catch (error) {
    alert(error.response?.data?.title || 'Произошла ошибка при сохранении');
  } finally {
    isSubmitting.value = false;
  }
};

const handleDelete = async (id) => {
  if (confirm('Вы уверены, что хотите удалить эту статью?')) {
    try {
      await articlesStore.deleteArticle(id);
    } catch (error) {
      alert('Не удалось удалить статью. Возможно, по ней уже есть транзакции.');
    }
  }
};
</script>

<style scoped>
.animate-slide-up {
  animation: slideUp 0.3s ease-out forwards;
}

@keyframes slideUp {
  from { transform: translateY(100%); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}
</style>