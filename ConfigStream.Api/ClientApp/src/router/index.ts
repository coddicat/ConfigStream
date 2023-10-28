import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/default/default-layout.vue'),
    redirect: '/configs',
    children: [
      {
        path: '/configs',
        name: 'Configs',
        component: () =>
          import(
            /* webpackChunkName: "configs-view" */ '@/views/config/configs-view.vue'
          )
      },
      {
        path: '/values',
        name: 'Values',
        component: () =>
          import(
            /* webpackChunkName: "values-view" */ '@/views/value/values-view.vue'
          )
      },
      {
        path: '/targets',
        name: 'Targets',
        component: () =>
          import(
            /* webpackChunkName: "targets-view" */ '@/views/target/targets-view.vue'
          )
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

export default router;
