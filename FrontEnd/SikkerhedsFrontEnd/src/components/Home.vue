<!--
  FILE: HomeDashboard.vue
  DESCRIPTION: Merged view of Home (sidebar + system stats + logs) and Dashboard (user dashboard cards + live status)
 -->
  <template>
  <div class="home-dashboard">
    <!-- ==================== SIDEBAR (from home.vue) ==================== -->
    
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
