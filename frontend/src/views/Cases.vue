<script setup lang="ts">
import { computed } from 'vue';
import { useQuery } from '@tanstack/vue-query';

interface Case {
  id: string;
  name: string;
  image: string;
  type: string;
}

const { isPending, isError, data, error } = useQuery<Case[]>({
  queryKey: ['cases'],
  queryFn: async () => {
    const response = await fetch('http://localhost:5015/api/case');
    return response.json();
  },
});

const reversedCases = computed(() => {
  if (!data.value) return [];
  return [...data.value].filter((c) => c.type === 'Case').reverse();
});
</script>

<template>
  <div class="mt-6">
    <div v-if="isPending">Loading...</div>
    <div v-else-if="isError">Error: {{ error?.message }}</div>
    <div v-else class="gap-6 responsive-grid">
      <router-link v-for="_case in reversedCases" :key="_case.id" :to="`/case/${_case.id}`">
        <div class="transition-transform duration-75 cursor-pointer hover:scale-105">
          <img :src="_case.image || '/images/placeholder.webp'" alt="" width="300" height="300" />
          <p class="mt-2 text-sm text-center">{{ _case.name }}</p>
        </div>
      </router-link>
    </div>
  </div>
</template>

<style scoped></style>
