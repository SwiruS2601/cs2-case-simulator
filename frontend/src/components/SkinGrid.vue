<script setup lang="ts">
import type { Skin } from '@/query/skins';
import { getSkinRarityColor } from '@/utils/color';
const props = defineProps<{
  skins: Skin[];
}>();
</script>

<template>
  <div class="p-4 my-5 mb-24 rounded gap-x-3 gap-y-2 bg-slate-800 responsive-grid sm:mb-0">
    <div v-for="(skin, i) in props.skins" :key="i">
      <img
        :src="skin?.image ?? '/images/placeholder.webp'"
        :alt="skin?.name"
        :style="{ borderColor: getSkinRarityColor(skin) }"
        class="border-l-4 bg-slate-700 p-4 rounded-sm"
      />
      <p class="mt-2 text-xs text-center">{{ skin.name }}</p>
    </div>
  </div>
</template>
