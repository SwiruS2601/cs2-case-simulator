<script setup lang="ts">
import Backbutton from '../components/Backbutton.vue';
import Button from '../components/Button.vue';
import SkinGrid from '../components/SkinGrid.vue';
import { RARITY_INDEX } from '../constants';
import { useInventoryStore } from '../store/inventoryStore';
import { getSkinPrice } from '../utils/balance';
import { computed, effect, ref } from 'vue';
import { knivesAndGlovesSkinFilter } from '../utils/sortAndfilters';

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
      return [...inventory.skins].sort((a, b) => {
        const aIsYellow = knivesAndGlovesSkinFilter(a) || knivesAndGlovesSkinFilter(a);
        const bIsYellow = knivesAndGlovesSkinFilter(b) || knivesAndGlovesSkinFilter(b);
        return (
          RARITY_INDEX[bIsYellow ? 'rarity_ancient' : b.rarity_id] -
          RARITY_INDEX[aIsYellow ? 'rarity_ancient' : a.rarity_id]
        );
      });
    default:
      return 0;
  }
});

const onChange = (event: Event) => {
  selectedSort.value = (event.target as HTMLSelectElement).value;
};
</script>

<template>
  <div
    className="relative w-full max-w-5xl px-4 mx-auto backdrop-blur-xs bg-black/40 sm:rounded-xl py-4 my-0 sm:my-4 border border-black/10"
  >
    <div class="flex justify-between items-center flex-wrap gap-4 pb-6">
      <div class="flex gap-4 items-center flex-wrap">
        <Backbutton />
        <select
          class="rounded-lg py-2 w-fit border pr-4 h-[42px] border-black/10 bg-black/20 focus:outline-none px-4 font-semibold cursor-pointer hover:bg-black/10 select:outline-none"
          @input="onChange"
          @change="onChange"
        >
          <option
            v-for="option in selectOptions"
            :value="option.value"
            :selected="option.value === selectedSort"
            class="bg-black/50"
          >
            {{ option.label }}
          </option>
        </select>
      </div>

      <Button @click="inventory.clearInventory" variant="danger"> Reset </Button>
    </div>
    <p class="text-lg pb-4 text-white/90">Items: {{ inventory.skins.length }}</p>

    <SkinGrid v-if="sortedSkins" :skins="sortedSkins" inventoryView />
  </div>
</template>
