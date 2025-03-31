<script setup lang="ts">
import { BACKGROUNDS } from '~/constants';
import { useLayoutSeo } from '~/services/metaSeoService';
import Image from '~/components/Image.vue';
import AdPlaceholder from '~/components/AdPlaceholder.vue';
import { ref } from 'vue';

// For testing different ad positions
const showTopAd = ref(true);
const showBottomAd = ref(true);
const showFixedBottomAd = ref(true);

// For testing different ad sizes
const topAdSize = ref('leaderboard');
const bottomAdSize = ref('leaderboard');
const fixedBottomAdSize = ref('mobile');

// Add new refs for side ads
const showLeftSideAd = ref(true);
const showRightSideAd = ref(true);
const leftSideAdSize = ref('skyscraper');
const rightSideAdSize = ref('skyscraper');

const toggleAd = (position: 'top' | 'bottom' | 'fixedBottom' | 'leftSide' | 'rightSide') => {
    if (position === 'top') showTopAd.value = !showTopAd.value;
    if (position === 'bottom') showBottomAd.value = !showBottomAd.value;
    if (position === 'fixedBottom') showFixedBottomAd.value = !showFixedBottomAd.value;
    if (position === 'leftSide') showLeftSideAd.value = !showLeftSideAd.value;
    if (position === 'rightSide') showRightSideAd.value = !showRightSideAd.value;
};

const changeAdSize = (position: 'top' | 'bottom' | 'fixedBottom' | 'leftSide' | 'rightSide', size: string) => {
    if (position === 'top') topAdSize.value = size;
    if (position === 'bottom') bottomAdSize.value = size;
    if (position === 'fixedBottom') fixedBottomAdSize.value = size;
    if (position === 'leftSide') leftSideAdSize.value = size;
    if (position === 'rightSide') rightSideAdSize.value = size;
};

useHead({
    htmlAttrs: {
        lang: 'en',
    },
    link: [
        { rel: 'icon', href: '/favicon.ico', sizes: 'any' },
        { rel: 'icon', type: 'image/png', sizes: '16x16', href: '/favicon-16x16.png' },
        { rel: 'icon', type: 'image/png', sizes: '32x32', href: '/favicon-32x32.png' },
        { rel: 'apple-touch-icon', sizes: '180x180', href: '/apple-touch-icon.png' },
        { rel: 'manifest', href: '/site.webmanifest' },
    ],
});

useLayoutSeo();
</script>

<template>
    <main class="min-h-[calc(100vh+0.5px)]">
        <ClientOnly>
            <Image
                :src="`${useRuntimeConfig().public.baseUrl}backgrounds/${
                    BACKGROUNDS[Math.floor(Math.random() * BACKGROUNDS.length)]
                }`"
                class="fixed inset-0 object-cover size-full -z-10"
                alt="CS2 background map"
                fetchpriority="high"
                :width="1280"
                :height="720"
            ></Image>
        </ClientOnly>
        <div class="fixed inset-0 object-cover size-full z-0 backdrop-blur-xs"></div>
        <Nav></Nav>

        <!-- Desktop Layout with Side Ads -->
        <div class="desktop-layout">
            <!-- Left Side Ad (Desktop Only) -->
            <div v-if="showLeftSideAd" class="side-ad left-side">
                <AdPlaceholder
                    :size="leftSideAdSize"
                    @toggle="toggleAd('leftSide')"
                    @change-size="(size) => changeAdSize('leftSide', size)"
                ></AdPlaceholder>
            </div>

            <!-- Main Content Area -->
            <div class="main-content">
                <slot></slot>
            </div>

            <!-- Right Side Ad (Desktop Only) -->
            <div v-if="showRightSideAd" class="side-ad right-side">
                <AdPlaceholder
                    :size="rightSideAdSize"
                    @toggle="toggleAd('rightSide')"
                    @change-size="(size) => changeAdSize('rightSide', size)"
                ></AdPlaceholder>
            </div>
        </div>

        <!-- Fixed Bottom Ad -->
        <AdPlaceholder
            v-if="showFixedBottomAd"
            :size="fixedBottomAdSize"
            is-fixed
            :custom-style="{ bottom: '0', padding: '5px' }"
            @toggle="toggleAd('fixedBottom')"
            @change-size="(size) => changeAdSize('fixedBottom', size)"
        ></AdPlaceholder>
    </main>
</template>

<style>
/* Add some padding at the bottom to prevent fixed ad from covering content */
body {
    padding-bottom: 60px;
}

/* Desktop Layout Styles */
.desktop-layout {
    display: flex;
    justify-content: center;
    align-items: flex-start;
    max-width: 1280px;
    margin: 0 auto;
    padding: 0 1rem;
    position: relative;
}

@media (max-width: 768px) {
    .desktop-layout {
        padding: 0;
    }
}

/* Side Ad Styles */
.side-ad {
    display: none;
    width: 160px;
    height: 600px;
    position: fixed;
    top: 50%;
    transform: translateY(-50%);
    z-index: 10;
}

.left-side {
    right: calc(50% + 640px + 1rem);
}

.right-side {
    left: calc(50% + 640px + 1rem);
}

@media (min-width: 1600px) {
    .side-ad {
        display: flex;
        justify-content: center;
        align-items: center;
        background-color: rgb(0 0 0 / 0.4);
        backdrop-filter: blur(4px);
        border-radius: 8px;
        padding: 0.5rem;
    }
}

/* Main content styles */
.main-content {
    width: 100%;
    max-width: 1280px;
}

@media (max-width: 768px) {
    .main-content {
        padding: 0;
    }
}
</style>
