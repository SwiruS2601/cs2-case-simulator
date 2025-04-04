<script setup lang="ts">
import { useRouter } from 'vue-router';
import Button from './Button.vue';
import BackIcon from './BackIcon.vue';

const { fallbackUrl } = defineProps<{
    fallbackUrl?: string;
}>();

const router = useRouter();

const handleBack = () => {
    if (history.state && history.state.back) {
        router.go(-1);
        return;
    }

    if (fallbackUrl) {
        router.push(fallbackUrl);
        return;
    }

    router.push('/');
};
</script>

<template>
    <Button class="flex items-center gap-2" @click="handleBack"> <BackIcon></BackIcon> Back </Button>
</template>
