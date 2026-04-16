<template>
  <div class="bg-white rounded-[24px] p-4 flex items-center justify-between mb-3 shadow-[0_4px_20px_-10px_rgba(0,0,0,0.05)]">
    <div class="flex items-center gap-4 overflow-hidden">
      <div class="w-12 h-12 shrink-0 rounded-full bg-stone-100 flex items-center justify-center text-xl">
        {{ transaction.emoji || '💳' }} 
      </div>
      <div class="truncate">
        <div class="font-semibold text-stone-800 text-sm truncate">
          {{ transaction.articleName || 'Статья расходов' }}
        </div>
        <div class="text-xs text-stone-500 mt-0.5 truncate">
          {{ transaction.comment || 'Без комментария' }}
        </div>
      </div>
    </div>
    <div class="flex items-center gap-4 shrink-0 mt-4 md:mt-0">
      <div class="font-bold text-stone-800 text-lg">
        -{{ formatCurrency(transaction.amount) }}
      </div>
      <div class="flex gap-2">
        <button
          @click.stop="$emit('edit', transaction.id)"
          class="w-8 h-8 rounded-full bg-stone-100 flex items-center justify-center text-stone-500 hover:text-emerald-600 hover:bg-emerald-50 transition-colors"
          title="Редактировать"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
  <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
  <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
</svg>
        </button>
        <button
          @click.stop="$emit('delete', transaction.id)"
          class="w-8 h-8 rounded-full bg-stone-100 flex items-center justify-center text-stone-500 hover:text-red-600 hover:bg-red-50 transition-colors"
          title="Удалить"
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-4 h-4">
            <path stroke-linecap="round" stroke-linejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { formatCurrency } from '../../utils/formatters';

defineProps({
  transaction: {
    type: Object,
    required: true
  }
});

defineEmits(['edit', 'delete']);
</script>