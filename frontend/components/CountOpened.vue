<script setup lang="ts">
const opened = ref<number>();
const prevCount = ref(0);
const isAnimating = ref(false);
const difference = ref(0);
const interval = ref<NodeJS.Timeout | null>(null);

const { data, refetch } = useQuery<{ totalCount: number }>({
    key: ['crate-count'],
    query: () => fetch(`${useRuntimeConfig().public.apiUrl}/api/crate-opening/count`).then((res) => res.json()),
});

onMounted(() => {
    opened.value = data.value?.totalCount;
    interval.value = setInterval(refetch, 2500);
});

onUnmounted(() => clearInterval(interval.value!));

watch(
    () => data.value?.totalCount,
    (newCount) => {
        if (newCount && prevCount.value && newCount > prevCount.value) {
            difference.value = newCount - prevCount.value;
            isAnimating.value = true;

            setTimeout(() => {
                isAnimating.value = false;
                difference.value = 0;
            }, 1500);
        }

        prevCount.value = newCount || 0;
        if (data.value) {
            opened.value = data.value?.totalCount;
        }
    },
);
</script>

<template>
    <ClientOnly>
        <div class="flex items-center">
            <div
                class="flex relative gap-1.5 flex-wrap items-center bg-black/30 rounded-md p-4 py-2 border border-black/10"
            >
                <span :class="{ 'animate-count': isAnimating }">
                    {{ opened || 0 }}
                </span>
                <span class="min-w-10 absolute -top-1.5 left-3 inline-block">
                    <span
                        v-if="isAnimating && difference > 0"
                        class="animate-pulse text-green-400 text-sm font-bold inline-block"
                    >
                        +{{ difference }}
                    </span>
                </span>
                <div class="text-xs sm:text-base">Opened globally</div>
            </div>
        </div>

        <template #fallback>
            <div class="flex items-center">
                <span>0 total crates opened</span>
            </div>
        </template>
    </ClientOnly>
</template>

<style scoped>
.animate-count {
    display: inline-block;
    animation: highlight 1.5s ease-in-out;
}

@keyframes highlight {
    0% {
        color: #fff;
        transform: scale(1);
    }
    30% {
        color: #05df72;
        transform: scale(1.1);
    }
    100% {
        color: #fff;
        transform: scale(1);
    }
}

.animate-pulse {
    animation: pulse 1.5s cubic-bezier(0.4, 0, 0.6, 1);
}

@keyframes pulse {
    0%,
    100% {
        opacity: 1;
    }
    50% {
        opacity: 0.7;
    }
}
</style>
