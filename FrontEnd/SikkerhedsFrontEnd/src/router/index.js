import { createRouter, createWebHistory } from 'vue-router'
import Login from '../components/Login.vue'
import Dashboard from '../components/Dashboard.vue'
import Home from '../components/Home.vue'
import { auth } from '../firebase'
import { onAuthStateChanged } from "firebase/auth";

let isAuthResolved = false;

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { path: '/home', component: Home },
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
  if (!isAuthResolved) {
    onAuthStateChanged(auth, (user) => {
      isAuthResolved = true;

      if (to.meta.requiresAuth && !user) {
        next('/login');
      } else {
        next();
      }
    });
  } else {
    const user = auth.currentUser;

    if (to.meta.requiresAuth && !user) {
      next('/login');
    } else {
      next();
    }
  }
});

export default router