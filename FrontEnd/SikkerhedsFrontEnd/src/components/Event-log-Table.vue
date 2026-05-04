<template>
  <section class="event-log-table">
    <div class="actions-row">
      <button class="refresh-btn" @click="asyncGetItems" :disabled="loading">
        <span v-if="loading" class="spinner"></span>
        {{ loading ? 'Loading...' : 'Refresh' }}
      </button>
    </div>

    <table>
      <thead>
        <tr>
          <th>Timestamp</th>
          <th>Confidence</th>
        </tr>
      </thead>

      <tbody v-if="items.length > 0">
        <tr v-for="(item, index) in items" :key="item.id ?? index">
          <td>{{ formatTimestamp(item) }}</td>
          <td>{{ formatConfidence(item.confidence ?? item.Confidence) }}</td>
        </tr>
      </tbody>

      <tbody v-else>
        <tr>
          <td colspan="2" class="no-data">Ingen data endnu.</td>
        </tr>
      </tbody>
    </table>

    <p v-if="error" class="error-message">{{ error }}</p>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'

onMounted(() => { // køre når komponenten er monteret
  asyncGetItems()
})


const url = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/Sikker"
const items = ref([])
const error = ref('')
const loading = ref(false)

function formatTimestamp(item) {
  const value = item.timeStamp ?? item.timestamp ?? item.TimeStamp
  if (!value) return '-'

  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) {
    return value
  }

  const day = String(parsed.getDate()).padStart(2, '0')
  const month = String(parsed.getMonth() + 1).padStart(2, '0')
  const year = parsed.getFullYear()
  const hours = String(parsed.getHours()).padStart(2, '0')
  const minutes = String(parsed.getMinutes()).padStart(2, '0')

  return `${day}-${month}-${year} ${hours}:${minutes}`
}

function formatConfidence(value) {
  if (value === null || value === undefined || value === '') {
    return '-'
  }

  const numeric = Number(value)
  if (Number.isNaN(numeric)) {
    return value
  }

  return `${(numeric * 100).toFixed(1)}%`
}


async function asyncGetItems() { // Fetch items from the API

  loading.value = true

  try {
    const response = await fetch(url, {
      method: 'GET',
    })

    if (!response.ok) {
      throw new Error(`Request failed with status ${response.status}`)
    }

    const data = await response.json()
    items.value = Array.isArray(data) ? data : []
    error.value = ''
  } catch (err) {
    console.error('Error fetching items:', err)
    error.value = 'Kunne ikke hente data.'
  } finally {
    loading.value = false
  }
}


</script>

<style scoped>
.event-log-table {
  margin-top: 1.15rem;
}

.no-data {
  text-align: center;
  color: #94a3b8;
  padding: 1.1rem 0.75rem;
}

.actions-row {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 0.85rem;
}

.refresh-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  padding: 0.75rem 1rem;
  background: #0f1729;
  border: 1px solid rgba(255, 255, 255, 0.08);
  color: #e2e8f0;
  border-radius: 14px;
  font-weight: 600;
  cursor: pointer;
}

.refresh-btn:disabled {
  opacity: 0.75;
  cursor: not-allowed;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 0.95rem 0.75rem;
  text-align: left;
}

th {
  color: #94a3b8;
  font-size: 0.95rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

td {
  color: #cbd5e1;
}

tbody tr {
  border-bottom: 1px solid rgba(255, 255, 255, 0.04);
}

.error-message {
  margin-top: 0.85rem;
  color: #fda4af;
  font-size: 0.9rem;
}

.spinner {
  display: inline-block;
  width: 14px;
  height: 14px;
  border: 2px solid rgba(255, 255, 255, 0.4);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  vertical-align: middle;
  margin-right: 6px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>