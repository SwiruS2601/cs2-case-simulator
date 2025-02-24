<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useCreates } from '../query/crate';
import Button from '../components/Button.vue';

const { data } = useCreates();

const route = useRoute();
const router = useRouter();
const search = ref(route.query.search?.toString() || '');
const type = ref<string>(route.query.type?.toString() || 'Case');

const crates = computed(() => {
  if (!data.value) return [];
  return data.value
    .filter((crate) => crate.type === type.value)
    .filter((crate) => crate.name.toLowerCase().includes(search.value.toLowerCase()))
    .reverse();
});

watch(search, (newValue) => {
  if (route.path === '/') {
    router.replace({
      query: newValue ? { search: newValue, type: type.value } : {},
    });
  }
});

watch(type, (newValue) => {
  if (route.path === '/') {
    search.value = '';
    router.replace({
      query: newValue ? { type: newValue, search: search.value } : {},
    });
  }
});
</script>

<template>
  <div class="relative w-full max-w-5xl px-4 mx-auto backdrop-blur-xs bg-black/30 sm:my-4 sm:rounded-sm py-4">
    <div class="flex gap-4 flex-col">
      <input
        type="text"
        v-model="search"
        placeholder="Search..."
        class="w-full p-2 border rounded-md px-4 flex text-white placeholder:text-white/70"
      />
      <div class="flex gap-2 flex-wrap">
        <Button @click="type = 'Case'" :variant="type === 'Case' ? 'cta' : 'text'" size="pill">Cases</Button>
        <Button @click="type = 'Souvenir'" :variant="type === 'Souvenir' ? 'cta' : 'text'" size="pill"
          >Souvenirs</Button
        >
        <Button @click="type = 'Sticker Capsule'" :variant="type === 'Sticker Capsule' ? 'cta' : 'text'" size="pill"
          >Stickers</Button
        >
        <Button @click="type = 'Autograph Capsule'" :variant="type === 'Autograph Capsule' ? 'cta' : 'text'" size="pill"
          >Autographs</Button
        >
      </div>
    </div>
    <div class="gap-3 sm:gap-4 mt-8 sm:mt-6 responsive-grid justify-between">
      <router-link v-for="crate in crates" :key="crate.id" :to="`/crate/${crate.id}`" class="mx-auto">
        <div class="transition-transform flex flex-col h-full duration-75 cursor-pointer hover:scale-105 items-center">
          <p class="text-xs text-center mt-auto mb-1">{{ crate.name }}</p>
          <img :src="crate.image || '/images/placeholder.webp'" alt="" />
        </div>
      </router-link>
    </div>
  </div>
</template>
