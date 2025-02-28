<script setup lang="ts">
import Backbutton from '../components/Backbutton.vue';
import Button from '../components/Button.vue';
import SkinGrid from '../components/SkinGrid.vue';
import { useCreateByName } from '../query/crate';
import { useOptionsStore } from '../store/optionsStore';
import { gunSkinFilter, knivesAndGlovesSkinFilter, sortSkinByName, sortSkinByRarity } from '../utils/sortAndfilters';
import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useCrateOpeningStore } from '../store/crateOpeningStore';
import CaseOpeningSlider from '../components/CaseOpeningSlider.vue';
import { crateOpeningService } from '../services/crateOpeningService';
import { REAL_ODDS } from '../constants';
import { audioService } from '../services/audioService';
import { useInventoryStore } from '../store/inventoryStore';
import { getCratePrice, getSkinPrice } from '../utils/balance';
import type { Skin } from '../types';
import SoundIcon from '../components/SoundIcon.vue';
import MuteIcon from '../components/MuteIcon.vue';
import CheckMarkIcon from '../components/CheckMarkIcon.vue';
import BackIcon from '../components/BackIcon.vue';
import { getSkinRarityColor } from '../utils/color';
import AutomaticIcon from '../components/AutomaticIcon.vue';
import Container from '../components/Container.vue';
import CanonicalLink from '../components/CanonicalLink.vue';

const KEY_PRICE = 2.5;

const router = useRouter();
const crateName = router.currentRoute.value.params.id as string;
const { data: crate } = useCreateByName(encodeURIComponent(crateName));
const inventory = useInventoryStore();
const optionsStore = useOptionsStore();
const caseOpeningStore = useCrateOpeningStore();

const crateSliderSkins = ref<Skin[]>([]);
const wonSkinIndex = ref(0);
const showOptions = ref(false);
const wonSkin = ref<Skin | null>(null);
const showWonSkin = ref(false);
const showSlider = ref(false);
const timout = ref(false);
const autoOpen = ref(false);

let autoOpenInterval: ReturnType<typeof setInterval> | null = null;

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

const handleOpenCase = async () => {
  if (!crate.value) return;

  handleCloseWonSkinView();

  if (optionsStore.quickOpen) {
    handleQuickOpen();
    return;
  }

  caseOpeningStore.startCaseOpening();

  showOptions.value = false;
  document.body.style.overflow = 'hidden';

  audioService.playUnlockSound();

  await new Promise((resolve) => setTimeout(resolve, 0.1));

  const opnenedCrate = crateOpeningService.openCrate(crate.value, REAL_ODDS);

  crateSliderSkins.value = opnenedCrate.sliderSkins;
  wonSkinIndex.value = opnenedCrate.wonSkinIndex;
  wonSkin.value = opnenedCrate.wonSkin;

  showSlider.value = true;
  inventory.incrementOpenCount();
  inventory.setBalance(inventory.balance - (getCratePrice(crate.value) + KEY_PRICE));
};

const handleQuickOpen = () => {
  if (!crate.value) return;

  showSlider.value = false;
  showOptions.value = false;
  document.body.style.overflow = 'hidden';

  audioService.playUnlockImmidiateSound();

  const _wonSkin = crateOpeningService.getWinningSkin(crate.value, REAL_ODDS);
  wonSkin.value = _wonSkin;

  if (_wonSkin.rarity_id.includes('ancient') || knivesAndGlovesSkinFilter(_wonSkin)) {
    timout.value = true;
    setTimeout(() => {
      timout.value = false;
    }, 2000);
  }

  handleCaseOpeningFinished(_wonSkin);
  inventory.incrementOpenCount();
  inventory.setBalance(inventory.balance - (getCratePrice(crate.value) + KEY_PRICE));
};

const handleCaseOpeningFinished = (skin: Skin) => {
  showWonSkin.value = true;
  caseOpeningStore.endCaseOpening(skin);
  inventory.setBalance(inventory.balance + getSkinPrice(skin));
  inventory.addSkin(skin);
  audioService.playRevealSound(skin.rarity_id);
};

const handleBack = () => {
  handleCloseWonSkinView();
  autoOpen.value = false;
};

const handleCloseWonSkinView = () => {
  document.body.style.overflow = '';
  showWonSkin.value = false;
  showSlider.value = false;
  wonSkin.value = null;
  caseOpeningStore.isOpeningCase = false;
};

const escListener = (event: KeyboardEvent) => {
  if (event.key === 'Escape') handleCloseWonSkinView();
};

const handleClickQuickOpen = () => {
  optionsStore.quickOpen = !optionsStore.quickOpen;
  autoOpen.value = false;
};

watch(autoOpen, (newVal) => {
  if (newVal) {
    autoOpenInterval = setInterval(() => {
      if (!caseOpeningStore.isOpeningCase && !timout.value) {
        handleOpenCase();
      }
    }, 150);
  } else if (autoOpenInterval) {
    clearInterval(autoOpenInterval);
    autoOpenInterval = null;
  }
});

onMounted(() => {
  document.addEventListener('keyup', escListener);
});

onUnmounted(() => {
  if (autoOpenInterval) clearInterval(autoOpenInterval);
  if (wonSkin.value && !caseOpeningStore.wonSkin) {
    handleCaseOpeningFinished(wonSkin.value);
  }
  handleCloseWonSkinView();
  document.removeEventListener('keyup', escListener);
});
</script>

<template>
  <div v-bind="$attrs">
    <CanonicalLink />

    <Container v-if="!caseOpeningStore.isOpeningCase && !showWonSkin">
      <div class="flex gap-4 flex-wrap items-center">
        <Backbutton />
        <Button variant="success" @click="handleOpenCase" :disabled="caseOpeningStore.isOpeningCase">
          Unlock Container
        </Button>
      </div>

      <dialog
        v-if="showOptions"
        class="absolute bg-gray-800/95 inset-0 z-50 left-[20%] border border-black/20 top-[74px] flex p-4 rounded-lg shadow-2xl flex-col gap-4"
      >
        <Button @click="optionsStore.toggleFastAnimation">
          {{ optionsStore.fastAnimation ? 'Disable' : 'Enable' }} Fast Animation
        </Button>
        <Button @click="optionsStore.toggleSound"> {{ optionsStore.soundOn ? 'Disable' : 'Enable' }} Sound </Button>
      </dialog>

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
      v-if="(showWonSkin && wonSkin) || caseOpeningStore.isOpeningCase"
      class="max-w-5xl px-4 py-4 mx-auto relative z-100"
    >
      <Button class="flex items-center gap-2" @click="handleBack"><BackIcon /> Back </Button>
    </div>

    <div
      v-if="(caseOpeningStore.isOpeningCase || wonSkin) && showSlider"
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

    <div
      v-if="showWonSkin && wonSkin"
      class="absolute inset-0 h-dvh flex items-center justify-center p-4 z-[70] fade-scale-up backdrop-blur-xs"
    >
      <div class="flex items-center flex-col gap-6 rounded-xl pt-[15dvh] sm:pt-0">
        <img :src="wonSkin?.image" class="size-full select-none" />
        <div class="flex flex-col gap-4 items-center pb-6 pt-2">
          <div
            class="sm:mb-6 relative overflow-hidden bg-black/50 rounded-xl border border-black/10 sm:justify-normal justify-center w-fit"
          >
            <div class="absolute bottom-0 w-full h-2" :style="{ background: getSkinRarityColor(wonSkin) }"></div>
            <div class="flex items-center gap-x-4 gap-y-1.5 flex-col sm:flex-row p-5 pb-6">
              <p class="text-md font-semibold text-white border border-white/30 rounded px-2">
                {{ wonSkin.name }}
              </p>
              <div class="flex gap-1 justify-between items-center">
                <p class="text-sm translate-y-[1px] sm:translate-y-0 text-white/80">{{ wonSkin.wear_category }}</p>
                <p class="text-sm text-white/80">
                  <span class="text-green-400 font-semibold text-lg">€ {{ getSkinPrice(wonSkin).toFixed(2) }} </span>
                </p>
              </div>
            </div>
          </div>
          <div class="flex flex-wrap gap-4 justify-center sm:justify-normal">
            <Button @click="handleBack">Close</Button>
            <Button size="icon" @click="optionsStore.toggleSound">
              <MuteIcon v-if="!optionsStore.soundOn" fill="#fb2c36" class="size-5" />
              <SoundIcon v-if="optionsStore.soundOn" fill="#f0f0f0" class="size-5" />
            </Button>
            <Button
              :style="{ border: autoOpen ? '1px solid #05df72' : '1px solid #0000001a' }"
              size="icon"
              v-if="optionsStore.quickOpen"
              @click="autoOpen = !autoOpen"
            >
              <AutomaticIcon :fill="autoOpen ? '#05df72' : '#f0f0f0'" class="size-6" />
            </Button>
            <Button
              :style="{
                border: optionsStore.quickOpen ? '1px solid #05df72' : '1px solid #0000001a',
                paddingRight: optionsStore.quickOpen ? '0.75rem' : '',
                color: optionsStore.quickOpen ? '#05df72' : '',
              }"
              :disabled="caseOpeningStore.isOpeningCase"
              @click="handleClickQuickOpen"
            >
              Quick Open
              <CheckMarkIcon v-if="optionsStore.quickOpen" class="size-5 ml-2" />
            </Button>
            <Button
              :variant="autoOpen ? 'danger' : 'success'"
              :disabled="caseOpeningStore.isOpeningCase || timout"
              @click="autoOpen ? (autoOpen = false) : handleOpenCase()"
              >{{ autoOpen ? 'Stop' : 'Open Again' }}
            </Button>
          </div>
        </div>
      </div>
    </div>
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

.radial-fade {
  background: radial-gradient(ellipse at center, rgba(0, 0, 0, 0.5), transparent 70%);
}
</style>
