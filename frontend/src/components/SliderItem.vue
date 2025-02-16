<script setup lang="ts">
import type { Skin } from '@/query/skins';
import { getSkinRarityColor } from '@/utils/color';
const props = defineProps<{
  skin: Skin;
}>();
</script>

<template>
  <div
    :style="{
      backgroundImage: `linear-gradient(#5D5B63, 85%, ${getSkinRarityColor(skin)}B3)`,
      borderColor: getSkinRarityColor(skin),
      boxShadow: 'inset 0px -0px 1px black',
    }"
    class="size-full rounded-t-xs p-2 bg-gray-700 flex items-center justify-center border-b-[10px]"
  >
    <img
      v-if="skin?.image"
      :src="skin.image"
      :alt="skin.name"
      class="size-full object-contain"
      @error="console.error('Image failed to load:', skin.image)"
    />
    <div v-else class="text-red-500">No image</div>
  </div>
</template>
