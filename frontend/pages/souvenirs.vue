<script setup lang="ts">
import CrateGrid from '~/components/CrateGrid.vue';
import CrateNav from '~/components/CrateNav.vue';
import SearchCrates from '~/components/SearchCrates.vue';
import { useSouvenirPageSeo } from '~/services/metaSeoService';
import { useCrateList } from '~/composables/useCrateList';

const { data, searchData, crates } = useCrateList('souvenirs');

if (data.value) {
    useSouvenirPageSeo(data.value);
}
</script>

<template>
    <Container>
        <div class="px-4 pt-2 sm:pt-0">
            <h1 class="text-xl pb-4">Counter-Strike 2 Souvenir Cases</h1>
            <div class="flex gap-4 flex-col">
                <SearchCrates v-model="searchData"></SearchCrates>
                <CrateNav active-link="Souvenirs"></CrateNav>
            </div>
            <CrateGrid v-bind="{ crates }"></CrateGrid>
        </div>
    </Container>
</template>
