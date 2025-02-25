import { createRouter, createWebHistory, type RouteLocationNormalized, type RouteRecordRaw } from 'vue-router';
import HomeView from './views/HomeView.vue';

type MetaTag = {
  [key: string]: string;
};

type RouteMeta = {
  title: string;
  metaTags: MetaTag[];
};

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
    meta: {
      title: 'Counter-Strike 2 Case Simulator',
      metaTags: [
        {
          name: 'description',
          content: 'Open Counter-Strike 2 cases for free in this case unboxing simulator.',
        },
        {
          property: 'og:title',
          content: 'Counter-Strike 2 Case Simulator',
        },
      ],
    },
  },
  {
    path: '/inventory',
    name: 'inventory',
    component: () => import('./views/InventoryView.vue'),
    meta: {
      title: 'Your Inventory - CS2 Case Simulator',
      metaTags: [
        {
          name: 'description',
          content: 'View your unboxed CS2 skins in your personal inventory.',
        },
        {
          property: 'og:title',
          content: 'Your Inventory - CS2 Case Simulator',
        },
      ],
    },
  },
  {
    path: '/crate/:id',
    name: 'crate',
    component: () => import('./views/CrateView.vue'),
    props: true,
    beforeEnter: (to: RouteLocationNormalized) => {
      const slug = to.params.id as string;
      const name = decodeURIComponent(slug);
      to.meta = {
        title: `Open ${name} Case - CS2 Case Simulator`,
        metaTags: [
          {
            name: 'description',
            content: `Open the ${name} case and unbox a skin in this CS2 case simulator.`,
          },
          {
            property: 'og:title',
            content: `Open ${name} Case - CS2 Case Simulator`,
          },
        ],
      } as RouteMeta;
    },
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  },
});

router.beforeEach((to, from, next) => {
  // @ts-ignore
  document.title = to.meta.title || 'Counter-Strike 2 Case Simulator';

  const metaTags = document.querySelectorAll('meta[data-vue-router-controlled]');
  metaTags.forEach((tag) => tag.remove());

  if (to.meta.metaTags) {
    // @ts-ignore
    to.meta.metaTags.forEach((tagDef) => {
      const tag = document.createElement('meta');
      Object.keys(tagDef).forEach((key) => {
        tag.setAttribute(key, tagDef[key]);
      });
      tag.setAttribute('data-vue-router-controlled', '');
      document.head.appendChild(tag);
    });
  }

  next();
});

export default router;
