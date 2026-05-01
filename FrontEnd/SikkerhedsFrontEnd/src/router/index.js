import { createRouter, createWebHistory } from 'vue-router'
import Login from '../components/Login.vue'
import Dashboard from '../components/Dashboard.vue'
import Home from '../components/Home.vue'
import { auth } from '../firebase'
import { onAuthStateChanged } from "firebase/auth";

let isAuthResolved = false;

function getCurrentUser() {
  return new Promise((resolve) => {
    const unsubscribe = onAuthStateChanged(auth, (user) => {
      unsubscribe()
      resolve(user)
    })
  })
}
const routes = [
  { path: '/', redirect: '/home' },

  {
    path: '/login',
    component: Login,
    meta: { guestOnly: true }
  },

  {
    path: '/home',
    component: Home,
    meta: { requiresAuth: true }
  },

  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard — redirect to login if not authenticated
router.beforeEach(async (to) => {
  const user = await getCurrentUser()

  if (to.meta.requiresAuth && !user) {
    return '/login'
  }

  if (to.meta.guestOnly && user) {
    return '/home'
  }

  return true
})

export default router