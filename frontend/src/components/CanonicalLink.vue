<script setup lang="ts">
import { onMounted, watchEffect } from 'vue';
import { useRoute } from 'vue-router';

const props = defineProps({
  path: {
    type: String,
    default: '',
  },
});

const route = useRoute();

const updateCanonicalLink = () => {
  let canonicalElement = document.querySelector('link[rel="canonical"]');

  if (!canonicalElement) {
    canonicalElement = document.createElement('link');
    canonicalElement.setAttribute('rel', 'canonical');
    document.head.appendChild(canonicalElement);
  }

  const baseUrl = window.location.origin;
  let canonicalPath = props.path || route.path;

  if (canonicalPath === '/') {
    canonicalElement.setAttribute('href', baseUrl);
  } else {
    if (!canonicalPath.startsWith('/')) {
      canonicalPath = '/' + canonicalPath;
    }
    canonicalElement.setAttribute('href', baseUrl + canonicalPath);
  }
};

onMounted(() => {
  updateCanonicalLink();
});

watchEffect(() => {
  if (route.path) {
    updateCanonicalLink();
  }
});
</script>

<template></template>
