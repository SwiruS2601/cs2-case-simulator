<script setup lang="ts">
import { RouterView } from 'vue-router';
import Nav from './components/Nav.vue';
import { BACKGROUNDS } from './constants';

const backgroundImage = BACKGROUNDS[Math.floor(Math.random() * BACKGROUNDS.length)];
</script>

<template>
  <main class="min-h-[calc(100dvh+0.5px)]">
    <img :src="`/backgrounds/${backgroundImage}`" class="fixed inset-0 object-cover w-full h-full z-0" />
    <div class="fixed inset-0 object-cover w-full h-full z-0 backdrop-blur-xs" />
    <Nav />
    <RouterView />
  </main>
</template>

<style scoped></style>
