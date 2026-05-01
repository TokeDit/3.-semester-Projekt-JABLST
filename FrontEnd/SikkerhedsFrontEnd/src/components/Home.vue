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
