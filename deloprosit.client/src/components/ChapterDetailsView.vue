<script setup>
import axios from 'axios';
import { computed, reactive, ref } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const chapter = computed(() => store.state.chapter);
const newThemes = ref([]);

const editedChapter = reactive({
    chapterId: null,
    chapterTitle: null,
    themes: []
});

const isEdit = ref(false);

function initializeEditMode() {
    editedChapter.chapterId = chapter.value.chapterId;
    editedChapter.chapterTitle = chapter.value.chapterTitle;
    editedChapter.dateCreated = chapter.value.dateCreated;
    editedChapter.themes = chapter.value.themes;

    isEdit.value = true;
};

function handleAddTheme() {
    newThemes.value.push({
        themeId: null,
        userId: null,
        chapterId: editedChapter.chapterId,
        description: null,
        dateCreated: null
    })
}

function handleDeleteNewTheme(index) {
    newThemes.value.splice(index, 1);
}

function handleSave() {
    isEdit.value = false;

    if(editedChapter.themes.length > 0) {
        editedChapter.themes.push(newThemes);
    }
    else {
        editedChapter.themes = newThemes;
    }

    let formData = new FormData();

    formData.append('chapterId', editedChapter.chapterId );
    formData.append('chapterTitle', editedChapter.chapterTitle );
    formData.append('themes', editedChapter.themes);

    const url = store.state.serverUrl;
    axios.post(`${url}/chapters/update`, formData,
    {
        headers: {
            'Content': 'multipart/form-data'
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
                <h3>Темы раздела:</h3>
                <h4>"{{chapter.chapterTitle}}"
                    <i v-if="store.getters.isAdmin || store.getters.isOwner" class="pi pi-pen-to-square edit-chapter-button" @click="initializeEditMode"></i>
                </h4>
            </div>
            <div v-else>
                <h2>Редактирование раздела:</h2>
                <hr/>
                <form  class="form-container" @submit.prevent="handleSave">
                    <div class="chapter-inputs">
                        <div class="spans">
                            <span>Заголовок: <span class="red-star">*</span></span>
                        </div>
                        <div class="inputs">
                            <textarea v-model="editedChapter.chapterTitle" type="text"></textarea>
                        </div>
                    </div>
                    <h3 class="themes-header">Темы раздела:</h3>
                    <hr/>
                    <div v-for="(theme, index) in editedChapter.themes" :key="index" class="chapter-inputs">
                    </div>
                    <div v-for="(newTheme, index) in newThemes" :key="index" class="new-theme">
                        <span>Тема:<span class="red-star">*</span></span>
                        <textarea v-model="newTheme.description" type="text"></textarea>
                        <button @click.prevent="handleDeleteNewTheme(index)"><i class="pi pi-times"></i>Удалить</button>
                    </div>
                    <hr/>
                    <button @click.prevent="handleAddTheme"><i class="pi pi-file-plus"></i>Добавить</button>
                    <div class="buttons">
                        <button type="submit" :disabled="!editedChapter.chapterTitle || newThemes.find(x => x.description === '' || x.description === null)">Сохранить</button>
                        <button type="button" @click.prevent="handleCancel">Отменить</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<style scoped>
.edit-chapter-button:hover {
    box-shadow: var(--GLOW-BOX-SHADOW);
    cursor: pointer;
}

.chapter-inputs {
    display: flex;
    flex-direction: row;
    font-weight: bold;
    align-items: center;
}

.spans {
    display: flex;
    flex-direction: column;
    align-content: center;
    text-align: end;
    gap: 20px;
    margin: 3px;
}

.new-theme {
    display: flex;
    flex-direction: row;
    gap: 5px;
    padding: 5px;
    align-items: center;
    width: 100%;
    height: 100%;
}

button {
    height: 25px;
}

.new-theme textarea {
    width: 55%;
}

.inputs {
    display: flex;
    flex-direction: column;
    gap: 14px;
    margin: 1px;
    width: 70%;
}

.buttons {
    display: flex;
    flex-direction: row;
    gap: 5px;
    padding-left: 15px;
}

.themes-header {
    text-align: left;
}

h4 i {
    margin-left: 8px;
}

button i {
    margin-right: 5px;
}

</style>