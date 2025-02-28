<script setup lang="ts">
import { inject } from 'vue';

const cspNonce = inject('cspNonce', '');

function loadExternalScript(src) {
  const script = document.createElement('script');
  script.src = src;
  script.nonce = cspNonce;
  document.head.appendChild(script);
}
</script>

<template>
  <button @click="loadExternalScript('https://example.com/script.js')">Load Script</button>
</template>
