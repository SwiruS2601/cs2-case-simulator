<script setup lang="ts">
import Backbutton from '@/components/Backbutton.vue';
import SkinGrid from '@/components/SkinGrid.vue';
import { useCreate } from '@/query/crate';
import {
  filterOnlyGlovesAndKnives,
  filterSkinsByOnlyGuns,
  sortSkinByName,
  sortSkinByRarity,
} from '@/utils/sortAndfilters';
import { computed, effect } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const crateId = router.currentRoute.value.params.id as string;
const { data } = useCreate(crateId);

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
</script>

<template>
  <div class="pt-5 pb-16">
    <Backbutton />
    <SkinGrid :skins="guns" />
    <SkinGrid :skins="knivesAndGloves" />
  </div>
</template>
