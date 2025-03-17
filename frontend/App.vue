<script setup lang="ts">
import '~/assets/css/main.css';
</script>

<template>
  <NuxtLayout>
    <NuxtPage ></NuxtPage>
  </NuxtLayout>
</template>

<style>
html,
dialog {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans',
    'Helvetica Neue', sans-serif;
  color: white;
}

body {
  background: #1a1c20;
}

.responsive-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(125px, 1fr));
}

@media screen and (max-width: 720px) {
  .responsive-grid {
    grid-template-columns: repeat(auto-fit, minmax(90px, 1fr));
  }
}
</style>
