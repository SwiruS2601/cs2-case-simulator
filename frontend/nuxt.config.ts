import tailwindcss from '@tailwindcss/vite';
import { serverConfig } from './config.server';
import type { Crate } from './types';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  css: ['~/assets/main.css'],
  vite: {
    plugins: [tailwindcss()],
  },
  modules: ['@pinia/nuxt', 'pinia-plugin-persistedstate/nuxt'],
  piniaPluginPersistedstate: {
    storage: 'localStorage',
  },
  ssr: true,
  nitro: {
    preset: 'bun',
    prerender: {
      routes: ['/', '/inventory', '/stickers', '/autographs', '/souvenirs'],
      ignore: ['/api'],
    },
  },
  routeRules: {
    '/': { prerender: true },
    '/stickers': { prerender: true },
    '/souvenirs': { prerender: true },
    '/inventory': { prerender: true },
    '/autographs': { prerender: true },
    '/crate/**': { prerender: true },
  },
  hooks: {
    async 'prerender:routes'(ctx) {
      const res = await fetch(serverConfig.apiUrl + '/api/crates');
      const data = (await res.json()) as Crate[];
      data.forEach((crate) => {
        ctx.routes.add(`/crate/${encodeURIComponent(crate.name)}`);
      });
    },
  },
});
