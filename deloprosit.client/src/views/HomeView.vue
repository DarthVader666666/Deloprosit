<script setup>
import { useStore } from 'vuex';
import { computed, ref } from 'vue';
import { RouterLink } from 'vue-router';
import axios from 'axios';
import { helper } from '@/helper/helper';
import { useToast } from 'vue-toastification';
import Button from 'primevue/button'

const store = useStore();
const toast = useToast();

const chapters = computed(() => store.state.chapters);
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const isDeleteButtonActive = computed(() => chapterIds.value.length > 0)

const chapterIds = ref([]);

function ClearForm() {
    chapterIds.value = [];
    const checkBoxes = document.querySelectorAll('input[type=checkbox]');
    checkBoxes.forEach(x => {x.checked = false});
}

function handleCheckboxChange(event) {
    const chapterId = event.target.value;

    if(event.target.checked) {
        chapterIds.value.push(chapterId);
    }
    else {
        const index = chapterIds.value.indexOf(chapterId);
        chapterIds.value.splice(index, 1);
    }
}

function handleDeleteChapters() {
    if(!window.confirm('Эти разделы и их темы будут удалены. Вы уверены?')) {
        return;
    }

    const chapterIdsQuery = helper.getQueryString(chapterIds.value, 'chapterIds');
    const url = `${store.state.serverUrl}/chapters/deletelist` + chapterIdsQuery;

    axios.delete(url, null)
        .then(async response => {
            const status = response.status
            if(status === 200) {
                toast.success('Разделы успешно удалены');
                await store.dispatch('downloadChapters');
                ClearForm();
            }
        })
        .catch(error => {
            const response =  error.response
            if(response.status === 500) {
                toast.error('Ошибка базы данных')
            }
            
            if(response.status === 400) {
                toast.error(response.data.errorText);
            }
        });
}

</script>

<template>
    <div class="chapters-container">
        <div class="chapters-header">
            <h1>Документационное обеспечение управления</h1>
            <div class="delete-button">
                <Button v-if="isAdmin || isOwner" severity="danger" @click="handleDeleteChapters" :disabled="!isDeleteButtonActive">
                    <i class="pi pi-trash"></i>
                    <span>Удалить</span>
                </Button>
            </div>            
        </div>
        <div class="chapter-links">
            <div v-for="(chapter, index) in chapters" :key="index" class="chapter">
                <input v-if="isAdmin || isOwner" type="checkbox" :value="chapter.chapterId" @change.prevent="handleCheckboxChange">
                <RouterLink :to="`/chapters/${chapter.chapterId}`" >
                    <img :src="helper.getImagePath(chapter.imagePath)" width="150px" height="120px">
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
}

.chapters-header {
    display: flex;
    flex-direction: column;
}

.delete-button {
    padding: 0 20px 20px 0;
}

.delete-button button {
    float: right;
}

.chapter-links {
    display: flex ;
    flex-flow: row wrap;
    justify-content: space-around;
    padding: 15px;
    gap: 60px;
}

.chapter {
    padding: 10px;
    max-width: 130px;
    max-height: 130px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-decoration: none;
    color: black;
}

.chapter-links input[type=checkbox] {
    z-index: 1;
    position: absolute;
    margin-left: 120px;
    transform: scale(1.2);
}

.chapter a {
    text-decoration: none;
    color: black;
}

.chapter p {
    font-size: medium;
    text-align: center;
    font-weight: bold;
}

.chapter p:hover {
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
  .chapter img{
    max-width: 90px;
    max-height: 90px;
  }

  .chapter p {
    font-size: small;
  } 

  h1 {
    font-size: medium;
  }
}
</style>