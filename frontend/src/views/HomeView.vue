<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import Backbutton from '../components/Backbutton.vue';
import { useCreates } from '../query/crate';

const { isPending, isError, data, error } = useCreates();

const route = useRoute();
const router = useRouter();
const searchTerm = ref(route.query.search?.toString() || '');

const crates = computed(() => {
  if (!data.value) return [];
  const souvenires = data.value.filter((crate) => crate.type === 'Souvenir').reverse();
  const cases = data.value.filter((crate) => crate.type === 'Case').reverse();
  return [...cases, ...souvenires].filter((crate) => crate.name.toLowerCase().includes(searchTerm.value.toLowerCase()));
});

watch(searchTerm, (newValue) => {
  if (route.path === '/') {
    router.replace({
      query: newValue ? { search: newValue } : {},
    });
  }
});
</script>

<template>
  <div class="pb-16">
    <div class="fixed left-0 right-0 z-10 px-5 sm:px-4 py-5 bg-slate-950 max-w-5xl mx-auto w-full">
      <Backbutton v-if="route.path != '/'" />
      <input type="text" v-model="searchTerm" placeholder="Search cases..." class="w-full p-2 border rounded-md px-4" />
    </div>
    <div class="pt-24">
      <div class="gap-2 sm:gap-0 responsive-grid">
        <router-link v-for="crate in crates" :key="crate.id" :to="`/crate/${crate.id}`" class="max-w-[122.5px] mx-auto">
          <div class="transition-transform duration-75 cursor-pointer hover:scale-105">
            <img :src="crate.image || '/images/placeholder.webp'" alt="" />
            <p class="mt-2 text-sm text-center">{{ crate.name }}</p>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>
