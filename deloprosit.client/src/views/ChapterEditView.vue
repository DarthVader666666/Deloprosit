<script setup>
import ThemeList from '@/components/ThemeList.vue';
import ChapterCreateUpdateForm from '@/components/ChapterCreateUpdateForm.vue';
import axios from 'axios';
import { computed, onActivated as onMounted, ref } from 'vue';
import { useStore } from 'vuex';
import { helper } from '@/helper/helper';
import { useToast } from 'vue-toastification';
import { useRoute, useRouter } from 'vue-router';
import Editor from 'primevue/editor';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { Form } from '@primevue/forms';

const store = useStore();
const toast = useToast();
const router = useRouter();
const route = useRoute();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.state.chapter);

const newThemes = ref([]);
const isDeleteButtonActive = ref(false);
const selectedThemeIds = ref([]);

onMounted(async () => {
    store.commit('setTitle', 'Редактирование раздела');
    await store.dispatch('downloadChapter', route.params.chapterId);

    console.log(store.state.title)
});

function addBlankTheme() {
    newThemes.value.push({
        themeId: null,
        userId: null,
        chapterId: chapter.value.chapterId,
        themeTitle: null,
        content: null,
        dateCreated: null,
        dateDeleted: null,
    })
}

function deleteBlankTheme(index) {
    newThemes.value.splice(index, 1);
}

function changeDeleteButton(themeIds) {
    isDeleteButtonActive.value = themeIds != null && themeIds.length > 0;
    selectedThemeIds.value = themeIds;
}

async function handleDeleteThemes() {
    if(!window.confirm('Эти темы будут удалены. Вы уверены?')) {
        return;
    }

    if(isDeleteButtonActive.value) {
        const themeIdsQuery = helper.getQueryString(selectedThemeIds.value, 'themeIds');
        const url = `${store.state.serverUrl}/themes/deletelist` + themeIdsQuery;

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

async function submitNewTheme(index) {
    chapter.value.themes.push(newThemes.value[index]);
    newThemes.value.splice(index, 1);
}

function Cancel() {
    router.push(`/chapters/${chapter.value.chapterId}`)
}

async function updateChapter(updatedChapter) {
    const currentDate = helper.getCurrentDate();

     updatedChapter.themes.forEach(theme => {
        if(!theme.dateCreated) {
            theme.dateCreated = currentDate;
        }        
    });

    const url = store.state.serverUrl;

    await axios.put(`${url}/chapters/update`,  updatedChapter,
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
            store.dispatch('downloadChapter',  updatedChapter.chapterId);
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
        <ChapterCreateUpdateForm :doClearThemes="true" :chapter="chapter" :handleSave="updateChapter" @cancel="Cancel"/>
        <hr/>
        <div class="add-new-theme">
            <h3>Темы:</h3>
            <Button @click="addBlankTheme" raised severity="secondary" label="Добавить"/>
        </div>
        <Form v-for="(newTheme, index) in newThemes" :key="index" @submit="submitNewTheme(index)" class="new-theme-form">
            <div class="new-theme-title">
                <InputText v-model="newTheme.themeTitle" type="text" placeholder="Заголовок темы" required/>
                <Button @click="deleteBlankTheme(index)" severity="danger">Удалить</Button>
            </div>
            <Editor v-model.content="newTheme.content" editorStyle="height: 500px"/>
            <div>
                <Button type="submit" class="ok-button" severity="primary" label="OK"/>
            </div>
        </Form>
    </div>
    <ThemeList @changeDeleteButton="changeDeleteButton" :themes="chapter.themes" :useCheckboxes="true" :useShortMode="true"></ThemeList>
</div>

</template>

<style scoped>

.edit-chapter-container {
    display: flex;
    flex-direction: column;
}

.ok-button {
    margin: 10px 0 10px 0;
    float: right;
    width: 90px;
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