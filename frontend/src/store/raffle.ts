import type { Skin } from '@/query/skins';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRaffleStore = defineStore('raffle', () => {
  const isRaffling = ref(false);
  const wonItem = ref<Skin | null>(null);

  const startRaffle = () => {
    isRaffling.value = true;
    wonItem.value = null;
  };

  const endRaffle = (item: any) => {
    wonItem.value = item;
    setTimeout(() => {
      isRaffling.value = false;
    }, 1000);
  };

  return {
    isRaffling,
    wonItem,
    startRaffle,
    endRaffle,
  };
});
