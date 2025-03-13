<script setup>
import ThemeList from '@/components/ThemeList.vue';
import ChapterCreateUpdateForm from '@/components/ChapterCreateUpdateForm.vue';
import axios from 'axios';
import { computed, ref } from 'vue';
import { useStore } from 'vuex';
import { helper } from '@/helper/helper';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import Editor from 'primevue/editor';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { Form } from '@primevue/forms';

const store = useStore();
const toast = useToast();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.state.chapter);

const newTheme = ref({
    themeTitle: null,
    content: null
});

const isFormActive = ref(false);

async function removeTheme(themeId) {
    if(!window.confirm('Вы уверены, что хотите удалить тему?')) {
        return;
    }

    const url = store.state.serverUrl;

    await axios.delete(`${url}/themes/delete/${themeId}`,  null,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success('Тема успешно удалена');
            store.dispatch('downloadChapters');
            store.dispatch('downloadChapter',  chapter.value.chapterId);
        }
    })
    .catch(error => {
        const status = error.response.status;
        if(status == 400) {
            toast.error('Не выбрана тема')
        }
        else {
            toast.error('Ошибка при удалении темы')
        }
    });

    clearNewTheme();    
}

function changeFormStatus() {
    const editor = document.getElementById("editor");
    editor.classList.toggle('expanded');
    editor.classList.toggle('collapsed');

    isFormActive.value = !isFormActive.value;
    clearNewTheme();
}

async function addNewTheme() {
    newTheme.value.chapterId = chapter.value.chapterId;
    newTheme.value.dateCreated = helper.getCurrentDate();

    const url = store.state.serverUrl;

    await axios.post(`${url}/themes/create`,  newTheme.value,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success('Тема успешно добавлена');
            store.dispatch('downloadChapters');
            store.dispatch('downloadChapter',  chapter.value.chapterId);
        }
    })
    .catch(error => {
        const data = error.response.data;

        if(data) {
            toast.error(data.detail);
        }
        else {
            toast.error('Ошибка при добавлении темы')
        }
    });

    changeFormStatus();
    clearNewTheme();
}

function Cancel() {
    router.push(`/chapters/${chapter.value.chapterId}`)
}

function clearNewTheme() {
    newTheme.value.themeTitle = null;
    newTheme.value.content = null;
}

async function updateChapter(updatedChapter) {
    chapter.value.chapterTitle = updatedChapter.chapterTitle;
    chapter.value.imagePath = updatedChapter.imagePath;

    const url = store.state.serverUrl;

    await axios.put(`${url}/chapters/update`,  chapter.value,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success('Раздел успешно обновлен');
            store.dispatch('downloadChapters');
            store.dispatch('downloadChapter',  chapter.value.chapterId);
            router.push(`/chapters/${chapter.value.chapterId}`)
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

</script>

<template>
<div v-if="chapter && (isAdmin || isOwner)" class="edit-chapter-container">
    <div>
        <ChapterCreateUpdateForm v-if="!isFormActive" :chapter="chapter" @updateChapter="updateChapter" @cancel="Cancel"/>
        <hr v-if="!isFormActive"/>
        <div class="add-new-theme">
            <h3>Темы:</h3>            
            <Button @click="changeFormStatus" raised :severity="isFormActive ? 'contrast' : 'secondary'">
                <i :class="isFormActive ? 'pi pi-minus' : 'pi pi-plus'"></i><span>Новая тема</span>
            </Button>
            <Button v-if="isFormActive" form="form" type="submit" raised severity="secondary">
                <i class="pi pi-save"></i><span>Добавить</span>
            </Button>
        </div>
        <div id="expand-container">
            <div class="collapsed" id="editor">
                <Form @submit="addNewTheme(index)" class="new-theme-form" id="form">
                    <InputText v-model="newTheme.themeTitle" type="text" placeholder="Заголовок темы" required/>
                    <Editor v-model.content="newTheme.content" editorStyle="height: 650px"/>
                </Form>
            </div>
        </div>
    </div>
    <ThemeList v-if="!isFormActive" :removeTheme="removeTheme" :themes="chapter.themes"></ThemeList>
</div>

</template>

<style scoped>

.edit-chapter-container {
    display: flex;
    flex-direction: column;
}

.add-new-theme {
    padding-left: 15px;
    margin-bottom: 10px;
    display: flex;
    flex-direction: row;
    gap: 90px;
    align-items: center;
    height: 35px;
}

.new-theme-form {
    display: flex;
    flex-direction: column;
    gap: 5px;
    width: 100%;
    padding-bottom: 10px;
}

#expand-container {
    overflow: hidden;
}

#editor {
    margin-top: -100%;
    transition: all 1s;
}

#editor.expanded {
    margin-top: 0;
}

.expanded {    
    animation-name: slide-in;
    animation-duration: 1s;
}

.collapsed {
    height: 300px;
    transform: translateY(-100%);
}

@keyframes slide-in {
    100% {
        transform: translateY(0%)
    }
}

@media (max-width: 800px) {
    .add-new-theme span {
        display: none;
    }
}
</style>