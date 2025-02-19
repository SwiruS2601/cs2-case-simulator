<script setup lang="ts">
import Backbutton from '../components/Backbutton.vue';
import Button from '../components/Button.vue';
import SkinGrid from '../components/SkinGrid.vue';
import { RARITY_INDEX } from '../constants';
import { useInventoryStore } from '../store/inventoryStore';
import { getSkinPrice } from '../utils/balance';
import { computed, ref } from 'vue';

const inventory = useInventoryStore();
const selectedSort = ref('latest');

const selectOptions = [
  { value: 'latest', label: 'Latest' },
  { value: 'price', label: 'Price' },
  { value: 'name', label: 'Name' },
  { value: 'rarity', label: 'Rarity' },
];

const sortedSkins = computed(() => {
  if (!inventory.skins || inventory.skins.length === 0) return [];
  switch (selectedSort.value) {
    case 'latest':
      return [...inventory.skins].reverse();
    case 'price':
      return [...inventory.skins].sort((a, b) => getSkinPrice(b) - getSkinPrice(a));
    case 'name':
      return [...inventory.skins].sort((a, b) => a.name.split('|')[1].localeCompare(b.name.split('|')[1]));
    case 'rarity':
      return [...inventory.skins].sort((a, b) => RARITY_INDEX[b.rarity] - RARITY_INDEX[a.rarity]);
    default:
      return 0;
  }
});

const onChange = (event: Event) => {
  selectedSort.value = (event.target as HTMLSelectElement).value;
};
</script>

<template>
  <div class="flex justify-between items-center mt-5 mb-6 flex-wrap gap-4">
    <div class="flex gap-4 items-center flex-wrap">
      <Backbutton />
      <select
        class="bg-slate-600 rounded py-2 w-fit border pr-4 border-slate-600 focus:outline-none px-4 text-slate-300 font-semibold cursor-pointer hover:bg-slate-500 hover:text-slate-200 select:outline-none"
        @input="onChange"
        @change="onChange"
      >
        <option v-for="option in selectOptions" :value="option.value" :selected="option.value === selectedSort">
          {{ option.label }}
        </option>
      </select>
      <p class="text-xl font-semibold">{{ inventory.skins.length }} skins</p>
    </div>

    <Button @click="inventory.clearInventory" variant="danger"> Clear Inventory </Button>
  </div>
  <SkinGrid v-if="sortedSkins" :skins="sortedSkins" inventoryView />
</template>
