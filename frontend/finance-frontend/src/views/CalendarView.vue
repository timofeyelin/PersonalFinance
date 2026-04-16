<template>
  <div class="pb-10">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-stone-800">Календарь</h1>     
      <div class="flex gap-2">
        <button @click="changeMonth(-1)" class="p-2 rounded-lg bg-stone-100 text-stone-600">←</button>
        <span class="font-semibold text-stone-700 min-w-[120px] text-center">   
          {{ monthLabel }}
        </span>
        <button @click="changeMonth(1)" class="p-2 rounded-lg bg-stone-100 text-stone-600">→</button>
      </div>
    </div>

    <div class="bg-white rounded-[28px] p-4 shadow-sm border border-stone-100"> 
      <div class="grid grid-cols-7 mb-2">
        <div v-for="day in weekDays" :key="day" class="text-center text-[10px] font-bold text-stone-400 uppercase py-2">
          {{ day }}
        </div>
      </div>

      <div class="grid grid-cols-7 gap-1">
        <div
          v-for="(date, index) in calendarDays"
          :key="index"
          :class="[
            'min-h-[80px] p-1 border rounded-xl flex flex-col items-center justify-start transition-colors',
            date.isCurrentMonth ? 'border-stone-50' : 'border-transparent opacity-20',
            isSelected(date.fullDate) ? 'bg-emerald-50 border-emerald-200' : '' 
          ]"
          @click="selectDate(date.fullDate)"
        >
          <span :class="['text-sm font-medium mb-1', date.isToday ? 'text-emerald-600 font-bold' : 'text-stone-700']">
            {{ date.day }}
          </span>

          <StatusSticker
            v-if="store.dayStatusesCache[date.fullDate]"
            :color="store.dayStatusesCache[date.fullDate].statusColor"
            :label="store.dayStatusesCache[date.fullDate].label"
          />
        </div>
      </div>
    </div>

    <div v-if="selectedDate" class="mt-8">
      <h3 class="text-sm font-bold text-stone-400 uppercase tracking-wider mb-4 px-2">
        Траты за {{ selectedDate }}
      </h3>
      <div v-if="dailyTransactions.length > 0">
        <TransactionCard
          v-for="t in dailyTransactions"
          :key="t.id"
          :transaction="t"
        />
      </div>
      <div v-else class="text-center py-10 bg-stone-50 rounded-3xl text-stone-400 text-sm border-2 border-dashed border-stone-200">
        В этот день не было расходов
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useTransactionsStore } from '../stores/transactionsStore';
import StatusSticker from '../components/finance/StatusSticker.vue';
import TransactionCard from '../components/finance/TransactionCard.vue';        

const store = useTransactionsStore();
const currentDate = ref(new Date());
const selectedDate = ref(new Date().toISOString().split('T')[0]);

const weekDays = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];      

const monthLabel = computed(() => {
  return new Intl.DateTimeFormat('ru-RU', { month: 'long', year: 'numeric' }).format(currentDate.value);
});

const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear();
  const month = currentDate.value.getMonth();

  const firstDay = new Date(year, month, 1);
  const lastDay = new Date(year, month + 1, 0);

  const days = [];

  let startOffset = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;        

  const prevLastDay = new Date(year, month, 0).getDate();
  for (let i = startOffset - 1; i >= 0; i--) {
    days.push({ day: prevLastDay - i, isCurrentMonth: false });
  }

  const today = new Date().toISOString().split('T')[0];
  for (let i = 1; i <= lastDay.getDate(); i++) {
    const fullDate = `${year}-${String(month + 1).padStart(2, '0')}-${String(i).padStart(2, '0')}`;
    days.push({
      day: i,
      isCurrentMonth: true,
      fullDate,
      isToday: fullDate === today
    });
  }

  return days;
});

const totalsMap = computed(() => {
  const map = {};
  store.transactions.forEach(t => {
    map[t.date] = (map[t.date] || 0) + t.amount;
  });
  return map;
});

const dailyTransactions = computed(() => {
  return store.transactions.filter(t => t.date === selectedDate.value);
});

onMounted(() => fetchMonthData());

const fetchMonthData = async () => {
  const year = currentDate.value.getFullYear();
  const month = currentDate.value.getMonth() + 1;
  await store.fetchByMonth(year, month, 1, 100);
};

watch(totalsMap, (newTotals) => {
  if (newTotals) {
    Object.keys(newTotals).forEach(date => {
      if (newTotals[date] > 0) {
        store.loadDayStatus(date);
      }
    });
  }
}, { immediate: true });

const changeMonth = (delta) => {
  currentDate.value = new Date(currentDate.value.setMonth(currentDate.value.getMonth() + delta));
  fetchMonthData();
};

const selectDate = (date) => {
  if (date) selectedDate.value = date;
};

const isSelected = (date) => selectedDate.value === date;

const getDailyTotal = (date) => totalsMap.value[date] || 0;
</script>
