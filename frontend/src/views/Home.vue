<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import Backbutton from '@/components/Backbutton.vue';
import { useCreates } from '@/query/crate';

const { isPending, isError, data, error } = useCreates();
const route = useRoute();
const router = useRouter();
const searchTerm = ref(route.query.search?.toString() || '');

watch(searchTerm, (newValue) => {
  if (route.path === '/') {
    router.replace({
      query: newValue ? { search: newValue } : {},
    });
  }
});

const crates = computed(() => {
  if (!data.value) return [];
  const reversed = data.value;
  const souvenirCases = reversed.filter((crate) => crate.type === 'Souvenir');
  const normalCases = reversed.filter((crate) => crate.type === 'Case');
  return [...normalCases, ...souvenirCases].filter((crate) =>
    crate.name.toLowerCase().includes(searchTerm.value.toLowerCase()),
  );
});
</script>

<template>
  <div class="pb-16">
    <div class="fixed left-0 right-0 z-10 px-5 sm:px-3 py-5 bg-slate-950 max-w-5xl mx-auto w-full">
      <Backbutton v-if="route.path != '/'" />
      <input type="text" v-model="searchTerm" placeholder="Search cases..." class="w-full p-2 border rounded-md" />
    </div>
    <div class="pt-24">
      <div v-if="isPending">Loading...</div>
      <div v-else-if="isError">Error: {{ error?.message }}</div>
      <div v-else class="gap-6 responsive-grid">
        <router-link
          v-for="_case in crates"
          :key="_case.id"
          :to="`/crate/${_case.id}`"
          class="sm:max-w-[122px] max-w-[220px]"
        >
          <div class="transition-transform duration-75 cursor-pointer hover:scale-105">
            <img :src="_case.image || '/images/placeholder.webp'" alt="" width="300" height="300" />
            <p class="mt-2 text-sm text-center">{{ _case.name }}</p>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>
