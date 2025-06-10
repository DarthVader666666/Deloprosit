<script setup>
import FileUpload from 'primevue/fileupload';
import Button from 'primevue/button';
import TreeTable from 'primevue/treetable';
import Column from 'primevue/column';
import Select from 'primevue/select';
import { computed, nextTick, onMounted, ref } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import axios from 'axios';

const store = useStore();
const toast = useToast();
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const documentNodes = computed(() => store.getters.getDocumentNodes);
const folderPaths = computed(() => store.getters.getFolderPaths);

const newFolderName = ref(null);
const newName = ref(null);
const moveFolder = ref(null);
const editedNode = ref(null);
const editedNodeId = ref(null);
const expandedNodes = { 'docs': true }

onMounted(() => {
    // this.$refs.treeTable.$el.querySelector('.p-treetable-wrapper').tabIndex = -1;
});

function createFolder(path) {
    if(!(path && newFolderName.value)) {
        return;
    }

    axios.post(`${store.state.serverUrl}/documents/addfolder`, 
        {
            folderPath: path + '\\' + newFolderName.value
        }
    )
    .then( async response => {
        if(response.status === 200) {
            hideButtons();
            resetTempValues();
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            resetTempValues();
            toast.error(error.response.data.errorText)
        }
    });
}

function hideButtons() {
    if(!editedNode.value) {
        return;
    }
    else if(!editedNodeId.value) {
        editedNodeId.value = `${editedNode.value.data.path}_${editedNode.value.data.type}`;
    }

    const name = document.getElementById(`${editedNodeId.value}_name`);
    const rename = document.getElementById(`${editedNodeId.value}_rename`);
    const newFolder = document.getElementById(`${editedNodeId.value}_new-folder`);
    const settings = document.getElementById(`${editedNodeId.value}_settings`);
    const closeSettings = document.getElementById(`${editedNodeId.value}_close-settings`);
    const pathSelector = document.getElementById(`${editedNodeId.value}_path-selector`);

    name.style.display = 'block';
    rename.style.display = 'none';
    newFolder.style.display = 'none';
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

function showNewFolderInput() {
    hideButtons();

    document.getElementById(`${editedNodeId.value}_name`).style.display = 'none';
    document.getElementById(`${editedNodeId.value}_new-folder`).style.display = 'block';
    document.getElementById(`${editedNodeId.value}_settings`).style.display = 'none';

    const newFolderInput = document.getElementById(`${editedNodeId.value}_new-folder-input`);
    newFolderInput.focus();
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

    if(!showSettings) {
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

function cancel() {
    resetTempValues();
    hideButtons();
}

function updateName(node) {
    if(newName.value === node.data.name) {
        cancel();
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
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
            editedNode.value = null;
            editedNodeId.value = null;
            hideButtons();
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

async function uploadFiles(event, path) {
    const files = event.files;
    let formData = new FormData();
    files.forEach(file => formData.append("files", file));

    if(!path) {
        hideButtons();
        return;
    }

    formData.append("folderName", path);

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

    hideButtons();
}

async function deleteDocument() {
    if(!window.confirm(`${(editedNode.value.data.type === 'file' ? `Файл "${editedNode.value.data.name}" будет удален` : `Папка "${editedNode.value.data.name}" и всё её содержимое будет удалено`)}, вы уверены?`)) {
        return;
    }

    await axios.post(`${store.state.serverUrl}/documents/delete`,
        {
            path: editedNode.value.data.path,
            type: editedNode.value.data.type
        }
    )
    .then( async response => {
        if(response.status === 200) {
            editedNode.value = null;
            editedNodeId.value = null;
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
    const oldPath = editedNode.value.data.path.replace('...', '');
    let fileName = '\\' + editedNode.value.data.path.split('\\').at(-1);
    const newPath = moveFolder.value.replace('...', '') + fileName;

    await axios.post(`${store.state.serverUrl}/documents/move`,
        {
            oldPath: oldPath,
            newPath: newPath
        }
    )
    .then( async response => {
        if(response.status === 200) {
            editedNode.value = null;
            editedNodeId.value = null;
            hideButtons();
            resetTempValues();
            toast.success(response.data.okText);
            await store.dispatch('downloadDocumentNodes');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText);
        }
    })
}

function resetTempValues() {
    // editedNode.value = null;
    // editedNodeId.value = null;
    moveFolder.value = null;
    newFolderName.value = null;
    newName.value = null;
}

async function copyUrlToClipboard() {
    hideButtons();

    const url = store.getters.serverUrl.replace('api', '') + editedNode.value.data.path.replace('\\', '/');
    navigator.clipboard.writeText(url);
    toast.success(`Ссылка для "${editedNode.value.data.name}" скопирована`);
}

function enableArrowKeysEvents(event) {
    if ([38, 40, 37, 39].includes(event.keyCode)) {
        event.stopPropagation();
        return true;
    }
}

function disableArrowKeysEvents(event) {
    if ([38, 40, 37, 39].includes(event.keyCode)) {
        event.preventDefault();
    }
}

</script>
<template>
    <div id="right-container">
        <div class="items">
            <div class="items-header">
                <strong style="margin: 5px 0 6px 0;">Документы:</strong>
            </div>
            <hr/>

            <TreeTable :value="documentNodes" v-model:expandedKeys="expandedNodes" scrollable scrollHeight="85vh" class="tree-table" @keydown="disableArrowKeysEvents">
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

                            <div v-if="(isAdmin || isOwner)">
                                <!-- Settings -->
                                <div class="setting-buttons" :id="`${node.data.path}_${node.data.type}_settings`">                                    
                                    <div v-if="editedNode && editedNode.data.type != 'folder' && node.data.type != 'root'">
                                        <Button @click="copyUrlToClipboard"
                                            text rounded severity="contrast" icon="pi pi-link" title="Копировать ссылку"/>
                                        <Button @click="showPathSelector"
                                            text rounded severity="contrast" icon="pi pi-file-export" title="Переместить"/>
                                    </div>
                                    <FileUpload v-if="node.data.type == 'folder' || node.data.type == 'root'"
                                            mode="basic" 
                                            name="files"
                                            :multiple="true"
                                            :maxFileSize="20000000"
                                            class="p-button-icon-only"
                                            chooseIcon="pi pi-upload"
                                            :auto="true"
                                            :chooseButtonProps="
                                            {
                                                'severity': 'contrast',
                                                'text': true,
                                                'raised': false
                                            }"
                                            customUpload
                                            title="Добавить файлы"
                                            @select="uploadFiles($event, node.data.path)"
                                        />
                                    <Button v-if="node.data.type == 'folder' || node.data.type == 'root'"
                                            @click="showNewFolderInput"
                                            text rounded severity="contrast" icon="pi pi-folder-plus" title="Добавить папку"/>
                                        
                                    <div v-if="node.data.type != 'root'">
                                        <Button @click="showRenameInput"
                                            text rounded severity="contrast" icon="pi pi-pencil" title="Переименовать"/>
                                        <Button v-if="node.data.type != 'root'" @click="deleteDocument"
                                            rounded severity="danger" text icon="pi pi-trash" title="Удалить"/>
                                    </div>                                    
                                </div>

                                <!-- Move File -->
                                <div style="display: none;" :id="`${node.data.path}_${node.data.type}_path-selector`">
                                   <Select class="path-selector" :options="folderPaths" v-model="moveFolder"
                                        v-on:change="moveFile" placeholder="Путь...">
                                        <template #option="{ option }">
                                            <span style="font-size: small;">
                                                {{option}}
                                            </span>
                                        </template>
                                    </Select>
                                   <Button  @click="showSettings(node)" text rounded severity="contrast" icon="pi pi-arrow-left" title="Назад"/>
                                </div>

                                <!-- Rename -->
                                <div style="display: none;" :id="`${node.data.path}_${node.data.type}_rename`">
                                    <input type="text" v-model="newName" class="settings-input"
                                        :id="`${node.data.path}_${node.data.type}_rename-input`"
                                        @keydown.stop="enableArrowKeysEvents" @keydown.esc="showSettings(node)" @keydown.enter="updateName(node)">

                                    <Button @click="updateName(node)"
                                        rounded severity="primary" text icon="pi pi-check" title="Ок"/>
                                    <Button @click="showSettings(node)"
                                        rounded severity="danger" text icon="pi pi-ban" title="Отмена"/>
                                </div>

                                <!-- New Folder -->
                                <div style="display: none;" :id="`${node.data.path}_${node.data.type}_new-folder`">
                                    <input type="text" v-model="newFolderName" class="settings-input"
                                        :id="`${node.data.path}_${node.data.type}_new-folder-input`"
                                        @keydown.stop="enableArrowKeysEvents" @keydown.esc="showSettings(node)" @keydown.enter="createFolder(node.data.path)">

                                    <Button @click="createFolder(node.data.path)"
                                        rounded severity="primary" text icon="pi pi-check" title="Ок"/>
                                    <Button @click="showSettings(node)"
                                        rounded severity="danger" text icon="pi pi-ban" title="Отмена"/>
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
        height: auto;
        background: var(--MENU-BCKGND-CLR);
        position: absolute;
        z-index: 1;
        box-shadow: var(--MENU-BOX-SHADOW);
        border-radius: 5px;
        right: 0;
        top: 60px;
    }

    .settings-input {
        max-width: 100px;
        background-color: white;
    }

    .setting-buttons {
        display: none; 
        border: solid; 
        border-width: 1px; 
        background-color: white;
        gap: 0px
    }

    .setting-buttons:deep(*) {
        background-color: white;
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