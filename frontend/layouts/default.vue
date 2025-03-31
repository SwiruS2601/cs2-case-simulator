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
                <!-- Top Banner Ad -->
                <div v-if="showTopAd" class="ad-section">
                    <AdPlaceholder
                        :size="topAdSize"
                        @toggle="toggleAd('top')"
                        @change-size="(size) => changeAdSize('top', size)"
                    ></AdPlaceholder>
                </div>

                <slot></slot>

                <!-- Bottom Banner Ad -->
                <AdPlaceholder
                    v-if="showBottomAd"
                    :size="bottomAdSize"
                    @toggle="toggleAd('bottom')"
                    @change-size="(size) => changeAdSize('bottom', size)"
                ></AdPlaceholder>
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

/* Full-width container behind the top ad */
.ad-section {
    display: none; /* Hidden by default on desktop */
    width: 100%;
    background-color: transparent;
    margin: 0;
}

@media (max-width: 768px) {
    .ad-section {
        display: block;
        background-color: rgb(0 0 0 / 0.4);
        backdrop-filter: blur(4px);
        padding: 8px 0;
        margin: 0;
        width: 100vw;
        position: relative;
        left: 50%;
        right: 50%;
        margin-left: -50vw;
        margin-right: -50vw;
    }
}

/* Desktop Layout Styles */
.desktop-layout {
    display: flex;
    justify-content: center;
    align-items: flex-start;
    gap: 2rem;
    max-width: 1920px;
    margin: 0 auto;
    padding: 0;
}

@media (min-width: 769px) {
    .desktop-layout {
        padding: 0 1rem;
    }
}

.main-content {
    flex: 1;
    min-width: 0; /* Prevent flex item from overflowing */
    max-width: 1280px; /* Match your current max-width */
}

/* Side Ad Styles */
.side-ad {
    display: none; /* Hidden by default on mobile */
    width: 160px; /* Wide skyscraper width */
    position: sticky;
    align-self: center; /* Center vertically */
    margin-top: auto;
    margin-bottom: auto;
    height: 600px; /* Standard skyscraper height */
}

@media (min-width: 1600px) {
    .side-ad {
        display: flex;
        justify-content: center;
        align-items: center;
        background-color: rgb(0 0 0 / 0.4); /* Match the mobile ad background */
        backdrop-filter: blur(4px);
        border-radius: 8px;
        padding: 0.5rem;
    }

    .desktop-layout {
        padding: 0 2rem;
    }
}

/* Ensure the slot content doesn't have unwanted margins */
main > :not(.ad-section) {
    margin-top: 0;
}
</style>
