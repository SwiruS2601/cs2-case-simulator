<script setup lang="ts">
import { RouterView } from 'vue-router';
import Nav from './components/Nav.vue';
import DynamicHeadTags from './components/DynamicHeadTags.vue';
import { BACKGROUNDS } from './constants';
import Image from './components/Image.vue';
import { config } from './config';
import CanonicalLink from './components/CanonicalLink.vue';

const backgroundImage = BACKGROUNDS[Math.floor(Math.random() * BACKGROUNDS.length)];
</script>

<template>
  <DynamicHeadTags />
  <CanonicalLink />
  <main class="min-h-[calc(100vh+0.5px)]">
    <Image
      :src="`${config.baseUrl}/backgrounds/${backgroundImage}`"
      class="fixed inset-0 object-cover size-full -z-10"
      alt="CS2 background map"
      fetchpriority="high"
      :width="1280"
      :height="720"
    />
    <div class="fixed inset-0 object-cover size-full z-0 backdrop-blur-xs" />
    <Nav />
    <RouterView />
  </main>
</template>

<style scoped></style>
