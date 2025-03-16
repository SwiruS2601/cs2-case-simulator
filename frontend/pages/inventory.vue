<script setup lang="ts">
import { useInventoryStore } from '~/composables/inventoryStore';
import { RARITY_INDEX } from '~/constants';
import { knivesAndGlovesSkinFilter } from '~/utils/sortAndfilters';

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

  const skins = [...inventory.skins];

  switch (selectedSort.value) {
    case 'latest':
      return skins.reverse();
    case 'price':
      return skins.sort((a, b) => getSkinPrice(b) - getSkinPrice(a));
    case 'name':
      return skins.sort((a, b) => {
        const aName = a.name.split('|')[1] || a.name;
        const bName = b.name.split('|')[1] || b.name;
        return aName.localeCompare(bName);
      });
    case 'rarity':
      return skins.sort((a, b) => {
        const aIsSpecial = knivesAndGlovesSkinFilter(a);
        const bIsSpecial = knivesAndGlovesSkinFilter(b);
        return (
          RARITY_INDEX[bIsSpecial ? 'exceedingly_rare' : b.rarity_id] -
          RARITY_INDEX[aIsSpecial ? 'exceedingly_rare' : a.rarity_id]
        );
      });
    default:
      return skins;
  }
});

const onChange = (event: Event) => {
  selectedSort.value = (event.target as HTMLSelectElement).value;
};
</script>

<template>
  <Container>
    <div class="flex justify-between items-center flex-wrap gap-4 pb-5">
      <div class="flex gap-4 items-center flex-wrap">
        <Backbutton />
        <div class="flex flex-col">
          <label for="inventory-sort" class="sr-only">Sort inventory by</label>
          <select
            id="inventory-sort"
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
      </div>

      <Button @click="inventory.clearInventory" variant="danger"> Reset </Button>
    </div>
    <ClientOnly>
      <p class="text-lg pb-4 text-white/90">Items: {{ inventory.skins.length }}</p>
      <template #fallback>
        <p class="text-lg pb-4 text-white/90">Items: 0</p>
      </template>
    </ClientOnly>

    <ClientOnly>
      <SkinGrid v-if="sortedSkins" :skins="sortedSkins" inventoryView />
    </ClientOnly>
  </Container>
</template>
