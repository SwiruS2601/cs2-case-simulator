import tailwindcss from '@tailwindcss/vite';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
    compatibilityDate: '2024-11-01',
    devtools: { enabled: true },
    css: ['~/assets/css/main.css'],
    vite: {
        plugins: [tailwindcss()],
    },
    modules: [
        '@pinia/nuxt',
        '@pinia/colada-nuxt',
        'pinia-plugin-persistedstate/nuxt',
        '@nuxtjs/sitemap',
        '@nuxt/test-utils/module',
    ],
    piniaPluginPersistedstate: {
        storage: 'localStorage',
    },
    site: {
        url: 'https://case.oki.gg',
        name: 'CS2 Case Simulator',
    },
    sitemap: {
        sources: ['/api/__sitemap__/urls'],
        autoLastmod: true,
        xsl: '/sitemap.xsl',
    },
    routeRules: {
        '/': { swr: 86400 },
        '/stickers': { swr: 86400 },
        '/souvenirs': { swr: 86400 },
        '/autographs': { swr: 86400 },
        '/crate/**': { swr: 86400 },
        '/inventory': { prerender: true },
        '/case/**': { redirect: '/' },
    },
    runtimeConfig: {
        public: {
            imageUrl: 'https://images.oki.gg/',
            baseUrl: 'https://case.oki.gg/',
        },
    },
    $development: {
        runtimeConfig: {
            public: {
                apiUrl: 'http://localhost:5015',
            },
        },
    },
    $production: {
        runtimeConfig: {
            public: {
                apiUrl: 'https://caseapi.oki.gg',
            },
        },
    },
});
