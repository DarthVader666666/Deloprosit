<script setup>
import { useStore } from 'vuex';
import { onMounted, computed } from 'vue';

const store = useStore();
const chapters = computed(() => store.state.chapters);

onMounted(() => {
    store.commit('renderSearchBar', true);
});

</script>

<template>
    <div class="central-container">
        <div v-for="(chapter, index) in chapters" :key="index" class="chapters">
            <img :src="chapter.imagePath" width="150px" height="120px">
            <h3>{{ chapter.chapterTitle }}</h3>     
        </div>
    </div>
</template>

<style scoped>
.central-container {
    display: flex;
    flex: row;
}

.chapters {
    margin: 20px;
    display: flex;
    flex-flow: column wrap;
    gap: 1rem;
    align-items: center;
}

.chapters h3 {
    max-width: 97%;
    text-align: center;
}

.chapters img:hover {
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

.chapters img {
    filter: drop-shadow(var(--PNG-IMAGE-SHADOW));
}
</style>