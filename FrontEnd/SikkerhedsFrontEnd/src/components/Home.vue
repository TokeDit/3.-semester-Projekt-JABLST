<!--
  FILE: HomeDashboard.vue
  DESCRIPTION: Merged view of Home (sidebar + system stats + logs) and Dashboard (user dashboard cards + live status)
  GIT COMMIT: "feat: merge home and dashboard views into one unified component"
-->

<template>
  <div class="home-dashboard">
    <!-- ==================== SIDEBAR (from home.vue) ==================== -->
    <!-- git commit: "feat: add sidebar navigation and user auth block from home view" -->
    <aside class="sidebar">
      <div class="brand">
        <div class="brand-icon">📷</div>
        <div>
          <h2>Vision Monitor</h2>
          <p>System overview</p>
        </div>
      </div>

      <nav class="nav-links">
        <button class="nav-item active">
          <span>Dashboard</span>
        </button>
        <button class="nav-item">
          <span>Events</span>
        </button>
        <button class="nav-item">
          <span>Logs</span>
        </button>
      </nav>

      <div class="sidebar-footer">
        <button class="nav-item">
          <span>Settings</span>
        </button>
        <button class="nav-item">
          <span>Telegram</span>
        </button>

        <div v-if="user" class="user-box">
          <div class="user-avatar">{{ userInitials }}</div>
          <div class="user-info">
            <strong>Logged in</strong>
            <span>{{ userEmail }}</span>
          </div>
          <button class="logout-btn" @click="handleLogout">Logout</button>
        </div>

        <button v-else class="login-nav-btn" @click="goToLogin">Login</button>
      </div>
    </aside>

    <!-- ==================== MAIN CONTENT ==================== -->
    <main class="content">
      <!-- ----- Topbar (from home.vue) ----- -->
      <!-- git commit: "style: keep original topbar with system status chip" -->
      <header class="topbar">
        <div>
          <h1>Dashboard</h1>
          <p>System status and overview</p>
        </div>
        <div class="status-chip">
          <span class="status-dot online"></span>
          System Online
        </div>
      </header>

      <!-- ----- ADDED: Logo + "User Dashboard" heading from dashboard.vue ----- -->
      <!-- git commit: "feat: add logo and User Dashboard heading from dashboard view" -->
      <div class="user-dashboard-header">
        <div class="logo-container">
          <img src="/logo.png" alt="Project Logo" class="logo" />
        </div>
        <h1 class="user-dashboard-title">User Dashboard</h1>
      </div>

      <!-- ----- Stats Grid (from home.vue) ----- -->
      <!-- git commit: "style: keep system stats cards from home view" -->
      <section class="stats-grid">
        <article class="stat-card">
          <p class="card-label">System Status</p>
          <h3>Online</h3>
          <p>All systems operational</p>
        </article>
        <article class="stat-card">
          <p class="card-label">Raspberry Pi</p>
          <h3>Online</h3>
          <p>Last seen: 14:32:10</p>
        </article>
        <article class="stat-card">
          <p class="card-label">AI Service</p>
          <h3>Online</h3>
          <p>Response time: 1.2s</p>
        </article>
        <article class="stat-card">
          <p class="card-label">Storage</p>
          <h3>72%</h3>
          <p>Used: 72 GB / 100 GB</p>
          <div class="progress-bar">
            <div class="progress-value" style="width: 72%"></div>
          </div>
        </article>
        <article class="stat-card">
          <p class="card-label">Telegram Bot</p>
          <h3>Connected</h3>
          <p>Last message: 14:31:58</p>
        </article>
      </section>

      <!-- ----- Overview Grid (recent image + system health) from home.vue ----- -->
      <!-- git commit: "style: keep recent image and health panels from home view" -->
      <section class="overview-grid">
        <div class="panel recent-image">
          <div class="panel-header">
            <h2>Recent Image</h2>
            <span class="badge live">Live</span>
          </div>
          <div class="image-placeholder">
            <span>Camera snapshot preview</span>
          </div>
          <div class="panel-meta">
            <span>Captured: 29 Apr 2025, 14:32:10</span>
            <span>Device: RASPI-01</span>
          </div>
        </div>

        <div class="panel health-panel">
          <div class="panel-header">
            <h2>System Health</h2>
            <button class="view-all">View Details</button>
          </div>
          <div class="health-row">
            <span>Image Capture</span><span class="status-dot online"></span
            ><span>Operational</span>
          </div>
          <div class="health-row">
            <span>AI Processing</span><span class="status-dot online"></span
            ><span>Operational</span>
          </div>
          <div class="health-row">
            <span>Database</span><span class="status-dot online"></span
            ><span>Operational</span>
          </div>
          <div class="health-row">
            <span>Telegram Notifications</span
            ><span class="status-dot online"></span><span>Operational</span>
          </div>
        </div>
      </section>

      <!-- ----- Event Logs Table (from home.vue) ----- -->
      <!-- git commit: "style: keep event logs table from home view" -->
      <section class="logs-panel">
        <div class="panel-header logs-header">
          <h2>Event Logs</h2>
          <div class="log-controls">
            <input type="text" placeholder="Search logs..." />
            <select>
              <option>All Types</option>
              <option>Info</option>
              <option>Warning</option>
              <option>Success</option>
            </select>
            <button>29 Apr 2025</button>
          </div>
        </div>
        <table>
          <thead>
            <tr>
              <th>Time</th>
              <th>Type</th>
              <th>Source</th>
              <th>Message</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>29 Apr 2025, 14:32:12</td>
              <td>AI Result</td>
              <td>AI Service</td>
              <td>Image processed successfully</td>
              <td><span class="status success">Success</span></td>
            </tr>
            <tr>
              <td>29 Apr 2025, 14:32:10</td>
              <td>Image Captured</td>
              <td>RASPI-01</td>
              <td>New image captured</td>
              <td><span class="status success">Success</span></td>
            </tr>
            <tr>
              <td>29 Apr 2025, 14:32:08</td>
              <td>Telegram</td>
              <td>Telegram Bot</td>
              <td>Notification sent to chat</td>
              <td><span class="status success">Success</span></td>
            </tr>
            <tr>
              <td>29 Apr 2025, 14:31:58</td>
              <td>System</td>
              <td>System</td>
              <td>System health check completed</td>
              <td><span class="status success">Success</span></td>
            </tr>
            <tr>
              <td>29 Apr 2025, 14:31:45</td>
              <td>AI Service</td>
              <td>AI Service</td>
              <td>AI service response time: 1.3s</td>
              <td><span class="status success">Success</span></td>
            </tr>
            <tr>
              <td>29 Apr 2025, 14:31:30</td>
              <td>Warning</td>
              <td>Storage</td>
              <td>Storage usage is above 70%</td>
              <td><span class="status warning">Warning</span></td>
            </tr>
          </tbody>
        </table>
      </section>

      <!-- ==================== ADDED: DASHBOARD SECTIONS FROM dashboard.vue ==================== -->
      <!-- git commit: "feat: add user dashboard cards (motion, events, status, telegram, controls, history)" -->
      <section class="dashboard-sections">
        <div class="dashboard-grid">
          <!-- Motion Detection Overview -->
          <div class="dashboard-card">
            <h2>Motion Detected</h2>
            <ul>
              <li>Detects motion</li>
              <li>Captures image</li>
              <li>Optional face recognition</li>
            </ul>
          </div>

          <!-- Events list (dynamic) -->
          <div class="dashboard-card">
            <h2>Events</h2>
            <ul class="event-list">
              <li v-for="event in events" :key="event.id" class="event-item">
                <span class="event-icon">📷</span>
                <div class="event-details">
                  <span class="event-type">{{ event.type }}</span>
                  <span class="event-timestamp">{{ event.timestamp }}</span>
                </div>
              </li>
            </ul>
            <p v-if="events.length === 0" class="no-events">No events yet</p>
          </div>

          <!-- System Status with live check -->
          <div class="dashboard-card">
            <h2>System Status</h2>
            <div class="status-indicator">
              <span :class="statusClass">{{ statusText }}</span>
            </div>
            <p class="status-time">Last checked: {{ lastChecked }}</p>
            <button class="control-btn" @click="checkStatus">
              Refresh Status
            </button>
          </div>

          <!-- Telegram Notification Preview -->
          <div class="dashboard-card">
            <h2>Telegram Notification</h2>
            <div class="phone">
              <div class="message">
                <strong>Motion Detected</strong>
                <div class="timestamp">12:34 PM</div>
                <div class="event-image-placeholder">Event Image Preview</div>
                <ul>
                  <li>Photo of Activity</li>
                  <li>Description</li>
                  <li>Timestamp</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- User Controls -->
          <div class="dashboard-card">
            <h2>User Controls</h2>
            <button class="control-btn" @click="onControl('/on')">/on System On</button>
            <button class="control-btn" @click="onControl('/off')">/off System Off</button>
            <button class="control-btn" @click="onControl('/status')">/status Check Status</button>
            <button class="control-btn" @click="onControl('/help')">/help Get Commands</button>
          </div>

          <!-- View History -->
          <div class="dashboard-card">
            <h2>View History</h2>
            <ul>
              <li>Access Dashboard</li>
              <li>View Past Events</li>
            </ul>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script>
// git commit: "chore: import firebase auth and preserve all original logic from both views"
import { auth } from "../firebase";
import { onAuthStateChanged, signOut } from "firebase/auth";

export default {
  name: "HomeDashboard",

  data() {
    return {
      // From home.vue
      user: null,
      unsubscribeAuth: null,

      // From dashboard.vue
      status: "unknown",
      lastChecked: "Never",
      statusText: "Checking...",
      statusClass: "status-unknown",
      events: [],
    };
  },

  computed: {
    // From home.vue
    userEmail() {
      return this.user?.email ?? "";
    },
    userInitials() {
      if (!this.userEmail) return "?";
      return this.userEmail.substring(0, 2).toUpperCase();
    },
  },

  mounted() {
    // home.vue auth listener
    this.unsubscribeAuth = onAuthStateChanged(auth, (user) => {
      this.user = user;
    });
    // dashboard.vue initialization
    this.checkStatus();
    this.loadEvents();
  },

  beforeUnmount() {
    if (this.unsubscribeAuth) {
      this.unsubscribeAuth();
    }
  },

  methods: {
    // home.vue methods
    async handleLogout() {
      await signOut(auth);
      this.$router.push("/login");
    },
    goToLogin() {
      this.$router.push("/login");
    },

    // dashboard.vue methods
    async checkStatus() {
      this.statusText = "Checking...";
      this.statusClass = "status-unknown";

      try {
        const res = await fetch("https://localhost:7018/Sikker/status");
        const data = await res.json();
        this.status = data.status;
        this.statusText = this.status === "online" ? "🟢 Online" : "🔴 Offline";
        this.statusClass = this.status === "online" ? "status-online" : "status-offline";
        this.lastChecked = new Date().toLocaleTimeString();
      } catch (err) {
        this.statusText = "⚠️ Could not reach system";
        this.statusClass = "status-unknown";
      }
    },

    async onControl(cmd) {
      try {
        const method = cmd === "/status" ? "GET" : "POST";
        const res = await fetch(`https://localhost:7018/Sikker${cmd}`, {
          method: method,
          headers: { "Content-Type": "application/json" },
        });
        const data = await res.json();

        this.status = data.status;
        this.statusText = this.status === "online" ? "🟢 Online" : "🔴 Offline";
        this.statusClass = this.status === "online" ? "status-online" : "status-offline";
        this.lastChecked = new Date().toLocaleTimeString();
      } catch (err) {
        alert("Could not reach system");
      }
    },

    formatTimestamp(date) {
      return new Intl.DateTimeFormat("da-DK", {
        dateStyle: "short",
        timeStyle: "medium",
      }).format(date);
    },

    loadEvents() {
      this.events = [
        { id: 1, type: "Bevægelse registreret", timestamp: this.formatTimestamp(new Date()) },
        { id: 2, type: "Bevægelse registreret", timestamp: this.formatTimestamp(new Date(Date.now() - 3600000)) },
        { id: 3, type: "Bevægelse registreret", timestamp: this.formatTimestamp(new Date(Date.now() - 7200000)) },
      ];
    },
  },
};
</script>

<style scoped>
/* ========== ALL STYLES FROM home.vue (unchanged) ========== */
/* git commit: "style: preserve all original dark theme styles from home view" */
.home-dashboard {
  display: grid;
  grid-template-columns: 260px 1fr;
  min-height: 100vh;
  background: #0f1729;
  color: #e7eefc;
  font-family: "Inter", Arial, sans-serif;
}

.sidebar {
  background: #111827;
  padding: 2rem 1.5rem;
  border-right: 1px solid rgba(255, 255, 255, 0.06);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.brand {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.brand-icon {
  width: 42px;
  height: 42px;
  border-radius: 12px;
  background: #1f2937;
  display: grid;
  place-items: center;
  font-size: 1.2rem;
}

.brand h2 {
  margin: 0;
  font-size: 1.05rem;
}

.brand p {
  margin: 0.2rem 0 0;
  color: #9ca3af;
  font-size: 0.9rem;
}

.nav-links,
.sidebar-footer {
  display: grid;
  gap: 0.65rem;
}

.nav-item {
  text-align: left;
  padding: 0.85rem 1rem;
  border: none;
  width: 100%;
  border-radius: 14px;
  background: transparent;
  color: #cbd5e1;
  font-size: 0.98rem;
  cursor: pointer;
}

.nav-item:hover,
.nav-item.active {
  background: rgba(59, 130, 246, 0.2);
  color: #f8fafc;
}

.content {
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.75rem;
}

.topbar {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
}

.topbar h1 {
  margin: 0;
  font-size: 2rem;
}

.topbar p {
  margin: 0.35rem 0 0;
  color: #94a3b8;
}

.status-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(16, 185, 129, 0.12);
  border: 1px solid rgba(16, 185, 129, 0.24);
  color: #a7f3d0;
  padding: 0.8rem 1rem;
  border-radius: 999px;
  font-weight: 600;
}

.status-dot {
  width: 0.75rem;
  height: 0.75rem;
  border-radius: 999px;
  display: inline-block;
}

.status-dot.online {
  background: #22c55e;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(5, minmax(0, 1fr));
  gap: 1rem;
}

.stat-card {
  background: #111827;
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 24px;
  padding: 1.5rem;
  min-height: 130px;
}

.card-label {
  margin: 0 0 0.65rem;
  color: #94a3b8;
  font-size: 0.9rem;
}

.stat-card h3 {
  margin: 0;
  font-size: 1.35rem;
}

.stat-card p {
  margin: 0.65rem 0 0;
  color: #94a3b8;
  font-size: 0.95rem;
}

.progress-bar {
  margin-top: 0.85rem;
  background: rgba(148, 163, 184, 0.15);
  height: 8px;
  border-radius: 999px;
  overflow: hidden;
}

.progress-value {
  height: 100%;
  background: linear-gradient(90deg, #4f46e5, #2563eb);
}

.overview-grid {
  display: grid;
  grid-template-columns: 1.6fr 1.1fr;
  gap: 1rem;
}

.panel {
  background: #111827;
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 24px;
  padding: 1.6rem;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.2rem;
}

.panel-header h2 {
  margin: 0;
  font-size: 1.1rem;
}

.view-all {
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.12);
  color: #e2e8f0;
  border-radius: 999px;
  padding: 0.65rem 1rem;
  cursor: pointer;
}

.live {
  color: #22c55e;
  font-weight: 700;
}

.image-placeholder {
  min-height: 250px;
  border-radius: 20px;
  background: linear-gradient(
    180deg,
    rgba(255, 255, 255, 0.06),
    rgba(255, 255, 255, 0.02)
  );
  display: grid;
  place-items: center;
  color: #94a3b8;
  font-size: 0.95rem;
}

.panel-meta {
  display: flex;
  justify-content: space-between;
  margin-top: 1rem;
  color: #94a3b8;
  font-size: 0.9rem;
}

.health-panel .health-row {
  display: grid;
  grid-template-columns: 1fr auto auto;
  align-items: center;
  gap: 0.75rem;
  padding: 0.95rem 0;
  border-top: 1px solid rgba(255, 255, 255, 0.04);
}

.health-panel .health-row:first-of-type {
  border-top: none;
}

.logs-panel {
  background: #111827;
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 24px;
  padding: 1.6rem;
}

.logs-header {
  align-items: flex-end;
}

.log-controls {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  justify-content: flex-end;
}

.log-controls input,
.log-controls select,
.log-controls button {
  background: #0f1729;
  border: 1px solid rgba(255, 255, 255, 0.08);
  color: #e2e8f0;
  border-radius: 14px;
  padding: 0.85rem 1rem;
}

.log-controls input {
  min-width: 180px;
}

.logs-panel table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1.45rem;
}

.logs-panel th,
.logs-panel td {
  padding: 0.95rem 0.75rem;
  text-align: left;
  color: #cbd5e1;
}

.logs-panel thead th {
  color: #94a3b8;
  font-size: 0.95rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.logs-panel tbody tr {
  border-bottom: 1px solid rgba(255, 255, 255, 0.04);
}

.status {
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  font-size: 0.82rem;
  font-weight: 700;
}

.status.success {
  background: rgba(16, 185, 129, 0.15);
  color: #a7f3d0;
}
.status.warning {
  background: rgba(234, 179, 8, 0.15);
  color: #facc15;
}

.user-box {
  margin-top: 1rem;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  display: grid;
  gap: 0.75rem;
}

.user-avatar {
  width: 42px;
  height: 42px;
  border-radius: 999px;
  background: #334155;
  display: grid;
  place-items: center;
  font-weight: 700;
}

.user-info {
  display: grid;
  gap: 0.2rem;
}

.user-info strong {
  color: #f8fafc;
  font-size: 0.95rem;
}

.user-info span {
  color: #94a3b8;
  font-size: 0.85rem;
  word-break: break-all;
}

.logout-btn,
.login-nav-btn {
  width: 100%;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 700;
}

.logout-btn {
  background: #ef4444;
  color: white;
}

.login-nav-btn {
  margin-top: 1rem;
  background: #2563eb;
  color: white;
}

/* ========== ADDED STYLES FOR DASHBOARD CARDS (adapted from dashboard.vue) ========== */
/* git commit: "style: add dark theme styles for dashboard cards from dashboard view" */
.user-dashboard-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: #111827;
  border-radius: 24px;
  padding: 1rem 1.5rem;
  margin-top: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.06);
}

.logo-container {
  display: flex;
  align-items: center;
}

.logo {
  width: 50px;
  height: auto;
}

.user-dashboard-title {
  margin: 0;
  font-size: 1.8rem;
  color: #e7eefc;
}

.dashboard-sections {
  margin-top: 1rem;
}

.dashboard-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  justify-content: flex-start;
}

.dashboard-card {
  background: #111827;
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 24px;
  padding: 1.5rem;
  flex: 1 1 300px;
  max-width: 360px;
  color: #e7eefc;
}

.dashboard-card h2 {
  margin: 0 0 1rem;
  font-size: 1.2rem;
  color: #e7eefc;
}

.dashboard-card ul {
  margin: 0;
  padding-left: 1.25rem;
  color: #cbd5e1;
}

.phone {
  background: #1f2937;
  border-radius: 12px;
  padding: 0.8rem;
  margin-top: 0.5rem;
}

.event-image-placeholder {
  background: rgba(255, 255, 255, 0.05);
  border: 1px dashed #4b5563;
  padding: 12px;
  margin: 10px 0;
  text-align: center;
  font-size: 0.8rem;
  border-radius: 8px;
}

.control-btn {
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 10px;
  margin: 0.4rem 0;
  padding: 0.6rem 1rem;
  font-size: 0.9rem;
  font-weight: 500;
  width: 100%;
  cursor: pointer;
  transition: background 0.2s;
}

.control-btn:hover {
  background: #1d4ed8;
}

.status-indicator {
  margin: 1rem 0;
  font-size: 1.2rem;
  font-weight: bold;
}

.status-online {
  color: #4ade80;
}
.status-offline {
  color: #f87171;
}
.status-unknown {
  color: #facc15;
}

.status-time {
  font-size: 0.85rem;
  opacity: 0.8;
  margin-bottom: 0.8rem;
}

.event-list {
  list-style: none;
  padding: 0;
  width: 100%;
}

.event-item {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  background: rgba(255, 255, 255, 0.04);
  border-radius: 12px;
  padding: 0.6rem 1rem;
  margin-bottom: 0.6rem;
}

.event-icon {
  font-size: 1.2rem;
}

.event-details {
  display: flex;
  flex-direction: column;
}

.event-type {
  font-weight: bold;
  font-size: 0.9rem;
}

.event-timestamp {
  font-size: 0.75rem;
  opacity: 0.7;
}

.no-events {
  opacity: 0.7;
  font-style: italic;
  text-align: center;
}

/* Responsive (extended from home.vue) */
@media (max-width: 1300px) {
  .stats-grid {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }
  .overview-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 900px) {
  .home-dashboard {
    grid-template-columns: 1fr;
  }
  .sidebar {
    flex-direction: row;
    flex-wrap: wrap;
    align-items: center;
    justify-content: space-between;
  }
  .nav-links,
  .sidebar-footer {
    grid-template-columns: repeat(2, minmax(0, 1fr));
    width: 100%;
  }
  .content {
    padding: 1.25rem;
  }
  .dashboard-card {
    max-width: 100%;
  }
  .user-dashboard-header {
    flex-direction: column;
    text-align: center;
  }
}
</style>