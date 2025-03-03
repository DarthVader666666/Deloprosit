<script setup>
import axios from 'axios';
import { ref, computed } from 'vue';
import { useToast } from 'vue-toastification';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import { RouterLink, useRouter } from 'vue-router';

const store = useStore();
const toast = useToast();
const router = useRouter();

const chapterTitle = ref('');
const imagePath = ref('');

const isDisabledCreateButton = computed(() => {    
    return chapterTitle.value.length === 0;
});

const handleCreate = async () => {
    let formData = new FormData();

    formData.append('chapterTitle', chapterTitle.value);
    formData.append('imagePath', imagePath.value);
    formData.append('dateCreated', helper.getCurrentDate());

    await axios.post(`${store.getters.serverUrl}/chapters/create`, formData,
        {
            headers:
            {
                'Content-Type': 'multipart/form-data'
            }
        })
        .then(response => {
            const status = response.status;

            if(status === 200) {
                toast.success('Раздел создан');
                chapterTitle.value = '';
                store.commit('downloadChapters');
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
    <form class="form-container" @submit.prevent="handleCreate">
        <div class="chapter-create-inputs">
            <span class="chapterTitle">Заголовок: <span class="red-star">*</span></span>
            <input v-model="chapterTitle" type="text" maxlength="120" required>
            <span class="chapterTitle">Путь к картинкe: </span>

            <select v-model="imagePath" height="50px">
                <option v-for="(path, index) in store.state.imagePaths" :key="index">
                    {{ path }}
                </option> 
            </select>

            <img :src="imagePath" width="150px" height="120px">
        </div>            
        <hr/>
        <div class="buttons">
            <button type="submit" :disabled="isDisabledCreateButton">Создать</button>
            <button type="button"><RouterLink to='/'>Отменить</RouterLink></button>
        </div>
    </form>
</template>

<style scoped>
.chapter-create-inputs {
    display: flex;
    flex-direction: column;
    font-weight: bold;
}

.chapter-create-inputs input {
    min-width: 68%;
}

.chapter-create-inputs span {
    align-content: center;
    margin: 3px;
}

.spans {
    display: flex;
    flex-direction: column;
    align-content: center;
    text-align: end;
    gap: 20px;
    margin: 3px;
}

.buttons {
    display: flex;
    flex-direction: row;
    gap: 5px;
    padding-left: 15px;
}

.red-star {
    color: red;
}

button a {
    text-decoration: none;
    color: black;
}

</style>
