<script setup lang="ts">
import type { Skin } from '@/query/skins';
import { getSkinRarityColor } from '@/utils/color';
const props = defineProps<{
  skins: Skin[];
}>();
</script>

<template>
  <div class="p-5 my-5 rounded-sm gap-5 bg-slate-800 responsive-grid sm:mb-0">
    <div v-for="(skin, i) in props.skins" :key="i">
      <img
        :src="skin?.image ?? '/images/placeholder.webp'"
        :alt="skin?.name"
        :style="{ borderColor: getSkinRarityColor(skin) }"
        class="border-b-5 bg-slate-600 p-1 rounded-xs"
      />
      <div class="mt-1 flex flex-col text-slate-300">
        <p class="text-xs font-bold text-left">{{ skin.name.split('|')[0] }}</p>
        <p class="text-xs text-left text-slate-300/80">{{ skin.name.split('|')[1] }}</p>
      </div>
    </div>
  </div>
</template>
