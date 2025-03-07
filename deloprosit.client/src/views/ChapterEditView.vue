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
import { useRoute } from 'vue-router';

const store = useStore();
const toast = useToast();
const router = useRouter();
const route = useRoute();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.state.chapter);

const newTheme = ref({
    themeTitle: null,
    content: null
});

const isFormActive = ref(false);

async function removeTheme(themeId) {
    const theme = await findTheme(themeId);
    const index =  chapter.value.themes.indexOf(theme);
    chapter.value.themes.splice(index, 1);
}

function changeFormStatus() {
    isFormActive.value = !isFormActive.value;
    clearNewTheme();
}

async function submitNewTheme() {
    const theme = 
    {
        themeId: null,
        userId: null,
        chapterId: null,
        themeTitle: newTheme.value.themeTitle,
        content: newTheme.value.content,
        dateCreated: null,
        dateDeleted: null
    }

    chapter.value.themes.push(theme);
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

async function findTheme(themeId) {
    let theme = chapter.value.themes.find(x => x.themeId === themeId);

    if(!theme) {
        await store.dispatch('downloadChapter', route.params.chapterId);
        theme = chapter.value.themes.find(x => x.themeId === themeId);
    }

    return theme;
}

async function updateChapter() {
    const currentDate = helper.getCurrentDate();

     chapter.value.themes.forEach(theme => {
        if(!theme.dateCreated) {
            theme.dateCreated = currentDate;
        }
    });

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
    <div >
        <ChapterCreateUpdateForm :chapter="chapter" @updateChapter="updateChapter" @cancel="Cancel"/>
        <hr/>
        <div class="add-new-theme">
            <h3>Темы:</h3>            
            <Button v-if="!isFormActive" @click="changeFormStatus" raised severity="secondary" icon="pi pi-arrow-down" label="Новая тема"/>
            <Button v-else @click="changeFormStatus" raised severity="contrast" icon="pi pi-arrow-up" label="Новая тема"/>
            <Button form="form" type="submit" raised severity="secondary" icon="pi pi-save" label="Добавить"/>
        </div>
        <Form v-if="isFormActive" @submit="submitNewTheme(index)" class="new-theme-form" id="form">
            <div class="new-theme-title">
                <InputText v-model="newTheme.themeTitle" type="text" placeholder="Заголовок темы" required/>
            </div>
            <Editor v-model.content="newTheme.content" editorStyle="height: 500px"/>
        </Form>
    </div>
    <ThemeList v-if="!isFormActive" @removeTheme="removeTheme" :themes="chapter.themes" :useDeleteButtons="true" :useShortMode="true"></ThemeList>
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

.add-new-theme button {
    height: 35px;
}

.new-theme-form {
    display: flex;
    flex-direction: column;
    gap: 5px;
    width: 100%;
    padding-bottom:10px;
}

.new-theme-title {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
}

.new-theme-title input {
    width: 85%;
}

.new-theme-title button {
    width: 90px;
}

</style>