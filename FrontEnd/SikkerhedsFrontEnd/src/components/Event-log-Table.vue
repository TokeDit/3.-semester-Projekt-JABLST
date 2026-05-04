<template>
  <div class="table-scroll">
    <table>
      <thead>
        <tr>
          <th>Time</th>
          <th>Confidence</th>
          <th>Status</th>
        </tr>
      </thead>

      <tbody v-if="items.length > 0">
        <tr v-for="(item, index) in items" :key="item.id ?? index">
          <td class="td-time">{{ formatTimestamp(item) }}</td>
          <td>
            <span class="confidence-badge" :class="getConfidenceClass(item.confidence ?? item.Confidence)">
              {{ formatConfidence(item.confidence ?? item.Confidence) }}
            </span>
          </td>
          <td>
            <span class="sdot" :class="getStatusClass(item.confidence ?? item.Confidence)"></span>
            <span class="slabel" :class="getStatusClass(item.confidence ?? item.Confidence)">
              {{ getStatusText(item.confidence ?? item.Confidence) }}
            </span>
          </td>
        </tr>
      </tbody>

      <tbody v-else>
        <tr>
          <td colspan="3" class="no-data">Ingen data endnu.</td>
        </tr>
      </tbody>
    </table>
  </div>

  <p v-if="error" class="error-message">{{ error }}</p>
</template>

<script setup>
import { ref, onMounted } from 'vue'

onMounted(() => { // køre når komponenten er monteret
  asyncGetItems()
})


const url = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/Sikker/images"
const items = ref([])
const error = ref('')
const loading = ref(false)

function getConfidenceClass(value) {
  const numeric = Number(value)
  if (Number.isNaN(numeric)) return ''
  if (numeric >= 0.75) return 'high'
  if (numeric >= 0.5) return 'medium'
  return 'low'
}

function getStatusClass(value) {
  const numeric = Number(value)
  if (Number.isNaN(numeric)) return ''
  if (numeric >= 0.75) return 'green'
  if (numeric >= 0.5) return 'yellow'
  return 'blue'
}

function getStatusText(value) {
  const numeric = Number(value)
  if (Number.isNaN(numeric)) return '-'
  if (numeric >= 0.75) return 'High'
  if (numeric >= 0.5) return 'Medium'
  return 'Low'
}

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
.table-scroll {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th {
  padding: 0.55rem 0.8rem;
  text-align: left;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: #4b5e77;
  border-bottom: 1px solid rgba(255, 255, 255, 0.07);
  white-space: nowrap;
}

td {
  padding: 0.7rem 0.8rem;
  font-size: 0.8rem;
  color: #cbd5e1;
  border-bottom: 1px solid rgba(255, 255, 255, 0.07);
  white-space: nowrap;
}

tbody tr:last-child td {
  border-bottom: none;
}

tbody tr:hover td {
  background: #182032;
}

.td-time {
  color: #4b5e77;
  font-size: 0.76rem;
}

.confidence-badge {
  display: inline-block;
  padding: 0.16rem 0.55rem;
  border-radius: 8px;
  font-size: 0.7rem;
  font-weight: 600;
}

.confidence-badge.high {
  background: rgba(34, 197, 94, 0.12);
  color: #4ade80;
}

.confidence-badge.medium {
  background: rgba(245, 158, 11, 0.12);
  color: #fcd34d;
}

.confidence-badge.low {
  background: rgba(96, 165, 250, 0.14);
  color: #93c5fd;
}

.sdot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  display: inline-block;
  margin-right: 5px;
  vertical-align: middle;
}

.sdot.green {
  background: #22c55e;
}

.sdot.yellow {
  background: #f59e0b;
}

.sdot.blue {
  background: #3b82f6;
}

.slabel {
  font-size: 0.78rem;
  color: #4ade80;
}

.slabel.yellow {
  color: #fcd34d;
}

.slabel.blue {
  color: #93c5fd;
}

.no-data {
  text-align: center;
  color: #94a3b8;
  padding: 1.1rem 0.75rem;
}

.error-message {
  margin-top: 0.85rem;
  color: #fda4af;
  font-size: 0.9rem;
}
</style>