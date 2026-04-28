// src/firebase.js

import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";

// Firebase config (fra din Firebase Console)
const firebaseConfig = {
  apiKey: "AIzaSyAhfN8fZnZnT8NZKjxSp3MdUO06UqLXM8U",
  authDomain: "security-system-login.firebaseapp.com",
  projectId: "security-system-login",
  storageBucket: "security-system-login.firebasestorage.app",
  messagingSenderId: "5693376443",
  appId: "1:5693376443:web:761dda0c7f0b46ef61269e"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

// Initialize Auth
export const auth = getAuth(app);

// (valgfrit) eksport app hvis du senere skal bruge det
export default app;