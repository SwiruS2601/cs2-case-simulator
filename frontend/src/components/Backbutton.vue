<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();
const goBack = () => router.go(-1);
</script>

<template>
  <button
    @click="goBack"
    class="flex items-center gap-2 px-4 py-2 rounded hover:bg-slate-800 hover:cursor-pointer hover:text-slate-100 transition-colors border-slate-700 border-2"
  >
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
      <path
        fill-rule="evenodd"
        d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z"
        clip-rule="evenodd"
      />
    </svg>
    Back
  </button>
</template>
