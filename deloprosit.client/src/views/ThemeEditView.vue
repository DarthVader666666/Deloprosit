<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';
import axios from 'axios';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import { Form } from '@primevue/forms';
import Editor from 'primevue/editor';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';

const store = useStore();
const toast = useToast();
const router = useRouter();

const chapter = computed(() => store.getters.getChapter);
const theme = computed(() => store.getters.getTheme);

function handleInput() {
    document.getElementById('save-button').disabled = false;
};

async function updateTheme() {
    const url = store.state.serverUrl;

    await axios.put(`${url}/themes/update`,  theme.value,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success('Тема успешно обновлена');
            store.dispatch('downloadChapters');
            store.dispatch('downloadChapter',  chapter.value.chapterId);
            store.dispatch('downloadTheme',  theme.value.themeId);

            router.push(`/chapters/${chapter.value.chapterId}/${theme.value.themeId}`)
        }
    })
    .catch(error => {
        toast.error('Ошибка при обновлении темы: ' + error.message);
    });
}

function handleCancel() {
    router.push(`/chapters/${chapter.value.chapterId}/${theme.value.themeId}`)
}

</script>

<template>
<div v-if="theme" class="theme-edit-container">
    <Form @submit="updateTheme" class="edit-theme-form" id="form">
        <div class="upper-part">
            <InputText v-model="theme.themeTitle" type="text" placeholder="Заголовок темы" required @input="handleInput"/>
            <div class="buttons">
                <Button type="submit" raised severity="secondary" id="save-button" disabled>
                    <i class="pi pi-save"></i>
                    <span>Сохранить</span>
                </Button>
                <Button type="button" @click="handleCancel" raised severity="contrast">
                    <i class="pi pi-ban"></i>
                    <span>Отменить</span>
                </Button>
            </div>
        </div>
        <Editor v-model.content="theme.content" editorStyle="height: 650px" @input="handleInput"/>
    </Form>
</div>

</template>

<style scoped>

.theme-edit-container {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.upper-part {
    display: flex;
    flex-direction: row;
    gap: 10px;
    justify-content: space-between;
}

.upper-part input {
    width: 70%;
}

.buttons {
    display: flex;
    flex-direction: row;
    justify-content: end;
    gap: 10px;
}

.buttons button {
    padding: 6px;
}

.edit-theme-form {
    display: flex;
    flex-direction: column;
    gap: 10px;
    width: 100%;
    padding-bottom: 10px;
}

@media (max-width:800px) {
    .buttons button {
        padding: 10px;
        margin-right: 5px;
    }

    .buttons span {
        display: none;
    }
}
</style>