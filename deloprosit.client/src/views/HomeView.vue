<script setup>
import { useStore } from 'vuex';
import { onMounted, computed, ref } from 'vue';
import { RouterLink } from 'vue-router';
import axios from 'axios';
import { helper } from '@/helper/helper';
import { useToast } from 'vue-toastification';

const store = useStore();
const toast = useToast();

const chapters = computed(() => store.state.chapters);
const isDeleteButtonActive = computed(() => chapterIds.value.length > 0)

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);

const chapterIds = ref([]);

onMounted(() => {
    store.commit('renderSearchBar', true);
});

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
        .then(response => {
            const status = response.status
            if(status === 200) {
                toast.success('Разделы успешно удалены');
                isDeleteButtonActive.value = false;
                store.commit('downloadChapters');

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
    <div class="chapter-links-container">        
        <h1>Документационное обеспечение управления 
            <i :class="'pi pi-trash' + ` ${isDeleteButtonActive ? 'active' : 'inactive'}`" @click.prevent="handleDeleteChapters" v-if="isAdmin || isOwner"></i>
        </h1>
        <div class="chapter-links">
            <div v-for="(chapter, index) in chapters" :key="index" class="chapter-link">
                <input v-if="isAdmin || isOwner" type="checkbox" :value="chapter.chapterId" @change.prevent="handleCheckboxChange">
                <RouterLink :to="`/chapter/${chapter.chapterId}`" >
                    <img :src="helper.getImagePath(chapter.imagePath)" width="150px" height="120px">
                    <p>{{ chapter.chapterTitle }}</p>
                </RouterLink>
            </div>
        </div>
    </div>    
</template>

<style scoped>
.chapter-links-container h1 {
    text-align: center;
    margin: 15px;
}

.chapter-links-container h1 i {
    margin-top: 5px;
    float: inline-end;
    font-size: large;
}

.chapter-links {
    display: flex ;
    flex-flow: row wrap;
    justify-content: space-around;
}

.chapter-link {
    padding: 10px;
    max-width: 130px;
    max-height: 130px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-decoration: none;
    color: black;
}

.chapter-link a {
    text-decoration: none;
    color: black;
}

.chapter-link p {
    font-size: medium;
    text-align: center;
    font-weight: bold;
}

.chapter-link p:hover {
    text-align: center;
    font-weight: bold;
}

.chapter-link img:hover {
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

.chapter-link img {
    filter: drop-shadow(var(--PNG-IMAGE-SHADOW));
}

.active {
    color:var(--DANGER-RED)
}

.active:hover {
    cursor: pointer;
    box-shadow: var(--GLOW-BOX-SHADOW);
    background: rgb(190, 190, 190);
}

.inactive {
    color: gray;
}

@media(max-width: 800px) {
  .chapter-link img{
    max-width: 100px;
    max-height: 100px;
  }

  .chapter-link p {
    font-size: x-small;
  } 

  h1 {
    font-size: medium;
  }
}
</style>