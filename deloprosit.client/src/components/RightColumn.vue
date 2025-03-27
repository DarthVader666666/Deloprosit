<script setup>
import FileUpload from 'primevue/fileupload';
import Button from 'primevue/button';
import Tree from 'primevue/tree';
import InputText from 'primevue/inputtext';
import Select from 'primevue/select';
import { computed, ref } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import axios from 'axios';

const store = useStore();
const toast = useToast();
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const documentNodes = computed(() => store.getters.getDocumentNodes);

const showNewFolderMenu = ref(false);
const showUploadMenu = ref(false);
const newFolderName = ref(null);
const folderName = ref('');

function createFolder() {
    if(!newFolderName.value) {
        newFolderName.value = '';
        return;
    }

    axios.post(`${store.state.serverUrl}/documents/addfolder`, 
        {
            folderName: newFolderName.value
        }
    )
    .then( async response => {
        if(response.status === 200) {
            newFolderName.value = '';
            showNewFolderMenu.value = false;
            toast.success('Папка успешно создана');
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data);
        }
        else {
            toast.error(error.message);
        }
    })
}

function download(node) {
    if(node.data)
        window.open(node.data);
}

async function uploadFiles(event) {
    const files = event.files;
    let formData = new FormData();
    files.forEach(file => formData.append("files", file));
    formData.append("folderName", folderName.value);

    await axios.post(`${store.state.serverUrl}/documents/upload`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
    .then( async response => {
        if(response.status === 200) {
            showUploadMenu.value = false;
            toast.success('Файл успешно загружен');
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data);
        }
        else {
            toast.error("Ошибка загрузки файла");
        }
    })
}

async function deleteFile(filePath) {
    if(!window.confirm('Файл будет удален, вы уверены')) {
        return;
    }

    await axios.post(`${store.state.serverUrl}/documents/delete`,
        {
            filePath: filePath
        }
    )
    .then( async response => {
        if(response.status === 200) {
            toast.success('Файл успешно удален');
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data);
        }
        else {
            toast.error("Ошибка при удалении файла");
        }
    })
}

</script>
<template>
    <div class="right-container">
        <div class="items">
            <div class="items-header">
                <strong>Документы:</strong>
                <Button v-if="isAdmin || isOwner"
                    @click="showNewFolderMenu = !showNewFolderMenu"
                    severity="secondary"
                    raised
                    icon="pi pi-folder-plus"
                    title="Создать папку"
                    style="height: 30px; min-width: 40px;"
                />
                <Button v-if="isAdmin || isOwner"
                    @click="showUploadMenu = !showUploadMenu"
                    severity="secondary"
                    raised
                    icon="pi pi-upload"
                    title="Выбрать файл"
                    style="height: 30px; min-width: 40px;"
                />
                <div v-if="showNewFolderMenu" class="right-column-menu">
                    <span>Новая папка:</span>
                    <InputText v-model="newFolderName" placeholder="Имя папки">
                    </InputText>
                    <div class="buttons">
                        <Button severity="secondary" @click="createFolder" @keydown.enter="createFolder" raised label="Создать"/>
                        <Button severity="contrast" @click="() => { newFolderName = null; showNewFolderMenu = false }" label="Отмена"/>
                    </div>
                </div>
                <div v-if="showUploadMenu" class="right-column-menu">
                    <span>Выберите путь:</span>
                    <Select :options="documentNodes.map(x => x.key)" v-model="folderName"></Select>
                    <div class="buttons">
                        <FileUpload
                            mode="basic" 
                            name="files"
                            :maxFileSize="20000000"
                            chooseLabel="Файл"
                            chooseIcon="pi pi-upload"
                            :auto="true"
                            :chooseButtonProps="
                            {
                                'severity': 'secondary',
                                'text': false,
                                'raised': true
                            }"
                            customUpload
                            title="Выгрузить файл"
                            @select="uploadFiles"
                        />
                        <Button severity="contrast" @click="() => { folderName = null; showUploadMenu = false }" label="Отмена"/>
                    </div>
                </div>
            </div>
            <hr/>
            <Tree :value="documentNodes" class="tree">
                <template #url="{ node }">
                    <div>
                        <span @click="download(node)" class="file-name">{{ node.label }}</span>
                        <Button v-if="isAdmin || isOwner" @click="deleteFile(node.key)" severity="danger" text rounded title="Удалить файл" icon="pi pi-times"/>
                    </div>
                </template>
            </Tree>
        </div>
    </div>        
</template>

<style scoped>
    .items {        
        text-align: start;
        padding: 1rem;
    }

    .items-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        min-height: 30px;
    }

    .items-header button {
        padding: 5px;
    }

    .items-header a {
        text-decoration: none;
        color: black;
    }

    .tree {
        padding: 0;
        font-size: small;
        background: var(--COLUMNS-BCKGND-CLR);
    }

    .tree:deep(div) {
        padding: 1px;
    }

    .tree:deep(li) {
        font-weight: bold;
    }

    .tree:deep(li) {
        font-weight: bold;
    }

    .tree:deep(*) {
        padding: 0;
        margin: 0;
        font-size: small;
    }

    .tree:deep(span span) {
        font-weight: normal;
    }

    .tree:deep(span button) {
        padding-top: 2px;
        margin-left: 10px;
        height: 16px;
        width: 16px;
    }
    
    .tree:deep(span button span) {
        font-size: x-small;
    }

    .tree:deep(.file-name:hover) {
        cursor: pointer;
        text-decoration: underline;
    }

    .right-column-menu {
        display: flex;
        flex-direction: column;
        padding: 20px;
        width: 300px;
        height: 140px;
        background: var(--MENU-BCKGND-CLR);
        position: absolute;
        z-index: 1;
        box-shadow: var(--MENU-BOX-SHADOW);
        border-radius: 5px;
        right: 0;
        top: 60px;
    }

    .buttons {
        display: flex;
        flex-direction: row;
        justify-content: end;
        gap: 15px;
        padding-top: 15px;
    }

    @media (max-width: 1500px) {
        .items-header a span {
            display: none;
        }
    }
</style>