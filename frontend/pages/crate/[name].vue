<script setup lang="ts">
import Image from '~/components/Image.vue';
import AdPlaceholder from '~/components/AdPlaceholder.vue';
import { useCrateOpening } from '~/composables/useCrateOpening';
import { gunSkinFilter, sortSkinByRarity, knivesAndGlovesSkinFilter, sortSkinByName } from '~/utils/sortAndfilters';
import { useCrateDetailSeo } from '~/services/metaSeoService';
import { useOptionsStore } from '~/composables/optionsStore';
import { useCrateOpeningStore } from '~/composables/crateOpeningStore';
import { useCrate } from '~/composables/useCrate';
import { ref } from 'vue';

const router = useRouter();
const name = decodeURIComponent(router.currentRoute.value.params.name as string);

const { data: crate } = useCrate(name);

if (name && crate.value) {
    useCrateDetailSeo(crate.value, name);
}

const optionsStore = useOptionsStore();
const crateOpeningStore = useCrateOpeningStore();

const {
    crateSliderSkins,
    wonSkinIndex,
    wonSkin,
    showWonSkin,
    showSlider,
    timeout,
    autoOpen,
    handleOpenCase,
    handleBack,
    handleCloseWonSkinView,
    toggleAutoOpen,
    toggleQuickOpen,
    handleCaseOpeningFinished,
} = useCrateOpening(crate);

const guns = computed(() => {
    const skins = crate?.value?.skins;
    if (!skins?.length) return [];
    return skins.filter(gunSkinFilter).sort(sortSkinByRarity);
});

const knivesAndGloves = computed(() => {
    const skins = crate?.value?.skins;
    if (!skins?.length) return [];
    return skins.filter(knivesAndGlovesSkinFilter).sort(sortSkinByName);
});

const escListener = (event: KeyboardEvent) => {
    if (event.key === 'Escape') handleCloseWonSkinView();
};

onMounted(() => {
    document.addEventListener('keyup', escListener);
});

onUnmounted(() => {
    document.removeEventListener('keyup', escListener);
});

// For testing different ad positions
const showPreOpenAd = ref(true);
const showPostOpenAd = ref(true);

// For testing different ad sizes
const preOpenAdSize = ref('leaderboard');
const postOpenAdSize = ref('leaderboard');

const toggleAd = (position: 'preOpen' | 'postOpen') => {
    if (position === 'preOpen') showPreOpenAd.value = !showPreOpenAd.value;
    if (position === 'postOpen') showPostOpenAd.value = !showPostOpenAd.value;
};

const changeAdSize = (position: 'preOpen' | 'postOpen', size: string) => {
    if (position === 'preOpen') preOpenAdSize.value = size;
    if (position === 'postOpen') postOpenAdSize.value = size;
};
</script>

<template>
    <div v-bind="$attrs">
        <Container v-if="!crateOpeningStore.isOpeningCase && !showWonSkin && crate">
            <!-- Pre-opening ad placement -->
            <AdPlaceholder
                v-if="showPreOpenAd && !crateOpeningStore.isOpeningCase && !showWonSkin"
                class="ad-container block md:hidden"
                :size="preOpenAdSize"
                @toggle="toggleAd('preOpen')"
                @change-size="(size) => changeAdSize('preOpen', size)"
            ></AdPlaceholder>
            <div class="px-4 pt-4 sm:pt-0">
                <div class="pb-4 flex gap-4 items-center">
                    <Image
                        :width="128"
                        :height="96"
                        :src="crate?.image"
                        :alt="crate?.name"
                        class-name="h-10 sm:h-11 w-auto"
                    ></Image>
                    <h1 class="text-xl">{{ crate?.name }}</h1>
                </div>
                <div class="flex gap-4 flex-wrap items-center">
                    <BackButton></BackButton>
                    <div class="flex gap-4 flex-wrap items-center">
                        <Button variant="success" :disabled="crateOpeningStore.isOpeningCase" @click="handleOpenCase">
                            Unlock Container
                        </Button>
                        <h2 class="text-green-400 bg-black/20 rounded-sm px-2 py-0.5">
                            {{ formatEuro(getCratePrice(crate)) }}
                        </h2>
                    </div>
                </div>

                <ItemGrid v-if="guns.length" :items="guns" class="mt-6"></ItemGrid>

                <Button v-if="knivesAndGloves.length" class="mt-8" @click="optionsStore.toggleShowKnivesAndGloves">
                    {{ optionsStore.showKnivesAndGloves ? 'Hide' : 'Show' }} Knives & Gloves
                </Button>

                <ItemGrid
                    v-if="knivesAndGloves.length && optionsStore.showKnivesAndGloves"
                    class="mt-6"
                    :items="knivesAndGloves"
                ></ItemGrid>
            </div>
        </Container>

        <div
            v-if="(showWonSkin && wonSkin) || crateOpeningStore.isOpeningCase"
            class="max-w-5xl px-4 py-4 mx-auto relative z-100 flex gap-4"
        >
            <Button class="flex items-center gap-2" @click="handleBack"><BackIcon></BackIcon> Back </Button>
            <LazyCountOpened></LazyCountOpened>
        </div>

        <div
            v-if="(crateOpeningStore.isOpeningCase || wonSkin) && showSlider"
            class="fixed inset-0 h-dvh flex items-center justify-center z-40 backdrop-blur-xs"
        >
            <div v-if="crate" class="w-full max-w-5xl">
                <LazyCaseOpeningSlider
                    :crate="crate"
                    :skins="crateSliderSkins"
                    :won-skin-index="wonSkinIndex"
                    @finished="handleCaseOpeningFinished"
                ></LazyCaseOpeningSlider>
            </div>
        </div>

        <LazyWonItemDisplay
            v-if="showWonSkin && wonSkin && crate"
            :crate="crate"
            :item="wonSkin"
            :auto-open="autoOpen"
            :timeout="timeout"
            @close="handleBack"
            @open-again="handleOpenCase"
            @toggle-auto-open="toggleAutoOpen"
            @quick-open-toggle="toggleQuickOpen"
        >
            <!-- Post-opening ad placement -->
            <template v-if="showPostOpenAd && !crateOpeningStore.isOpeningCase && !showWonSkin" #after-buttons>
                <AdPlaceholder
                    :size="postOpenAdSize"
                    class="mt-4"
                    @toggle="toggleAd('postOpen')"
                    @change-size="(size) => changeAdSize('postOpen', size)"
                ></AdPlaceholder>
            </template>
        </LazyWonItemDisplay>
        <AdPlaceholder size="mobile"></AdPlaceholder>
    </div>
</template>
