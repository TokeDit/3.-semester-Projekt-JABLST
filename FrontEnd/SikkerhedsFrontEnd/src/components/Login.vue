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
    router.push('/dashboard');
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
    await createUserWithEmailAndPassword(auth, email.value, password.value);
    message.value = 'Bruger oprettet. Du bliver sendt til dashboard.';
    router.push('/dashboard');
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
  height: 100vh;
  background: #eff2f7;
}

.login-card {
  background: #246BCE;
  padding: 2.5rem;
  border-radius: 16px;
  width: 100%;
  max-width: 350px;
  text-align: center;
  color: white;
}

.login-logo {
  width: 70px;
  margin-bottom: 1rem;
}

.input-group {
  text-align: left;
  margin-bottom: 1.2rem;
}

.input-group label {
  display: block;
  margin-bottom: 0.5rem;
}

input {
  width: 100%;
  padding: 0.8rem;
  border-radius: 8px;
  border: none;
  color: #333;
}

.login-btn,
.register-btn {
  width: 100%;
  padding: 0.8rem;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: bold;
}

.login-btn {
  background: #1976d2;
}

.register-btn {
  margin-top: 0.75rem;
  background: #16a34a;
}

.error-msg {
  color: #ff8a8a;
  margin-top: 1rem;
  font-weight: bold;
}

.success-msg {
  color: #bbf7d0;
  margin-top: 1rem;
  font-weight: bold;
}
</style>