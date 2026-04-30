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