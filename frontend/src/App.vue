<script setup lang="ts">
import { RouterView } from 'vue-router';
import Nav from './components/Nav.vue';
</script>

<template>
  <main class="min-h-dvh">
    <Nav />
    <div className="relative w-full max-w-5xl p-3 mx-auto">
      <RouterView />
    </div>
  </main>
</template>
