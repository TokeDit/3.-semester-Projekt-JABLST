<script setup>
import { ref } from "vue";
import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signOut,
  onAuthStateChanged
} from "firebase/auth";
import { auth } from "./firebase";

const email = ref("");
const password = ref("");
const message = ref("");
const currentUser = ref(null);
const token = ref("");

onAuthStateChanged(auth, async (user) => {
  currentUser.value = user;

  if (user) {
    token.value = await user.getIdToken();
    localStorage.setItem("firebaseToken", token.value);
  } else {
    token.value = "";
    localStorage.removeItem("firebaseToken");
  }
});

async function register() {
  message.value = "";

  try {
    const result = await createUserWithEmailAndPassword(
      auth,
      email.value,
      password.value
    );

    token.value = await result.user.getIdToken();
    localStorage.setItem("firebaseToken", token.value);

    message.value = "Bruger oprettet og logget ind.";
  } catch (error) {
    message.value = error.message;
  }
}

async function login() {
  message.value = "";

  try {
    const result = await signInWithEmailAndPassword(
      auth,
      email.value,
      password.value
    );

    token.value = await result.user.getIdToken();
    localStorage.setItem("firebaseToken", token.value);

    message.value = "Login successfuldt.";
  } catch (error) {
    message.value = error.message;
  }
}

async function logout() {
  await signOut(auth);
  message.value = "Du er logget ud.";
}
</script>

<template>
  <main class="page">
    <section class="card">
      <h1>Security System Login</h1>

      <div v-if="!currentUser">
        <input v-model="email" type="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />

        <div class="buttons">
          <button @click="register">Opret bruger</button>
          <button @click="login">Login</button>
        </div>
      </div>

      <div v-else>
        <p class="success">Du er logget ind som:</p>
        <strong>{{ currentUser.email }}</strong>

        <button class="logout" @click="logout">Logout</button>
      </div>

      <p v-if="message" class="message">{{ message }}</p>

      <div v-if="token" class="token-box">
        <h3>Firebase Token</h3>
        <textarea readonly :value="token"></textarea>
      </div>
    </section>
  </main>
</template>

<style scoped>
.page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  background: #f4f6f8;
  font-family: Arial, sans-serif;
}

.card {
  width: 100%;
  max-width: 430px;
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
}

h1 {
  margin-bottom: 24px;
  color: #1f2937;
}

input {
  width: 100%;
  padding: 12px;
  margin-bottom: 12px;
  border: 1px solid #ccc;
  border-radius: 8px;
}

.buttons {
  display: flex;
  gap: 10px;
}

button {
  flex: 1;
  padding: 10px;
  border: none;
  border-radius: 8px;
  background: #2563eb;
  color: white;
  cursor: pointer;
}

button:hover {
  background: #1d4ed8;
}

.logout {
  width: 100%;
  margin-top: 20px;
  background: #dc2626;
}

.logout:hover {
  background: #b91c1c;
}

.message {
  margin-top: 16px;
}

.success {
  color: #16a34a;
}

.token-box {
  margin-top: 20px;
}

textarea {
  width: 100%;
  height: 120px;
  margin-top: 8px;
  font-size: 12px;
}
</style>