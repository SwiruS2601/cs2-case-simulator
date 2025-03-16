<script setup lang="ts">
import Image from '~/components/Image.vue';
import { useCrateOpening } from '~/composables/useCrateOpening';
import { gunSkinFilter, sortSkinByRarity, knivesAndGlovesSkinFilter, sortSkinByName } from '~/utils/sortAndfilters';
import { useCrateDetailSeo } from '~/services/metaSeoService';
import { useOptionsStore } from '~/composables/optionsStore';
import { useCrateOpeningStore } from '~/composables/crateOpeningStore';
import { useCrate } from '~/composables/useCrate';

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
</script>

<template>
  <div v-bind="$attrs">
    <Container v-if="!crateOpeningStore.isOpeningCase && !showWonSkin && crate">
      <h1 class="text-xl pb-4">{{ crate?.name }}</h1>
      <div class="flex gap-4 flex-wrap items-center">
        <Backbutton />
        <Button
          id="unlock-crate-button"
          variant="success"
          @click="handleOpenCase"
          :disabled="crateOpeningStore.isOpeningCase"
        >
          Unlock Container
        </Button>
        <Image :width="128" :height="96" :src="crate?.image" :alt="crate?.name" className="h-10 sm:h-11 w-auto" />
      </div>

      <SkinGrid v-if="guns.length" :skins="guns" class="mt-6" />

      <Button v-if="knivesAndGloves.length" @click="optionsStore.toggleShowKnivesAndGloves" class="mt-8">
        {{ optionsStore.showKnivesAndGloves ? 'Hide' : 'Show' }} Knives & Gloves
      </Button>

      <SkinGrid
        class="mt-6"
        v-if="knivesAndGloves.length && optionsStore.showKnivesAndGloves"
        :skins="knivesAndGloves"
      />
    </Container>

    <div
      v-if="(showWonSkin && wonSkin) || crateOpeningStore.isOpeningCase"
      class="max-w-5xl px-4 py-4 mx-auto relative z-100"
    >
      <Button class="flex items-center gap-2" @click="handleBack"><BackIcon /> Back </Button>
    </div>

    <div
      v-if="(crateOpeningStore.isOpeningCase || wonSkin) && showSlider"
      class="fixed inset-0 h-dvh flex items-center justify-center z-40 backdrop-blur-xs"
    >
      <div v-if="crate" class="w-full max-w-5xl">
        <CaseOpeningSlider
          :crate="crate"
          :skins="crateSliderSkins"
          :wonSkinIndex="wonSkinIndex"
          @finished="handleCaseOpeningFinished"
        />
      </div>
    </div>

    <WonSkinDisplay
      v-if="showWonSkin && wonSkin && crate"
      :crate="crate"
      :skin="wonSkin"
      :auto-open="autoOpen"
      :timeout="timeout"
      @close="handleBack"
      @open-again="handleOpenCase"
      @toggle-auto-open="toggleAutoOpen"
      @quick-open-toggle="optionsStore.toggleQuickOpen"
    />
  </div>
</template>

<style scoped>
.radial-fade {
  background: radial-gradient(ellipse at center, rgba(0, 0, 0, 0.5), transparent 70%);
}
</style>
