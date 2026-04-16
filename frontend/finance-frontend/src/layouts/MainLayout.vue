<template>
  <div class="min-h-screen bg-stone-50 pb-24 font-sans text-stone-800">
    <main class="container mx-auto px-4 pt-6">
      <router-view />
    </main>

    <nav class="fixed bottom-0 left-0 right-0 bg-white border-t border-stone-200 px-4 sm:px-6 py-2 flex justify-between items-center z-50">
      
      <router-link to="/calendar" class="flex flex-col items-center text-stone-500 hover:text-stone-800 transition-colors w-16" active-class="!text-emerald-700">
        <svg class="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
        </svg>
        <span class="text-[13px] font-medium">Календарь</span>
      </router-link>
      
      <router-link to="/" class="flex flex-col items-center text-stone-500 hover:text-stone-800 transition-colors w-16" exact-active-class="!text-emerald-700" exact>
        <svg class="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 10h16M4 14h16M4 18h16"></path>
        </svg>
        <span class="text-[13px] font-medium">История</span>
      </router-link>

      <router-link to="/transaction/add" class="bg-[#83a693] w-14 h-14 flex items-center justify-center rounded-full -mt-8 shadow-[0_8px_16px_-6px_rgba(131,166,147,0.6)] border-[4px] border-stone-50 text-white hover:bg-[#6c8e7b] transition-colors shrink-0">
        <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
        </svg>
      </router-link>

      <router-link to="/categories" class="flex flex-col items-center text-stone-500 hover:text-stone-800 transition-colors w-16" active-class="!text-emerald-700">
        <svg class="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z"></path>
        </svg>
        <span class="text-[13px] font-medium">Категории</span>
      </router-link>

      <router-link to="/articles" class="flex flex-col items-center text-stone-500 hover:text-stone-800 transition-colors w-16" active-class="!text-emerald-700">
        <svg class="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
        </svg>
        <span class="text-[13px] font-medium">Статьи</span>
      </router-link>
      
    </nav>
  </div>
</template>

<script setup>
</script>