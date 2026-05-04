<template>
  <div class="table-header">
    <button class="refresh-btn" @click="asyncGetItems" :disabled="loading">
      <svg :class="{ spinning: loading }" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="23 4 23 10 17 10"/><polyline points="1 20 1 14 7 14"/><path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15"/></svg>
      {{ loading ? 'Refreshing…' : 'Refresh' }}
    </button>
  </div>
  <div class="table-scroll">
    <table>
      <thead>
        <tr>
          <th style="width: 50px;"></th>
          <th>Image</th>
          <th>Time</th>
          <th>Confidence</th>
          <th>Status</th>
        </tr>
      </thead>

      <tbody v-if="items.length > 0">
        <template v-for="(item, index) in items" :key="`row-${item.id ?? index}`">
          <tr class="data-row" @click="toggleExpanded(index)" :class="{ expanded: expandedIndex === index }">
            <td class="expand-icon">
              <svg v-if="expandedIndex === index" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="18 15 12 9 6 15"/></svg>
              <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="6 9 12 15 18 9"/></svg>
            </td>
            <td class="img-cell">
              <div v-if="item.imageDataBase64" class="thumbnail">
                <img :src="`data:image/jpeg;base64,${item.imageDataBase64}`" :alt="`Image ${item.id}`" />
              </div>
              <div v-else class="thumbnail placeholder">No image</div>
            </td>
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

          <tr v-if="expandedIndex === index" :key="`detail-${item.id ?? index}`" class="detail-row">
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
                    <span class="detail-value">{{ formatConfidence(item.confidence ?? item.Confidence) }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-label">Image Type</span>
                    <span class="detail-value">{{ item.imageType || '-' }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-label">Detected Object</span>
                    <span class="detail-value">{{ item.detectedObject || '-' }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-label">Description</span>
                    <span class="detail-value">{{ item.description || '-' }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-label">Owner UID</span>
                    <span class="detail-value detail-mono">{{ item.ownerUid || '-' }}</span>
                  </div>
                </div>
                <div v-if="item.imageDataBase64" class="detail-image">
                  <img :src="`data:image/jpeg;base64,${item.imageDataBase64}`" :alt="`Full image ${item.id}`" />
                </div>
              </div>
            </td>
          </tr>
        </template>
      </tbody>

      <tbody v-else>
        <tr>
          <td colspan="5" class="no-data">Ingen data endnu.</td>
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
const expandedIndex = ref(null)

function toggleExpanded(index) {
  expandedIndex.value = expandedIndex.value === index ? null : index
}

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
.table-header {
  display: flex;
  justify-content: flex-end;
  padding: 0 0 0.75rem 0;
}

.refresh-btn {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.4rem 0.9rem;
  background: rgba(59, 130, 246, 0.1);
  border: 1px solid rgba(59, 130, 246, 0.25);
  border-radius: 6px;
  color: #93c5fd;
  font-size: 0.8rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s, border-color 0.2s;
}

.refresh-btn:hover:not(:disabled) {
  background: rgba(59, 130, 246, 0.2);
  border-color: rgba(59, 130, 246, 0.4);
}

.refresh-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.spinning {
  animation: spin 0.8s linear infinite;
}

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

.data-row {
  cursor: pointer;
  transition: background-color 0.15s;
}

.data-row:hover {
  background: #182032 !important;
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

.expand-icon {
  padding: 0.7rem 0.4rem;
  color: #4b5e77;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 50px;
}

.expand-icon svg {
  transition: transform 0.2s;
}

.td-time {
  color: #4b5e77;
  font-size: 0.76rem;
}

.img-cell {
  padding: 0.5rem 0.8rem;
}

.thumbnail {
  width: 50px;
  height: 50px;
  border-radius: 8px;
  overflow: hidden;
  background: linear-gradient(135deg, #1a3a52, #0f1729);
  border: 1px solid rgba(59, 130, 246, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  transition: transform 0.2s, box-shadow 0.2s;
}

.thumbnail:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.2);
}

.thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumbnail.placeholder {
  font-size: 0.65rem;
  color: #4b5e77;
}

.detail-row {
  background: linear-gradient(180deg, #182032, #111827) !important;
  border: none;
  animation: slideDown 0.2s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.detail-cell {
  padding: 1.2rem !important;
  border-bottom: 1px solid rgba(59, 130, 246, 0.15) !important;
  background: rgba(17, 24, 39, 0.6) !important;
}

.detail-content {
  padding: 0;
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 1.5rem;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  padding: 0.8rem;
  background: rgba(27, 58, 82, 0.3);
  border-radius: 8px;
  border: 1px solid rgba(59, 130, 246, 0.1);
}

.detail-label {
  font-size: 0.65rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #64748b;
}

.detail-value {
  font-size: 0.85rem;
  color: #e2e8f0;
  font-weight: 500;
  word-break: break-word;
}

.detail-mono {
  font-family: 'Courier New', monospace;
  font-size: 0.75rem;
  color: #a5b4fc;
}

.detail-image {
  flex-shrink: 0;
}

.detail-image img {
  width: 100%;
  height: auto;
  max-height: 280px;
  border-radius: 12px;
  border: 1px solid rgba(59, 130, 246, 0.2);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.4);
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