<script setup lang="ts">
import type { Skin } from '../types';
import { getSkinPrice } from '../utils/balance';
import { getSkinRarityColor } from '../utils/color';
import Image from './Image.vue';
const props = defineProps<{
  skins: Skin[];
  inventoryView?: boolean;
}>();

const getSkinKey = (skin: Skin, index: number) => {
  return `${skin.id || ''}${skin.name}${skin.wear_category || ''}${index}`;
};
</script>

<template>
  <div class="rounded-sm gap-3 gap-y-2 sm:gap-y-3 responsive-grid">
    <div v-for="(skin, i) in props.skins" :key="getSkinKey(skin, i)" class="max-w-[133px]">
      <div class="bg-black/30 rounded-md duration-75 hover:shadow-xl border border-black/10">
        <div class="border-b-5 relative p-1 rounded-[5px]" :style="{ borderColor: getSkinRarityColor(skin) }">
          <Image
            :width="256"
            :height="192"
            :src="skin?.image ?? '/images/placeholder.webp'"
            :alt="skin?.name"
            className="transition-transform duration-75 hover:scale-[133%] p-0 m-0"
            :key="getSkinKey(skin, i) + '-img'"
            fetchpriority="high"
          />

          <span
            v-if="getSkinPrice(skin) > 0"
            class="absolute top-0 right-0.5 text-green-400 text-nowrap text-xs ml-auto"
            >€ {{ getSkinPrice(skin).toFixed(2) }}
          </span>
        </div>
      </div>
      <div class="flex flex-col justify-between mt-1">
        <p class="text-xs font-semibold text-left text-white/80">{{ skin.name }}</p>

        <span v-if="skin.wear_category !== 'Default'" class="text-nowrap text-xs text-white/70"
          >{{ skin.wear_category }}
        </span>
      </div>
    </div>
  </div>
</template>
