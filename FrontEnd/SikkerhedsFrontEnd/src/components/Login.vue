<template>
  <div class="login-container">
    <div class="login-card">

      <div class="brand">
        <div class="brand-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z"/>
            <circle cx="12" cy="13" r="4"/>
          </svg>
        </div>
        <span class="brand-name"><span class="brand-accent">Vision</span> Monitor</span>
      </div>

      <h2>System Login</h2>
      <p class="login-sub">Sign in to access the dashboard</p>

      <form @submit.prevent="handleLogin">
        <div class="input-group">
          <label>Email</label>
          <input
            id="email-input"
            type="email"
            v-model="email"
            placeholder="Enter email"
          />
        </div>

        <div class="input-group">
          <label>Password</label>
          <input
            id="password-input"
            type="password"
            v-model="password"
            placeholder="Enter password"
          />
        </div>

        <button id="login-button" class="login-btn" type="submit">Login</button>

        <button
          id="register-button"
          class="register-btn"
          type="button"
          @click="handleRegister"
        >
          Create account
        </button>
      </form>

      <p id="error-message" v-if="error" class="error-msg">{{ error }}</p>
      <p id="success-message" v-if="message" class="success-msg">{{ message }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { auth } from '../firebase';
import {
  signInWithEmailAndPassword,
  createUserWithEmailAndPassword
} from 'firebase/auth';

const router = useRouter();

const email = ref('');
const password = ref('');
const error = ref('');
const message = ref('');

function isValidEmail(value) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
}

function validateLoginForm() {
  if (!email.value.trim()) {
    error.value = 'Email må ikke være tom.';
    return false;
  }

  if (!isValidEmail(email.value)) {
    error.value = 'Indtast en gyldig email adresse.';
    return false;
  }

  if (!password.value) {
    error.value = 'Password må ikke være tomt.';
    return false;
  }

  return true;
}

function validateRegisterForm() {
  if (!validateLoginForm()) {
    return false;
  }

  if (password.value.length < 6) {
    error.value = 'Password skal være mindst 6 tegn.';
    return false;
  }

  return true;
}

function mapFirebaseError(code) {
  switch (code) {
    case 'auth/invalid-email':
      return 'Ugyldig email adresse.';
    case 'auth/email-already-in-use':
      return 'Email er allerede i brug.';
    case 'auth/weak-password':
      return 'Password skal være mindst 6 tegn.';
    case 'auth/invalid-credential':
      return 'Forkert email eller password.';
    case 'auth/missing-password':
      return 'Password mangler.';
    default:
      return 'Noget gik galt. Prøv igen.';
  }
}

async function handleLogin() {
  error.value = '';
  message.value = '';

  if (!validateLoginForm()) {
    return;
  }

  try {
    await signInWithEmailAndPassword(auth, email.value, password.value);
    router.push('/home');
  } catch (err) {
    error.value = mapFirebaseError(err.code);
  }
}

async function handleRegister() {
  error.value = '';
  message.value = '';

  if (!validateRegisterForm()) {
    return;
  }

  try {
    const userCredential = await createUserWithEmailAndPassword(auth, email.value, password.value);
    const idToken = await userCredential.user.getIdToken();
    await fetch('https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/api/Auth/me', {
      headers: { Authorization: `Bearer ${idToken}` }
    });
    message.value = 'Bruger oprettet. Du bliver sendt til dashboard.';
    router.push('/home');
  } catch (err) {
    error.value = mapFirebaseError(err.code);
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: #0b1120;
  font-family: 'IBM Plex Sans', 'Segoe UI', sans-serif;
  font-size: 14px;
  color: #f1f5f9;
}

.login-card {
  background: #111827;
  border: 1px solid rgba(255, 255, 255, 0.07);
  border-radius: 20px;
  padding: 2.5rem 2rem;
  width: 100%;
  max-width: 380px;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

/* Brand — matches sidebar brand in Home.vue */
.brand {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin-bottom: 1rem;
}

.brand-icon {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #1a3460, #2563eb);
  border-radius: 8px;
  display: grid;
  place-items: center;
  color: #93c5fd;
  flex-shrink: 0;
  box-shadow: 0 0 12px rgba(37, 99, 235, 0.3);
}
.brand-icon svg { width: 15px; height: 15px; }

.brand-name {
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 0.95rem;
  font-weight: 700;
  color: #f1f5f9;
}
.brand-accent { color: #3b82f6; }

h2 {
  margin: 0;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 1.4rem;
  font-weight: 800;
  letter-spacing: -0.03em;
}

.login-sub {
  margin: 0 0 1rem;
  color: #94a3b8;
  font-size: 0.8rem;
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  margin-bottom: 0.875rem;
}

.input-group label {
  font-size: 0.72rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #94a3b8;
}

input {
  width: 100%;
  padding: 0.65rem 0.8rem;
  background: #182032;
  border: 1px solid rgba(255, 255, 255, 0.07);
  border-radius: 12px;
  color: #f1f5f9;
  font-size: 0.85rem;
  font-family: inherit;
  outline: none;
  box-sizing: border-box;
  transition: border-color 0.12s;
}
input::placeholder { color: #4b5e77; }
input:focus { border-color: rgba(59, 130, 246, 0.5); }

.login-btn,
.register-btn {
  width: 100%;
  padding: 0.65rem 1rem;
  border: none;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 600;
  font-family: inherit;
  cursor: pointer;
  transition: opacity 0.12s;
  color: white;
}
.login-btn:hover,
.register-btn:hover { opacity: 0.85; }

.login-btn {
  background: #3b82f6;
  margin-top: 0.25rem;
}

.register-btn {
  margin-top: 0.5rem;
  background: #182032;
  border: 1px solid rgba(255, 255, 255, 0.07);
  color: #94a3b8;
}
.register-btn:hover { color: #f1f5f9; border-color: rgba(255, 255, 255, 0.13); }

.error-msg {
  margin: 0.5rem 0 0;
  color: #f87171;
  font-size: 0.8rem;
  font-weight: 600;
}

.success-msg {
  margin: 0.5rem 0 0;
  color: #4ade80;
  font-size: 0.8rem;
  font-weight: 600;
}
</style>