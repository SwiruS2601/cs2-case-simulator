import type { Skin } from '@/query/skins';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useCaseOpeningStore = defineStore('case-opening', () => {
  const isOpeningCase = ref(false);
  const wonSkin = ref<Skin | null>(null);

  const startCaseOpening = () => {
    isOpeningCase.value = true;
    wonSkin.value = null;
  };

  const endCaseOpening = (item: any) => {
    wonSkin.value = item;
    setTimeout(() => {
      isOpeningCase.value = false;
    }, 1000);
  };

  return {
    isOpeningCase,
    wonSkin,
    startCaseOpening,
    endCaseOpening,
  };
});
