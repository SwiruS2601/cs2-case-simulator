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
        class="flex flex-col h-full cursor-pointer items-center bg-black/20 transition-shadow duration-150 rounded-lg px-2 py-2 border border-black/10 hover:shadow-xl"
      >
        <p class="text-xs text-center my-auto text-white/95">{{ crate.name }}</p>
        <Image
          class="transition-transform duration-75 hover:scale-[120%]"
          :src="crate.image || '/images/placeholder.webp'"
          :alt="`${crate.name} - CS2 Case`"
          :width="128"
          :height="97"
        />
      </div>
    </NuxtLink>
  </div>
</template>
