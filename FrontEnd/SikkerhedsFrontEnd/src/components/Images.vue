<template>
  <div class="images-container">
    <!-- <div v-if="isLoading">Loading images...
      <svg class="loading-spinner" viewBox="0 0 24 24">
        <circle cx="12" cy="12" r="10" fill="none" stroke="currentColor" stroke-width="4" stroke-linecap="round"/>
      </svg>
    </div> -->
    <!-- <div v-else-if="error" class="error-message">{{ error }}</div> -->
    <!-- <div v-else-if="images.length === 0">No images available.</div> -->
     <div v-if="images.length === 0">No images available.</div>
    <div v-else class="images-grid">
      <div v-for="(image, index) in images" :key="index" class="image-card">
        <button type="button" class="image-button">
          <img :src="`data:image/jpeg;base64,${image.imageDataBase64}`" :alt="'Image ' + (index + 1)" />
        </button>
      </div>
    </div>
  </div>
  <div v-for="value in test">
    <p>{{ value }}</p>
  </div>
  <!-- <div @scroll.passive="handleScroll()"></div> -->
</template>


<script>
  import { ref, onMounted } from 'vue'

  const baseUrl = 'https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/api/image'

  export default {
    name: 'Images',

    data() {
      return {
        images: [],
        isLoading: false,
        error: null,
        noMoreImages: false,
        loadImagesCount: 1,
        test: [],
        startTime: new Date(),
        endTime: new Date()
      }
    },

    async created() {
      // no startup fetch here; DOM measurements need mounted
    },

    mounted() {
      window.addEventListener('scroll', this.handleScroll)
      window.addEventListener('wheel', this.handleScroll, { passive: true })
      window.addEventListener('touchmove', this.handleScroll, { passive: true })
      this.getImagesStartup(this.loadImagesCount)
    },

    beforeUnmount() {
      window.removeEventListener('scroll', this.handleScroll)
      window.removeEventListener('wheel', this.handleScroll, { passive: true })
      window.removeEventListener('touchmove', this.handleScroll, { passive: true })
    },

    methods: {

      async getImages(id, amount) {
        if (this.noMoreImages) {
          this.endTime = new Date()
          const timeDiff = (this.endTime - this.startTime) / 1000
          if (timeDiff > 10) {
            this.noMoreImages = false
          } else {
            return
          }
        }

        this.isLoading = true
        this.error = null

        try {
          const url = new URL(baseUrl)

          if (!this.noMoreImages)
          {
            url.searchParams.set('id', id);
            url.searchParams.set('amount', amount);
          }

          const response = await fetch(url);

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`)
          }
          const data = await response.json()
          this.images = this.images.concat(data || [])
          // this.noMoreImages = data.length < amount ? true : false;
        } catch (err) {
          this.error = 'Failed to load images: ' + err.message
          if (err.message.includes('404')) {
            this.noMoreImages = true
            this.startTime = new Date()
          }
        } finally {
          this.isLoading = false
        }
      },

      async getImagesStartup(amount) {
        this.error = null

        while (true) {
          const lastImage = this.images[this.images.length - 1]
          const lastImageId = lastImage ? lastImage.id : 0
          const beforeCount = this.images.length

          await this.getImages(lastImageId, amount)
          await this.$nextTick()

          const pageHeight = document.documentElement.scrollHeight || document.body.scrollHeight
          if (this.noMoreImages) {
            break
          }
          if (pageHeight > window.innerHeight) {
            break
          }
          if (this.images.length === beforeCount) {
            break
          }
        }
      },

      handleScroll() {
        if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 100 && !this.isLoading) {
          const lastImage = this.images[this.images.length - 1]
          const lastImageId = lastImage ? lastImage.id : 1
          this.getImages(lastImageId, this.loadImagesCount)
        }
      }
    }
  }

</script>

<style scoped>
.images-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(20rem, 1fr));
  gap: 12px;
  padding: 10px;
}

.image-card {
  background: rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  overflow: hidden;
}

.image-button {
  border: none;
  padding: 0;
  width: 100%;
  display: block;
  background: transparent;
  cursor: pointer;
}

.image-card img {
  width: 100%;
  display: block;
  object-fit: cover;
  aspect-ratio: 4 / 3;
}
</style>