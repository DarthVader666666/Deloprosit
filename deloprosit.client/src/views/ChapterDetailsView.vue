<script setup>
import ThemeList from '@/components/ThemeList.vue';
import { computed } from 'vue';
import { useStore } from 'vuex';
import { RouterLink } from 'vue-router';
import Button from 'primevue/button';

const store = useStore();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.getters.getChapter);

</script>

<template>
<div class="chapter-details-container">
    <div v-if="chapter">
        <div class="title">
            <h3>
                <Button text rounded severity="contrast" class="back-button">
                    <RouterLink to="/"><i class="pi pi-arrow-left"></i></RouterLink>
                </Button>

                {{chapter.chapterTitle}}

                <Button v-if="isAdmin || isOwner" text rounded severity="contrast" title="Редактировать" class="edit-button">
                    <RouterLink :to="`/chapters/${chapter.chapterId}/edit`"><i class="pi pi-pen-to-square"></i></RouterLink>
                </Button>
            </h3>
        </div>
        <hr/>    
    </div>
    <ThemeList :themes="chapter ? chapter.themes : []"></ThemeList>
</div>

</template>

<style scoped>

.chapter-details-container {
    display: flex;
    flex-direction: column;
}

.title {
    display: flex;
    flex-direction: row;
    padding-right: 15px;
    align-items: center;
}

.title input {
    margin-top: 5px;
    height: 22px;
    font-size: 15px;
    font-weight: bold;
    width: 66%;
}

.back-button {
    padding-left: 9px;
    padding-bottom: 6px;
}

.back-button a {
    color: black;
    width: 14px;
}

.edit-button {
    padding-left: 10px;
    padding-bottom: 6px;
}

.edit-button a {
    color: black;
    width: 14px;
}

.delete-button {
    margin: 10px 0 10px 0;
    float: right;
}

.ok-button {
    margin: 10px 0 10px 0;
    float: right;
    width: 90px;
}

</style>