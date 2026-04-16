<template>
  <div class="pb-6">
    <div class="flex bg-stone-100 p-1 rounded-2xl mb-6 shadow-inner">
      <button
        v-for="mode in modes"
        :key="mode.id"
        @click="filterMode = mode.id"
        :class="[
          'flex-1 py-2 text-xs font-bold rounded-xl transition-all',
          filterMode === mode.id ? 'bg-white text-emerald-700 shadow-sm' : 'text-stone-500 hover:text-stone-700'
        ]"
      >
        {{ mode.label }}
      </button>
    </div>

    <div v-if="filterMode !== 'all'" class="mb-6">
      <input
        v-if="filterMode === 'day'"
        v-model="selectedDate"
        type="date"
        class="w-full px-4 py-3 rounded-xl border border-stone-200 bg-white focus:outline-none focus:ring-2 focus:ring-emerald-500 font-medium"
      />
      <input
        v-if="filterMode === 'month'"
        v-model="selectedMonth"
        type="month"
        class="w-full px-4 py-3 rounded-xl border border-stone-200 bg-white focus:outline-none focus:ring-2 focus:ring-emerald-500 font-medium"
      />
    </div>

    <div class="text-center mb-8 pt-4">
      <div class="text-xs text-stone-500 uppercase tracking-wider mb-1 font-medium">
        {{ currentPeriodLabel }}
      </div>
      <div class="text-4xl font-bold text-stone-800">
        -{{ formatCurrency(totalSpent) }}
      </div>
    </div>

    <div v-if="store.isLoading" class="text-center text-stone-400 mt-10">       
      Загрузка...
    </div>

    <div v-else-if="Object.keys(groupedTransactions).length === 0" class="text-center text-stone-400 mt-10">
      Нет транзакций
    </div>

    <div v-else>
      <div v-for="(group, date) in groupedTransactions" :key="date" class="mb-6">
        <div class="flex items-center justify-between mb-3 px-4">
          <h2 class="text-[11px] text-stone-400 uppercase tracking-wide font-medium">
            {{ formatDateGroup(date) }}
          </h2>
          <StatusSticker
            v-if="store.dayStatusesCache[date]"
            :color="store.dayStatusesCache[date].statusColor"
            :label="store.dayStatusesCache[date].label"
          />
        </div>

        <TransactionCard
          v-for="transaction in group"
          :key="transaction.id"
          :transaction="transaction"
          @edit="handleEdit"
          @delete="handleDelete"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useTransactionsStore } from '../stores/transactionsStore';
import TransactionCard from '../components/finance/TransactionCard.vue';        
import StatusSticker from '../components/finance/StatusSticker.vue';
import { formatDateGroup, formatCurrency } from '../utils/formatters';

const store = useTransactionsStore();
const router = useRouter();


const filterMode = ref('month');
const selectedDate = ref(new Date().toISOString().split('T')[0]);
const selectedMonth = ref(new Date().toISOString().slice(0, 7));

const modes = [
  { id: 'all', label: 'Все время' },
  { id: 'month', label: 'Месяц' },
  { id: 'day', label: 'День' }
];

const currentPeriodLabel = computed(() => {
  if (filterMode.value === 'all') return 'Всего за все время';   
  if (filterMode.value === 'month') return 'Всего за месяц';        
  return 'Всего за день';
});

const loadData = async () => {
  if (filterMode.value === 'all') {
    await store.fetchTransactions(1, 100);
  } else if (filterMode.value === 'month') {
    const [year, month] = selectedMonth.value.split('-');
    await store.fetchByMonth(parseInt(year, 10), parseInt(month, 10), 1, 100);  
  } else if (filterMode.value === 'day') {
    await store.fetchByDate(selectedDate.value, 1, 100);
  }
};

onMounted(loadData);

const handleEdit = (id) => {
  router.push(`/transaction/edit/${id}`);
};

const handleDelete = async (id) => {
  if (confirm('Удалить эту транзакцию?')) {
    try {
      await store.deleteTransaction(id);
      await loadData();
    } catch (e) {
      console.error(e);
      alert('Ошибка при удалении');
    }
  }
};

watch([filterMode, selectedDate, selectedMonth], loadData);

const groupedTransactions = computed(() => {
  if (!store.transactions) return {};

  return store.transactions.reduce((acc, transaction) => {
    const date = transaction.date;
    if (!acc[date]) {
      acc[date] = [];
    }
    acc[date].push(transaction);
    return acc;
  }, {});
});

watch(groupedTransactions, (newGroups) => {
  if (newGroups) {
    Object.keys(newGroups).forEach(date => {
      store.loadDayStatus(date);
    });
  }
}, { immediate: true });

const totalSpent = computed(() => {
  if (!store.transactions) return 0;
  return store.transactions.reduce((sum, t) => sum + t.amount, 0);
});

const getGroupTotal = (group) => {
  return group.reduce((sum, t) => sum + t.amount, 0);
};
</script>
