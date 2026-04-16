<template>
  <div class="relative inline-block">
    <button 
      type="button"
      @click="togglePicker"
      class="w-12 h-12 shrink-0 rounded-full bg-stone-100 flex items-center justify-center text-xl hover:bg-stone-200 transition-colors focus:outline-none focus:ring-2 focus:ring-emerald-500"
    >
      {{ modelValue || defaultEmoji }}
    </button>

    <div 
      v-if="isOpen" 
      class="absolute top-14 left-0 z-50 bg-white border border-stone-200 rounded-2xl p-3 shadow-xl w-64"
    >
      <div class="grid grid-cols-5 gap-2">
        <button
          v-for="emoji in emojiList"
          :key="emoji"
          type="button"
          @click="selectEmoji(emoji)"
          class="w-10 h-10 flex items-center justify-center text-xl hover:bg-stone-100 rounded-xl transition-colors"
        >
          {{ emoji }}
        </button>
      </div>
    </div>
    
    <div 
      v-if="isOpen" 
      @click="isOpen = false" 
      class="fixed inset-0 z-40"
    ></div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  defaultEmoji: {
    type: String,
    default: '💳'
  }
});

const emit = defineEmits(['update:modelValue']);

const isOpen = ref(false);

const emojiList = [
  '💳', '💵', '🛒', '🍔', '☕', 
  '🚗', '⛽', '🏠', '💡', '📱', 
  '🏥', '💊', '🎮', '✈️', '🎁',
  '🎓', '👗', '💇', '🐶', '🛠️'
];

const togglePicker = () => {
  isOpen.value = !isOpen.value;
};

const selectEmoji = (emoji) => {
  emit('update:modelValue', emoji);
  isOpen.value = false;
};
</script>