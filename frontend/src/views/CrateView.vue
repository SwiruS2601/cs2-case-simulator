<script setup lang="ts">
import Backbutton from '@/components/Backbutton.vue';
import Button from '@/components/Button.vue';
import SkinGrid from '@/components/SkinGrid.vue';
import { useCreate } from '@/query/crate';
import { useOptionsStore } from '@/store/options';
import {
  filterOnlyGlovesAndKnives,
  filterSkinsByOnlyGuns,
  sortSkinByName,
  sortSkinByRarity,
} from '@/utils/sortAndfilters';
import { computed, effect, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useRaffleStore } from '@/store/raffle';
import { generateRaffleItems } from '@/utils/raffle';
import RaffleSlider from '@/components/RaffleSlider.vue';
import type { Skin } from '@/query/skins';

const router = useRouter();
const crateId = router.currentRoute.value.params.id as string;
const { data } = useCreate(crateId);

const optionsStore = useOptionsStore();
const raffleStore = useRaffleStore();

const raffleItems = ref<Skin[]>([]);
const winningItem = ref<Skin | null>(null);
const raffleWinningIndex = ref(0);

const guns = computed(() => {
  const skins = data?.value?.skins;
  if (!skins?.length) return [];
  return skins.filter(filterSkinsByOnlyGuns).sort(sortSkinByRarity);
});

const knivesAndGloves = computed(() => {
  const skins = data?.value?.skins;
  if (!skins?.length) return [];
  return skins.filter(filterOnlyGlovesAndKnives).sort(sortSkinByName);
});

const handleOpenCase = () => {
  const allSkins = [...guns.value, ...knivesAndGloves.value];
  const { items, wonItem, winningIndex } = generateRaffleItems(allSkins);
  raffleStore.startRaffle();
  raffleItems.value = items;
  winningItem.value = wonItem;
  raffleWinningIndex.value = winningIndex;
};

const handleRaffleFinished = (item: Skin) => {
  raffleStore.endRaffle(item);
};

effect(() => {
  if (raffleStore.isRaffling) {
    document.body.style.overflow = 'hidden';
  } else {
    document.body.style.overflow = '';
  }
});
</script>

<template>
  <div class="pt-5 pb-16">
    <div class="flex gap-4">
      <Backbutton />
      <Button variant="success" @click="handleOpenCase" :disabled="raffleStore.isRaffling"> OPEN CASE </Button>
    </div>

    <div
      v-if="raffleStore.isRaffling"
      class="fixed inset-0 bg-black/70 backdrop-blur-xs flex items-center justify-center z-50"
    >
      <div class="w-full max-w-5xl">
        <RaffleSlider :skins="raffleItems" :winning-index="raffleWinningIndex" @finished="handleRaffleFinished" />
      </div>
    </div>

    <SkinGrid v-if="guns.length" :skins="guns" class="mt-6" />
    <Button v-if="knivesAndGloves.length" @click="optionsStore.toggleShowKnivesAndGloves" class="mt-8">
      {{ optionsStore.showKnivesAndGloves ? 'Hide' : 'Show' }} Knives & Gloves
    </Button>
    <SkinGrid v-if="knivesAndGloves.length && optionsStore.showKnivesAndGloves" :skins="knivesAndGloves" />
  </div>
</template>
