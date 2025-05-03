<script setup>
import { onMounted, reactive, computed, ref } from 'vue';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext'
import Select from 'primevue/select'
import { useRouter } from 'vue-router';

const store = useStore();
const router = useRouter();
const emit = defineEmits(['cancel', 'updateChapter']);
const imageNames = computed(() => store.getters.getImageNames);
const imagePath = ref(null);

const props = defineProps({
    chapter: {
        typeof: Object,
        require: false,
        default: {
            chapterId: null,
            chapterTitle: null,
            imagePath: null,
            userId: null,
            dateCreated: null,
            dateDeleted: null,
            themes: []
        }
    },
    createChapter: {
        typeof: Function,
        require: false
    },
    doClearChapter: {
        typeof: Boolean,
        default: false
    },
    isCreateForm: {
        typeof: Boolean,
        default: false
    }
});

const chapter = reactive(props.chapter)

function clearChapter() {
    chapter.chapterId = null,
    chapter.chapterTitle = null,
    chapter.imagePath = null,
    chapter.userId = null,
    chapter.dateCreated = null,
    chapter.dateDeleted = null,
    chapter.themes = []
}

onMounted(() => {
    if(props.doClearChapter) {
        clearChapter();
    }
})

function handleCancel() {
    const chapterId = chapter.chapterId;

    if(chapterId) {
        emit('cancel');
    }
    else {
        router.push('/');
    }
}

function handleSave(chapter) {
    if(props.isCreateForm) {
        props.createChapter(chapter)
    }
    else {
        emit('updateChapter', chapter)
    }
}

function handleInput() {
    document.getElementById('save-button').disabled = false;
};

function handleSelect(value) {
    imagePath.value = helper.getImagePath() + value;
    handleInput();
}

</script>

<template>
    <div>
        <form class="form-container" @submit.prevent="handleSave(chapter)">
            <div class="inputs">
                <InputText v-model="chapter.chapterTitle" @input="handleInput" type="text" required placeholder="Заголовок раздела"/>
                <Select v-model="chapter.imagePath" @update:model-value="handleSelect" :options="imageNames" placeholder="Путь к картинке"/>
            </div>
            <div class="buttons">
                <Button type="submit" disabled raised severity="secondary" label="Сохранить" id="save-button"/>
                <Button type="button" @click="handleCancel" raised severity="contrast" label="Отменить"/>
            </div>        
        </form>
        <div class="image">
            <img :src="imagePath ? imagePath : (helper.getImagePath() + chapter.imagePath)" width="150px" height="120px">
        </div>
    </div>    
</template>

<style scoped>
.image {
    margin: 10px;
}

.form-container {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    gap: 10px;
}

.inputs {
    display: flex;
    flex-direction: column;
    min-width: 70%;
    gap: 10px;
}

.buttons {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.buttons button {
    height: 35px;
    width: 100px;
}

</style>
