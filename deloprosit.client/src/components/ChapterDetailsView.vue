<script setup>
import axios from 'axios';
import { computed, reactive, ref } from 'vue';
import { useStore } from 'vuex';
import { helper } from '@/helper/helper';
import { useToast } from 'vue-toastification';

const store = useStore();
const toast = useToast();

const chapter = computed(() => store.state.chapter);
const isButtonDisabled = computed(() => !editedChapter.chapterTitle || newThemes.value.find(x => x.description === '' || x.description === null));

const newThemes = ref([]);

let editedChapter = reactive({
    chapterId: null,
    chapterTitle: null,
    themes: reactive([])
});

const isEdit = ref(false);

function initializeEditMode() {
    editedChapter.chapterId = chapter.value.chapterId;    
    editedChapter.chapterTitle = chapter.value.chapterTitle;
    editedChapter.userId = chapter.value.userId;
    editedChapter.dateCreated = chapter.value.dateCreated;
    editedChapter.dateDeleted = chapter.value.dateDeleted;
    editedChapter.themes = [];

    isEdit.value = true;
};

function handleAddTheme() {
    newThemes.value.push({
        themeId: null,
        userId: null,
        chapterId: editedChapter.chapterId,
        description: null,
        dateCreated: null,
        dateDeleted: null,
    })
}

function handleDeleteNewTheme(index) {
    newThemes.value.splice(index, 1);
}

async function handleSave() {
    isEdit.value = false;
    const currentDate = helper.getCurrentDate();

    newThemes.value.forEach(theme => {
        theme.dateCreated = currentDate;
    });

    if(newThemes.value.length > 0) {
        editedChapter.themes = newThemes;
    }

    const url = store.state.serverUrl;
    await axios.post(`${url}/chapters/update`, editedChapter,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success('Раздел успешно обновлен');
            store.commit('downloadChapters');
            store.commit('downloadChapter', editedChapter.chapterId);
            editedChapter.themes = [];
        }
    })
    .catch(error => {
        const data = error.response.data;

        if(data) {
            toast.error(data.detail);
        }
        else {
            toast.error('Ошибка обновления раздела')
        }
    });    
}

function handleCancel() {
    isEdit.value = false;
    newThemes.value = [];
}

</script>
<template>
    <div class="central-container">
        <div v-if="chapter">
            <div v-if="!isEdit">
                <div class="title">
                    <h3>{{chapter.chapterTitle}}
                        <i v-if="store.getters.isAdmin || store.getters.isOwner" class="pi pi-pen-to-square edit-chapter-button" @click="initializeEditMode"></i>
                    </h3>
                </div>
                <hr/>
                <div v-for="(theme, index) in chapter.themes" :key="index" class="theme">
                    <span class="date">{{ helper.getDateString(theme.dateCreated) }}</span>
                    <RouterLink :to="`/themes/details/${theme.themeId}`"><i class="pi pi-question-circle"></i>{{ theme.description }}</RouterLink>
                </div>
            </div>
            <div v-else>
                <div class="title">
                    <input v-model="editedChapter.chapterTitle" type="text" autofocus>
                    <div class="buttons">
                        <button type="button" @click.prevent="handleSave" :disabled="isButtonDisabled">Сохранить</button>
                        <button type="button" @click.prevent="handleCancel">Отменить</button>
                    </div>
                </div>
                <hr/>
                <div class="new-themes-header">
                    <h3>Темы:</h3>
                    <button @click.prevent="handleAddTheme"><i class="pi pi-file-plus"></i>Добавить</button>
                </div>
                <hr/>

                <div v-for="(newTheme, index) in newThemes" :key="index" class="new-theme-inputs">
                    <input v-model="newTheme.description" type="text">
                    <button @click.prevent="handleDeleteNewTheme(index)"><i class="pi pi-times"></i>Удалить</button>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.title {
    display: flex;
    flex-direction: row;
    padding-left: 15px;
    padding-right: 15px;
    justify-content: space-between;
}

.title input {
    margin-top: 10px;
    height: 16px;
    font-size: 15px;
    font-weight: bold;
    width: 66%;
}

.buttons {
    display: flex;
    flex-direction: row;
    gap: 5px;
    align-items: end;
    padding-left: 5px;
}

.buttons button {
    height: 25px;
}

.date {
    font-size: x-small;
    text-align: end;
    color: rgb(63, 62, 62);
    background: linear-gradient(to top,rgb(180, 231, 180),rgb(148, 216, 148));
    padding: 3px;
}

.theme {
    text-align: start;    
    display: flex;
    flex-direction: column;
    justify-content: start;
    margin: 15px;
}

.theme a {    
    color: black;
    background: lightgray;
    padding: 8px;
}

.theme i {
    color: var(--TEXT-GLOW-COLOR);
    margin-right: 5px;
}

.new-themes-header {
    padding-left: 15px;
    display: flex;
    flex-direction: row;
    gap: 125px;
    align-items: center;
    height: 25px;
}

.new-themes-header button {
    height: 25px;
}

.new-theme-inputs {
    margin-left: 10px;
    padding-right: 10px;
    display: flex;
    flex-direction: row;
    padding: 5px;
    align-items: center;
    gap: 5px;
    width: 92%;
    height: 100%;
}

.new-theme-inputs input {
    width: 70%;
}

.edit-chapter-button:hover {
    box-shadow: var(--GLOW-BOX-SHADOW);
    cursor: pointer;
}

h3 i {
    margin-left: 8px;
}

h3 {
    text-align: start;
}

button i {
    margin-right: 5px;
}

</style>