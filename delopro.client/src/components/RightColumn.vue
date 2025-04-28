<script setup>
import FileUpload from 'primevue/fileupload';
import Button from 'primevue/button';
import TreeTable from 'primevue/treetable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import Select from 'primevue/select';
import { computed, onMounted, ref } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import axios from 'axios';
import { helper } from '@/helper/helper';

const store = useStore();
const toast = useToast();
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const documentNodes = computed(() => store.getters.getDocumentNodes);

const showNewFolderMenu = ref(false);
const showUploadMenu = ref(false);
const newFolderName = ref(null);
const folderName = ref('');
const newName = ref(null);

onMounted(() => {
    window.addEventListener('click', (event) => { if(!helper.closeMenu(event, ['create-folder-menu', 'create-folder-button'])) showNewFolderMenu.value = false });
    window.addEventListener('click', (event) => { if(!helper.closeMenu(event, ['upload-file-menu', 'upload-file-button', 'folder-select'], true)) showUploadMenu.value = false });
});

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
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    })
}

function showRenameInput(node) {
    newName.value = node.data.name;

    const names = document.querySelectorAll('[id$=_name]');
    const inputs = document.querySelectorAll('[id$=_input]');
    const editButtons = document.querySelectorAll('[id$=_edit-button]');
    const buttons = document.querySelectorAll('[id$=_buttons]');
    
    names.forEach(name => name.style.display = 'block');
    inputs.forEach(input => input.style.display = 'none');
    editButtons.forEach(editButton => editButton.style.display = 'inline-flex');
    buttons.forEach(button => button.style.display = 'none');

    document.getElementById(`${node.data.path}_${node.data.type}_edit-button`).style.display = 'none';    
    document.getElementById(`${node.data.path}_${node.data.type}_name`).style.display = 'none';
    document.getElementById(`${node.data.path}_${node.data.type}_input`).style.display = 'block';
    document.getElementById(`${node.data.path}_${node.data.type}_buttons`).style.display = 'flex';

    if(node.data.type === 'file') {
        document.getElementById(`${node.data.path}_${node.data.type}_copy-url-button`).style.display = 'none';
    }
}

function cancelEdit(node) {
    newName.value = null;

    document.getElementById(`${node.data.path}_${node.data.type}_input`).style.display = 'none';
    document.getElementById(`${node.data.path}_${node.data.type}_name`).style.display = 'block';
    document.getElementById(`${node.data.path}_${node.data.type}_edit-button`).style.display = 'inline-flex';
    document.getElementById(`${node.data.path}_${node.data.type}_buttons`).style.display = 'none';

    if(node.data.type === 'file') {
        document.getElementById(`${node.data.path}_${node.data.type}_copy-url-button`).style.display = 'inline-flex';
    }
}

function updateName(node) {
    if(newName.value === node.data.name) {
        cancelEdit(node);
        return;
    }

    axios.put(`${store.state.serverUrl}/documents/update`, 
        {
            newName: newName.value,
            oldName: node.data.name,
            path: node.data.path,
            type: node.data.type
        }
    )
    .then( async response => {
        newName.value = null;
        if(response.status === 200) {
            showUploadMenu.value = false;
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    })
}

function download(node) {
    if(node.data.path)
        window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'));
}

async function uploadFiles(event) {
    const files = event.files;
    let formData = new FormData();
    files.forEach(file => formData.append("files", file));

    if(!folderName.value) {
        folderName.value = '';
    }

    formData.append("folderName", folderName.value);

    await axios.post(`${store.state.serverUrl}/documents/upload`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
    .then( async response => {
        if(response.status === 200) {
            showUploadMenu.value = false;
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    })
}

async function deleteDocument(node) {
    if(!window.confirm(`${(node.data.type === 'file' ? `Файл "${node.data.name}" будет удален` : `Папка "${node.data.name}" и всё её содержимое будет удалено`)}, вы уверены?`)) {
        return;
    }

    await axios.post(`${store.state.serverUrl}/documents/delete`,
        {
            path: node.data.path,
            type: node.data.type
        }
    )
    .then( async response => {
        if(response.status === 200) {
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    })
}

async function copyUrlToClipboard(node) {
    const url = store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/');
    navigator.clipboard.writeText(url);
    toast.success(`Ссылка для "${node.data.name}" скопирована`);
}

</script>
<template>
    <div id="right-container">
        <div class="items">
            <div class="items-header">
                <strong style="margin: 5px 0 6px 0;">Документы:</strong>
                <Button v-if="isAdmin || isOwner"
                    @click="showNewFolderMenu = !showNewFolderMenu"
                    id="create-folder-button"
                    severity="secondary"
                    raised
                    icon="pi pi-folder-plus"
                    title="Создать папку"
                    style="height: 30px; min-width: 40px;"
                />
                <Button v-if="isAdmin || isOwner"
                    @click="showUploadMenu = !showUploadMenu"
                    id="upload-file-button"
                    severity="secondary"
                    raised
                    icon="pi pi-upload"
                    title="Выбрать файл"
                    style="height: 30px; min-width: 40px;"
                />
                <div v-if="showNewFolderMenu" class="right-column-menu" id="create-folder-menu">
                    <span>Новая папка:</span>
                    <InputText v-model="newFolderName" placeholder="Имя папки" @keydown.enter="createFolder">
                    </InputText>
                    <div class="buttons">
                        <Button severity="secondary" @click="createFolder" raised label="Создать"/>
                        <Button severity="contrast" @click="() => { newFolderName = null; showNewFolderMenu = false }" raised label="Отмена"/>
                    </div>
                </div>
                <div v-if="showUploadMenu" class="right-column-menu" id="upload-file-menu">
                    <span>Выберите путь:</span>
                    <Select :options="documentNodes.map(node => node.data.name)" v-model="folderName" id="folder-select"></Select>
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
                        <Button severity="contrast" raised @click="() => { folderName = ''; showUploadMenu = false }" label="Отмена"/>
                    </div>
                </div>
            </div>
            <hr/>
            <TreeTable :value="documentNodes" class="tree-table">
                <Column field="name" expander style="width: 90%">
                    <template #body="{ node }">
                        <i :class="node.icon" style="font-size: small;"></i>
                        <span :title="node.data.size"
                            :class="node.data.type"
                            @click="node.data.type === 'file' ? download(node) : null"
                            :style="node.data.type === 'folder' ? 'font-weight:bold;' : 'font-weight:normal;'" :id="`${node.data.path}_${node.data.type}_name`">
                            {{ node.data.name }}
                        </span>
                        <input type="text" v-model="newName" @keydown.esc="cancelEdit(node)" @keydown.enter="updateName(node)" class="rename-input" style="display: none;" :id="`${node.data.path}_${node.data.type}_input`">
                        <Button v-if="node.data.type != 'root' && (isAdmin || isOwner)" @click="showRenameInput(node)" :id="`${node.data.path}_${node.data.type}_edit-button`"
                            class="document-button" text rounded severity="contrast" icon="pi pi-pencil" title="Переименовать"></Button>
                        <Button v-if="node.data.type != 'root' && node.data.type != 'folder' && (isAdmin || isOwner)" @click="copyUrlToClipboard(node)" :id="`${node.data.path}_${node.data.type}_copy-url-button`"
                            class="document-button" text rounded severity="contrast" icon="pi pi-link" title="Копировать ссылку"></Button>

                        <div v-if="node.data.type != 'root' && (isAdmin || isOwner)" style="display: none;" :id="`${node.data.path}_${node.data.type}_buttons`">
                            <Button @click="cancelEdit(node)"
                                class="document-button" rounded severity="danger" text icon="pi pi-ban" title="Отмена"></Button>
                            <Button @click="updateName(node)"
                                class="document-button" rounded severity="primary" text icon="pi pi-check" title="Ок"></Button>
                        </div>                        
                    </template>
                </Column>
                <Column v-if="isAdmin || isOwner" style="width: 10%">
                    <template #body="{node}">
                        <Button v-if="node.data.type != 'root'" @click="deleteDocument(node)" 
                            class="document-button" rounded severity="danger" text icon="pi pi-times" title="Удалить"/>
                    </template>
                </Column>
            </TreeTable>
        </div>
    </div>        
</template>

<style scoped>
    .items {        
        text-align: start;
        padding: 10px;
    }

    .items-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        min-height: 30px;
        padding: 6px 0 0 0;
    }

    .items-header button {
        padding: 5px;
    }

    .items-header a {
        text-decoration: none;
        color: black;
    }

    .tree-table {
        font-size: small;        
    }

    .tree-table:deep(th) {
        display: none;
    }

    .tree-table:deep(td) {
        padding: 0;
        border: none;
    }

    .tree-table:deep(button) {
        height: 25px;
        width: 25px;
    }

    .tree-table:deep(.document-button) {
        height: 20px;
        width: 17px;
    }

    .tree-table:deep(.delete-button span) {
        font-size: x-small;
    }

    .tree-table:deep(.document-button span) {
        font-size: x-small;
    }

    .tree-table:deep(.file:hover) {
        cursor: pointer;
        text-decoration: underline;
    }

    .tree-table:deep(*) {
        background: var(--COLUMNS-BCKGND-CLR);
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

    .rename-input {
        max-width: 100px;
    }

    .buttons {
        display: flex;
        flex-direction: row;
        justify-content: end;
        gap: 15px;
        padding-top: 15px;
    }

    .buttons button {
        height: 38px;
        padding-left: 8px;
        padding-right: 8px;
    }

    @media (max-width: 1500px) {
        .items-header a span {
            display: none;
        }
    }
</style>