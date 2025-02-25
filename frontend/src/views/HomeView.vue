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
  <div
    class="relative w-full max-w-5xl px-4 mx-auto backdrop-blur-xs bg-black/40 sm:my-4 sm:rounded-xl py-4 shadow-xl border border-black/10"
  >
    <div class="flex gap-4 flex-col">
      <input
        type="text"
        v-model="search"
        placeholder="Search..."
        class="w-full p-2 border border-black/20 bg-black/30 rounded-lg px-4 flex text-white placeholder:text-white/80"
      />
      <div class="flex gap-2 flex-wrap">
        <Button @click="type = 'Case'" :variant="type === 'Case' ? 'cta' : 'primary'" size="pill">Cases</Button>
        <Button @click="type = 'Souvenir'" :variant="type === 'Souvenir' ? 'cta' : 'primary'" size="pill"
          >Souvenirs</Button
        >
        <Button @click="type = 'Sticker Capsule'" :variant="type === 'Sticker Capsule' ? 'cta' : 'primary'" size="pill"
          >Stickers</Button
        >
        <Button
          @click="type = 'Autograph Capsule'"
          :variant="type === 'Autograph Capsule' ? 'cta' : 'primary'"
          size="pill"
          >Autographs</Button
        >
      </div>
    </div>
    <div class="gap-3 sm:gap-4 mt-4 responsive-grid justify-between">
      <router-link
        v-for="crate in crates"
        :key="crate.id"
        :to="`/crate/${encodeURIComponent(crate.name)}`"
        class="mx-auto"
      >
        <div
          class="flex flex-col h-full cursor-pointer items-center bg-black/20 transition-shadow duration-150 rounded-lg px-2 py-2 border border-black/10 hover:shadow-xl"
        >
          <p class="text-xs text-center my-auto text-white/95">{{ crate.name }}</p>
          <img
            class="transition-transform duration-75 hover:scale-[120%]"
            :src="crate.image || '/images/placeholder.webp'"
            :alt="`${crate.name} - CS2 Case`"
            width="110"
            :height="type === 'Case' ? 85 : 82.5"
          />
        </div>
      </router-link>
    </div>
  </div>
</template>
