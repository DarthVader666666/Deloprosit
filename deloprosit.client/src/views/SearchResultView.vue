<script setup>
import { helper } from '@/helper/helper';
import { computed } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const chapterSearchResult = computed(() => store.getters.getChapterSearchResult);

</script>

<template>
<div v-if="chapterSearchResult.length > 0" class="search-result-container">
    <div v-for="(searchResult, index) in chapterSearchResult" :key="index">
        <div class="search-result-header">
            <RouterLink :to="`/chapters/${searchResult.chapterId}/${searchResult.themeId}`">
                {{ searchResult.themeTitle }}
            </RouterLink>
            <span class="date">{{ helper.getDateString(searchResult.dateCreated) }}</span>
        </div>
        <div v-html="searchResult.searchFragment" class="search-result-content"></div>
    </div>
</div>
<h1 v-else>
    Поиск не дал результатов
</h1>
</template>

<style scoped>
.search-result-container {
    display: flex;
    flex-direction: column;
    justify-content: start;
    gap: 15px;
}

.search-result-header {
    display: flex;
    flex: row;
    justify-content: space-between;
    align-items: center;
    font-size: large;
    background: var(--THEME-HEADER-BCKGND-GRADIENT);
    padding: 6px;
    min-height: 34px;
}

.search-result-header a {
    text-decoration: none;
    margin-left: 5px;
    color:  var(--TEXT-COLOR);
}

.search-result-header a:hover {
    text-decoration: underline;
}

.search-result-content {
    padding: 18px 20px 20px 20px;
    background: white;
}

.date {
    font-size: small;
}
</style>