<script setup>
import FileUpload from 'primevue/fileupload';
import Button from 'primevue/button';
import { computed } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import axios from 'axios';

const store = useStore();
const toast = useToast();
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const documents = computed(() => store.getters.getDocuments);

function download(url, label) {
    // axios.get(url, { responseType: 'blob' })
    //   .then(response => {
    //     const blob = new Blob([response.data], { type: 'image/png' })
    //     const link = document.createElement('a')
    //     link.href = URL.createObjectURL(blob)
    //     link.download = label
    //     link.click()
    //     URL.revokeObjectURL(link.href)
    //   }).catch(console.error)
    window.open(url + label);
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
            await store.dispatch('downloadDocuments');
        }
    })
    .catch(error => {
        if(error.response.status) {
            toast.error("Ошибка загрузки файла")
        }
    })
}

async function deleteFile(fileName) {
    if(!window.confirm('Файл будет удален, вы уверены')) {
        return;
    }

    await axios.delete(`${store.state.serverUrl}/documents/delete/${fileName}`, null)
    .then( async response => {
        if(response.status === 200) {
            toast.success('Файл успешно удален');
            await store.dispatch('downloadDocuments');
        }
    })
    .catch(error => {
        if(error.response.status) {
            toast.error("Ошибка при удалении файла")
        }
    })
}

</script>
<template>
    <div class="right-container">
        <div class="items">
            <div class="items-header">
                <strong>Документы:</strong>

                <FileUpload v-if="isOwner || isAdmin" class="file-upload"
                    mode="basic" 
                    name="files"
                    :maxFileSize="20000000"
                    chooseLabel="Выгруз."
                    chooseIcon="pi pi-upload"
                    :auto="true"
                    :chooseButtonProps="
                    {
                        'severity': 'contrast',
                        'text': true,
                        'raised': true
                    }"
                    customUpload
                    @select="uploadFiles"
                />
            </div>
            <hr/>
            <div class="link" v-for="(document, index) in documents" :key="index">
                <span @click.prevent="download(document.path, document.name)">
                    <i class="pi pi-file"></i>
                    {{ document.name }} 
                </span>                
                <Button v-if="isAdmin || isOwner" @click.prevent="deleteFile(document.name)" severity="danger" text rounded><i class="pi pi-times"></i></Button>
           </div>
        </div>
    </div>        
</template>

<style scoped>
    .file-upload button {
        padding: 0;
    }

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

    .link {
        display: flex;
        flex-direction: row;
        align-items: center;
        text-decoration: none;
        color: black;
        font-size: small;
        padding: 3px 0 3px 0;
        margin: 3px 0 0 0;
        word-break: break-all;
    }

    .link i {
        font-size: small;
        margin-right: 3px;
    }

    .link button {
        height: 20px;
        width: 20px;
        margin-left: 5px;
        padding: 0;
    }

    .link button i {
        font-size: xx-small;
        padding-left: 3px;
    }

    .link span:hover {
        background: var(--SELECTED-LINK-BCKGND-CLR);
        cursor: pointer;
        color:white;
    }

    .active {
        background: var(--SELECTED-LINK-BCKGND-CLR);
        cursor: pointer;
        color:white;
    }

    @media (max-width: 1500px) {
        .items-header a span {
            display: none;
        }
    }
</style>