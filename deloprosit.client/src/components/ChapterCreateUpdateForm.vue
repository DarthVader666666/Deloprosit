<script setup>
import { computed, onMounted, reactive } from 'vue';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext'
import Select from 'primevue/select'
import { useRouter } from 'vue-router';

const store = useStore();
const router = useRouter();
const emit = defineEmits(['cancel', 'updateChapter']);

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

const isDisabled = computed(() => {    
    return chapter.chapterTitle === null ||  chapter.chapterTitle.length === 0;
});

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

</script>

<template>
    <div>
        <form class="form-container" @submit.prevent="handleSave(chapter)">
            <div class="inputs">
                <InputText v-model="chapter.chapterTitle" type="text" :required="!isDisabled" placeholder="Заголовок раздела"/>
                <Select v-model="chapter.imagePath" :options="store.state.imagePaths" placeholder="Путь к картинке"/>
            </div>
            <div class="buttons">
                <Button type="submit" :disabled="isDisabled" raised severity="contrast" label="Сохранить"/>
                <Button type="button" @click="handleCancel" raised severity="secondary" label="Отменить"/>
            </div>        
        </form>
        <div class="image">
            <img :src="helper.getImagePath(chapter.imagePath)" width="150px" height="120px">
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
