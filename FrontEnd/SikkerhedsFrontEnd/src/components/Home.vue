<!--
  FILE: HomeDashboard.vue
  DESCRIPTION: Full redesign matching sample screenshot layout.
  All original Vue logic (auth, checkStatus, onControl, loadEvents, Firebase) preserved unchanged.

  CHANGES:
  - git commit -m "style: full redesign matching sample UI - sidebar with grouped nav sections"
  - git commit -m "style: topbar with date display and refresh button"
  - git commit -m "style: stat cards with colored icon badges per service"
  - git commit -m "style: middle row - Recent Image + AI Summary + System Health 3-panel layout"
  - git commit -m "style: event logs with colored type badges, pagination row, result count"
  - git commit -m "style: bottom dashboard cards section preserved, styled to match new design system"
  - git commit -m "refactor: remove duplicate event logs table"
  - git commit -m "refactor: remove redundant user-dashboard-header block"
  - git commit -m "chore: all script/logic completely untouched"
-->

<template>
  <div class="home-dashboard">

    <!-- ==================== SIDEBAR ==================== -->
    <!-- git commit -m "style: sidebar - grouped sections (Monitoring/Configuration), icon nav items" -->
    <aside class="sidebar">

      <div class="brand">
        <div class="brand-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z"/>
            <circle cx="12" cy="13" r="4"/>
          </svg>
        </div>
        <span class="brand-name"><span class="brand-accent">Vision</span> Monitor</span>
      </div>

      <nav class="nav-main">
        <button class="nav-item active">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/></svg>
          <span>Dashboard</span>
        </button>
      </nav>

      <div class="nav-group">
        <p class="nav-group-label">Monitoring</p>
        <button class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="3" width="20" height="14" rx="2"/><line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/></svg>
          <span>Live Feed</span>
        </button>
        <button class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/><polyline points="13 2 13 9 20 9"/></svg>
          <span>Events</span>
        </button>
        <button class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="8" y1="6" x2="21" y2="6"/><line x1="8" y1="12" x2="21" y2="12"/><line x1="8" y1="18" x2="21" y2="18"/><line x1="3" y1="6" x2="3.01" y2="6"/><line x1="3" y1="12" x2="3.01" y2="12"/><line x1="3" y1="18" x2="3.01" y2="18"/></svg>
          <span>Logs</span>
        </button>
        <button class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="2" width="20" height="8" rx="2"/><rect x="2" y="14" width="20" height="8" rx="2"/><line x1="6" y1="6" x2="6.01" y2="6"/><line x1="6" y1="18" x2="6.01" y2="18"/></svg>
          <span>Devices</span>
        </button>
      </div>

      <div class="nav-group">
        <p class="nav-group-label">Configuration</p>
        <button class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="3"/><path d="M19.07 4.93a10 10 0 0 1 0 14.14M4.93 4.93a10 10 0 0 0 0 14.14"/></svg>
          <span>Settings</span>
        </button>
        <button class="nav-item">
          <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="22" y1="2" x2="11" y2="13"/><polygon points="22 2 15 22 11 13 2 9 22 2"/></svg>
          <span>Telegram</span>
        </button>
      </div>

      <div class="sidebar-spacer"></div>

      <!-- User footer — logic unchanged from home.vue -->
      <div v-if="user" class="user-box">
        <div class="user-avatar">{{ userInitials }}</div>
        <div class="user-info">
          <strong>Admin</strong>
          <span>{{ userEmail }}</span>
        </div>
        <button class="logout-icon-btn" @click="handleLogout" title="Logout">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="15" height="15"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/></svg>
        </button>
      </div>
      <button v-else class="login-nav-btn" @click="goToLogin">Login</button>

    </aside>

    <!-- ==================== MAIN CONTENT ==================== -->
    <main class="content">

      <!-- TOPBAR -->
      <!-- git commit -m "style: topbar - title left, status chip + datetime + refresh right" -->
      <header class="topbar">
        <div class="topbar-left">
          <h1>Dashboard</h1>
          <p>System status and overview</p>
        </div>
        <div class="topbar-right">
          <div class="status-chip">
            <span class="pulse-dot"></span>
            System Online
          </div>
          <div class="topbar-date">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
            <span>29 Apr 2025, 14:32:45</span>
          </div>
          <button class="refresh-btn" @click="checkStatus" title="Refresh">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="1 4 1 10 7 10"/><path d="M3.51 15a9 9 0 1 0 .49-4.95"/></svg>
          </button>
        </div>
      </header>

      <!-- STATS GRID -->
      <!-- git commit -m "style: stat cards with colored icon badges matching sample screenshot" -->
      <section class="stats-grid">

        <article class="stat-card">
          <div class="stat-icon green">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="20 6 9 17 4 12"/></svg>
          </div>
          <div class="stat-body">
            <p class="card-label">System Status</p>
            <h3 class="c-green">Online</h3>
            <p>All systems operational</p>
          </div>
        </article>

        <article class="stat-card">
          <div class="stat-icon orange">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><circle cx="12" cy="12" r="4"/><circle cx="12" cy="4" r="1.5"/><circle cx="12" cy="20" r="1.5"/><circle cx="4" cy="12" r="1.5"/><circle cx="20" cy="12" r="1.5"/></svg>
          </div>
          <div class="stat-body">
            <p class="card-label">Raspberry Pi</p>
            <h3 :class="piStatus.isAlive ? 'c-green' : 'c-red'" >
              {{ piStatus.isAlive ? 'Online' : 'Offline' }}
            </h3>
            <p>Last seen: {{ piStatus.lastSeen ?? 'Never' }}</p>
          </div>
        </article>

       <article class="stat-card">
  <div class="stat-icon teal">
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
      <line x1="22" y1="2" x2="11" y2="13"/>
      <polygon points="22 2 15 22 11 13 2 9 22 2"/>
    </svg>
  </div>
  <div class="stat-body">
    <p class="card-label">Telegram Bot</p>
    <h3 :class="telegramStatus.connected ? 'c-teal' : 'c-red'">
      {{ telegramStatus.connected ? 'Connected' : 'Disconnected' }}
    </h3>
    <p>Last message: {{ telegramStatus.lastMessageTime ?? 'Never' }}</p>
    <p style="font-size: 0.8rem; opacity: 0.7;">{{ telegramStatus.lastMessage }}</p>

    <div style="display: flex; align-items: center; gap: 8px; margin-top: 8px;">
      <button @click="pingBot" class="btn-secondary">Ping Bot</button>
      <!-- Inline so result appears next to button, not below the card -->
      <span v-if="pingResult" style="font-size: 0.8rem; opacity: 0.8;">{{ pingResult }}</span>
    </div>
  </div>
</article>

        <article class="stat-card">
          <div class="stat-icon indigo">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><ellipse cx="12" cy="5" rx="9" ry="3"/><path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3"/><path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"/></svg>
          </div>
          <div class="stat-body">
            <p class="card-label">Storage</p>
            <h3 class="c-yellow">72%</h3>
            <p>Used: 72 GB / 100 GB</p>
            <div class="progress-bar"><div class="progress-fill" style="width:72%"></div></div>
          </div>
        </article>
     
      </section>

      <!-- MIDDLE ROW -->
      <!-- git commit -m "style: 3-panel middle row - Recent Image | AI Summary | System Health" -->
      <section class="middle-row">

        <!-- Recent Image -->
        <div class="panel">
          <div class="panel-hd">
            <h2>Recent Image</h2>
            <span class="badge-live"><span class="pulse-dot sm"></span>Live</span>
          </div>
          <div class="img-preview">
            <svg width="34" height="34" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2" opacity="0.2"><path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z"/><circle cx="12" cy="13" r="4"/></svg>
            <span>Camera snapshot preview</span>
          </div>
          <div class="panel-meta">
            <span>Captured: 29 Apr 2025, 14:32:10</span>
            <span class="link-text">Device: RASPI-01</span>
          </div>
        </div>

        <!-- AI Summary -->
        <!-- git commit -m "style: AI summary panel with star icon, blockquote, confidence footer" -->
        <div class="panel ai-panel">
          <div class="panel-hd">
            <div class="ai-title">
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="color:var(--c-indigo)"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
              <h2>AI Summary</h2>
            </div>
            <button class="btn-ghost">View All</button>
          </div>
          <blockquote class="ai-quote">
            "A person is walking on a driveway at night near a parked car. The area is illuminated by outdoor lights."
          </blockquote>
          <div class="ai-footer">
            <span>Confidence: <strong class="c-green">92%</strong></span>
            <span>Processed: <strong>14:32:12</strong></span>
          </div>
        </div>

        <!-- System Health -->
        <div class="panel">
          <div class="panel-hd">
            <h2>System Health</h2>
            <button class="btn-ghost">View Details</button>
          </div>
          <div class="health-list">
            <div class="health-row">
              <div class="health-left">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z"/><circle cx="12" cy="13" r="4"/></svg>
                <span>Image Capture</span>
              </div>
              <span class="health-pill">
                <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3"><polyline points="20 6 9 17 4 12"/></svg>
                Operational
              </span>
            </div>
            <div class="health-row">
              <div class="health-left">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M8 12s1.5-3 4-3 4 3 4 3-1.5 3-4 3-4-3-4-3z"/><circle cx="12" cy="12" r="1.5" fill="currentColor"/></svg>
                <span>AI Processing</span>
              </div>
              <span class="health-pill">
                <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3"><polyline points="20 6 9 17 4 12"/></svg>
                Operational
              </span>
            </div>
            <div class="health-row">
              <div class="health-left">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><ellipse cx="12" cy="5" rx="9" ry="3"/><path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3"/><path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"/></svg>
                <span>Database</span>
              </div>
              <span class="health-pill">
                <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3"><polyline points="20 6 9 17 4 12"/></svg>
                Operational
              </span>
            </div>
            <div class="health-row">
  <div class="health-left">
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <line x1="22" y1="2" x2="11" y2="13"/>
      <polygon points="22 2 15 22 11 13 2 9 22 2"/>
    </svg>
    <span>Telegram Notifications</span>
  </div>
  <!-- Dynamically green or red based on connection -->
  <span class="health-pill" :class="telegramStatus.connected ? 'pill-green' : 'pill-red'">
    <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
      <polyline points="20 6 9 17 4 12"/>
    </svg>
    {{ telegramStatus.connected ? 'Operational' : 'Unavailable' }}
  </span>
</div>
          </div>
        </div>


  </section>

  <!-- EVENT LOGS — single table, duplicate removed -->
      <!-- git commit -m "refactor: remove duplicate event logs section" -->
      <!-- git commit -m "style: logs - colored type badges, dot+label status, pagination footer" -->
      <section class="logs-panel">
        <div class="logs-hd">
          <h2>Event Logs</h2>
          <div class="log-controls">
            <div class="input-wrap">
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
              <input type="text" placeholder="Search logs..." />
            </div>
            <div class="select-wrap">
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/></svg>
              <select>
                <option>All Types</option>
                <option>Info</option>
                <option>Warning</option>
                <option>Success</option>
              </select>
            </div>
            <div class="date-btn">
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
              <span>29 Apr 2025</span>
              <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="6 9 12 15 18 9"/></svg>
            </div>
          </div>
        </div>

        <EventLogTable />
        
      </section>

      <!-- DASHBOARD CARDS — all logic unchanged -->
      <!-- git commit -m "style: dashboard cards - 3-col grid, icon badges, matches new design system" -->
      <section class="dash-section">
        <p class="section-label">Quick Actions &amp; Status</p>
        <div class="dash-grid">

          <div class="dash-card">
            <div class="dc-hd">
              <div class="dc-icon blue"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12s2.545-5 7-5c4.454 0 7 5 7 5s-2.546 5-7 5c-4.455 0-7-5-7-5z"/><circle cx="12" cy="12" r="3"/></svg></div>
              <h4>Motion Detected</h4>
            </div>
            <ul class="dc-list">
              <li>Detects motion</li>
              <li>Captures image</li>
              <li>Optional face recognition</li>
            </ul>
          </div>

          <div class="dash-card">
            <div class="dc-hd">
              <div class="dc-icon orange"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/><polyline points="13 2 13 9 20 9"/></svg></div>
              <h4>Events</h4>
            </div>
            <ul class="event-list">
              <li v-for="event in events" :key="event.id" class="event-item">
                <span>📷</span>
                <div>
                  <span class="ev-type">{{ event.type }}</span>
                  <span class="ev-ts">{{ event.timestamp }}</span>
                </div>
              </li>
            </ul>
            <p v-if="events.length === 0" class="no-events">No events yet</p>
          </div>

          <div class="dash-card">
            <div class="dc-hd">
              <div class="dc-icon green"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg></div>
              <h4>System Status</h4>
            </div>
            <div class="status-indicator"><span :class="statusClass">{{ statusText }}</span></div>
            <p class="status-time">Last checked: {{ lastChecked }}</p>
            <button class="action-btn" @click="checkStatus">Refresh Status</button>
          </div>

          <div class="dash-card">
            <div class="dc-hd">
              <div class="dc-icon teal"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="22" y1="2" x2="11" y2="13"/><polygon points="22 2 15 22 11 13 2 9 22 2"/></svg></div>
              <h4>Telegram Notification</h4>
            </div>
            <div class="phone-mock">
              <strong>Motion Detected</strong>
              <div class="phone-ts">12:34 PM</div>
              <div class="phone-img">Event Image Preview</div>
              <ul class="phone-ul">
                <li>Photo of Activity</li>
                <li>Description</li>
                <li>Timestamp</li>
              </ul>
            </div>
          </div>

          <div class="dash-card">
            <div class="dc-hd">
              <div class="dc-icon indigo"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="7" width="20" height="14" rx="2"/><path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/></svg></div>
              <h4>User Controls</h4>
            </div>
            <div class="ctrl-grid">
              <button class="ctrl-btn c-green" @click="onControl('/on')">/on System On</button>
              <button class="ctrl-btn c-red" @click="onControl('/off')">/off System Off</button>
              <button class="ctrl-btn" @click="onControl('/status')">/status Check Status</button>
              <button class="ctrl-btn c-ghost" @click="onControl('/help')">/help Get Commands</button>
            </div>
          </div>

          <div class="dash-card">
            <div class="dc-hd">
              <div class="dc-icon purple"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="1 4 1 10 7 10"/><path d="M3.51 15a9 9 0 1 0 .49-4.95"/></svg></div>
              <h4>View History</h4>
            </div>
            <ul class="dc-list">
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
import EventLogTable from './Event-log-Table.vue'
import { auth } from "../firebase";
import { onAuthStateChanged, signOut } from "firebase/auth";

export default {
  name: "HomeDashboard",

  components: {
    EventLogTable,
  },

  data() {
    return {
      telegramStatus: {
        lastMessage: "",
        lastMessageTime: null,
        connected: false,
        pingResult: "",
      },
      piStatus: {
        lastSeen: null,
        isAlive: false,
      },
      user: null,
      unsubscribeAuth: null,
      status: "unknown",
      lastChecked: "Never",
      statusText: "Checking...",
      statusClass: "status-unknown",
      events: [],
    };
  },

  computed: {
    userEmail() { return this.user?.email ?? ""; },
    userInitials() {
      if (!this.userEmail) return "?";
      return this.userEmail.substring(0, 2).toUpperCase();
    },
  },

  mounted() {
    this.unsubscribeAuth = onAuthStateChanged(auth, (user) => { this.user = user; });
    this.checkStatus();
    this.loadEvents();
    this.fetchTelegramStatus();
    this.fetchPiStatus();
    this.telegramPollInterval = setInterval(this.fetchTelegramStatus, 5000);
    this.piPollInterval = setInterval(this.fetchPiStatus, 30000);
  },

  beforeUnmount() {
    if (this.unsubscribeAuth) this.unsubscribeAuth();
    if (this.telegramPollInterval) clearInterval(this.telegramPollInterval);
    if (this.piPollInterval) clearInterval(this.piPollInterval);
  },

  methods: {
    async handleLogout() {
      await signOut(auth);
      this.$router.push("/login");
    },

    goToLogin() {
  this.$router.push("/login");
},

async pingBot() {
  const start = Date.now();
  try {
    const res = await fetch("https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/Sikker/ping");
    const ms = Date.now() - start;
    this.pingResult = `Pong! ${ms}ms`;
    this.telegramStatus.connected = true;
  } catch {
    this.pingResult = "Bot unreachable";
    this.telegramStatus.connected = false;
  }
},

async checkStatus() {
  this.statusText = "Checking...";
  this.statusClass = "status-unknown";
  try {
    const res = await fetch("https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/Sikker/status");
    const data = await res.json();
    this.status = data.status;
    this.statusText = this.status === "online" ? "🟢 Online" : "🔴 Offline";
    this.statusClass = this.status === "online" ? "status-online" : "status-offline";
    this.lastChecked = new Date().toLocaleTimeString();
  } catch {
    this.statusText = "⚠️ Could not reach system";
    this.statusClass = "status-unknown";
  }
},

  async fetchTelegramStatus() {
  try {
    const res = await fetch(`${this.apiBase}/telegram/status`);
    const data = await res.json();
    this.telegramStatus.lastMessage = data.lastMessage || "No messages yet";
    this.telegramStatus.lastMessageTime = data.lastMessageTime
      ? new Date(data.lastMessageTime).toLocaleTimeString("da-DK")
      : "Never";
    // FIX: only update connected from poll if ping hasn't already confirmed it
    if (!this.telegramStatus.connected) {
      this.telegramStatus.connected = !!data.lastMessageTime;
    }
  } catch {
    this.telegramStatus.connected = false;
    this.telegramStatus.lastMessage = "Could not reach bot";
  }
},
    async fetchPiStatus() {                                                                                                                                                         try {
        const res = await fetch("https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/api/PI/status");
        const data = await res.json();
        this.piStatus.lastSeen = data.lastSeen
          ? new Date(data.lastSeen).toLocaleTimeString("da-DK")
          : "Never";
        this.piStatus.isAlive = data.isAlive;
      } catch {
        this.piStatus.isAlive = false;
        this.piStatus.lastSeen = "Unreachable";
      }
    },
    
    async onControl(cmd) {
      try {
        const method = cmd === "/status" ? "GET" : "POST";
        const res = await fetch(`https://localhost:7018/Sikker${cmd}`, {
          method,
          headers: { "Content-Type": "application/json" },
        });
        const data = await res.json();
        this.status = data.status;
        this.statusText = this.status === "online" ? "🟢 Online" : "🔴 Offline";
        this.statusClass = this.status === "online" ? "status-online" : "status-offline";
        this.lastChecked = new Date().toLocaleTimeString();
      } catch {
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
/*
  git commit -m "style: full CSS redesign - Plus Jakarta Sans + IBM Plex Sans, deep navy dark theme"

  Add to index.html:
  <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@600;700;800&family=IBM+Plex+Sans:wght@400;500;600&display=swap" rel="stylesheet">
*/

/* ─── Design tokens ──────────────────────────────────────── */
.home-dashboard {
  --bg:       #0b1120;
  --surface:  #111827;
  --raised:   #182032;
  --hover:    #1c2a42;

  --border:   rgba(255,255,255,0.07);
  --border2:  rgba(255,255,255,0.13);

  --accent:      #3b82f6;
  --accent-dim:  rgba(59,130,246,0.14);

  --c-green:  #22c55e;  --c-green-bg:  rgba(34,197,94,0.12);  --c-green-t:  #4ade80;
  --c-yellow: #f59e0b;  --c-yellow-bg: rgba(245,158,11,0.12); --c-yellow-t: #fcd34d;
  --c-orange: #f97316;  --c-orange-bg: rgba(249,115,22,0.14);
  --c-teal:   #14b8a6;  --c-teal-bg:   rgba(20,184,166,0.14);
  --c-indigo: #818cf8;  --c-indigo-bg: rgba(129,140,248,0.14);
  --c-purple: #a855f7;  --c-purple-bg: rgba(168,85,247,0.14);
  --c-red:    #f87171;  --c-red-bg:    rgba(248,113,113,0.14);
  --c-blue:   #60a5fa;  --c-blue-bg:   rgba(96,165,250,0.14);

  --t1: #f1f5f9;
  --t2: #94a3b8;
  --t3: #4b5e77;

  --r-s: 8px;  --r-m: 12px;  --r-l: 16px;  --r-xl: 20px;

  display: grid;
  grid-template-columns: 220px 1fr;
  min-height: 100vh;
  background: var(--bg);
  color: var(--t1);
  font-family: 'IBM Plex Sans', 'Segoe UI', sans-serif;
  font-size: 14px;
}

/* ─── Sidebar ────────────────────────────────────────────── */
.sidebar {
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
}

.brand { display: flex; align-items: center; gap: 0.6rem; padding: 0 0.25rem; }

.brand-icon {
  width: 32px; height: 32px;
  background: linear-gradient(135deg,#1a3460,#2563eb);
  border-radius: var(--r-s);
  display: grid; place-items: center;
  color: #93c5fd; flex-shrink: 0;
  box-shadow: 0 0 12px rgba(37,99,235,.3);
}
.brand-icon svg { width: 15px; height: 15px; }

.brand-name {
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 0.95rem; font-weight: 700; color: var(--t1);
}
.brand-accent { color: var(--accent); }

.nav-main { display: flex; flex-direction: column; gap: 2px; }

.nav-group { display: flex; flex-direction: column; gap: 2px; }

.nav-group-label {
  font-size: 0.68rem; font-weight: 700;
  text-transform: uppercase; letter-spacing: 0.09em;
  color: var(--t3); padding: 0.2rem 0.75rem; margin-bottom: 2px;
}

.nav-item {
  display: flex; align-items: center; gap: 0.6rem;
  padding: 0.58rem 0.75rem;
  border: none; width: 100%; border-radius: var(--r-m);
  background: transparent; color: var(--t2);
  font-size: 0.85rem; font-weight: 500; font-family: inherit;
  cursor: pointer; transition: background .12s, color .12s;
  position: relative;
}
.nav-item:hover { background: var(--raised); color: var(--t1); }
.nav-item.active { background: var(--accent-dim); color: #93c5fd; font-weight: 600; }
.nav-item.active::before {
  content: ''; position: absolute; left: 0; top: 20%; height: 60%;
  width: 3px; background: var(--accent); border-radius: 0 3px 3px 0;
}
.nav-icon { width: 15px; height: 15px; flex-shrink: 0; }

.sidebar-spacer { flex: 1; }

.user-box {
  display: flex; align-items: center; gap: 0.55rem;
  padding: 0.7rem; background: var(--raised);
  border: 1px solid var(--border); border-radius: var(--r-l);
}
.user-avatar {
  width: 30px; height: 30px; border-radius: 50%;
  background: linear-gradient(135deg,#1a3460,#2563eb);
  display: grid; place-items: center;
  font-weight: 700; font-size: 0.75rem; color: #93c5fd; flex-shrink: 0;
}
.user-info { flex: 1; min-width: 0; display: flex; flex-direction: column; }
.user-info strong { font-size: 0.8rem; color: var(--t1); }
.user-info span   { font-size: 0.72rem; color: var(--t3); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

.logout-icon-btn {
  background: none; border: none; color: var(--t3);
  cursor: pointer; padding: 3px; border-radius: 5px;
  display: grid; place-items: center; transition: color .12s;
}
.logout-icon-btn:hover { color: var(--c-red); }

.login-nav-btn {
  padding: 0.58rem 1rem; border: none; border-radius: var(--r-m);
  background: var(--accent); color: white;
  font-weight: 600; font-family: inherit; cursor: pointer;
}

/* ─── Content ────────────────────────────────────────────── */
.content {
  padding: 1.75rem 2rem;
  display: flex; flex-direction: column; gap: 1.1rem;
  min-width: 0; overflow-x: hidden;
}

/* ─── Topbar ─────────────────────────────────────────────── */
.topbar { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 0.75rem; }

.topbar h1 {
  margin: 0;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 1.4rem; font-weight: 800; letter-spacing: -.03em;
}
.topbar p { margin: 0.15rem 0 0; color: var(--t2); font-size: 0.8rem; }

.topbar-right { display: flex; align-items: center; gap: 0.5rem; flex-wrap: wrap; }

.status-chip {
  display: inline-flex; align-items: center; gap: 0.4rem;
  background: var(--c-green-bg); border: 1px solid rgba(34,197,94,.22);
  color: var(--c-green-t); padding: 0.42rem 0.8rem;
  border-radius: 999px; font-size: 0.78rem; font-weight: 600;
}

.pulse-dot {
  width: 7px; height: 7px; border-radius: 50%; background: var(--c-green);
  animation: pulse 2s infinite; flex-shrink: 0;
}
.pulse-dot.sm { width: 5px; height: 5px; }

@keyframes pulse {
  0%  { box-shadow: 0 0 0 0 rgba(34,197,94,.6); }
  70% { box-shadow: 0 0 0 5px rgba(34,197,94,0); }
  100%{ box-shadow: 0 0 0 0 rgba(34,197,94,0); }
}

.topbar-date {
  display: flex; align-items: center; gap: 0.4rem;
  background: var(--surface); border: 1px solid var(--border);
  border-radius: var(--r-m); padding: 0.42rem 0.8rem;
  font-size: 0.78rem; color: var(--t2);
}

.refresh-btn {
  width: 32px; height: 32px; border-radius: var(--r-m);
  border: 1px solid var(--border); background: var(--surface);
  color: var(--t2); cursor: pointer; display: grid; place-items: center;
  transition: background .12s, color .12s;
}
.refresh-btn:hover { background: var(--raised); color: var(--t1); }

/* ─── Stats grid ─────────────────────────────────────────── */
.stats-grid {
  display: grid; grid-template-columns: repeat(5,1fr); gap: 0.7rem;
}

.stat-card {
  background: var(--surface); border: 1px solid var(--border);
  border-radius: var(--r-xl); padding: 1rem;
  display: flex; align-items: flex-start; gap: 0.75rem;
  transition: border-color .15s;
}
.stat-card:hover { border-color: var(--border2); }

.stat-icon {
  width: 38px; height: 38px; border-radius: var(--r-s);
  display: grid; place-items: center; flex-shrink: 0;
}
.stat-icon svg { width: 17px; height: 17px; }
.stat-icon.green  { background: var(--c-green-bg);  color: var(--c-green); }
.stat-icon.orange { background: var(--c-orange-bg); color: var(--c-orange); }
.stat-icon.blue   { background: var(--c-blue-bg);   color: var(--c-blue); }
.stat-icon.indigo { background: var(--c-indigo-bg); color: var(--c-indigo); }
.stat-icon.teal   { background: var(--c-teal-bg);   color: var(--c-teal); }

.stat-body { min-width: 0; }
.card-label {
  margin: 0 0 0.15rem; color: var(--t2);
  font-size: 0.72rem; font-weight: 600;
  text-transform: uppercase; letter-spacing: .05em;
}
.stat-card h3 {
  margin: 0;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 1.1rem; font-weight: 700;
}
.stat-card p   { margin: 0.15rem 0 0; color: var(--t2); font-size: 0.76rem; }
.c-green  { color: var(--c-green-t) !important; }
.c-yellow { color: var(--c-yellow-t) !important; }
.c-teal   { color: var(--c-teal) !important; }

.progress-bar {
  margin-top: 0.45rem; background: rgba(148,163,184,.1);
  height: 4px; border-radius: 999px; overflow: hidden; width: 100%;
}
.progress-fill {
  height: 100%; background: linear-gradient(90deg,#2563eb,#60a5fa); border-radius: 999px;
}

/* ─── Middle row ─────────────────────────────────────────── */
.middle-row {
  display: grid; grid-template-columns: 1.1fr 1fr 1fr;
  gap: 0.7rem; align-items: start;
}

.panel {
  background: var(--surface); border: 1px solid var(--border);
  border-radius: var(--r-xl); padding: 1.1rem;
}

.panel-hd {
  display: flex; justify-content: space-between;
  align-items: center; margin-bottom: 0.875rem;
}
.panel-hd h2 {
  margin: 0;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 0.9rem; font-weight: 700;
}

.btn-ghost {
  background: var(--raised); border: 1px solid var(--border);
  color: var(--t2); padding: 0.3rem 0.7rem;
  border-radius: 999px; font-size: 0.73rem;
  font-family: inherit; cursor: pointer;
  transition: color .12s, border-color .12s;
}
.btn-ghost:hover { color: var(--t1); border-color: var(--border2); }

.badge-live {
  display: inline-flex; align-items: center; gap: 0.3rem;
  font-size: 0.73rem; font-weight: 700; color: var(--c-green-t);
  background: var(--c-green-bg); padding: 0.22rem 0.55rem;
  border-radius: 999px; border: 1px solid rgba(34,197,94,.2);
}

.img-preview {
  min-height: 185px; border-radius: var(--r-l);
  background:
    linear-gradient(var(--border) 1px, transparent 1px),
    linear-gradient(90deg,var(--border) 1px, transparent 1px),
    var(--raised);
  background-size: 26px 26px, 26px 26px;
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  gap: 0.5rem; color: var(--t3); font-size: 0.8rem;
}

.panel-meta {
  display: flex; justify-content: space-between;
  margin-top: 0.7rem; font-size: 0.73rem; color: var(--t2);
}
.link-text { color: var(--accent); }

/* AI panel */
.ai-panel { display: flex; flex-direction: column; }
.ai-title { display: flex; align-items: center; gap: 0.35rem; color: var(--c-indigo); }

.ai-quote {
  margin: 0; padding: 0.8rem 0.875rem;
  background: var(--raised); border-left: 3px solid var(--c-indigo);
  border-radius: 0 var(--r-m) var(--r-m) 0;
  font-size: 0.84rem; color: var(--t1);
  line-height: 1.65; font-style: normal;
}

.ai-footer {
  display: flex; gap: 1.1rem; margin-top: 0.75rem;
  font-size: 0.76rem; color: var(--t2);
}
.ai-footer strong { color: var(--t1); }

/* Health */
.health-list { display: flex; flex-direction: column; }
.health-row {
  display: flex; justify-content: space-between; align-items: center;
  padding: 0.7rem 0; border-bottom: 1px solid var(--border);
  font-size: 0.82rem;
}
.health-row:last-child { border-bottom: none; }

.health-left { display: flex; align-items: center; gap: 0.5rem; color: var(--t2); }

.health-pill {
  display: inline-flex; align-items: center; gap: 0.28rem;
  font-size: 0.7rem; font-weight: 600; padding: 0.2rem 0.55rem;
  border-radius: 999px;
  background: var(--c-green-bg); color: var(--c-green-t);
  border: 1px solid rgba(34,197,94,.2);
}

/* ─── Event Logs ─────────────────────────────────────────── */
.logs-panel {
  background: var(--surface); border: 1px solid var(--border);
  border-radius: var(--r-xl); padding: 1.1rem;
}

.logs-hd {
  display: flex; justify-content: space-between;
  align-items: center; margin-bottom: 0.875rem;
  flex-wrap: wrap; gap: 0.6rem;
}
.logs-hd h2 {
  margin: 0;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 0.9rem; font-weight: 700;
}

.log-controls { display: flex; gap: 0.45rem; align-items: center; flex-wrap: wrap; }

.input-wrap, .select-wrap, .date-btn {
  display: flex; align-items: center; gap: 0.35rem;
  background: var(--raised); border: 1px solid var(--border);
  border-radius: var(--r-m); padding: 0.42rem 0.7rem;
  color: var(--t2); font-size: 0.78rem;
}

.input-wrap input {
  background: none; border: none; color: var(--t1);
  font-size: 0.78rem; font-family: inherit; min-width: 130px; outline: none;
}
.input-wrap input::placeholder { color: var(--t3); }

.select-wrap select {
  background: none; border: none; color: var(--t2);
  font-size: 0.78rem; font-family: inherit; cursor: pointer; outline: none;
}

.table-scroll { overflow-x: auto; }

table { width: 100%; border-collapse: collapse; }

th {
  padding: 0.55rem 0.8rem; text-align: left;
  font-size: 0.7rem; font-weight: 700;
  text-transform: uppercase; letter-spacing: .06em;
  color: var(--t3); border-bottom: 1px solid var(--border);
  white-space: nowrap;
}

td {
  padding: 0.7rem 0.8rem; font-size: 0.8rem; color: var(--t2);
  border-bottom: 1px solid var(--border); white-space: nowrap;
}
tbody tr:last-child td { border-bottom: none; }
tbody tr:hover td { background: var(--raised); }

.td-time { color: var(--t3); font-size: 0.76rem; }

/* Type badges */
.tbadge {
  display: inline-block; padding: 0.16rem 0.55rem;
  border-radius: var(--r-s); font-size: 0.7rem; font-weight: 600;
}
.tbadge.ai   { background: rgba(129,140,248,.18); color: #a5b4fc; }
.tbadge.img  { background: rgba(59,130,246,.17);  color: #93c5fd; }
.tbadge.tg   { background: rgba(20,184,166,.17);  color: #5eead4; }
.tbadge.sys  { background: rgba(34,197,94,.14);   color: #86efac; }
.tbadge.warn { background: rgba(245,158,11,.17);  color: #fcd34d; }

/* Status */
.sdot {
  width: 6px; height: 6px; border-radius: 50%;
  display: inline-block; margin-right: 5px; vertical-align: middle;
}
.sdot.green  { background: var(--c-green); }
.sdot.yellow { background: var(--c-yellow); }
.sdot.blue   { background: var(--accent); }

.slabel       { font-size: 0.78rem; color: var(--c-green-t); }
.slabel.warn  { color: var(--c-yellow-t); }
.slabel.info  { color: #93c5fd; }

/* Pagination */
.logs-footer {
  display: flex; justify-content: space-between; align-items: center;
  margin-top: 0.875rem; padding-top: 0.875rem;
  border-top: 1px solid var(--border); flex-wrap: wrap; gap: 0.6rem;
}
.result-count { font-size: 0.76rem; color: var(--t3); }

.pagination { display: flex; align-items: center; gap: 3px; }
.pg-btn {
  min-width: 28px; height: 28px; display: grid; place-items: center;
  border: 1px solid var(--border); background: var(--raised);
  color: var(--t2); border-radius: var(--r-s);
  font-size: 0.76rem; font-family: inherit; cursor: pointer;
  padding: 0 5px; transition: background .1s, color .1s;
}
.pg-btn:hover { background: var(--hover); color: var(--t1); }
.pg-btn.active { background: var(--accent); border-color: var(--accent); color: white; }
.pg-btn.muted  { border-color: transparent; background: transparent; }
.pg-btn:disabled { opacity: 0.35; cursor: default; }

/* ─── Dashboard cards ────────────────────────────────────── */
.dash-section { display: flex; flex-direction: column; gap: 0.6rem; }

.section-label {
  font-size: 0.7rem; font-weight: 700;
  text-transform: uppercase; letter-spacing: .08em;
  color: var(--t3); padding-left: 0.25rem; margin: 0;
}

.dash-grid { display: grid; grid-template-columns: repeat(3,1fr); gap: 0.7rem; }

.dash-card {
  background: var(--surface); border: 1px solid var(--border);
  border-radius: var(--r-xl); padding: 1rem;
  display: flex; flex-direction: column; gap: 0.65rem;
  transition: border-color .15s;
}
.dash-card:hover { border-color: var(--border2); }

.dc-hd { display: flex; align-items: center; gap: 0.55rem; }

.dc-icon {
  width: 30px; height: 30px; border-radius: var(--r-s);
  display: grid; place-items: center; flex-shrink: 0;
}
.dc-icon svg { width: 14px; height: 14px; }
.dc-icon.green  { background: var(--c-green-bg);  color: var(--c-green); }
.dc-icon.orange { background: var(--c-orange-bg); color: var(--c-orange); }
.dc-icon.blue   { background: var(--c-blue-bg);   color: var(--c-blue); }
.dc-icon.indigo { background: var(--c-indigo-bg); color: var(--c-indigo); }
.dc-icon.teal   { background: var(--c-teal-bg);   color: var(--c-teal); }
.dc-icon.purple { background: var(--c-purple-bg); color: var(--c-purple); }

.dash-card h4 {
  margin: 0;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 0.85rem; font-weight: 700;
}

.dc-list { margin: 0; padding: 0; list-style: none; display: flex; flex-direction: column; gap: 0.3rem; }
.dc-list li {
  font-size: 0.8rem; color: var(--t2);
  padding-left: 0.8rem; position: relative;
}
.dc-list li::before { content: '›'; position: absolute; left: 0; color: var(--accent); }

.event-list { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 0.3rem; }
.event-item {
  display: flex; align-items: center; gap: 0.45rem;
  background: var(--raised); border-radius: var(--r-s);
  padding: 0.4rem 0.55rem;
}
.event-item div { display: flex; flex-direction: column; min-width: 0; }
.ev-type  { font-size: 0.76rem; font-weight: 600; color: var(--t1); }
.ev-ts    { font-size: 0.68rem; color: var(--t3); }
.no-events { color: var(--t3); font-style: italic; font-size: 0.8rem; text-align: center; margin: 0; }

.status-indicator { font-size: 0.95rem; font-weight: 700; font-family: 'Plus Jakarta Sans', sans-serif; }
.status-online  { color: var(--c-green-t); }
.status-offline { color: var(--c-red); }
.status-unknown { color: var(--c-yellow-t); }
.status-time    { font-size: 0.73rem; color: var(--t3); margin: 0; }

.action-btn {
  background: var(--accent); color: white; border: none;
  border-radius: var(--r-s); padding: 0.48rem 0.875rem;
  font-size: 0.78rem; font-weight: 600; font-family: inherit;
  cursor: pointer; width: 100%; transition: opacity .12s;
}
.action-btn:hover { opacity: 0.85; }

.phone-mock { background: var(--raised); border-radius: var(--r-m); padding: 0.7rem; }
.phone-mock strong { font-size: 0.8rem; display: block; margin-bottom: 0.15rem; }
.phone-ts   { font-size: 0.68rem; color: var(--t3); margin-bottom: 0.35rem; }
.phone-img  {
  background: rgba(255,255,255,.04); border: 1px dashed var(--t3);
  text-align: center; padding: 0.45rem; font-size: 0.7rem; color: var(--t3);
  border-radius: var(--r-s); margin: 0.35rem 0;
}
.phone-ul { margin: 0; padding-left: 1rem; color: var(--t2); font-size: 0.73rem; }

.ctrl-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0.35rem; }
.ctrl-btn {
  padding: 0.48rem 0.35rem; font-size: 0.73rem; font-weight: 600;
  font-family: inherit; border: none; border-radius: var(--r-s);
  cursor: pointer; transition: opacity .12s;
  background: var(--accent); color: white; text-align: center;
}
.ctrl-btn:hover { opacity: 0.85; }
.ctrl-btn.c-green { background: #15803d; }
.ctrl-btn.c-red   { background: #991b1b; }
.ctrl-btn.c-ghost { background: var(--raised); color: var(--t2); border: 1px solid var(--border); }

/* ─── Responsive ─────────────────────────────────────────── */
@media (max-width: 1400px) {
  .stats-grid  { grid-template-columns: repeat(3,1fr); }
  .middle-row  { grid-template-columns: 1fr 1fr; }
  .dash-grid   { grid-template-columns: repeat(2,1fr); }
}
@media (max-width: 1100px) {
  .middle-row  { grid-template-columns: 1fr; }
}
@media (max-width: 900px) {
  .home-dashboard { grid-template-columns: 1fr; }
  .sidebar { position: static; height: auto; flex-direction: row; flex-wrap: wrap; padding: 1rem; }
  .nav-group, .nav-main { flex-direction: row; flex-wrap: wrap; }
  .stats-grid { grid-template-columns: repeat(2,1fr); }
  .dash-grid  { grid-template-columns: 1fr; }
  .content    { padding: 1rem; }
}
@media (max-width: 560px) {
  .stats-grid  { grid-template-columns: 1fr; }
  .topbar      { flex-direction: column; align-items: flex-start; }
  .ctrl-grid   { grid-template-columns: 1fr; }
}
</style>
