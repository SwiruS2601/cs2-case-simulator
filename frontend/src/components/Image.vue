<script setup lang="ts">
import { config } from '../config';

const { src, width, height, quality, format, alt, className, skipResize } = defineProps<{
  src: string;
  alt?: string;
  width?: number;
  height?: number;
  quality?: number;
  format?: 'webp' | 'jpeg';
  className?: string;
  skipResize?: boolean;
}>();

if (!src) throw new Error('src is required');

const url = new URL(config.imageOptimizationUrl);

url.searchParams.set('url', src);
if (format) url.searchParams.set('f', format);
if (quality) url.searchParams.set('q', quality.toString());
if (width && !skipResize) url.searchParams.set('w', width.toString());
if (height && !skipResize) url.searchParams.set('h', height.toString());
</script>

<template>
  <img :src="url.toString()" :alt="alt" :width="width" :height="height" :class="className" />
</template>
