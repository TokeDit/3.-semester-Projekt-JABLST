<template>
  <div class="dashboard-container">
    <div class="logo-container">
      <img src="/logo.png" alt="Project Logo" class="logo" />
    </div>

    <h1>User Dashboard</h1>

    <div class="dashboard-grid">
      <section class="dashboard-section">
        <h2>Motion Detected</h2>
        <ul>
          <li>Detects motion</li>
          <li>Captures image</li>
          <li>Optional face recognition</li>
        </ul>
      </section>

      <section class="dashboard-section">
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
      </section>

      <section class="dashboard-section">
        <h2>User Controls</h2>
        <button class="control-btn" @click="onControl('/on')">/on System On</button>
        <button class="control-btn" @click="onControl('/off')">/off System Off</button>
        <button class="control-btn" @click="onControl('/status')">/status Check Status</button>
        <button class="control-btn" @click="onControl('/help')">/help Get Commands</button>
      </section>

      <section class="dashboard-section">
        <h2>View History</h2>
        <ul>
          <li>Access Dashboard</li>
          <li>View Past Events</li>
        </ul>
      </section>
    </div>
  </div>
</template>

<script setup>
// NO IMPORT FOR THE LOGO HERE.
// By leaving this empty, Vite won't try to "resolve" the file.
function onControl(cmd) {
  alert(`Command sent: ${cmd}`);
}
</script>

<style scoped>
.dashboard-container {
  padding: 2rem;
  background: #eff2f7;
  min-height: 100vh;
  font-family: 'Segoe UI', Arial, sans-serif;
}
.logo-container {
  display: flex;
  justify-content: center;
  align-items: center;
}
.logo {
  width: 80px;
  height: auto;
  margin-bottom: 1rem;
}
h1 {
  color: #246BCE;
  text-align: center;
  margin-bottom: 2rem;
}
.dashboard-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 2rem;
  justify-content: center;
}
.dashboard-section {
  background: #246BCE;
  color: #fff;
  border-radius: 16px;
  box-shadow: 0 3px 16px rgba(0,0,0,0.1);
  padding: 1.7rem 2rem;
  min-width: 260px;
  flex: 1 1 340px;
  max-width: 360px;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.phone {
  background: #225bb7;
  border-radius: 10px;
  padding: 0.6rem 0.8rem;
  margin-top: 0.5rem;
  width: 100%;
}
.event-image-placeholder {
  background: rgba(255, 255, 255, 0.1);
  border: 1px dashed #fff;
  padding: 15px;
  margin: 10px 0;
  text-align: center;
  font-size: 0.8rem;
}
.control-btn {
  background: #1976d2;
  color: #fff;
  border: none;
  border-radius: 7px;
  margin: 0.3rem 0;
  padding: 0.55rem 1.3rem;
  font-size: 1rem;
  font-weight: 500;
  width: 90%;
  cursor: pointer;
}
</style>