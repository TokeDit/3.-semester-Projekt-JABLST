<template>
  <section class="Event-log-Table">
    <h2>{{ title }}</h2>

    <div>
        <button @click="asyncGetItems" :disabled="loading">
            <span v-if="loading" class="spinner"></span>
            {{ loading ? 'Loading...' : 'Refresh' }}
        </button>
        <table>
            <thead>
                <tr>
                    
                    <th>Timestamp</th>
                    <th>Event Type</th>
                    <th>Confidence</th>
                </tr>
            </thead>
            <tbody v-if="items.length > 0">
                <tr v-for="item in items" :key="item.id">
                    
                    <td>{{ item.timestamp }}</td>
                    <td>{{ item.eventType }}</td>
                    <td>{{ item.confidence }}</td>
                </tr>
            </tbody>
            <p v-else class="no-data">Ingen data endnu.</p>
        </table>
    </div>

  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'

onMounted(() => { // køre når komponenten er monteret
  asyncGetItems()
})

const title = 'Historik Log'
const url = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/"  // Replace with your actual API endpoint
var items = ref([
    
])  
var error = ref()
var loading = ref(false)


async function asyncGetItems() { // Fetch items from the API
    
    loading.value = true;

    var response = fetch(url, {
        method: 'GET',
    }).then(response => response.json())
    .then(data => {
        items.value = data
    }).catch(err => {
        console.error('Error fetching items:', err)
        error.value = err
    }).finally(() => {
        loading.value = false;
    })
}


</script>

<style scoped>
.historik-log {
  padding: 1rem;
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