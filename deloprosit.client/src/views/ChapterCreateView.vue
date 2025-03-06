<script setup>
import ChapterCreateUpdateForm from '@/components/ChapterCreateUpdateForm.vue';
import { useStore } from 'vuex';
import { onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { helper } from '@/helper/helper';

const toast = useToast();
const router = useRouter();
const store = useStore();

onMounted(async () => {
    store.commit('setTitle', 'Создание нового раздела');
});

async function createChapter(newChapter) {
    let formData = new FormData();

    formData.append('chapterTitle', newChapter.chapterTitle);
    formData.append('imagePath', newChapter.imagePath);
    formData.append('dateCreated', helper.getCurrentDate());

    await axios.post(`${store.getters.serverUrl}/chapters/create`, formData,
        {
            headers:
            {
                'Content-Type': 'multipart/form-data'
            }
        })
        .then(async response => {
            const status = response.status;

            if(status === 200) {
                toast.success('Раздел создан');
                newChapter.chapterTitle = '';
                await store.dispatch('downloadChapters');
                router.push('/');
            }
        })
        .catch(error => {
            const status = error.response.status;

            if(status === 400) {
                toast.error("Не удалось создать раздел");
            }

            if(status === 500) {
                toast.error("Ошибка сервера")
            }
        }
    );
}

</script>

<template>
    <ChapterCreateUpdateForm :doClearChapter="true" :handleSave="createChapter"/>
</template>

<style scoped>

</style>
