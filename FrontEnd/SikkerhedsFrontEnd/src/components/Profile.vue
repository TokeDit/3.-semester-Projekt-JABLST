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
        <!-- Identity -->
        <div class="identity">
          <div class="avatar">{{ userInitials }}</div>
          <div>
            <p class="role">Signed in user</p>
            <h2>{{ user.email || "No email available" }}</h2>
          </div>
        </div>

        <!-- Firebase info -->
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

        <!-- Settings section -->
        <div class="settings-section">
          <h3 class="settings-title">Notification Settings</h3>

          <div v-if="settingsLoading" class="state-msg">Loading settings...</div>

          <div v-else class="settings-grid">
            <!-- Telegram Chat ID -->
            <div class="setting-item">
              <label>Telegram Chat ID</label>
              <input
                v-model="form.telegramChatId"
                type="text"
                placeholder="e.g. 8386100582"
                class="setting-input"
              />
            </div>

            <!-- Report frequency -->
            <div class="setting-item">
              <label>Report Frequency</label>
              <select v-model="form.reportFrequency" class="setting-input">
                <option :value="1">Daily</option>
                <option :value="7">Weekly</option>
                <option :value="30">Monthly</option>
              </select>
            </div>

            <!-- Reports enabled -->
            <div class="setting-item">
              <label>Reports Enabled</label>
              <div class="toggle-row">
                <input
                  type="checkbox"
                  v-model="form.reportEnabled"
                  id="reportEnabled"
                  class="toggle-checkbox"
                />
                <label for="reportEnabled" class="toggle-label">
                  {{ form.reportEnabled ? "Enabled" : "Disabled" }}
                </label>
              </div>
            </div>
          </div>

          <!-- Save button -->
          <div class="settings-actions">
            <button
              @click="saveSettings"
              :disabled="saving"
              class="save-btn">
              {{ saving ? "Saving..." : "Save Settings" }}
            </button>
            <span v-if="saveSuccess" class="success-msg">✓ Saved successfully</span>
            <span v-if="saveError" class="error-message">{{ saveError }}</span>
          </div>
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

//const apiBase = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net";
const apiBase = "http://localhost:5180";

export default {
  name: "ProfilePage",
  components: { AppSidebar },

  data() {
    return {
      user: null,
      unsubscribeAuth: null,

      // Settings form
      form: {
        telegramChatId: "",
        reportFrequency: 7,
        reportEnabled: true,
      },

      // UI state
      settingsLoading: false,
      saving: false,
      saveSuccess: false,
      saveError: "",
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
  },

  mounted() {
    this.unsubscribeAuth = onAuthStateChanged(auth, (user) => {
      this.user = user;
      if (user) this.fetchSettings();
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
    ////fix(user-settings): allow local testing by loading default settings when API returns 404 or fails

//using Azure.Core;

//Updated fetchSettings() to support local development without requiring Azure or existing user records.
//When the API returns 404 (user not found), the component now loads the settings form with default values instead of showing an error.
//Removed settingsError usage and prevented the UI from hiding the form on load failures.
//Added fallback behavior: if the request fails or the user does not exist, the component logs a warning and continues with default settings.
//Removed the error message block from the template since settingsError is no longer used.
// This change ensures the settings page remains usable during local testing even when the backend has no user data.


    async fetchSettings() {
      this.settingsLoading = true;
      try {
        const token = await this.user.getIdToken();
        const res = await fetch(`${apiBase}/api/User/${this.user.uid}`, {
          headers: { Authorization: `Bearer ${token}` },
        });

        if (res.status === 404) {
          // User exists in Firebase but not in DB yet — use defaults
          return;
        }

        if (!res.ok) throw new Error(`Status ${res.status}`);

        const data = await res.json();
        this.form.telegramChatId = data.telegramChatId || "";
        this.form.reportFrequency = data.reportFrequency ?? 7;
        this.form.reportEnabled = data.reportEnabled ?? true;
      } catch (err) {
        // Don't show error — just use defaults so form is still usable
        console.warn("Could not load settings, using defaults.", err);
      } finally {
        this.settingsLoading = false;
      }
    },

    async saveSettings() {
      this.saving = true;
      this.saveSuccess = false;
      this.saveError = "";
      try {
        const token = await this.user.getIdToken();
        const res = await fetch(`${apiBase}/api/User/${this.user.uid}`, {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            telegramChatId: this.form.telegramChatId || null,
            reportFrequency: this.form.reportFrequency,
            reportEnabled: this.form.reportEnabled,
          }),
        });

        if (!res.ok) throw new Error(`Status ${res.status}`);
        this.saveSuccess = true;
        setTimeout(() => (this.saveSuccess = false), 3000);
      } catch (err) {
        this.saveError = "Could not save settings.";
      } finally {
        this.saving = false;
      }
    },
  },
};
</script>

<style scoped>
.profile-page {
  display: flex;
  min-height: 100vh;
  background: var(--bg, #0b1120);
}

.profile-shell {
  flex: 1;
  padding: 2rem;
  color: #cbd5e1;
}

.profile-topbar {
  margin-bottom: 2rem;
}

.eyebrow {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: #4b5e77;
  margin: 0 0 0.25rem;
}

.profile-topbar h1 {
  font-size: 1.6rem;
  font-weight: 600;
  color: #e2e8f0;
  margin: 0;
}

.profile-panel {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.07);
  border-radius: 12px;
  padding: 2rem;
}

.identity {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.avatar {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: linear-gradient(135deg, #1d4ed8, #0ea5e9);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  font-weight: 700;
  color: #fff;
}

.role {
  font-size: 0.75rem;
  color: #4b5e77;
  margin: 0 0 0.25rem;
}

.identity h2 {
  font-size: 1.1rem;
  font-weight: 600;
  color: #e2e8f0;
  margin: 0;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.info-item {
  background: rgba(27, 58, 82, 0.3);
  border: 1px solid rgba(59, 130, 246, 0.1);
  border-radius: 8px;
  padding: 0.9rem 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.info-item span {
  font-size: 0.65rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #64748b;
}

.info-item strong {
  font-size: 0.85rem;
  color: #e2e8f0;
  word-break: break-all;
}

.mono {
  font-family: 'Courier New', monospace;
  font-size: 0.75rem !important;
  color: #a5b4fc !important;
}

/* Settings section */
.settings-section {
  border-top: 1px solid rgba(255, 255, 255, 0.07);
  padding-top: 1.5rem;
}

.settings-title {
  font-size: 1rem;
  font-weight: 600;
  color: #e2e8f0;
  margin: 0 0 1.25rem;
}

.settings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.setting-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.setting-item label {
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #64748b;
}

.setting-input {
  background: rgba(27, 58, 82, 0.4);
  border: 1px solid rgba(59, 130, 246, 0.2);
  border-radius: 6px;
  padding: 0.5rem 0.75rem;
  color: #e2e8f0;
  font-size: 0.85rem;
  outline: none;
  transition: border-color 0.2s;
}

.setting-input:focus {
  border-color: rgba(59, 130, 246, 0.5);
}

.toggle-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.toggle-checkbox {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.toggle-label {
  font-size: 0.85rem;
  color: #e2e8f0;
  text-transform: none;
  letter-spacing: 0;
}

.settings-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.save-btn {
  background: rgba(59, 130, 246, 0.15);
  border: 1px solid rgba(59, 130, 246, 0.3);
  border-radius: 6px;
  color: #93c5fd;
  padding: 0.5rem 1.25rem;
  font-size: 0.85rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.save-btn:hover:not(:disabled) {
  background: rgba(59, 130, 246, 0.25);
}

.save-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.success-msg {
  font-size: 0.85rem;
  color: #4ade80;
}

.error-message {
  color: #fda4af;
  font-size: 0.85rem;
}

.state-msg {
  color: #94a3b8;
  font-size: 0.9rem;
  padding: 1rem 0;
}

.loading-panel {
  text-align: center;
  color: #94a3b8;
}

<style scoped>
.profile-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  background: #1e293b;
  border: 1px solid #334155;
  border-radius: 6px;
  color: #e2e8f0;
  text-decoration: none;
  font-size: 13px;
}
.profile-btn:hover {
  background: #334155;
}
</style>