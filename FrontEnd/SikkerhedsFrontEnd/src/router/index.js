import { createRouter, createWebHistory } from 'vue-router'
import Login from '../components/Login.vue'
import Dashboard from '../components/Dashboard.vue'
import { auth } from '../firebase'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }  // protected route
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard — redirect to login if not authenticated
router.beforeEach((to, from, next) => {
  const user = auth.currentUser
  if (to.meta.requiresAuth && !user) {
    next('/login')
  } else {
    next()
  }
})

export default router