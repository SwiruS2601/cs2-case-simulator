<script setup lang="ts">
import type { Skin } from '../types';
import Image from './Image.vue';
const props = defineProps<{
    items: Skin[];
    inventoryView?: boolean;
}>();

const getSkinKey = (skin: Skin, index: number) => {
    return `${skin.id || ''}${skin.name}${skin.wear_category || ''}${index}`;
};
</script>

<template>
    <div class="rounded-sm gap-3 gap-y-2 sm:gap-y-3 responsive-grid">
        <div v-for="(item, i) in props.items" :key="getSkinKey(item, i)" class="max-w-[133px]">
            <div class="bg-black/30 rounded-md duration-75 hover:shadow-xl border border-black/10">
                <div class="border-b-5 relative p-1 rounded-[5px]" :style="{ borderColor: getItemRarityColor(item) }">
                    <Image
                        :key="getSkinKey(item, i) + '-img'"
                        :width="160"
                        :height="120"
                        :src="item?.image ?? '/images/placeholder.webp'"
                        :alt="item?.name"
                        class-name="relative transition-transform duration-75 pb-4 hover:scale-[133%] p-0 m-0 z-1"
                    ></Image>
                    <div
                        v-if="item?.prices?.[0]?.wear_category == 'Default'"
                        class="absolute flex justify-end w-full bottom-0 right-0 text-green-400 pb-0.5 text-xs bg-black/20 px-1"
                    >
                        {{ formatEuro(getItemMinPrice(item)) }}
                    </div>
                    <div
                        v-else
                        class="absolute flex justify-between flex-wrap w-full bottom-0 right-0 text-green-400 pb-0.5 text-xs bg-black/20 px-1"
                    >
                        <span> {{ formatEuro(getItemMinPrice(item)) }} </span>
                        <span class="text-white/20">-</span>
                        <span> {{ formatEuro(getItemMaxPrice(item)) }}</span>
                    </div>
                </div>
            </div>
            <div class="flex flex-col justify-between mt-1">
                <p class="text-xs font-semibold text-left text-white/85">{{ item.name }}</p>

                <span v-if="item.wear_category !== 'Default'" class="text-nowrap text-xs text-white/70"
                    >{{ item.wear_category }}
                </span>
            </div>
        </div>
    </div>
</template>
