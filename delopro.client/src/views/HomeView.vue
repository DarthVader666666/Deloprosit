<script setup>
import { useStore } from 'vuex';
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { helper } from '@/helper/helper';

const store = useStore();
const chapters = computed(() => store.state.chapters);

</script>

<template>
    <div class="chapters-container">
        <div class="chapters-header">
            <h1>Документационное обеспечение управления</h1>    
        </div>
        <div class="chapter-links">
            <div v-for="(chapter, index) in chapters" :key="index" class="chapter">
                <RouterLink :to="`/chapters/${chapter.chapterId}`" >
                    <img :src="helper.getImagePath(chapter.imagePath)" width="150px" height="auto">
                    <p>{{ chapter.chapterTitle }}</p>
                </RouterLink>
            </div>
        </div>
    </div>    
</template>

<style scoped>
.chapters-container h1 {
    text-align: center;
    margin: 15px;
    filter: drop-shadow(2px 2px 0px rgba(0, 0, 0, 0.5));
    color: var(--HOME-HEADER-COLOR);
}

.chapters-header {
    display: flex;
    flex-direction: column;
}

.chapter-links {
    display: flex ;
    flex-flow: row wrap;
    justify-content: space-around;
    padding: 15px;
    gap: 30px;
}

.chapter {
    padding: 10px;
    max-width: 130px;
    max-height: 150px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-decoration: none;    
}

.chapter a {
    text-decoration: none;
    color: var(--TEXT-COLOR);    
}

.chapter p {
    font-size: medium;
    text-align: center;
    font-weight: bold;
}

.chapter img:hover {
    -webkit-transform: scale(1.1);
    -moz-transform: scale(1.1);
    -o-transform: scale(1.1);
    transform: scale(1.1);

    -webkit-transition: all 0.2s ease-in-out;
    -moz-transition: all 0.2s ease-in-out;
    -o-transition: all 0.2s ease-in-out;
    transition: all 0.2s ease-in-out;

    cursor: pointer;
}

.chapter img {
    filter: drop-shadow(var(--PNG-IMAGE-SHADOW));
}

@media(max-width: 800px) {
  .chapter img {
    max-width: 120px;
    max-height: auto;
  }

  .chapter p {
    font-size: small;
  } 

  h1 {
    font-size: large;
  }
}
</style>