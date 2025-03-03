<script setup lang="ts">
import 'assets/main.css';
import { clientConfig } from '~/config.client';
import { BACKGROUNDS } from '~/constants';

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

const title = 'Counter-Strike 2 Case Simulator';
const description = 'Open Counter-Strike 2 cases for free in this case unboxing simulator.';

useSeoMeta({
  viewport: 'width=device-width, initial-scale=1.0',
  title: title,
  ogTitle: title,
  description: description,
  keywords: 'counter strike 2, cs2, case, opening, unboxing, simulator, skins',
  themeColor: '#1a1c20',
  ogDescription: description,
  ogImage: 'https://case.oki.gg/preview.webp',
  twitterCard: 'summary_large_image',
  twitterTitle: title,
  twitterDescription: description,
  ogSiteName: title,
  ogUrl: 'https://case.oki.gg',
  ogType: 'website',
  author: 'Oki',
});
</script>

<template>
  <main class="min-h-[calc(100vh+0.5px)]">
    <ClientOnly>
      <Image
        :src="`${clientConfig.baseUrl}/backgrounds/${BACKGROUNDS[Math.floor(Math.random() * BACKGROUNDS.length)]}`"
        class="fixed inset-0 object-cover size-full -z-10"
        alt="CS2 background map"
        fetchpriority="high"
        :width="1280"
        :height="720"
      />
    </ClientOnly>
    <div class="fixed inset-0 object-cover size-full z-0 backdrop-blur-xs" />
    <Nav />
    <slot />
  </main>
</template>

<style>
html,
dialog {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans',
    'Helvetica Neue', sans-serif;
  color: white;
}

body {
  background: #1a1c20;
}

img {
  display: block;
  max-width: 100%;
  user-select: none;
}

.responsive-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(125px, 1fr));
}

@media screen and (max-width: 720px) {
  .responsive-grid {
    grid-template-columns: repeat(auto-fit, minmax(90px, 1fr));
  }
}
</style>
