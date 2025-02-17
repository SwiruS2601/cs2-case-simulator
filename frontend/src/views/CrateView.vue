<script setup lang="ts">
import Backbutton from '@/components/Backbutton.vue';
import Button from '@/components/Button.vue';
import SkinGrid from '@/components/SkinGrid.vue';
import { useCreate } from '@/query/crate';
import { useOptionsStore } from '@/store/optionsStore';
import {
  filterOnlyGlovesAndKnives,
  filterSkinsByOnlyGuns,
  sortSkinByName,
  sortSkinByRarity,
} from '@/utils/sortAndfilters';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useCaseOpeningStore } from '@/store/caseOpeningStore';
import CaseOpeningSlider from '@/components/CaseOpeningSlider.vue';
import type { Skin } from '@/query/skins';
import { caseOpeningService } from '@/services/caseOpeningService';
import { FUN_ODDS, REAL_RARITY_ODDS } from '@/constants';
import { audioService } from '@/services/audioService';

const router = useRouter();
const crateId = router.currentRoute.value.params.id as string;
const { data: crate } = useCreate(crateId);

const optionsStore = useOptionsStore();
const caseOpeningStore = useCaseOpeningStore();

const caseSkins = ref<Skin[]>([]);
const wonSkinIndex = ref(0);

const guns = computed(() => {
  const skins = crate?.value?.skins;
  if (!skins?.length) return [];
  return skins.filter(filterSkinsByOnlyGuns).sort(sortSkinByRarity);
});

const knivesAndGloves = computed(() => {
  const skins = crate?.value?.skins;
  if (!skins?.length) return [];
  return skins.filter(filterOnlyGlovesAndKnives).sort(sortSkinByName);
});

const handleOpenCase = async () => {
  if (!crate.value) return;

  handleCloseWonSkin();

  const {
    skins,
    wonSkin: _wonSkin,
    wonSkinIndex: _wonSkinIndex,
  } = caseOpeningService.generateSkinsForCaseOpening(
    crate.value,
    [...guns.value, ...knivesAndGloves.value],
    50,
    optionsStore.moreRareSkins ? FUN_ODDS : REAL_RARITY_ODDS,
  );

  if (!_wonSkin) return;

  document.body.style.overflow = 'hidden';

  audioService.playUnlockSound();
  caseSkins.value = skins;
  wonSkinIndex.value = _wonSkinIndex;
  await new Promise((resolve) => setTimeout(resolve, 100));
  caseOpeningStore.startCaseOpening();
};

const handleCaseOpeningFinished = (skin: Skin) => {
  caseOpeningStore.endCaseOpening(skin);
  audioService.playRevealSound(skin.rarity);
};

const handleCloseWonSkin = () => {
  caseOpeningStore.setWonSkin(null);
  document.body.style.overflow = '';
};
</script>

<template>
  <div class="pt-5 pb-16">
    <div class="flex gap-4">
      <Backbutton />
      <Button variant="success" @click="handleOpenCase" :disabled="caseOpeningStore.isOpeningCase">
        Unlock Container
      </Button>

      <div class="relative">
        <Button @click="optionsStore.toggleShowOptions">
          {{ optionsStore.showOptions ? 'Hide' : 'Show' }} Options
        </Button>

        <div
          v-if="optionsStore.showOptions"
          class="absolute flex top-12 -right-2 p-4 bg-gray-800 rounded-lg shadow-lg flex-col gap-4"
        >
          <Button @click="optionsStore.toggleFastAnimation">
            {{ optionsStore.fastAnimation ? 'Disable' : 'Enable' }} Fast Animation
          </Button>
          <Button @click="optionsStore.toggleMoreRareSkins">
            {{ optionsStore.moreRareSkins ? 'Disable' : 'Enable' }} Guaranteed Rare Skins
          </Button>
          <Button @click="optionsStore.toggleSound"> {{ optionsStore.soundOn ? 'Disable' : 'Enable' }} Sound </Button>
        </div>
      </div>
    </div>

    <div
      v-if="caseOpeningStore.isOpeningCase || caseOpeningStore?.wonSkin"
      class="fixed inset-0 bg-black/20 backdrop-blur-xs flex items-center justify-center z-50"
    >
      <div v-if="crate" class="w-full max-w-5xl">
        <CaseOpeningSlider
          :crate="crate"
          :skins="caseSkins"
          :wonSkinIndex="wonSkinIndex"
          @finished="handleCaseOpeningFinished"
        />
      </div>
    </div>

    <div
      @click="handleCloseWonSkin"
      v-if="caseOpeningStore?.wonSkin"
      class="absolute inset-0 h-[90dvh] flex items-center justify-center p-4 z-100 fade-scale-up backdrop-blur-xs"
    >
      <div @click.stop class="flex items-center flex-col gap-6 rounded-xl backdrop-blur-xs">
        <img :src="caseOpeningStore.wonSkin.image" class="size-full select-none" />
        <div class="flex flex-col gap-4 items-center">
          <div>
            <p class="text-lg font-semibold">{{ caseOpeningStore.wonSkin.name }}</p>
            <p class="text-sm text-gray-400">{{ caseOpeningStore.wonSkin.rarity }}</p>
          </div>
          <div class="flex flex-wrap gap-4">
            <Button @click="handleCloseWonSkin">Close</Button>
            <Button variant="success" :disabled="caseOpeningStore.isOpeningCase" @click="handleOpenCase"
              >Open Another</Button
            >
          </div>
        </div>
      </div>
    </div>

    <SkinGrid v-if="guns.length" :skins="guns" class="mt-6" />

    <Button v-if="knivesAndGloves.length" @click="optionsStore.toggleShowKnivesAndGloves" class="mt-8">
      {{ optionsStore.showKnivesAndGloves ? 'Hide' : 'Show' }} Knives & Gloves
    </Button>

    <SkinGrid v-if="knivesAndGloves.length && optionsStore.showKnivesAndGloves" :skins="knivesAndGloves" />
  </div>
</template>

<style scoped>
@keyframes fadeScaleUp {
  0% {
    opacity: 0;
    transform: scale(0.4);
  }
  100% {
    opacity: 1;
    transform: scale(1);
  }
}

.fade-scale-up {
  animation: fadeScaleUp 0.18s ease-out forwards;
}
</style>
