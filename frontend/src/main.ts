import './assets/main.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router/index.ts';
import { VueQueryPlugin } from '@tanstack/vue-query';

const app = createApp(App);

VueQueryPlugin.install(app, {
  queryClientConfig: {
    defaultOptions: {
      queries: {
        staleTime: 1000 * 60 * 15,
      },
    },
  },
});

app.use(createPinia());
app.use(router);

app.mount('#app');
