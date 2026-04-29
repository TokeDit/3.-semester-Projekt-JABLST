<template>
  <div class="login-container">
    <div class="login-card">
      <img src="/logo.png" alt="Logo" class="login-logo" />
      <h2>System Login</h2>
      <form @submit.prevent="handleLogin">
        <div class="input-group">
          <label>Email</label>
          <input type="email" v-model="email" placeholder="Enter email" required />
        </div>
        <div class="input-group">
          <label>Password</label>
          <input type="password" v-model="password" placeholder="Enter password" required />
        </div>
        <button type="submit" class="login-btn">Login</button>
      </form>
      <p v-if="error" class="error-msg">{{ error }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';        // ADD this
import { auth } from '../firebase'; 
import { signInWithEmailAndPassword } from 'firebase/auth';

const router = useRouter();                     // ADD this
const email = ref('');
const password = ref('');
const error = ref('');

async function handleLogin() {
  error.value = ""; 
  try {
    await signInWithEmailAndPassword(auth, email.value, password.value);
    router.push('/dashboard');                  // CHANGE this (was emit)
  } catch (err) {
    if (err.code === 'auth/invalid-credential') {
      error.value = "Invalid email or password.";
    } else {
      error.value = "Error: " + err.message;
    }
  }
}
</script>

<style scoped>
.login-container { display: flex; justify-content: center; align-items: center; height: 100vh; background: #eff2f7; }
.login-card { background: #246BCE; padding: 2.5rem; border-radius: 16px; width: 100%; max-width: 350px; text-align: center; color: white; }
.login-logo { width: 70px; margin-bottom: 1rem; }
.input-group { text-align: left; margin-bottom: 1.2rem; }
.input-group label { display: block; margin-bottom: 0.5rem; }
input { width: 100%; padding: 0.8rem; border-radius: 8px; border: none; color: #333; }
.login-btn { width: 100%; padding: 0.8rem; background: #1976d2; color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: bold; }
.error-msg { color: #ff8a8a; margin-top: 1rem; font-weight: bold; }
</style>