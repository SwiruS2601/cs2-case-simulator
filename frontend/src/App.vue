<script setup lang="ts">
import { RouterView } from 'vue-router';
import Nav from './components/Nav.vue';
</script>

<template>
  <Nav />
  <div className="relative w-full max-w-5xl px-4 mx-auto">
    <RouterView />
  </div>
</template>
