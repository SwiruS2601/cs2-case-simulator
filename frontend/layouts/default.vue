<script setup lang="ts">
import { BACKGROUNDS } from '~/constants';
import { useLayoutSeo } from '~/services/metaSeoService';
import Image from '~/components/Image.vue';
import GoogleAd from '~/components/GoogleAd.vue';

useHead({
    htmlAttrs: {
        lang: 'en',
    },
    meta: [{ name: 'google-adsense-account', content: 'ca-pub-2203615625330226' }],
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

        <div class="desktop-layout">
            <div class="side-ad left-side">
                <GoogleAd size="skyscraper" :hide-during-opening="false" adSlot="left-skyscraper"></GoogleAd>
            </div>

            <div class="main-content">
                <slot></slot>
            </div>

            <div class="side-ad right-side">
                <GoogleAd size="skyscraper" :hide-during-opening="false" adSlot="right-skyscraper"></GoogleAd>
            </div>
        </div>

        <div class="fixed-bottom-ad">
            <GoogleAd size="mobile" :hide-during-opening="false" adSlot="fixed-bottom"></GoogleAd>
        </div>
    </main>
</template>

<style>
body {
    padding-bottom: 60px;
}

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

.side-ad {
    display: none;
    width: 160px;
    height: 600px;
    position: fixed;
    top: 50%;
    transform: translateY(-50%);
    z-index: 9999;
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
        border-radius: 8px;
    }
}

.main-content {
    width: 100%;
    max-width: 1280px;
}

@media (max-width: 768px) {
    .main-content {
        padding: 0;
    }
}

.fixed-bottom-ad {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    z-index: 99999;
}
</style>
