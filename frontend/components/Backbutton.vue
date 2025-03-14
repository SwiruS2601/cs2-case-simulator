<script setup lang="ts">
import { useRouter } from 'vue-router';
import Button from './Button.vue';
import BackIcon from './BackIcon.vue';

const { fallbackUrl } = defineProps<{
  fallbackUrl?: string;
}>();

const router = useRouter();

const handleBack = () => {
  if (window.history.length > 1) {
    return router.go(-1);
  }

  if (fallbackUrl) {
    router.push(fallbackUrl);
  }
};
</script>

<template>
  <Button @click="handleBack" class="flex items-center gap-2"> <BackIcon /> Back </Button>
</template>
