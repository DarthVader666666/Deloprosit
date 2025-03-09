<script setup>
import ThemeList from '@/components/ThemeList.vue';
import { computed } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';

const store = useStore();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.getters.getChapter);

</script>

<template>
<div class="chapter-details-container">
    <div v-if="chapter">
        <div class="title">
            <h3>
                <Button text rounded severity="contrast" icon="pi pi-arrow-left" title="Назад" @click="router.push('/')"/>
                {{chapter.chapterTitle}}
                <Button v-if="isAdmin || isOwner" text rounded severity="contrast" icon="pi pi-pen-to-square" title="Редактировать" 
                    @click="router.push(`/chapters/${chapter.chapterId}/edit`)"/>
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