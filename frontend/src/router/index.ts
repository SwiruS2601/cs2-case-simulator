import { createRouter, createWebHistory } from 'vue-router';
import CasesView from '@/views/Cases.vue';
import HomeView from '@/views/Home.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/case',
      name: 'case',
      component: CasesView,
    },
    {
      path: '/lazy',
      name: 'lazy',
      component: () => import('../views/TestLazy.vue'),
    },
  ],
});

export default router;
