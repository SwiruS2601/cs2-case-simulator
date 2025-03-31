<template>
    <div
        ref="adContainer"
        :class="[
            'flex items-center justify-center overflow-hidden sm:rounded mx-auto',
            'md:border-black/15 md:rounded-lg',
            { 'fixed left-0 right-0 z-10': isFixed },
            {
                'h-[90px] sm:max-w-[728px] w-full': size === 'banner',
                'h-[250px] w-[250px]': size === 'square',
                'h-[50px] sm:max-w-[320px] w-full': size === 'leaderboard' || size === 'mobile',
            },
        ]"
        :style="customStyle"
    >
        <ClientOnly>
            <ins
                v-if="isVisible"
                class="adsbygoogle"
                :style="adStyles"
                :data-ad-client="adClient"
                :data-ad-slot="adSlot"
                :data-ad-format="adFormat"
                :data-full-width-responsive="isResponsive ? 'true' : 'false'"
            ></ins>
        </ClientOnly>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';

interface Props {
    adSlot: string;
    adFormat?: string;
    adClient?: string;
    size?: 'banner' | 'square' | 'leaderboard' | 'mobile';
    isResponsive?: boolean;
    isFixed?: boolean;
    customStyle?: Record<string, string>;
}

const props = withDefaults(defineProps<Props>(), {
    adFormat: 'auto',
    adClient: 'ca-pub-2203615625330226',
    size: 'banner',
    isResponsive: true,
    isFixed: false,
    customStyle: () => ({}),
});

const { $adsense } = useNuxtApp();
const adContainer = ref<HTMLElement | null>(null);
const adInitialized = ref(false);
const isVisible = ref(false);

// Calculate ad dimensions based on size
const adStyles = computed(() => {
    const baseStyle = { display: 'block' };

    switch (props.size) {
        case 'banner':
            return { ...baseStyle, width: '728px', height: '90px' };
        case 'square':
            return { ...baseStyle, width: '250px', height: '250px' };
        case 'leaderboard':
        case 'mobile':
            return { ...baseStyle, width: '320px', height: '50px' };
        default:
            return baseStyle;
    }
});

// Use intersection observer to lazy load ads
onMounted(() => {
    if (!adContainer.value) return;

    const observer = new IntersectionObserver(
        (entries) => {
            const [entry] = entries;
            isVisible.value = entry.isIntersecting;
        },
        {
            threshold: 0,
            rootMargin: '200px', // Load ads when they're 200px from viewport
        },
    );

    observer.observe(adContainer.value);

    // Cleanup observer on component unmount
    onUnmounted(() => {
        observer.disconnect();
    });
});

// When ad becomes visible, initialize it
watch(isVisible, (visible) => {
    if (visible && !adInitialized.value) {
        $adsense?.refresh();
        adInitialized.value = true;
    }
});
</script>

<style scoped>
.adsbygoogle {
    display: block;
    text-align: center;
}
</style>
