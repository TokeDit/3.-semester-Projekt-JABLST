<template>
  <div class="home-dashboard">
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
      </div>
    </aside>

    <main class="content">
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
          <div class="progress-bar"><div class="progress-value" style="width: 72%"></div></div>
        </article>
        <article class="stat-card">
          <p class="card-label">Telegram Bot</p>
          <h3>Connected</h3>
          <p>Last message: 14:31:58</p>
        </article>
      </section>

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
            <span>Image Capture</span><span class="status-dot online"></span><span>Operational</span>
          </div>
          <div class="health-row">
            <span>AI Processing</span><span class="status-dot online"></span><span>Operational</span>
          </div>
          <div class="health-row">
            <span>Database</span><span class="status-dot online"></span><span>Operational</span>
          </div>
          <div class="health-row">
            <span>Telegram Notifications</span><span class="status-dot online"></span><span>Operational</span>
          </div>
        </div>
      </section>

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
    </main>
  </div>
</template>

<script>
export default {
  name: 'Home'
}
</script>

<style scoped>
.home-dashboard {
  display: grid;
  grid-template-columns: 260px 1fr;
  min-height: 100vh;
  background: #0f1729;
  color: #e7eefc;
  font-family: 'Inter', Arial, sans-serif;
}

.sidebar {
  background: #111827;
  padding: 2rem 1.5rem;
  border-right: 1px solid rgba(255,255,255,0.06);
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
  background: rgba(59,130,246,0.2);
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
  background: rgba(16,185,129,0.12);
  border: 1px solid rgba(16,185,129,0.24);
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

.status-dot.online { background: #22c55e; }

.stats-grid {
  display: grid;
  grid-template-columns: repeat(5, minmax(0, 1fr));
  gap: 1rem;
}

.stat-card {
  background: #111827;
  border: 1px solid rgba(255,255,255,0.06);
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
  background: rgba(148,163,184,0.15);
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
  grid-template-columns: 1.6fr 1.1fr 0fr;
  gap: 1rem;
}

.panel {
  background: #111827;
  border: 1px solid rgba(255,255,255,0.06);
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
  background: rgba(255,255,255,0.06);
  border: 1px solid rgba(255,255,255,0.12);
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
  background: linear-gradient(180deg, rgba(255,255,255,0.06), rgba(255,255,255,0.02));
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

.ai-summary p {
  margin: 0;
  line-height: 1.75;
  color: #cbd5e1;
}

.summary-footer {
  margin-top: 1.35rem;
  display: flex;
  justify-content: space-between;
  color: #94a3b8;
  font-size: 0.9rem;
}

.health-panel .health-row {
  display: grid;
  grid-template-columns: 1fr auto auto;
  align-items: center;
  gap: 0.75rem;
  padding: 0.95rem 0;
  border-top: 1px solid rgba(255,255,255,0.04);
}

.health-panel .health-row:first-of-type { border-top: none; }

.logs-panel {
  background: #111827;
  border: 1px solid rgba(255,255,255,0.06);
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
  border: 1px solid rgba(255,255,255,0.08);
  color: #e2e8f0;
  border-radius: 14px;
  padding: 0.85rem 1rem;
}

.log-controls input { min-width: 180px; }

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
  border-bottom: 1px solid rgba(255,255,255,0.08);
}

.logs-panel tbody tr {
  border-bottom: 1px solid rgba(255,255,255,0.04);
}

.status {
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  font-size: 0.82rem;
  font-weight: 700;
}

.status.success { background: rgba(16,185,129,0.15); color: #a7f3d0; }
.status.warning { background: rgba(234,179,8,0.15); color: #facc15; }

@media (max-width: 1300px) {
  .stats-grid { grid-template-columns: repeat(3, minmax(0, 1fr)); }
  .overview-grid { grid-template-columns: 1fr; }
}

@media (max-width: 900px) {
  .home-dashboard { grid-template-columns: 1fr; }
  .sidebar { flex-direction: row; flex-wrap: wrap; align-items: center; justify-content: space-between; }
  .nav-links, .sidebar-footer { grid-template-columns: repeat(2, minmax(0, 1fr)); width: 100%; }
  .content { padding: 1.25rem; }
}
</style>