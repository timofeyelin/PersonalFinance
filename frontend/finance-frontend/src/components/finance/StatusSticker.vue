<template>
  <div
    v-if="label && color"
    :class="[
      'px-2 py-0.5 rounded-full text-[10px] font-bold text-white inline-block',
      statusClass
    ]"
  >
    {{ label }}
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  color: {
    type: String,
    required: true
  },
  label: {
    type: String,
    required: true
  }
});

const statusClass = computed(() => {
  if (props.color === 'Green') return 'bg-emerald-500';
  if (props.color === 'Yellow') return 'bg-amber-400';
  if (props.color === 'Red') return 'bg-rose-500';
  return 'bg-stone-500';
});
</script>
