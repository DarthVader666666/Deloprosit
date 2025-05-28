<script setup>
import ThemeComponent from '@/components/ThemeComponent.vue';
import { computed } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import { helper } from '@/helper/helper';

const store = useStore();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.getters.getChapter);
const theme = computed(() => store.getters.getTheme);
const themeIds = computed(() => store.getters.getThemes.map(x => x.themeId));

function previousTheme() {
    const themeIndex = themeIds.value.indexOf(theme.value.themeId);

    if(themeIndex != 0) {
        router.push(`/chapters/${theme.value.chapterId}/${themeIds.value[themeIndex - 1]}`);
    }
}

function nextTheme() {
    const themeIndex = themeIds.value.indexOf(theme.value.themeId);

    if(!(themeIndex >= (themeIds.value.length - 1))) {
        router.push(`/chapters/${theme.value.chapterId}/${themeIds.value[themeIndex + 1]}`);
    }
}

</script>

<template>
<div class="chapter-details-container">
    <div v-if="chapter">
        <div class="chapter-title">
            <h3>
                <Button text rounded severity="contrast" icon="pi pi-home" title="На главную" @click.prevent="() => router.push('/')"/>
                {{chapter.chapterTitle}}
                <Button v-if="isAdmin || isOwner" text rounded severity="contrast" icon="pi pi-pen-to-square" title="Редактировать" 
                    @click="router.push(`/chapters/${chapter.chapterId}/edit`)"/>
            </h3>
            <span>{{ helper.getDateString(chapter.dateCreated) }}</span>
        </div>
        <hr/>    
    </div>
    <ThemeComponent v-if="theme" :theme="theme"></ThemeComponent>
    <div class="theme-buttons">
        <Button @click="previousTheme" icon="pi pi-arrow-left" rounded raised></Button>
        <Button @click="nextTheme" icon="pi pi-arrow-right" rounded raised></Button>
    </div>
</div>

</template>

<style scoped>

.chapter-details-container {
    display: flex;
    flex-direction: column;
}

.chapter-title {
    display: flex;
    flex-direction: row;
    padding-right: 15px;
    align-items: center;
    justify-content: space-between;
    height: 35px;
}

.chapter-title input {
    margin-top: 5px;
    height: 22px;
    font-size: 15px;
    font-weight: bold;
    width: 66%;
}

.chapter-title span {
    font-size: small;
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

.theme-buttons {
    display: none;
}

.theme-buttons button {
    height: 50px;
    width: 50px;
    background-color: #10b9818f;
}

@media (max-width: 1100px) {
    .theme-buttons {
        display: flex;
        justify-content: center;
        gap: 50px;
        width: inherit;
        position: fixed;
        z-index: 1;
        bottom: 30px;
    }
}

@media (max-width: 800px) {
    .chapter-title span {
        display: none;
    }
}

</style>