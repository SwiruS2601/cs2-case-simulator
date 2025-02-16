<script setup lang="ts">
import type { Skin } from '@/query/skins';
import { getSkinRarityColor } from '@/utils/color';
import { onMounted, ref } from 'vue';

const props = defineProps<{
  skins: Skin[];
  winningIndex: number;
}>();

const emit = defineEmits<{
  finished: [skin: Skin];
}>();

const containerRef = ref<HTMLDivElement>();
const isAnimating = ref(false);

onMounted(() => {
  if (!containerRef.value) return;

  isAnimating.value = true;
  const duration = 6000;
  const itemWidth = 192;

  const parentEl = containerRef.value.parentElement;
  const parentWidth = parentEl ? parentEl.clientWidth : window.innerWidth;
  const centerOffset = parentWidth / 2 - itemWidth / 2;

  const targetScroll = props.winningIndex * itemWidth - centerOffset;

  containerRef.value.style.transform = 'translateX(0px)';
  containerRef.value.style.transition = 'none';

  requestAnimationFrame(() => {
    if (!containerRef.value) return;

    containerRef.value.style.transform = `translateX(-${targetScroll}px)`;
    containerRef.value.style.transition = `transform ${duration}ms cubic-bezier(0.15, 0.75, 0.25, 1)`;

    setTimeout(() => {
      isAnimating.value = false;
      emit('finished', props.skins[props.winningIndex]);
    }, duration);
  });
});
</script>

<template>
  <div class="relative h-48 overflow-hidden bg-zinc-800">
    <div class="absolute w-full h-48 pointer-events-none z-10 fade-overlay"></div>
    <div class="absolute left-1/2 w-1 h-full bg-yellow-400 z-20 -translate-x-[2px]"></div>
    <div ref="containerRef" class="flex absolute left-0 top-0 will-change-transform">
      <div v-for="(skin, index) in skins" :key="index" class="w-48 h-48 flex-shrink-0 p-2">
        <div
          :style="{
            backgroundImage: `linear-gradient(#5D5B63, 90%, ${getSkinRarityColor(skin)}B3)`,
            borderColor: getSkinRarityColor(skin),
            boxShadow: 'inset 0px -0px 1px black',
          }"
          :class="['size-full rounded-t-xs p-2 bg-gray-700', 'flex items-center justify-center border-b-8']"
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
      </div>
    </div>
  </div>
</template>

<style scoped>
.fade-overlay {
  pointer-events: none;
  background: linear-gradient(
    90deg,
    rgba(0, 0, 0, 1) 0%,
    rgba(0, 0, 0, 0) 10%,
    rgba(0, 0, 0, 0) 90%,
    rgba(0, 0, 0, 1) 100%
  );
}
</style>
