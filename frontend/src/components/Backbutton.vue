<script setup lang="ts">
import { useRouter } from 'vue-router';
import Button from './Button.vue';
import BackIcon from './BackIcon.vue';

const router = useRouter();
</script>

<template>
  <Button @click="router.go(-1)" class="flex items-center gap-3"> <BackIcon /> Back </Button>
</template>
