<script setup>
import { computed, reactive, ref } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const chapter = computed(() => store.state.chapter);
//const themes = computed(() => store.state.themes);

const editedChapter = reactive({
    chapterId: null,
    chapterTitle: null,
    themes: []
});

const isEdit = ref(false);

function initializeEditMode() {
    editedChapter.chapterId = chapter.value.chapterId;
    editedChapter.chapterTitle = chapter.value.chapterTitle;
    editedChapter.themes = chapter.value.themes;

    isEdit.value = true;
};

function handleAddTheme() {
    editedChapter.themes.push({
        themeId: null,
        userId: null,
        chapterId: editedChapter.chapterId,
        description: null,
        dateCreated: null
    })
}

function handleSave() {
    isEdit.value = false;

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
            <form v-else class="form-container">
                <h2>Редактирование раздела:</h2>
                <hr/>
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
                    <div class="new-theme">
                        <span>Тема: <span class="red-star">*</span></span>
                        <textarea v-model="theme.description" type="text"></textarea><button>Удалить</button>
                    </div>
                </div>
                <hr/>
                <button @click.prevent="handleAddTheme"><i class="pi pi-file-plus"></i>Добавить</button>
                <div class="buttons">
                    <button type="submit" :disabled="!editedChapter.chapterTitle" @click.prevent="handleSave">Сохранить</button>
                    <button type="button" @click.prevent="() => isEdit = false">Отменить</button>
                </div>
            </form>
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
    width: 600px;
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