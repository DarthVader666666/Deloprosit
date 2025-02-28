<script setup>
import { defineProps } from 'vue';
import { helper } from '@/helper/helper';

const props = defineProps({
    chapter: {
        typeof: Object,
        default: null,
        required: true
    }
});

</script>
<template>
<div class="theme">
    <div v-for="(theme, index) in props.chapter.themes" :key="index">
        <div class="theme-header">
            <RouterLink :to="`/chapter-details/${chapter.chapterId}`">{{ chapter.chapterTitle }}</RouterLink>
            <span>{{ helper.getDateString(theme.dateCreated) }}</span>
        </div>
        <div class="theme-content">
            <RouterLink :to="`/themes/details/${theme.themeId}`"><i class="pi pi-question-circle"></i>{{ theme.description }}</RouterLink>
        </div>
    </div>
</div>
</template>
<style scoped>
.theme {
    text-align: start;    
    display: flex;
    flex-direction: column;
    justify-content: start;
    margin: 0 15px 15px 15px;
    gap: 15px;
}

.theme-header {
    display: flex;
    flex: row;
    justify-content: space-between;
    align-items: center;
    font-size: x-small;
    color: var(--THEME-HEADER-COLOR);
    background: var(--THEME-HEADER-BCKGND-GRADIENT);
    padding: 3px;
    height: 20px;
}

.theme-header a {
    margin-left: 5px;
    color:  var(--THEME-HEADER-COLOR);
}

.theme-header span {
    margin-right: 5px;
}

.theme-content {
    padding: 8px 0 8px 0;
    background: lightgray;
}

.theme-content a {
    color: black;
    padding: 8px;
}

.theme i {
    color: var(--TEXT-GLOW-COLOR);
    margin-right: 5px;
}
</style>