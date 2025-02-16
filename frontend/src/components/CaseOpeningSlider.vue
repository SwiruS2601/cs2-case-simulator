<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import type { Skin } from '@/query/skins';
import itemScrollSound from '../assets/audio/csgo_ui_crate_item_scroll.mp3';
import type { Crate } from '@/query/crate';
import SliderItem from './SliderItem.vue';
import { useCaseOpeningStore } from '@/store/caseOpeningStore';
import { useOptionsStore } from '@/store/options';

const props = defineProps<{
  skins: Skin[];
  winningIndex: number;
  crate: Crate;
}>();
const emit = defineEmits<{
  finished: [skin: Skin];
}>();

const optionsStore = useOptionsStore();

const duration = optionsStore.fastAnimation ? 1500 : 6000;
const isMobile = window.innerWidth < 768;
const skinWidth = isMobile ? 160 : 256;
const magnifiedSliderScale = 1.15;
const magnifyingGlassRadius = isMobile ? 160 : 256;

const easing = `transform ${duration}ms cubic-bezier(0.15, 0.4, 0.35, 1)`;
const maskCircle = `radial-gradient(circle at center, transparent ${magnifyingGlassRadius}px, black ${magnifyingGlassRadius}px)`;

const scrollX = ref(0);
const delta = ref(0);

const backgroundTransform = computed(() => `translateX(-${scrollX.value + 15}px)`);
const magnifiedTransform = computed(() => `translateX(-${scrollX.value + delta.value}px)`);

const bgSlider = ref<HTMLDivElement | null>(null);
const magSlider = ref<HTMLDivElement | null>(null);

onMounted(() => {
  const parentEl = bgSlider.value?.parentElement;
  const parentWidth = parentEl ? parentEl.clientWidth : window.innerWidth;

  const bgCenterOffset = parentWidth / 2 - skinWidth / 2;
  const magCenterOffset = magnifyingGlassRadius - skinWidth / 2;

  delta.value = bgCenterOffset - magCenterOffset;

  const targetScroll = props.winningIndex * skinWidth - bgCenterOffset;

  if (bgSlider.value && magSlider.value) {
    bgSlider.value.style.transformOrigin = 'center center';
    magSlider.value.style.transformOrigin = 'center center';
    bgSlider.value.style.transition = 'none';
    bgSlider.value.style.transform = 'translateX(0)';
    magSlider.value.style.transition = 'none';
    magSlider.value.style.transform = 'translateX(0)';
  }

  const thresholds = props.skins.map((_, i) => i * skinWidth - parentWidth / 2 + 2);
  const playedIndices = new Set<number>();
  const startTime = Date.now();

  function checkPosition() {
    if (!bgSlider.value) return;
    const style = getComputedStyle(bgSlider.value);
    const transform = style.transform;
    let currentScroll = 0;
    if (transform && transform !== 'none') {
      const match = transform.match(/matrix.*\((.+)\)/);
      if (match) {
        const values = match[1].split(', ');
        currentScroll = -parseFloat(values[4]);
      }
    }
    if (optionsStore.soundOn) {
      thresholds.forEach((threshold, index) => {
        if (!playedIndices.has(index) && currentScroll >= threshold) {
          playedIndices.add(index);
          const audio = new Audio(itemScrollSound);
          audio.volume = 0.07;
          audio.play();
        }
      });
    }
    if (Date.now() - startTime < duration) {
      requestAnimationFrame(checkPosition);
    }
  }

  setTimeout(() => {
    if (bgSlider.value) bgSlider.value.style.transition = easing;
    if (magSlider.value) magSlider.value.style.transition = easing;
    scrollX.value = targetScroll;
    requestAnimationFrame(checkPosition);
  }, 50);

  setTimeout(
    () => {
      emit('finished', props.skins[props.winningIndex]);
    },
    optionsStore.fastAnimation ? 1000 : duration + 100,
  );
});
</script>

<template>
  <div class="relative h-32 sm:h-48 select-none">
    <!-- Outer container with radial mask -->
    <div
      :style="{ maskImage: maskCircle, WebkitMaskImage: maskCircle }"
      class="overflow-hidden relative slider-blur slider-mask w-full h-full"
    >
      <!-- Inner container with linear fade -->
      <div class="slider-fade h-full">
        <div
          ref="bgSlider"
          class="flex absolute left-0 top-0 will-change-transform origin-center"
          :style="{ transform: backgroundTransform }"
        >
          <div v-for="(skin, index) in props.skins" :key="index" class="sm:w-64 w-40 h-32 sm:h-48 flex-shrink-0 px-2">
            <SliderItem :skin="skin" />
          </div>
        </div>
      </div>
    </div>

    <!-- Magnifying lens container -->
    <div
      class="magnifying-lens"
      :style="{
        width: `${magnifyingGlassRadius * 2}px`,
        height: `${magnifyingGlassRadius * 2}px`,
      }"
    >
      <div :style="{ scale: magnifiedSliderScale }" class="magnified-wrapper overflow-hidden relative w-full h-full">
        <!-- The magnified slider uses the extra delta offset -->
        <div
          ref="magSlider"
          class="flex absolute left-0 top-1/2 -translate-y-1/2 will-change-transform origin-center"
          :style="{ transform: magnifiedTransform }"
        >
          <div v-for="(skin, index) in props.skins" :key="index" class="sm:w-64 w-40 h-32 sm:h-48 flex-shrink-0 px-2">
            <SliderItem :skin="skin" />
          </div>
        </div>
      </div>
    </div>

    <!-- Center indicator (optional) -->
    <div class="absolute left-1/2 top-1/2 -translate-y-1/2 z-100">
      <div
        class="w-1 bg-yellow-300/80"
        :style="`height: calc(${isMobile ? '8rem' : '12rem'}* ${magnifiedSliderScale})`"
      ></div>
    </div>
  </div>
</template>

<style scoped>
.slider-blur {
  filter: blur(2px);
}

.slider-fade {
  -webkit-mask-image: linear-gradient(to right, transparent 0%, black 10%, black 90%, transparent 100%);
  mask-image: linear-gradient(to right, transparent 0%, black 10%, black 90%, transparent 100%);
}

.magnifying-lens {
  position: absolute;
  pointer-events: none;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 50%;
  overflow: hidden;
  z-index: 60;
  background-color: rgba(0, 0, 0, 0.15);
  box-shadow: 0 0 8px rgba(0, 0, 0, 0.15);
}

.magnified-wrapper {
  clip-path: circle(50%);
  transform-origin: center center;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
