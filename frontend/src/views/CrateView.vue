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
import { useCaseOpeningStore } from '@/store/caseOpeningStore';
import CaseOpeningSlider from '@/components/CaseOpeningSlider.vue';
import type { Skin } from '@/query/skins';
import { generateRandomCaseSkins } from '@/utils/caseOpening';
import { FUN_ODDS, REAL_RARITY_ODDS } from '@/constants';

const router = useRouter();
const crateId = router.currentRoute.value.params.id as string;
const { data } = useCreate(crateId);

const optionsStore = useOptionsStore();
const caseOpeningStore = useCaseOpeningStore();

const caseItems = ref<Skin[]>([]);
const winningItem = ref<Skin | null>(null);
const winningIndex = ref(0);

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
  const {
    skins,
    wonSkin,
    winningIndex: _winningIndex,
  } = generateRandomCaseSkins(
    [...guns.value, ...knivesAndGloves.value],
    50,
    optionsStore.moreRareSkins ? FUN_ODDS : REAL_RARITY_ODDS,
  );

  console.log({
    skins,
    wonSkin,
    _winningIndex,
  });

  document.body.style.overflow = 'hidden';
  caseOpeningStore.startCaseOpening();
  caseItems.value = skins;
  winningItem.value = wonSkin;
  winningIndex.value = _winningIndex;
};

const handleCaseOpeningFinished = (item: Skin) => {
  document.body.style.overflow = '';
  caseOpeningStore.endCaseOpening(item);
  if (optionsStore.autoOpen) {
    setTimeout(handleOpenCase, 1000);
  }
};
</script>

<template>
  <div class="pt-5 pb-16">
    <div class="flex gap-4">
      <Backbutton />
      <Button variant="success" @click="handleOpenCase" :disabled="caseOpeningStore.isOpeningCase">
        Unlock Container
      </Button>
      <div class="relative">
        <Button @click="optionsStore.toggleShowOptions">
          {{ optionsStore.showOptions ? 'Hide' : 'Show' }} Options
        </Button>

        <div
          v-if="optionsStore.showOptions"
          class="absolute flex top-12 -right-2 p-4 bg-gray-800 rounded-lg shadow-lg flex-col gap-4"
        >
          <!-- <Button @click="optionsStore.toggleAutoOpen">
            {{ optionsStore.autoOpen ? 'Disable' : 'Enable' }} Auto Open
          </Button> -->
          <Button @click="optionsStore.toggleFastAnimation">
            {{ optionsStore.fastAnimation ? 'Disable' : 'Enable' }} Fast Animation
          </Button>
          <Button @click="optionsStore.toggleMoreRareSkins">
            {{ optionsStore.moreRareSkins ? 'Disable' : 'Enable' }} Guaranteed Rare Skins
          </Button>
          <Button @click="optionsStore.toggleSound"> {{ optionsStore.soundOn ? 'Disable' : 'Enable' }} Sound </Button>
        </div>
      </div>
    </div>

    <div
      v-if="caseOpeningStore.isOpeningCase"
      class="fixed inset-0 bg-black/20 backdrop-blur-xs flex items-center justify-center z-50"
    >
      <div v-if="data" class="w-full max-w-5xl">
        <CaseOpeningSlider
          :crate="data"
          :skins="caseItems"
          :winning-index="winningIndex"
          @finished="handleCaseOpeningFinished"
        />
      </div>
    </div>

    <SkinGrid v-if="guns.length" :skins="guns" class="mt-6" />
    <Button v-if="knivesAndGloves.length" @click="optionsStore.toggleShowKnivesAndGloves" class="mt-8">
      {{ optionsStore.showKnivesAndGloves ? 'Hide' : 'Show' }} Knives & Gloves
    </Button>
    <SkinGrid v-if="knivesAndGloves.length && optionsStore.showKnivesAndGloves" :skins="knivesAndGloves" />
  </div>
</template>
