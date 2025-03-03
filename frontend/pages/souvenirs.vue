<script setup lang="ts">
import CrateGrid from '~/components/CrateGrid.vue';
import CrateNav from '~/components/CrateNav.vue';
import SearchCrates from '~/components/SearchCrates.vue';
import type { Crate } from '~/types';

const { data } = await useFetch<Crate[]>('/api/crates');
const searchData = ref<Crate[]>([]);

const crates = computed(() => {
  if (searchData.value.length) return searchData.value;
  if (!data.value) return [];
  return data.value.filter((c) => c.type === 'Souvenir').reverse();
});
</script>

<template>
  <Container>
    <h1 class="text-xl pb-4">Counter-Strike 2 Souvenir Cases</h1>
    <div class="flex gap-4 flex-col">
      <SearchCrates v-model="searchData" />
      <CrateNav activeLink="Souvenirs" />
    </div>
    <CrateGrid v-bind="{ crates }" />
  </Container>
</template>
