<script setup lang="ts">
import CrateGrid from '~/components/CrateGrid.vue';
import CrateNav from '~/components/CrateNav.vue';
import SearchCrates from '~/components/SearchCrates.vue';
import { useSouvenirPageSeo } from '~/services/metaSeoService';
import type { Crate } from '~/types';

const { data } = await useFetch<Crate[]>(`${useRuntimeConfig().public.apiUrl}/api/crates/souvenirs`);
const searchData = ref<Crate[] | null>(null);

const crates = computed(() => {
  if (searchData?.value) return searchData.value;
  return data.value?.length ? data.value : [];
});

if (data.value) {
  useSouvenirPageSeo(data.value);
}
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
