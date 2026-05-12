<template>
  <div class="monthly-log">

    <!-- Header -->
    <div class="log-header">
      <h2>Monthly Event Log</h2>
      <div class="month-selector">
        <button @click="prevMonth" class="nav-btn">&#8592;</button>
        <span class="month-label">{{ monthLabel }}</span>
        <button @click="nextMonth" class="nav-btn" :disabled="isCurrentMonth">&#8594;</button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="state-msg">Loading...</div>

    <!-- No data -->
    <div v-else-if="items.length === 0" class="state-msg">
      No events found for {{ monthLabel }}.
    </div>

    <!-- Table — reuses same style as Event-log-Table -->
    <div v-else class="table-scroll">
      <table>
        <thead>
          <tr>
            <th></th>
            <th>Image</th>
            <th>Time</th>
            <th>Confidence</th>
          </tr>
        </thead>
        <tbody>
          <template v-for="(item, index) in items" :key="`row-${item.id ?? index}`">
            <tr class="data-row" @click="toggleExpanded(index)"
                :class="{ expanded: expandedIndex === index }">
              <td class="expand-icon">
                <svg v-if="expandedIndex === index" width="14" height="14"
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <polyline points="18 15 12 9 6 15"/>
                </svg>
                <svg v-else width="14" height="14"
                     viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <polyline points="6 9 12 15 18 9"/>
                </svg>
              </td>
              <td class="img-cell">
                <div v-if="item.imageDataBase64" class="thumbnail">
                  <img :src="`data:image/jpeg;base64,${item.imageDataBase64}`"
                       :alt="`Image ${item.id}`" />
                </div>
                <div v-else class="thumbnail placeholder">No image</div>
              </td>
              <td class="td-time">{{ formatTimestamp(item) }}</td>
              <td>
                <span class="sdot"
                      :class="getStatusClass(item.confidence ?? item.Confidence)"></span>
                <span class="slabel"
                      :class="getStatusClass(item.confidence ?? item.Confidence)">
                  {{ getStatusText(item.confidence ?? item.Confidence) }}
                </span>
              </td>
            </tr>

            <tr v-if="expandedIndex === index"
                :key="`detail-${item.id ?? index}`" class="detail-row">
              <td colspan="5" class="detail-cell">
                <div class="detail-content">
                  <div class="detail-grid">
                    <div class="detail-item">
                      <span class="detail-label">ID</span>
                      <span class="detail-value">{{ item.id }}</span>
                    </div>
                    <div class="detail-item">
                      <span class="detail-label">Timestamp</span>
                      <span class="detail-value">{{ formatTimestamp(item) }}</span>
                    </div>
                    <div class="detail-item">
                      <span class="detail-label">Confidence</span>
                      <span class="detail-value">
                        {{ formatConfidence(item.confidence ?? item.Confidence) }}
                      </span>
                    </div>
                    <div class="detail-item">
                      <span class="detail-label">Description</span>
                      <span class="detail-value">{{ item.description || '-' }}</span>
                    </div>
                  </div>
                  <div v-if="item.imageDataBase64" class="detail-image">
                    <img :src="`data:image/jpeg;base64,${item.imageDataBase64}`"
                         :alt="`Full image ${item.id}`" />
                  </div>
                </div>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <p v-if="error" class="error-message">{{ error }}</p>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { auth } from '../firebase'
import { onAuthStateChanged } from 'firebase/auth'

const apiBase = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net"

const items = ref([])
const error = ref('')
const loading = ref(false)
const expandedIndex = ref(null)
const user = ref(null)

const now = new Date()
const selectedYear = ref(now.getFullYear())
const selectedMonth = ref(now.getMonth() + 1)

const HigEnd = 0.85
const MedEnd = 0.65

// Month label e.g. "May 2026"
const monthLabel = computed(() => {
  return new Date(selectedYear.value, selectedMonth.value - 1, 1)
    .toLocaleString('en-DK', { month: 'long', year: 'numeric' })
})

const isCurrentMonth = computed(() => {
  const n = new Date()
  return selectedYear.value === n.getFullYear() &&
         selectedMonth.value === n.getMonth() + 1
})

function prevMonth() {
  if (selectedMonth.value === 1) {
    selectedMonth.value = 12
    selectedYear.value--
  } else {
    selectedMonth.value--
  }
}

function nextMonth() {
  if (isCurrentMonth.value) return
  if (selectedMonth.value === 12) {
    selectedMonth.value = 1
    selectedYear.value++
  } else {
    selectedMonth.value++
  }
}

function toggleExpanded(index) {
  expandedIndex.value = expandedIndex.value === index ? null : index
}

function getStatusClass(value) {
  const n = Number(value)
  if (Number.isNaN(n)) return ''
  if (n >= HigEnd) return 'green'
  if (n >= MedEnd) return 'yellow'
  return 'blue'
}

function getStatusText(value) {
  const n = Number(value)
  if (Number.isNaN(n)) return '-'
  if (n >= HigEnd) return 'High'
  if (n >= MedEnd) return 'Medium'
  return 'Low'
}

function formatTimestamp(item) {
  const value = item.timeStamp ?? item.timestamp ?? item.TimeStamp
  if (!value) return '-'
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return value
  const d = String(parsed.getDate()).padStart(2, '0')
  const m = String(parsed.getMonth() + 1).padStart(2, '0')
  const y = parsed.getFullYear()
  const h = String(parsed.getHours()).padStart(2, '0')
  const min = String(parsed.getMinutes()).padStart(2, '0')
  return `${d}-${m}-${y} ${h}:${min}`
}

function formatConfidence(value) {
  if (value === null || value === undefined || value === '') return '-'
  const n = Number(value)
  if (Number.isNaN(n)) return value
  return `${(n * 100).toFixed(1)}%`
}

async function fetchMonthlyLog() {
  if (!user.value) return
  loading.value = true
  error.value = ''
  try {
    const res = await fetch(
      `${apiBase}/api/Image/user/${user.value.uid}/monthly?year=${selectedYear.value}&month=${selectedMonth.value}`
    )
    if (res.status === 204) { items.value = []; return }
    if (!res.ok) throw new Error(`Status ${res.status}`)
    const data = await res.json()
    items.value = Array.isArray(data) ? data : []
  } catch (err) {
    error.value = 'Could not load events.'
    items.value = []
  } finally {
    loading.value = false
  }
}

// Refetch when month changes
watch([selectedMonth, selectedYear], fetchMonthlyLog)

onMounted(() => {
  onAuthStateChanged(auth, (u) => {
    user.value = u
    if (u) fetchMonthlyLog()
  })
})
</script>

<style scoped>
.monthly-log {
  padding: 1.5rem;
  color: #cbd5e1;
}

.log-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
}

.log-header h2 {
  font-size: 1.2rem;
  font-weight: 600;
  color: #e2e8f0;
  margin: 0;
}

.month-selector {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.month-label {
  font-size: 0.9rem;
  color: #93c5fd;
  min-width: 120px;
  text-align: center;
}

.nav-btn {
  background: rgba(59, 130, 246, 0.1);
  border: 1px solid rgba(59, 130, 246, 0.25);
  border-radius: 6px;
  color: #93c5fd;
  padding: 0.3rem 0.7rem;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background 0.2s;
}

.nav-btn:hover:not(:disabled) { background: rgba(59, 130, 246, 0.2); }
.nav-btn:disabled { opacity: 0.4; cursor: not-allowed; }

.state-msg {
  text-align: center;
  color: #94a3b8;
  padding: 2rem 0;
  font-size: 0.9rem;
}

.error-message {
  margin-top: 0.85rem;
  color: #fda4af;
  font-size: 0.9rem;
}

/* Reuse same table styles as Event-log-Table */
.table-scroll { overflow-x: auto; }
table { width: 100%; border-collapse: collapse; }
th {
  padding: 0.55rem 0.8rem;
  text-align: left;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: #4b5e77;
  border-bottom: 1px solid rgba(255,255,255,0.07);
}
.data-row { cursor: pointer; transition: background-color 0.15s; }
.data-row:hover { background: #182032 !important; }
td {
  padding: 0.7rem 0.8rem;
  font-size: 0.8rem;
  color: #cbd5e1;
  border-bottom: 1px solid rgba(255,255,255,0.07);
  white-space: nowrap;
}
.expand-icon { padding: 0.7rem 0.4rem; color: #4b5e77; width: 50px; }
.td-time { color: #4b5e77; font-size: 0.76rem; }
.img-cell { padding: 0.5rem 0.8rem; }
.thumbnail {
  width: 50px; height: 50px; border-radius: 8px; overflow: hidden;
  background: linear-gradient(135deg, #1a3a52, #0f1729);
  border: 1px solid rgba(59,130,246,0.2);
  display: flex; align-items: center; justify-content: center;
}
.thumbnail img { width: 100%; height: 100%; object-fit: cover; }
.thumbnail.placeholder { font-size: 0.65rem; color: #4b5e77; }
.detail-row { background: linear-gradient(180deg, #182032, #111827) !important; }
.detail-cell {
  padding: 1.2rem !important;
  border-bottom: 1px solid rgba(59,130,246,0.15) !important;
  background: rgba(17,24,39,0.6) !important;
}
.detail-content {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 1.5rem;
}
.detail-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px,1fr)); gap: 1rem; }
.detail-item {
  display: flex; flex-direction: column; gap: 0.4rem;
  padding: 0.8rem;
  background: rgba(27,58,82,0.3);
  border-radius: 8px;
  border: 1px solid rgba(59,130,246,0.1);
}
.detail-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; color: #64748b; }
.detail-value { font-size: 0.85rem; color: #e2e8f0; font-weight: 500; word-break: break-word; }
.detail-image img {
  width: 100%; height: auto; max-height: 280px;
  border-radius: 12px;
  border: 1px solid rgba(59,130,246,0.2);
}
.no-data { text-align: center; color: #94a3b8; padding: 1.1rem 0.75rem; }
.sdot { width: 6px; height: 6px; border-radius: 50%; display: inline-block; margin-right: 5px; vertical-align: middle; }
.sdot.green { background: #22c55e; }
.sdot.yellow { background: #f59e0b; }
.sdot.blue { background: #3b82f6; }
.slabel { font-size: 0.78rem; color: #4ade80; }
.slabel.yellow { color: #fcd34d; }
.slabel.blue { color: #93c5fd; }
</style>