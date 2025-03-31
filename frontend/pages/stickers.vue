<script setup lang="ts">
import CrateGrid from '~/components/CrateGrid.vue';
import CrateNav from '~/components/CrateNav.vue';
import SearchCrates from '~/components/SearchCrates.vue';
import { useStickerPageSeo } from '~/services/metaSeoService';
import { useCrateList } from '~/composables/useCrateList';

const { data, searchData, crates } = useCrateList('stickers');

if (data.value) {
    useStickerPageSeo(data.value);
}
</script>

<template>
    <div>
        <Container>
            <AdPlaceholder class="block md:hidden" size="mobile"></AdPlaceholder>
            <div class="px-4 pt-2 sm:pt-0">
                <h1 class="text-xl pb-4">Counter-Strike 2 Sticker Capsules</h1>
                <div class="flex gap-4 flex-col">
                    <SearchCrates v-model="searchData"></SearchCrates>
                    <CrateNav active-link="Stickers"></CrateNav>
                </div>
                <CrateGrid v-bind="{ crates }"></CrateGrid>
            </div>
        </Container>
        <AdPlaceholder size="mobile"></AdPlaceholder>
    </div>
</template>
