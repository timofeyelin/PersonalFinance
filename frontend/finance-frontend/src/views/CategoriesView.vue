<template>
  <div class="pb-24">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-stone-800">Категории</h1>
      <button 
        @click="openModal()" 
        class="bg-emerald-100 text-emerald-700 px-4 py-2 rounded-xl font-bold text-sm"
      >
        + Добавить
      </button>
    </div>

    <div class="bg-emerald-500 text-white rounded-[24px] p-6 mb-6 shadow-lg">
      <div class="text-emerald-100 text-sm mb-1">Общий плановый бюджет</div>
      <div class="text-3xl font-bold">{{ formatCurrency(store.totalBudget) }} / мес</div>
    </div>

    <div v-if="store.isLoading" class="text-center text-stone-400 mt-10">
      Загрузка...
    </div>

    <div v-else class="space-y-3">
      <div 
        v-for="category in store.categories" 
        :key="category.id"
        class="bg-white p-4 rounded-[20px] shadow-sm border border-stone-100 flex items-center justify-between"
      >
        <div>
          <div class="flex items-center gap-2 mb-1">
            <span class="font-bold text-stone-800">{{ category.name }}</span>
            <span 
              v-if="!category.isActive" 
              class="bg-stone-100 text-stone-500 text-[10px] px-2 py-0.5 rounded-full font-bold uppercase"
            >
              Неактивна
            </span>
          </div>
          <div class="text-sm text-stone-500">
            Бюджет: {{ formatCurrency(category.monthlyBudget) }}
          </div>
        </div>

        <div class="flex gap-2">
          <button @click="openModal(category)" class="p-2 text-stone-400 hover:text-emerald-600 bg-stone-50 rounded-lg">
            ✎
          </button>
          <button @click="handleDelete(category.id)" class="p-2 text-stone-400 hover:text-rose-500 bg-stone-50 rounded-lg">
            ✕
          </button>
        </div>
      </div>
    </div>

    <div v-if="isModalOpen" class="fixed inset-0 bg-stone-900/50 z-50 flex items-end sm:items-center justify-center p-4 pb-24 sm:pb-4">
      <div class="bg-white w-full max-w-md rounded-[32px] p-6 shadow-2xl animate-slide-up sm:animate-fade-in">
        <h2 class="text-xl font-bold text-stone-800 mb-6">
          {{ editingId ? 'Редактировать категорию' : 'Новая категория' }}
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
            <label class="block text-sm font-medium text-stone-700 mb-1">Месячный бюджет (₽)</label>
            <input
              v-model.number="form.monthlyBudget"
              type="number"
              min="0"
              required
              class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500"
            />
          </div>

          <div class="flex items-center gap-3 pt-2">
            <input
              v-model="form.isActive"
              type="checkbox"
              id="isActive"
              class="w-5 h-5 text-emerald-500 rounded border-stone-300 focus:ring-emerald-500"
            />
            <label for="isActive" class="text-sm font-medium text-stone-700">Активная категория</label>
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
import { ref, onMounted } from 'vue';
import { useCategoriesStore } from '../stores/categoriesStore';
import { formatCurrency } from '../utils/formatters';

const store = useCategoriesStore();

const isModalOpen = ref(false);
const isSubmitting = ref(false);
const editingId = ref(null);

const form = ref({
  name: '',
  monthlyBudget: 0,
  isActive: true
});

onMounted(async () => {
  await store.fetchCategories();
  await store.fetchTotalBudget();
});

const openModal = (category = null) => {
  if (category) {
    editingId.value = category.id;
    form.value = { 
      name: category.name, 
      monthlyBudget: category.monthlyBudget, 
      isActive: category.isActive 
    };
  } else {
    editingId.value = null;
    form.value = { name: '', monthlyBudget: 0, isActive: true };
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
      monthlyBudget: form.value.monthlyBudget
    };
    
    if (editingId.value) {
      await store.updateCategory(editingId.value, { 
        ...payload, 
        isActive: form.value.isActive 
      });
    } else {
      await store.createCategory(payload);
    }
    await store.fetchTotalBudget();
    closeModal();
  } catch (error) {
    const msg = error.response?.data?.errors 
      ? Object.values(error.response.data.errors).flat().join('\n')
      : error.response?.data?.title || 'Ошибка';
    alert(msg);
  } finally {
    isSubmitting.value = false;
  }
};

const handleDelete = async (id) => {
  if (confirm('Вы уверены, что хотите удалить эту категорию?')) {
    try {
      await store.deleteCategory(id);
      await store.fetchTotalBudget();
    } catch (error) {
      alert('Не удалось удалить категорию. Возможно, к ней привязаны статьи расходов.');
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