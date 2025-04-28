<script setup>
import ChapterCreateUpdateForm from '@/components/ChapterCreateUpdateForm.vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { helper } from '@/helper/helper';

const toast = useToast();
const router = useRouter();
const store = useStore();

async function createChapter(newChapter) {
    let formData = new FormData();

    formData.append('chapterTitle', newChapter.chapterTitle);
    formData.append('imagePath', newChapter.imagePath);
    formData.append('dateCreated', helper.getCurrentDate());

    await axios.post(`${store.getters.serverUrl}/chapters/create`, formData,
        {
            headers:
            {
                'Content-Type': 'multipart/form-data',
                'Accept': ''
            }
        })
        .then(async response => {
            const status = response.status;

            if(status === 200) {
                toast.success('Раздел создан');
                newChapter.chapterTitle = '';
                await store.dispatch('downloadChapters');
                router.push(`/chapters/${response.data.chapterId}`);
            }
        })
        .catch(error => {
            if(error.response) {
                toast.error(error.response.data.errorText)
            }
        }
    );
}

</script>

<template>
    <ChapterCreateUpdateForm :isCreateForm="true" :doClearChapter="true" :createChapter="createChapter"/>
</template>

<style scoped>

</style>
