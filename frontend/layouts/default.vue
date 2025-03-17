<script setup lang="ts">
import { BACKGROUNDS } from '~/constants';
import { useLayoutSeo } from '~/services/metaSeoService';
import Image from '~/components/Image.vue';

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
    <div class="fixed inset-0 object-cover size-full z-0 backdrop-blur-xs" ></div>
    <Nav ></Nav>
    <slot ></slot>
  </main>
</template>
