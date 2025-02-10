<script setup lang="ts">
import { RouterLink } from 'vue-router';
</script>

<template>
  <div class="grid min-h-[80vh] place-items-center">
    <div class="flex flex-col gap-8 text-2xl font-light sm:gap-28 sm:flex-row">
      <RouterLink
        class="flex sm:w-[175px] w-[112.5px] flex-col items-center gap-3 transition-transform duration-75 hover:scale-105"
        to="/case"
      >
        <p>Cases</p>
        <img src="../assets/images/case.webp" alt="" width="175" height="175" />
      </RouterLink>
      <RouterLink
        class="flex sm:w-[175px] w-[112.5px] flex-col items-center gap-3 transition-transform duration-75 hover:scale-105"
        to="/souvenir"
      >
        <p>Souvenirs</p>
        <img src="../assets/images/souvenir.webp" alt="" width="175" height="175" />
      </RouterLink>
    </div>
  </div>
</template>
