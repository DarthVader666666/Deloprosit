<script setup>
import { helper } from '@/helper/helper';
import { computed } from 'vue';
import { useStore } from 'vuex';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';

const store = useStore();
const chapterSearchResult = computed(() => store.getters.getChapterSearchResult);

</script>

<template>
<div v-if="chapterSearchResult.length > 0" class="search-result-container">
    <DataTable :value="chapterSearchResult" paginator :rows="5" :rowsPerPageOptions="[5, 10, 20, 50]" class="table-class">
        <Column>
            <template #body="slotProps">
                <div class="search-result-header">
                    <RouterLink :to="`/chapters/${slotProps.data.chapterId}/${slotProps.data.themeId}`">
                        {{ slotProps.data.themeTitle }}
                    </RouterLink>
                    <span class="date">{{ helper.getDateString(slotProps.data.dateCreated) }}</span>
                </div>
                <div v-html="slotProps.data.searchFragment" class="search-result-content"></div>
            </template>
        </Column>        
    </DataTable>
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

.table-class:deep(td) {
    background-color: var(--CENTRAL-BCKGND-CLR);
}

.table-class:deep(th) {
    display: none;
}

</style>