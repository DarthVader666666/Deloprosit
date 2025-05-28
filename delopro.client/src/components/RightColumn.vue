<script setup>
import FileUpload from 'primevue/fileupload';
import Button from 'primevue/button';
import TreeTable from 'primevue/treetable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import Select from 'primevue/select';
import { computed, nextTick, onMounted, ref } from 'vue';
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
const moveFolder = ref(null);
const editedNode = ref(null);
const editedNodeId = ref(null);

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

function hideButtons() {
    const name = document.getElementById(`${editedNodeId.value}_name`);
    const rename = document.getElementById(`${editedNodeId.value}_rename`);
    const settings = document.getElementById(`${editedNodeId.value}_settings`);
    const closeSettings = document.getElementById(`${editedNodeId.value}_close-settings`);
    const pathSelector = document.getElementById(`${editedNodeId.value}_path-selector`);
    
    name.style.display = 'block';
    rename.style.display = 'none';
    settings.style.display = 'none';
    closeSettings.style.display = 'none';
    pathSelector.style.display = 'none';
}

async function showSettings(node) {
    if(editedNode.value) {
        hideButtons();
    }

    editedNode.value = node;
    editedNodeId.value = `${node.data.path}_${node.data.type}`;

    const settingButtons = document.getElementById(`${editedNodeId.value}_settings`);
    const showSettings = document.getElementById(`${editedNodeId.value}_show-settings`);
    const closeSettings = document.getElementById(`${editedNodeId.value}_close-settings`);
    const name = document.getElementById(`${editedNodeId.value}_name`);

    await nextTick();

    settingButtons.style.display = 'inline-flex';
    showSettings.style.display = 'none';
    closeSettings.style.display = 'inline-flex';
    name.style.display = 'none';
}

function showRenameInput() {
    hideButtons();

    newName.value = editedNode.value.data.name;

    document.getElementById(`${editedNodeId.value}_name`).style.display = 'none';
    document.getElementById(`${editedNodeId.value}_rename`).style.display = 'block';
    document.getElementById(`${editedNodeId.value}_settings`).style.display = 'none';

    const renameInput = document.getElementById(`${editedNodeId.value}_rename-input`);
    renameInput.focus();
}

function showPathSelector() {
    hideButtons();

    const name = document.getElementById(`${editedNodeId.value}_name`);
    const pathSelector = document.getElementById(`${editedNodeId.value}_path-selector`);

    name.style.display = 'none';
    pathSelector.style.display = 'inline-flex';
}

function mouseEnterDocumentHandler(node) {
    const showSettings = document.getElementById(`${node.data.path}_${node.data.type}_show-settings`);

    if(!showSettings || node.data.type === 'root') {
        return;
    }

    showSettings.style.display = 'inline-flex';
}

function mouseLeaveDocumentHandler(node) {
    const showSettings = document.getElementById(`${node.data.path}_${node.data.type}_show-settings`);

    if(!showSettings) {
        return;  
    }

    showSettings.style.display = 'none';
}

function cancelRename() {
    newName.value = null;
    hideButtons();
}

function updateName(node) {
    if(newName.value === node.data.name) {
        cancelRename();
        return;
    }

    resetTempValues();

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
            editedNode.value = null;
            editedNodeId.value = null;
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

    showUploadMenu.value = false;

    await axios.post(`${store.state.serverUrl}/documents/upload`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
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

async function deleteDocument() {
    if(!window.confirm(`${(editedNode.value.data.type === 'file' ? `Файл "${editedNode.value.data.name}" будет удален` : `Папка "${editedNode.value.data.name}" и всё её содержимое будет удалено`)}, вы уверены?`)) {
        return;
    }

    resetTempValues();

    await axios.post(`${store.state.serverUrl}/documents/delete`,
        {
            path: editedNode.value.data.path,
            type: editedNode.value.data.type
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

async function moveFile() {
    hideButtons();

    const oldPath = editedNode.value.data.path;
    let pathArray = editedNode.value.data.path.split('\\');
    
    if(pathArray.length > 2 && moveFolder.value) {
        pathArray.splice(pathArray.length - 2, 1, moveFolder.value);
    }
    else if(pathArray.length === 2 && moveFolder.value) {
        pathArray.splice(pathArray.length - 1, 0, moveFolder.value);
    }
    else if (pathArray.length > 2 && !moveFolder.value) {
        pathArray.splice(pathArray.length - 2, 1);
    }
    else {
        return;
    }

    const newPath = pathArray.join('\\');

    if(newPath === oldPath) {
        return;
    }

    resetTempValues();
    
    await axios.post(`${store.state.serverUrl}/documents/move`,
        {
            oldPath: oldPath,
            newPath: newPath
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

function resetTempValues() {
    moveFolder.value = null;
    editedNode.value = null;
    editedNodeId.value = null;
}

async function copyUrlToClipboard() {
    hideButtons();

    const url = store.getters.serverUrl.replace('api', '') + editedNode.value.data.path.replace('\\', '/');
    navigator.clipboard.writeText(url);
    toast.success(`Ссылка для "${editedNode.value.data.name}" скопирована`);
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
                            :multiple="true"
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

            <TreeTable :value="documentNodes" scrollable scrollHeight="85vh" class="tree-table">
                <Column field="name" expander>
                    <template #body="{ node }">
                        <div style="display: flex; flex-direction: row; align-items: center; gap: 5px;"
                            @mouseleave="mouseLeaveDocumentHandler(node)">

                            <!-- Document name -->
                            <div @mouseenter="mouseEnterDocumentHandler(node)" 
                                :id="`${node.data.path}_${node.data.type}_name`">
                                
                                <i :class="node.icon" style="font-size: small; padding-right: 3px;"></i>

                                <span :title="node.data.size "
                                    :class="node.data.type"
                                    @click="node.data.type === 'file' ? download(node) : null"
                                    :style="node.data.type === 'folder' ? 'font-weight:bold;' : 'font-weight:normal;'">
                                    {{ node.data.name }}
                                </span>
                            </div>

                            <div v-if="node.data.type != 'root' && (isAdmin || isOwner)">
                                <!-- Settings -->
                                <div class="setting-buttons" :id="`${node.data.path}_${node.data.type}_settings`">
                                    <Button @click="showRenameInput"
                                        text rounded severity="contrast" icon="pi pi-pencil" title="Переименовать"/>
                                    <Button v-if="editedNode && editedNode.data.type != 'folder'" @click="copyUrlToClipboard"
                                        text rounded severity="contrast" icon="pi pi-link" title="Копировать ссылку"/>
                                    <Button v-if="editedNode && editedNode.data.type != 'folder'" @click="showPathSelector"
                                        text rounded severity="contrast" icon="pi pi-file-export" title="Переместить"/>
                                    <Button @click="deleteDocument" 
                                        rounded severity="danger" text icon="pi pi-trash" title="Удалить"/>
                                </div>

                                <!-- Path selector -->
                                <div style="display: none;" :id="`${node.data.path}_${node.data.type}_path-selector`">
                                   <Select class="path-selector" :options="documentNodes.map(node => node.data.name)" v-model="moveFolder"
                                        v-on:change="moveFile" placeholder="Путь..."></Select>
                                   <Button  @click="showSettings(node)" text rounded severity="contrast" icon="pi pi-arrow-left" title="Назад"/>
                                </div>

                                <!-- Rename -->
                                <div style="display: none;" :id="`${node.data.path}_${node.data.type}_rename`">
                                    <input type="text" v-model="newName" class="rename-input"
                                        :id="`${node.data.path}_${node.data.type}_rename-input`"
                                        @keydown.esc="cancelRename(node)" @keydown.enter="updateName(node)">

                                    <Button @click="cancelRename(node)"
                                        rounded severity="danger" text icon="pi pi-ban" title="Отмена"/>
                                    <Button @click="updateName(node)"
                                        rounded severity="primary" text icon="pi pi-check" title="Ок"/>
                                </div>

                                <!-- Show settings -->
                                <Button  @click="showSettings(node)"
                                    text rounded severity="contrast" icon="pi pi-cog" title="Настройки"
                                    style="display: none;"
                                    :id="`${node.data.path}_${node.data.type}_show-settings`"/>

                                <!-- Close settings -->
                                <Button  @click="hideButtons"
                                    text rounded severity="contrast" icon="pi pi-times" title="Закрыть"
                                    style="display: none;"
                                    :id="`${node.data.path}_${node.data.type}_close-settings`"/>
                            </div>
                        </div>
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
        height: 20px;
        width: 20px;
    }

    .tree-table:deep(button span) {
        font-size: small;
        background-color: transparent;
    }

    .tree-table:deep(*) {
        background: var(--COLUMNS-BCKGND-CLR);
    }

    .file:hover {
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

    .rename-input {
        max-width: 100px;
    }

    .setting-buttons {
        display: none; 
        border: solid; 
        border-width: 1px; 
        background-color: white; 
        gap: 3px
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

    .path-selector {
        height: 20px;
    }

    .path-selector:deep(span) {
        padding: 2px 0 2px 4px;
        border-radius: 20%;
    }

    @media (max-width: 1500px) {
        .items-header a span {
            display: none;
        }
    }
</style>