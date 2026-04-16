import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    component: () => import('../layouts/MainLayout.vue'),
    children: [
      {
        path: '',
        name: 'history',
        component: () => import('../views/HistoryView.vue')
      },
      {
        path: 'calendar',
        name: 'calendar',
        component: () => import('../views/CalendarView.vue')
      },
      {
        path: 'categories',
        name: 'categories',
        component: () => import('../views/CategoriesView.vue')
      },
      {
        path: 'articles',
        name: 'articles',
        component: () => import('../views/ArticlesView.vue')
      },
      {
        path: 'transaction/add',
        name: 'transaction-add',
        component: () => import('../views/TransactionFormView.vue')
      },
      {
        path: 'transaction/edit/:id',
        name: 'transaction-edit',
        component: () => import('../views/TransactionFormView.vue')
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;