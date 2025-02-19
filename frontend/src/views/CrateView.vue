<script setup lang="ts">
import Backbutton from '../components/Backbutton.vue';
import Button from '../components/Button.vue';
import SkinGrid from '../components/SkinGrid.vue';
import { useCreate } from '../query/crate';
import { useOptionsStore } from '../store/optionsStore';
import {
  filterOnlyGlovesAndKnives,
  filterSkinsByOnlyGuns,
  sortSkinByName,
  sortSkinByRarity,
} from '../utils/sortAndfilters';
import { computed, onMounted, onUnmounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useCaseOpeningStore } from '../store/caseOpeningStore';
import CaseOpeningSlider from '../components/CaseOpeningSlider.vue';
import { crateOpeningService } from '../services/crateOpeningService';
import { FUN_ODDS, REAL_RARITY_ODDS } from '../constants';
import { audioService } from '../services/audioService';
import { useInventoryStore } from '../store/inventoryStore';
import { getSkinPrice } from '../utils/balance';
import OptionsIcon from '../components/OptionsIcon.vue';
import type { Skin } from '../types';

const router = useRouter();
const crateId = router.currentRoute.value.params.id as string;
const { data: crate } = useCreate(crateId);
const inventory = useInventoryStore();

const optionsStore = useOptionsStore();
const caseOpeningStore = useCaseOpeningStore();

const crateSliderSkins = ref<Skin[]>([]);
const wonSkinIndex = ref(0);
const showOptions = ref(false);
const wonSkin = ref<Skin | null>(null);
const showWonSkin = ref(false);
const showSlider = ref(false);

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

  handleCloseWonSkinView();

  showOptions.value = false;
  document.body.style.overflow = 'hidden';

  audioService.playUnlockSound();

  await new Promise((resolve) => setTimeout(resolve, 100));

  const odds = optionsStore.moreRareSkins ? FUN_ODDS : REAL_RARITY_ODDS;
  const opnenedCrate = crateOpeningService.openCrate(crate.value, odds);

  crateSliderSkins.value = opnenedCrate.sliderSkins;
  wonSkinIndex.value = opnenedCrate.wonSkinIndex;
  wonSkin.value = opnenedCrate.wonSkin;

  caseOpeningStore.startCaseOpening();
  showSlider.value = true;
  inventory.incrementOpenCount();
  inventory.setBalance(inventory.balance - 2.5);
};

const handleCaseOpeningFinished = (skin: Skin) => {
  showWonSkin.value = true;
  caseOpeningStore.endCaseOpening(skin);
  inventory.setBalance(inventory.balance + getSkinPrice(skin));
  inventory.addSkin(skin);
  audioService.playRevealSound(skin.rarity);
};

const handleCloseWonSkinView = () => {
  if (caseOpeningStore.isOpeningCase) return;
  document.body.style.overflow = '';
  showWonSkin.value = false;
  showSlider.value = false;
};

const escListener = (event: KeyboardEvent) => {
  if (event.key === 'Escape') handleCloseWonSkinView();
};

onMounted(() => {
  document.addEventListener('keyup', escListener);
});

onUnmounted(() => {
  if (wonSkin.value && !caseOpeningStore.wonSkin) {
    handleCaseOpeningFinished(wonSkin.value);
  }
  handleCloseWonSkinView();
  document.removeEventListener('keyup', escListener);
});
</script>

<template>
  <div class="pt-5 pb-16 relative">
    <div class="flex gap-4 flex-wrap items-center">
      <Backbutton />
      <Button variant="success" @click="handleOpenCase" :disabled="caseOpeningStore.isOpeningCase">
        Unlock Container
      </Button>

      <Button size="icon" @click="showOptions = !showOptions">
        <OptionsIcon fill="#f0f0f0" class="size-4.5 text-gray-400" />
      </Button>
    </div>

    <dialog
      v-if="showOptions"
      class="absolute inset-0 z-50 left-[18%] top-20 flex p-4 bg-gray-700/95 rounded-lg shadow-2xl flex-col gap-4"
    >
      <Button @click="optionsStore.toggleFastAnimation">
        {{ optionsStore.fastAnimation ? 'Disable' : 'Enable' }} Fast Animation
      </Button>
      <Button @click="optionsStore.toggleMoreRareSkins">
        {{ optionsStore.moreRareSkins ? 'Disable' : 'Enable' }} More Rare Skins
      </Button>
      <Button @click="optionsStore.toggleSound"> {{ optionsStore.soundOn ? 'Disable' : 'Enable' }} Sound </Button>
    </dialog>

    <div
      v-if="(caseOpeningStore.isOpeningCase || caseOpeningStore?.wonSkin) && showSlider"
      class="fixed inset-0 bg-black/20 backdrop-blur-xs flex items-center justify-center z-50"
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

    <div
      v-if="showWonSkin && caseOpeningStore.wonSkin"
      class="absolute inset-0 h-[90dvh] flex items-center justify-center p-4 z-100 fade-scale-up backdrop-blur-xs"
    >
      <div class="flex items-center flex-col gap-6 rounded-xl backdrop-blur-xs">
        <img :src="caseOpeningStore.wonSkin.image" class="size-full select-none" />
        <div class="flex flex-col gap-4 items-center">
          <div>
            <p class="text-lg font-semibold">{{ caseOpeningStore.wonSkin.name }}</p>
            <div class="flex gap-4 justify-between">
              <p class="text-sm text-gray-400">{{ caseOpeningStore.wonSkin.wearCategory }}</p>
              <p class="text-sm text-gray-400">
                Price <span class="text-green-400">€ {{ getSkinPrice(caseOpeningStore.wonSkin) }} </span>
              </p>
            </div>
          </div>
          <div class="flex flex-wrap gap-4">
            <Button @click="handleCloseWonSkinView">Close</Button>
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
