<script setup>
import ThemeList from '@/components/ThemeList.vue';
import axios from 'axios';
import { computed, reactive, ref, onMounted } from 'vue';
import { useStore } from 'vuex';
import { helper } from '@/helper/helper';
import { useToast } from 'vue-toastification';

const store = useStore();
const toast = useToast();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.state.chapter);
const isButtonDisabled = computed(() => !editedChapter.chapterTitle || newThemes.value.find(x => x.description === '' || x.description === null));
const isEditMode = computed(() => store.state.isEditMode);

const newThemes = ref([]);
const isDeleteButtonActive = ref(false);
const selectedThemeIds = ref([]);

let editedChapter = reactive({
    chapterId: null,
    chapterTitle: null,
    themes: reactive([])
});

onMounted(() => {
    store.commit('renderSearchBar', true);
});

function initializeEditMode() {
    editedChapter.chapterId = chapter.value.chapterId;    
    editedChapter.chapterTitle = chapter.value.chapterTitle;
    editedChapter.userId = chapter.value.userId;
    editedChapter.dateCreated = chapter.value.dateCreated;
    editedChapter.dateDeleted = chapter.value.dateDeleted;
    editedChapter.themes = [];

    store.commit('setIsEditMode', true);
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
    store.commit('setIsEditMode', false);
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
    store.commit('setIsEditMode', false);
    newThemes.value = [];
}

function handleDeleteButtonStatusChange(isActive, selectedIds) {
    isDeleteButtonActive.value = isActive;
    selectedThemeIds.value = selectedIds;
}

async function handleDeleteThemes() {
    if(isDeleteButtonActive.value) {
        const themeIdsQuery = selectedThemeIds.value.map(id => `themeIds=${id}&`).join('').slice(0, -1);
        const url = `${store.state.serverUrl}/themes/deletelist?` + themeIdsQuery;

        await axios.delete(url, null)
            .then(response => {
                const status = response.status;

                if(status === 200) {
                    toast.success('Темы успешно удалены');
                    isDeleteButtonActive.value = false;
                    store.commit('downloadChapter', chapter.value.chapterId);
                }
            })
            .catch(error => {
                const response =  error.response;

                if(response.status === 500) {
                    toast.error('Ошибка базы данных')
                }

                if(response.status === 400) {
                    toast.error(response.data.errorText);
                }
            });
    }
}

</script>
<template>
    <div class="central-container">
        <div v-if="chapter">
            <div v-if="!isEditMode">
                <div class="title">
                    <h3>{{chapter.chapterTitle}}
                        <i v-if="isAdmin || isOwner" class="pi pi-pen-to-square edit-chapter-button" @click="initializeEditMode"></i>
                    </h3>
                    <div class="delete-button">
                        <i :class="'pi pi-trash' + ` ${isDeleteButtonActive ? 'active' : 'inactive'}`" @click.prevent="handleDeleteThemes"></i>
                    </div>            
                </div>
                <hr/>
                <ThemeList @deleteButtonStatusChanged="handleDeleteButtonStatusChange" :useCheckboxes="true" :chapter="chapter"></ThemeList>
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
    align-items: center;
    justify-content: space-between;
}

.delete-button {
    margin: 5px 10px 0 0;
}

.delete-button i {
    font-size: medium;
}

.active {
    color: black;
}

.active:hover {
    cursor: pointer;
    background: var(--GLOW-BOX-SHADOW);
}

.inactive {
    color: gray;
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
    padding: 7px 0 0 5px;
}

.buttons button {
    height: 25px;
}

.new-themes-header {
    padding-left: 15px;
    display: flex;
    flex-direction: row;
    gap: 90px;
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
    width: 90%;
    height: 100%;
}

.new-theme-inputs input {
    width: 70%;
}

.new-theme-inputs button {
    width: 85px;
    height: 24px;
}

.edit-chapter-button:hover {
    box-shadow: var(--GLOW-BOX-SHADOW);
    cursor: pointer;
}

h3 i {
    margin-left: 8px;
}

h3 {
    padding-top: 5px;
    text-align: start;
}

button i {
    margin-right: 5px;
}

</style>