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