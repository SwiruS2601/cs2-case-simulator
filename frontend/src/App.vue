<script setup lang="ts">
import { RouterView } from 'vue-router';
import Nav from './components/Nav.vue';
import DynamicHeadTags from './components/DynamicHeadTags.vue';
import Breadcrumbs from './components/Breadcrumbs.vue';
import { BACKGROUNDS } from './constants';

const backgroundImage = BACKGROUNDS[Math.floor(Math.random() * BACKGROUNDS.length)];
</script>

<template>
  <DynamicHeadTags />
  <main class="min-h-[calc(100dvh+0.5px)]">
    <img
      :src="`/backgrounds/${backgroundImage}`"
      class="fixed inset-0 object-cover size-full z-0"
      alt="CS2 background map"
      fetchpriority="high"
    />
    <div class="fixed inset-0 object-cover size-full z-0 backdrop-blur-xs" />
    <Nav />
    <RouterView />
    <Breadcrumbs />
  </main>
</template>

<style scoped></style>
