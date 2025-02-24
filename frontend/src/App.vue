<script setup lang="ts">
import { RouterView } from 'vue-router';
import Nav from './components/Nav.vue';
import { BACKGROUNDS } from './constants';

const backgroundImage = BACKGROUNDS[Math.floor(Math.random() * BACKGROUNDS.length)];
</script>

<template>
  <img :src="`/backgrounds/${backgroundImage}`" class="fixed inset-0 object-cover w-full h-full z-0" />
  <Nav />
  <RouterView />
</template>

<style scoped></style>
