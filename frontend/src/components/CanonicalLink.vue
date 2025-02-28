<script setup lang="ts">
import { onMounted } from 'vue';

const props = defineProps({
  path: {
    type: String,
    default: '',
  },
});

let canonicalElement: HTMLLinkElement | null = null;

onMounted(() => {
  canonicalElement = document.querySelector('link[rel="canonical"]') || document.createElement('link');
  canonicalElement.setAttribute('rel', 'canonical');

  const baseUrl = window.location.origin;
  const canonicalUrl = baseUrl + (props.path ? props.path : '/');

  canonicalElement.setAttribute('href', canonicalUrl);

  if (!document.querySelector('link[rel="canonical"]')) {
    document.head.appendChild(canonicalElement);
  }
});
</script>

<template></template>
