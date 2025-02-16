import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useOptionsStore = defineStore('options', () => {
  const showKnivesAndGloves = ref(false);

  const toggleShowKnivesAndGloves = () => {
    showKnivesAndGloves.value = !showKnivesAndGloves.value;
  };

  return { showKnivesAndGloves, toggleShowKnivesAndGloves };
});
