import { ref, type ShallowRef } from 'vue';
import { useCrateOpeningStore } from './crateOpeningStore';
import { useInventoryStore } from './inventoryStore';
import { useOptionsStore } from './optionsStore';
import { REAL_ODDS } from '~/constants';
import { audioService } from '~/services/audioService';
import { crateOpeningService } from '~/services/crateOpeningService';
import type { Crate, Skin } from '~/types';
import { knivesAndGlovesSkinFilter } from '../utils/sortAndfilters';

export function useCrateOpening(crate: ShallowRef<Crate | undefined>) {
  const inventory = useInventoryStore();
  const optionsStore = useOptionsStore();
  const caseOpeningStore = useCrateOpeningStore();

  const KEY_PRICE = 2.5;

  const crateSliderSkins = ref<Skin[]>([]);
  const wonSkinIndex = ref(0);
  const wonSkin = ref<Skin | null>(null);
  const showWonSkin = ref(false);
  const showSlider = ref(false);
  const timeout = ref(false);
  const autoOpen = ref(false);

  let autoOpenInterval: ReturnType<typeof setInterval> | null = null;

  const handleOpenCase = async () => {
    if (!crate.value) return;

    handleCloseWonSkinView();

    if (optionsStore.quickOpen) {
      handleQuickOpen();
      return;
    }

    caseOpeningStore.startCaseOpening();
    document.body.style.overflow = 'hidden';
    audioService.playUnlockSound();

    await new Promise((resolve) => setTimeout(resolve, 0.1));
    const openedCrate = crateOpeningService.openCrate(crate.value, REAL_ODDS);

    crateSliderSkins.value = openedCrate.sliderSkins;
    wonSkinIndex.value = openedCrate.wonSkinIndex;
    wonSkin.value = openedCrate.wonSkin;

    showSlider.value = true;
    inventory.incrementOpenCount();
    inventory.setBalance(inventory.balance - (getCratePrice(crate.value) + KEY_PRICE));
  };

  const handleQuickOpen = () => {
    if (!crate.value) return;

    showSlider.value = false;
    document.body.style.overflow = 'hidden';

    audioService.playUnlockImmidiateSound();

    const _wonSkin = crateOpeningService.getWinningSkin(crate.value, REAL_ODDS);
    wonSkin.value = _wonSkin;

    if (_wonSkin.rarity_id.includes('ancient') || knivesAndGlovesSkinFilter(_wonSkin)) {
      timeout.value = true;
      setTimeout(() => {
        timeout.value = false;
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
    if (autoOpenInterval) {
      clearInterval(autoOpenInterval);
      autoOpenInterval = null;
    }
  };

  const handleCloseWonSkinView = () => {
    document.body.style.overflow = '';
    showWonSkin.value = false;
    showSlider.value = false;
    wonSkin.value = null;
    caseOpeningStore.isOpeningCase = false;
  };

  const toggleAutoOpen = () => {
    autoOpen.value = !autoOpen.value;
  };

  watch(autoOpen, (newVal) => {
    if (newVal) {
      autoOpenInterval = setInterval(() => {
        if (!caseOpeningStore.isOpeningCase && !timeout.value) {
          handleOpenCase();
        }
      }, 150);
    } else if (autoOpenInterval) {
      clearInterval(autoOpenInterval);
      autoOpenInterval = null;
    }
  });

  onUnmounted(() => {
    if (autoOpenInterval) clearInterval(autoOpenInterval);
    if (wonSkin.value && !caseOpeningStore.wonSkin) {
      handleCaseOpeningFinished(wonSkin.value);
    }
    handleCloseWonSkinView();
  });

  return {
    crateSliderSkins,
    wonSkinIndex,
    wonSkin,
    showWonSkin,
    showSlider,
    timeout,
    autoOpen,
    handleOpenCase,
    handleQuickOpen,
    handleCaseOpeningFinished,
    handleBack,
    handleCloseWonSkinView,
    toggleAutoOpen,
  };
}
