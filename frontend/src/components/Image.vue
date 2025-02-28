<script setup lang="ts">
import { config } from '../config';
import { computed } from 'vue';

const { src, width, height, quality, format, alt, className, skipResize } = defineProps<{
  src: string;
  alt?: string;
  width?: number;
  height?: number;
  quality?: number;
  format?: 'webp' | 'jpeg';
  className?: string;
  skipResize?: boolean;
  priority?: boolean;
}>();

if (!src) throw new Error('src is required');

const imageUrl = computed(() => {
  const url = new URL(config.imageOptimizationUrl);

  url.searchParams.set('url', src);
  if (format) url.searchParams.set('f', format);
  if (quality) url.searchParams.set('q', quality.toString());
  if (width && !skipResize) url.searchParams.set('w', width.toString());
  if (height && !skipResize) url.searchParams.set('h', height.toString());

  return url.toString();
});
</script>

<template>
  <img :src="imageUrl" :alt="alt" :width="width" :height="height" :class="className" :fetchpriority="priority" />
</template>
