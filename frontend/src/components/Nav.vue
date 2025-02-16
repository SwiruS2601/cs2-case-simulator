<script setup lang="ts">
import Button from './Button.vue';
</script>

<template>
  <header class="sticky top-0 z-[10] bg-slate-900">
    <nav class="flex items-center justify-between w-full max-w-5xl p-3 mx-auto">
      <router-link to="/">
        <Button>Home</Button>
      </router-link>
      <router-link to="/inventory">
        <Button>Inventory</Button>
      </router-link>
    </nav>
  </header>
</template>
