import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useOptionsStore = defineStore(
  'options-store-v2',
  () => {
    const showKnivesAndGloves = ref(true);
    const autoOpen = ref(false);
    const fastAnimation = ref(false);
    const moreRareSkins = ref(false);
    const soundOn = ref(true);
    const quickOpen = ref(false);

    const toggleShowKnivesAndGloves = () => (showKnivesAndGloves.value = !showKnivesAndGloves.value);
    const toggleAutoOpen = () => (autoOpen.value = !autoOpen.value);
    const toggleFastAnimation = () => (fastAnimation.value = !fastAnimation.value);
    const toggleMoreRareSkins = () => (moreRareSkins.value = !moreRareSkins.value);
    const toggleSound = () => (soundOn.value = !soundOn.value);
    const toggleQuickOpen = () => (quickOpen.value = !quickOpen.value);

    return {
      showKnivesAndGloves,
      toggleShowKnivesAndGloves,
      autoOpen,
      toggleAutoOpen,
      fastAnimation,
      toggleFastAnimation,
      moreRareSkins,
      toggleMoreRareSkins,
      soundOn,
      toggleSound,
      quickOpen,
      toggleQuickOpen,
    };
  },
  {
    persist: true,
  },
);
