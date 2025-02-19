<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import SliderItem from './SliderItem.vue';
import { useOptionsStore } from '@/store/optionsStore';
import { audioService } from '@/services/audioService';
import type { Crate, Skin } from '@/types';

const props = defineProps<{
  skins: Skin[];
  wonSkinIndex: number;
  crate: Crate;
}>();

const emit = defineEmits<{
  finished: [skin: Skin];
}>();

const optionsStore = useOptionsStore();

const FAST_DURATION = 1500;
const NORMAL_DURATION = 6000;

const duration = optionsStore.fastAnimation ? FAST_DURATION : NORMAL_DURATION;
const isMobile = window.innerWidth < 640;
const skinWidth = isMobile ? 160 : 256;
const magnifiedSliderScale = 1.15;
const magnifyingGlassRadius = isMobile ? 160 : 256;

const easing = `transform ${duration}ms cubic-bezier(0.1, 0.4, 0.4, 1)`;
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

  const targetScroll = props.wonSkinIndex * skinWidth - bgCenterOffset;

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

  const checkPosition = () => {
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
          audioService.playItemScrollSound();
        }
      });
    }

    if (Date.now() - startTime < duration) {
      requestAnimationFrame(checkPosition);
    }
  };

  setTimeout(() => {
    if (bgSlider.value) bgSlider.value.style.transition = easing;
    if (magSlider.value) magSlider.value.style.transition = easing;
    scrollX.value = targetScroll;
    requestAnimationFrame(checkPosition);
  }, 50);

  setTimeout(() => {
    emit('finished', props.skins[props.wonSkinIndex]);
  }, duration);
});
</script>

<template>
  <div class="relative h-32 sm:h-48 select-none fast-fade-scale-up">
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

@keyframes fadeScaleUp {
  0% {
    opacity: 0;
    transform: scale(0.5);
  }
  100% {
    opacity: 1;
    transform: scale(1);
  }
}

.fast-fade-scale-up {
  animation: fadeScaleUp 0.15s ease-out forwards;
}
</style>
