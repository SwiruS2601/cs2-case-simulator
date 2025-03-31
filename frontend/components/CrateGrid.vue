<script setup lang="ts">
import type { Crate } from '~/types';
import Image from '~/components/Image.vue';
defineProps<{
    crates: Crate[];
    imageHeight?: number;
}>();
</script>

<template>
    <div class="gap-3 mt-4 responsive-grid justify-between">
        <NuxtLink
            v-for="crate in crates"
            :key="crate.id"
            :to="`/crate/${encodeURIComponent(crate.name)}`"
            class="mx-auto"
            prefetch-on="interaction"
        >
            <div
                class="relative flex flex-col cursor-pointer items-center bg-black/20 transition-shadow duration-150 rounded-lg border border-black/10 hover:shadow-xl"
            >
                <div
                    class="absolute flex justify-end w-full bottom-0 right-0 text-green-400 pb-0.5 text-xs bg-black/20 px-1"
                >
                    {{ formatEuro(getCratePrice(crate)) }}
                </div>
                <Image
                    class="transition-transform duration-75 hover:scale-[120%] pt-1 px-2 pb-5"
                    :src="crate.image || '/images/placeholder.webp'"
                    :alt="`${crate.name} - CS2 Case`"
                    :width="128"
                    :height="97"
                ></Image>
            </div>
            <p class="text-xs font-semibold mt-1 text-left text-white/85">{{ crate.name }}</p>
        </NuxtLink>
    </div>
</template>
