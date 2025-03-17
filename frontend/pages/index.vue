<script setup lang="ts">
import CrateGrid from '~/components/CrateGrid.vue';
import CrateNav from '~/components/CrateNav.vue';
import SearchCrates from '~/components/SearchCrates.vue';
import { useHomePageSeo } from '~/services/metaSeoService';
import { useCrateList } from '~/composables/useCrateList';

const { data, searchData, crates } = useCrateList('cases');

if (data.value) {
    useHomePageSeo(data.value);
}
</script>

<template>
    <Container>
        <h1 class="text-xl pb-4">Counter-Strike 2 Cases</h1>
        <div class="flex gap-4 flex-col">
            <SearchCrates v-model="searchData" ></SearchCrates>
            <CrateNav active-link="Cases" ></CrateNav>
        </div>
        <CrateGrid v-bind="{ crates, imageHeight: 192 }" ></CrateGrid>
    </Container>
</template>
