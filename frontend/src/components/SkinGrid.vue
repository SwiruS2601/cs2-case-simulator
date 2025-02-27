<script setup lang="ts">
import type { Skin } from '../types';
import { getSkinPrice } from '../utils/balance';
import { getSkinRarityColor } from '../utils/color';
import Image from './Image.vue';
const props = defineProps<{
  skins: Skin[];
  inventoryView?: boolean;
}>();
</script>

<template>
  <div class="rounded-sm gap-3 gap-y-2 sm:gap-y-3 responsive-grid">
    <div v-for="(skin, i) in props.skins" :key="i" class="max-w-[133px]">
      <div class="bg-black/30 rounded-md duration-75 hover:shadow-xl border border-black/10">
        <div class="border-b-5 p-1 rounded-[5px]" :style="{ borderColor: getSkinRarityColor(skin) }">
          <Image
            :width="256"
            :height="192"
            :src="skin?.image ?? '/images/placeholder.webp'"
            :alt="skin?.name"
            className="transition-transform duration-75 hover:scale-[133%] p-0 m-0"
          />
        </div>
      </div>
      <div class="flex justify-between mt-1 gap-2">
        <div class="flex flex-col text-slate-100">
          <p class="text-[11px] font-semibold text-left">{{ skin.name.split('|')[0] }}</p>
          <p class="text-[10px] text-left text-slate-100/80">{{ skin.name.split('|')[1] }}</p>
        </div>

        <div v-if="inventoryView" class="flex flex-col">
          <span v-if="skin.wear_category !== 'Default'" class="text-nowrap text-[10px] text-slate-100"
            >{{ skin.wear_category }}
          </span>
          <span v-if="getSkinPrice(skin) > 0" class="text-green-400 text-nowrap text-[11px] ml-auto"
            >€ {{ getSkinPrice(skin).toFixed(2) }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
