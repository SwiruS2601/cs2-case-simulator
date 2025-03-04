<script setup lang="ts">
import { computed, ref, onMounted, onBeforeUnmount } from 'vue';

const { src, width, height, quality, format, alt, className, skipResize, fetchpriority } = defineProps<{
  src: string;
  alt?: string;
  width?: number;
  height?: number;
  quality?: number;
  format?: 'webp' | 'jpeg';
  className?: string;
  skipResize?: boolean;
  fetchpriority?: 'auto' | 'high' | 'low' | 'normal';
}>();

if (!src) throw new Error('src is required');

const imageUrl = computed(() => {
  const url = new URL(useRuntimeConfig().public.imageUrl);

  url.searchParams.set('url', src);
  if (format) url.searchParams.set('f', format);
  if (quality) url.searchParams.set('q', quality.toString());
  if (width && !skipResize) url.searchParams.set('w', width.toString());
  if (height && !skipResize) url.searchParams.set('h', height.toString());

  return url.toString();
});

const imageRef = ref<HTMLImageElement | null>(null);
const isInViewport = ref(false);
const { observe } = useIntersectionObserver();
let unobserveFunc: (() => void) | null = null;

onMounted(() => {
  if (imageRef.value) {
    const { unobserve } = observe(imageRef.value, (visible) => {
      isInViewport.value = visible;
    });
    unobserveFunc = unobserve;
  } else {
    isInViewport.value = true;
  }
});

onBeforeUnmount(() => {
  if (unobserveFunc) {
    unobserveFunc();
  }
});

const effectiveFetchPriority = computed(() => {
  if (fetchpriority) return fetchpriority;
  return isInViewport.value ? 'high' : 'low';
});

const loadingAttribute = computed(() => {
  return isInViewport.value ? undefined : 'lazy';
});
</script>

<template>
  <img
    ref="imageRef"
    :src="imageUrl"
    :alt="alt"
    :width="width"
    :height="height"
    :class="className"
    :fetchpriority="effectiveFetchPriority"
    :loading="loadingAttribute"
  />
</template>
