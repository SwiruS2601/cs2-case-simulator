import { ref } from 'vue';
import { defineStore } from 'pinia';
import type { Skin } from '../types';

export const useInventoryStore = defineStore(
  'inventory-store',
  () => {
    const balance = ref(0);
    const skins = ref<Skin[]>([]);
    const openCount = ref(0);

    const setBalance = (newBalance: number) => (balance.value = newBalance);
    const addSkin = (skin: Skin) => skins.value.push(skin);
    const incrementOpenCount = () => openCount.value++;
    const removeSkin = (index: number) => skins.value.splice(index, 1);
    const clearInventory = () => {
      skins.value = [];
      openCount.value = 0;
      balance.value = 0;
    };

    return {
      balance,
      setBalance,
      skins,
      addSkin,
      openCount,
      incrementOpenCount,
      clearInventory,
      removeSkin,
    };
  },
  {
    persist: true,
  },
);
