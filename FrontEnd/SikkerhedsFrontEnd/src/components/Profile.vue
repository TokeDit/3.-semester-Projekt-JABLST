<template>
  <div class="profile-page">
    <AppSidebar active-page="profile" />

    <main class="profile-shell">
      <header class="profile-topbar">
        <div>
          <p class="eyebrow">Account</p>
          <h1>Profile</h1>
        </div>
      </header>

      <section v-if="user" class="profile-panel">
        <div class="identity">
          <div class="avatar">{{ userInitials }}</div>
          <div>
            <p class="role">Signed in user</p>
            <h2>{{ user.email || "No email available" }}</h2>
          </div>
        </div>

        <div class="info-grid">
          <article class="info-item">
            <span>Email</span>
            <strong>{{ user.email || "Not available" }}</strong>
          </article>
          <article class="info-item">
            <span>Firebase ID</span>
            <strong class="mono">{{ user.uid }}</strong>
          </article>
          <article class="info-item">
            <span>Email verified</span>
            <strong>{{ user.emailVerified ? "Yes" : "No" }}</strong>
          </article>
          <article class="info-item">
            <span>Display name</span>
            <strong>{{ user.displayName || "Not set" }}</strong>
          </article>
          <article class="info-item">
            <span>Created</span>
            <strong>{{ createdAt }}</strong>
          </article>
          <article class="info-item">
            <span>Last sign-in</span>
            <strong>{{ lastSignInAt }}</strong>
          </article>
        </div>
      </section>

      <section v-else class="profile-panel loading-panel">
        <p>Loading profile...</p>
      </section>
    </main>
  </div>
</template>

<script>
import AppSidebar from "./Sidebar.vue";
import { auth } from "../firebase";
import { onAuthStateChanged } from "firebase/auth";

export default {
  name: "ProfilePage",

  components: {
    AppSidebar,
  },

  data() {
    return {
      user: null,
      unsubscribeAuth: null,
    };
  },

  computed: {
    userInitials() {
      if (!this.user?.email) return "?";
      return this.user.email.substring(0, 2).toUpperCase();
    },

    createdAt() {
      return this.formatFirebaseDate(this.user?.metadata?.creationTime);
    },

    lastSignInAt() {
      return this.formatFirebaseDate(this.user?.metadata?.lastSignInTime);
    },

    providerNames() {
      const providers = this.user?.providerData?.map((provider) => provider.providerId) ?? [];
      return providers.length ? providers.join(", ") : "Not available";
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
    formatFirebaseDate(value) {
      if (!value) return "Not available";

      return new Intl.DateTimeFormat("da-DK", {
        dateStyle: "medium",
        timeStyle: "short",
      }).format(new Date(value));
    },
  },
};
</script>

<style scoped>
.profile-page {
  min-height: 100vh;
  display: grid;
  grid-template-columns: 220px 1fr;
  background: #0b1120;
  color: #f1f5f9;
  font-family: "IBM Plex Sans", "Segoe UI", sans-serif;
}

.profile-shell {
  width: min(1040px, calc(100% - 4rem));
  margin: 0 auto;
  padding: 2rem 0;
  min-width: 0;
}

.profile-topbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.eyebrow {
  margin: 0 0 0.2rem;
  color: #94a3b8;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

h1,
h2,
p {
  margin: 0;
}

h1 {
  font-family: "Plus Jakarta Sans", sans-serif;
  font-size: 1.55rem;
}

.profile-panel {
  background: #111827;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  padding: 1.25rem;
}

.identity {
  display: flex;
  align-items: center;
  gap: 0.9rem;
  padding-bottom: 1.1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: linear-gradient(135deg, #1a3460, #2563eb);
  color: #bfdbfe;
  display: grid;
  place-items: center;
  font-weight: 800;
}

.role {
  color: #94a3b8;
  font-size: 0.78rem;
  font-weight: 700;
  text-transform: uppercase;
}

.identity h2 {
  margin-top: 0.2rem;
  font-family: "Plus Jakarta Sans", sans-serif;
  font-size: 1.15rem;
  overflow-wrap: anywhere;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 0.75rem;
  margin-top: 1rem;
}

.info-item {
  background: #182032;
  border: 1px solid rgba(255, 255, 255, 0.07);
  border-radius: 8px;
  padding: 0.85rem;
  min-width: 0;
}

.info-item span {
  display: block;
  color: #94a3b8;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  margin-bottom: 0.35rem;
}

.info-item strong {
  display: block;
  color: #f8fafc;
  font-size: 0.92rem;
  overflow-wrap: anywhere;
}

.mono {
  font-family: "Consolas", "Courier New", monospace;
  font-size: 0.84rem;
}

.loading-panel {
  color: #94a3b8;
}

@media (max-width: 720px) {
  .profile-topbar {
    align-items: flex-start;
    flex-direction: column;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 900px) {
  .profile-page {
    grid-template-columns: 1fr;
  }

  .profile-shell {
    width: min(1040px, calc(100% - 2rem));
    padding: 1rem 0;
  }
}
</style>
