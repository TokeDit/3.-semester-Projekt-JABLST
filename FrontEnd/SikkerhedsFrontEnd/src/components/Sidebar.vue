<template>
  <aside class="sidebar">
    <div class="brand">
      <div class="brand-icon">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z" />
          <circle cx="12" cy="13" r="4" />
        </svg>
      </div>
      <span class="brand-name"><span class="brand-accent">Vision</span> Monitor</span>
    </div>

    <nav class="nav-main">
      <button class="nav-item" :class="{ active: activePage === 'dashboard' }" @click="goToDashboard">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <rect x="3" y="3" width="7" height="7" rx="1" />
          <rect x="14" y="3" width="7" height="7" rx="1" />
          <rect x="14" y="14" width="7" height="7" rx="1" />
          <rect x="3" y="14" width="7" height="7" rx="1" />
        </svg>
        <span>Dashboard</span>
      </button>
    </nav>

    <div class="nav-group">
      <p class="nav-group-label">Monitoring</p>
      <button class="nav-item">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <rect x="2" y="3" width="20" height="14" rx="2" />
          <line x1="8" y1="21" x2="16" y2="21" />
          <line x1="12" y1="17" x2="12" y2="21" />
        </svg>
        <span>Live Feed</span>
      </button>
      <button class="nav-item">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z" />
          <polyline points="13 2 13 9 20 9" />
        </svg>
        <span>Events</span>
      </button>
      <button class="nav-item">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="8" y1="6" x2="21" y2="6" />
          <line x1="8" y1="12" x2="21" y2="12" />
          <line x1="8" y1="18" x2="21" y2="18" />
          <line x1="3" y1="6" x2="3.01" y2="6" />
          <line x1="3" y1="12" x2="3.01" y2="12" />
          <line x1="3" y1="18" x2="3.01" y2="18" />
        </svg>
        <span>Logs</span>
      </button>
      <button class="nav-item">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <rect x="2" y="2" width="20" height="8" rx="2" />
          <rect x="2" y="14" width="20" height="8" rx="2" />
          <line x1="6" y1="6" x2="6.01" y2="6" />
          <line x1="6" y1="18" x2="6.01" y2="18" />
        </svg>
        <span>Devices</span>
      </button>
    </div>

    <div class="nav-group">
      <p class="nav-group-label">Configuration</p>
      <button class="nav-item">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="12" cy="12" r="3" />
          <path d="M19.07 4.93a10 10 0 0 1 0 14.14M4.93 4.93a10 10 0 0 0 0 14.14" />
        </svg>
        <span>Settings</span>
      </button>
      <button class="nav-item">
        <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="22" y1="2" x2="11" y2="13" />
          <polygon points="22 2 15 22 11 13 2 9 22 2" />
        </svg>
        <span>Telegram</span>
      </button>
    </div>

    <div class="sidebar-spacer"></div>

    <div
      v-if="user"
      class="user-box user-box-link"
      :class="{ active: activePage === 'profile' }"
      role="button"
      tabindex="0"
      @click="goToProfile"
      @keydown.enter="goToProfile"
      @keydown.space.prevent="goToProfile"
      title="View profile"
    >
      <div class="user-avatar">{{ userInitials }}</div>
      <div class="user-info">
        <strong>Admin</strong>
        <span>{{ userEmail }}</span>
        <span class="user-id">ID: {{ userUid }}</span>
      </div>
      <button class="logout-icon-btn" @click.stop="handleLogout" title="Logout">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="15" height="15">
          <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
          <polyline points="16 17 21 12 16 7" />
          <line x1="21" y1="12" x2="9" y2="12" />
        </svg>
      </button>
    </div>
    <button v-else class="login-nav-btn" @click="goToLogin">Login</button>
  </aside>
</template>

<script>
import { auth } from "../firebase";
import { onAuthStateChanged, signOut } from "firebase/auth";

export default {
  name: "AppSidebar",

  props: {
    activePage: {
      type: String,
      default: "",
    },
  },

  data() {
    return {
      user: null,
      unsubscribeAuth: null,
    };
  },

  computed: {
    userEmail() {
      return this.user?.email ?? "";
    },

    userUid() {
      return this.user?.uid ?? "";
    },

    userInitials() {
      if (!this.userEmail) return "?";
      return this.userEmail.substring(0, 2).toUpperCase();
    },
  },

  mounted() {
    this.unsubscribeAuth = onAuthStateChanged(auth, (user) => {
      this.user = user;
    });
  },

  beforeUnmount() {
    if (this.unsubscribeAuth) this.unsubscribeAuth();
  },

  methods: {
    goToDashboard() {
      this.$router.push("/home");
    },

    goToProfile() {
      this.$router.push("/profile");
    },

    goToLogin() {
      this.$router.push("/login");
    },

    async handleLogout() {
      await signOut(auth);
      this.$router.push("/login");
    },
  },
};
</script>

<style scoped>
.sidebar {
  --surface: #111827;
  --raised: #182032;
  --hover: #1c2a42;
  --border: rgba(255, 255, 255, 0.07);
  --border2: rgba(255, 255, 255, 0.13);
  --accent: #3b82f6;
  --accent-dim: rgba(59, 130, 246, 0.11);
  --c-red: #f87171;
  --t1: #f1f5f9;
  --t2: #94a3b8;
  --t3: #4b5e77;
  --r-s: 8px;
  --r-m: 12px;
  --r-l: 16px;

  background: var(--surface);
  border-right: 1px solid var(--border);
  padding: 1.5rem 1rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  height: 100vh;
  position: sticky;
  top: 0;
  overflow-y: auto;
  box-sizing: border-box;
}

.brand {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  padding: 0 0.25rem;
}

.brand-icon {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #1a3460, #2563eb);
  border-radius: var(--r-s);
  display: grid;
  place-items: center;
  color: #93c5fd;
  flex-shrink: 0;
  box-shadow: 0 0 12px rgba(37, 99, 235, 0.3);
}

.brand-icon svg {
  width: 15px;
  height: 15px;
}

.brand-name {
  font-family: "Plus Jakarta Sans", sans-serif;
  font-size: 0.95rem;
  font-weight: 700;
  color: var(--t1);
}

.brand-accent {
  color: var(--accent);
}

.nav-main,
.nav-group {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.nav-group-label {
  font-size: 0.68rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.09em;
  color: var(--t3);
  padding: 0.2rem 0.75rem;
  margin: 0 0 2px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  padding: 0.58rem 0.75rem;
  border: none;
  width: 100%;
  border-radius: var(--r-m);
  background: transparent;
  color: var(--t2);
  font-size: 0.85rem;
  font-weight: 500;
  font-family: inherit;
  cursor: pointer;
  transition:
    background 0.12s,
    color 0.12s;
  position: relative;
}

.nav-item:hover {
  background: var(--raised);
  color: var(--t1);
}

.nav-item.active {
  background: var(--accent-dim);
  color: #93c5fd;
  font-weight: 600;
}

.nav-item.active::before {
  content: "";
  position: absolute;
  left: 0;
  top: 20%;
  height: 60%;
  width: 3px;
  background: var(--accent);
  border-radius: 0 3px 3px 0;
}

.nav-icon {
  width: 15px;
  height: 15px;
  flex-shrink: 0;
}

.sidebar-spacer {
  flex: 1;
}

.user-box {
  display: flex;
  align-items: center;
  gap: 0.55rem;
  padding: 0.7rem;
  background: var(--raised);
  border: 1px solid var(--border);
  border-radius: var(--r-l);
}

.user-box-link {
  cursor: pointer;
  transition:
    border-color 0.12s,
    background 0.12s,
    transform 0.12s;
}

.user-box-link:hover,
.user-box-link:focus-visible,
.user-box-link.active {
  background: var(--hover);
  border-color: var(--border2);
  outline: none;
}

.user-box-link:active {
  transform: translateY(1px);
}

.user-avatar {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background: linear-gradient(135deg, #1a3460, #2563eb);
  display: grid;
  place-items: center;
  font-weight: 700;
  font-size: 0.75rem;
  color: #93c5fd;
  flex-shrink: 0;
}

.user-info {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

.user-info strong {
  font-size: 0.8rem;
  color: var(--t1);
}

.user-info span,
.user-id {
  font-size: 0.72rem;
  color: var(--t3);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.user-id {
  font-size: 0.65rem;
}

.logout-icon-btn {
  background: none;
  border: none;
  color: var(--t3);
  cursor: pointer;
  padding: 3px;
  border-radius: 5px;
  display: grid;
  place-items: center;
  transition: color 0.12s;
}

.logout-icon-btn:hover {
  color: var(--c-red);
}

.login-nav-btn {
  padding: 0.58rem 1rem;
  border: none;
  border-radius: var(--r-m);
  background: var(--accent);
  color: white;
  font-weight: 600;
  font-family: inherit;
  cursor: pointer;
}

@media (max-width: 900px) {
  .sidebar {
    position: static;
    height: auto;
    flex-direction: row;
    flex-wrap: wrap;
    padding: 1rem;
  }

  .nav-group,
  .nav-main {
    flex-direction: row;
    flex-wrap: wrap;
  }
}
</style>
