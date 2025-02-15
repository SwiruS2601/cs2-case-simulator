import { createRouter, createWebHistory } from 'vue-router';
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
      path: '/crate/:id',
      name: 'crate',
      component: () => import('@/views/Crate.vue'),
    },
  ],
});

export default router;
