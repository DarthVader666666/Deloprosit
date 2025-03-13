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

</script>

<template>
<div class="chapter-details-container">
    <div v-if="chapter">
        <div class="title">
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
    justify-content: space-between;;
}

.title input {
    margin-top: 5px;
    height: 22px;
    font-size: 15px;
    font-weight: bold;
    width: 66%;
}

.title span {
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

@media (max-width: 1200px) {
    .title span {
        display: none;
    }
}

</style>