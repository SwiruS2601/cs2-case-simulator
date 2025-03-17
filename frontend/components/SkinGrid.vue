<script setup lang="ts">
import type { Skin } from '../types';
import { getItemPrice } from '../utils/balance';
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
                <div class="border-b-5 relative p-1 rounded-[5px]" :style="{ borderColor: getItemRarityColor(skin) }">
                    <Image
                        :key="getSkinKey(skin, i) + '-img'"
                        :width="160"
                        :height="120"
                        :src="skin?.image ?? '/images/placeholder.webp'"
                        :alt="skin?.name"
                        class-name="transition-transform duration-75 hover:scale-[133%] p-0 m-0"
                    ></Image>

                    <span
                        v-if="getItemPrice(skin) > 0"
                        class="absolute top-0 right-0.5 text-green-400 text-nowrap text-xs ml-auto"
                        >€ {{ getItemPrice(skin).toFixed(2) }}
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
