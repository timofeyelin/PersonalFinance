<template>
  <div class="pb-24">
    <h1 class="text-2xl font-bold text-stone-800 mb-6">{{ isEditing ? 'Редактировать транзакцию' : 'Новая транзакция' }}</h1>
    
    <div v-if="errorMessage" class="bg-red-100 text-red-700 p-4 rounded-xl mb-6">
      {{ errorMessage }}
    </div>

    <form @submit.prevent="handleSubmit" class="space-y-4">
      <div>
        <label class="block text-sm font-medium text-stone-700 mb-1">Дата</label>
        <input
          v-model="form.date"
          type="date"
          required
          class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500"
          @change="handleDateChange"
        />
      </div>

      <div>
        <label class="block text-sm font-medium text-stone-700 mb-1">Категория</label>
        <select
          v-model="form.categoryId"
          required
          class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white"
          @change="form.articleId = ''"
        >
          <option value="" disabled>Выберите категорию</option>
          <option v-for="cat in activeCategories" :key="cat.id" :value="cat.id">
            {{ cat.name }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium text-stone-700 mb-1">Статья расходов</label>
        <select
          v-model="form.articleId"
          required
          :disabled="!form.categoryId"
          class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white disabled:bg-stone-100"
        >
          <option value="" disabled>Выберите статью</option>
          <option v-for="art in availableArticles" :key="art.id" :value="art.id">
            {{ art.name }}
          </option>
        </select>
      </div>

      <<div class="flex items-center gap-4 mb-4">
  <div class="flex flex-col items-center">
    <label class="block text-xs font-medium text-stone-500 mb-1">Иконка</label>
    <EmojiPicker v-model="form.emoji" />
  </div>
  <div class="flex-1">
    <label class="block text-sm font-medium text-stone-700 mb-1">Сумма (₽)</label>
    <input
      v-model.number="form.amount"
      type="number"
      min="0.01"
      step="0.01"
      required
      class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500"
      @input="validateLimit"
    />
  </div>
</div>

      <div>
        <label class="block text-sm font-medium text-stone-700 mb-1">Комментарий</label>
        <input
          v-model="form.comment"
          type="text"
          class="w-full px-4 py-3 rounded-xl border border-stone-200 focus:outline-none focus:ring-2 focus:ring-emerald-500"
        />
      </div>

      <button
        type="submit"
        :disabled="isSubmitting || isLimitExceeded"
        class="w-full bg-emerald-500 text-white font-bold py-4 rounded-xl mt-6 disabled:bg-stone-300 transition-colors"
      >
        {{ isEditing ? 'Обновить транзакцию' : 'Сохранить транзакцию' }}
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useCategoriesStore } from '../stores/categoriesStore';
import { useArticlesStore } from '../stores/articlesStore';
import { useTransactionsStore } from '../stores/transactionsStore';
import api from '../api/transactions';
import EmojiPicker from '../components/ui/EmojiPicker.vue';

const router = useRouter();
const route = useRoute();
const categoriesStore = useCategoriesStore();
const articlesStore = useArticlesStore();
const transactionsStore = useTransactionsStore();

const transactionId = computed(() => route.params.id);
const isEditing = computed(() => !!transactionId.value);

const form = ref({
  date: new Date().toISOString().split('T')[0],
  categoryId: '',
  articleId: '',
  amount: null,
  comment: '',
  emoji: '💳'
});

const isSubmitting = ref(false);
const errorMessage = ref('');
const currentDailyTotal = ref(0);
const initialAmount = ref(0);
const initialDate = ref('');

onMounted(async () => {
  await categoriesStore.fetchCategories();
  await articlesStore.fetchArticles();
  
  if (isEditing.value) {
    try {
      const resp = await api.getById(transactionId.value);
      const data = resp.data;
      
      const article = articlesStore.articles.find(a => a.id === data.articleId);
      
      form.value = {
        date: data.date,
        categoryId: article ? article.categoryId : '',
        articleId: data.articleId,
        amount: data.amount,
        comment: data.comment || '',
        emoji: data.emoji || '💳'
      };
      initialAmount.value = data.amount;
      initialDate.value = data.date;
    } catch (e) {
      errorMessage.value = 'Ошибка загрузки транзакции';
    }
  }

  await fetchDailyTotal(form.value.date);
});

const activeCategories = computed(() => categoriesStore.activeCategories);

const availableArticles = computed(() => {
  if (!form.value.categoryId) return [];
  return articlesStore.activeArticles.filter(a => a.categoryId === form.value.categoryId);
});

const isLimitExceeded = computed(() => {
  let adjustedTotal = currentDailyTotal.value;
  if (isEditing.value && form.value.date === initialDate.value) {
    adjustedTotal -= initialAmount.value;
  }
  return (adjustedTotal + (form.value.amount || 0)) > 1000000;
});

const fetchDailyTotal = async (date) => {
  await transactionsStore.fetchDayStatus(date);
  currentDailyTotal.value = transactionsStore.currentDayStatus?.totalAmount || 0;
  validateLimit();
};

const handleDateChange = async () => {
  await fetchDailyTotal(form.value.date);
};

const validateLimit = () => {
  if (isLimitExceeded.value) {
    let current = currentDailyTotal.value;
    if (isEditing.value && form.value.date === initialDate.value) {
      current -= initialAmount.value;
    }
    errorMessage.value = `Превышен лимит 1 000 000 ₽. В этот день уже потрачено: ${current} ₽.`;
  } else {
    errorMessage.value = '';
  }
};

const handleSubmit = async () => {
  if (isLimitExceeded.value) return;

  isSubmitting.value = true;
  errorMessage.value = '';

  try {
    const payload = {
      date: form.value.date,
      amount: form.value.amount,
      comment: form.value.comment || "",
      articleId: form.value.articleId,
      emoji: form.value.emoji
    };

    if (isEditing.value) {
      await transactionsStore.updateTransaction(transactionId.value, payload);
    } else {
      await transactionsStore.createTransaction(payload);
    }
    router.push('/');
  } catch (error) {
    errorMessage.value = error.response?.data?.title || 'Ошибка сервера при сохранении';
  } finally {
    isSubmitting.value = false;
  }
};
</script>