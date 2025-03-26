<script setup>
import FileUpload from 'primevue/fileupload';
import Button from 'primevue/button';
import Tree from 'primevue/tree'
import { computed, ref } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import axios from 'axios';

const store = useStore();
const toast = useToast();
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const documentNodes = computed(() => store.getters.getDocumentNodes);

const selectedKey = ref(null);

function download(node) {
    if(node.data)
        window.open(node.data);
}

async function uploadFiles(event) {
    const files = event.files;
    let formData = new FormData();
    files.forEach(file => formData.append("files", file));

    await axios.post(`${store.state.serverUrl}/documents/upload`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
    .then( async response => {
        if(response.status === 200) {
            toast.success('Файл успешно загружен');
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response.status) {
            toast.error("Ошибка загрузки файла")
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

                <FileUpload v-if="isOwner || isAdmin"
                    mode="basic" 
                    name="files"
                    :maxFileSize="20000000"
                    chooseLabel=" "
                    chooseIcon="pi pi-upload"
                    style="height: 30px; width: 40px; padding-left: 20px;"
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
            </div>
            <hr/>
            <Tree :value="documentNodes" class="tree" v-model:selectionKeys="selectedKey" selectionMode="single" @nodeSelect="download">
                <template #url="{ node }">
                    <div>
                        <span>{{ node.label }}</span>                    
                        <Button v-if="isAdmin || isOwner" @click="deleteFile(node.data)" severity="danger" text rounded icon="pi pi-times"/>
                    </div>
                </template>
            </Tree>
            <!-- <div class="link" v-for="(document, index) in documents" :key="index">
                <span @click.prevent="download(document.path, document.name)">
                    <i class="pi pi-file"></i>
                    {{ document.name }} 
                </span>                
                <Button v-if="isAdmin || isOwner" @click.prevent="deleteFile(document.name)" severity="danger" text rounded><i class="pi pi-times"></i></Button>
           </div> -->
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
        margin-left: 10px;
        height: 16px;
        width: 16px;
    }
    
    .tree:deep(span button span) {
        font-size: x-small;
    }    

    @media (max-width: 1500px) {
        .items-header a span {
            display: none;
        }
    }
</style>