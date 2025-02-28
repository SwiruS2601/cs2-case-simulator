<script setup lang="ts">
import { onMounted } from 'vue';

onMounted(() => {
  document.addEventListener('securitypolicyviolation', (e) => {
    console.error('CSP Violation:', {
      'blocked-uri': e.blockedURI,
      'violated-directive': e.violatedDirective,
      'original-policy': e.originalPolicy,
    });
  });
});
</script>

<template></template>
