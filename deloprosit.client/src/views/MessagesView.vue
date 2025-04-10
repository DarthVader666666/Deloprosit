<script setup>
import { helper } from '@/helper/helper';
import { computed, onBeforeUnmount, ref } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { FilterMatchMode } from '@primevue/core/api';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import Column from 'primevue/column';
import ToggleSwitch from 'primevue/toggleswitch'
import MessageComponent from '@/components/MessageComponent.vue';
import axios from 'axios';

const store = useStore();
const toast = useToast();
const messages = computed(() => store.getters.getMessages);
const message = computed(() => store.getters.getMessage);

const isRead = ref(false);
const showMessage = ref(false);

const filters = ref({
    text: { value: null, matchMode: FilterMatchMode.CONTAINS },
    name: { value: null, matchMode: FilterMatchMode.CONTAINS },
    contacts: { value: null, matchMode: FilterMatchMode.CONTAINS },
    dateSent: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

onBeforeUnmount(() => {
    showMessage.value = false;
    store.commit('setMessages', []);
    message.value - null;
})

async function onRowSelect(event) {
    const messageId = event.data.messageId;
    //await store.dispatch('downloadMessage', messageId);
    store.commit('setMessageById', messageId);

    if(message.value) {
        await axios.put(`${store.state.serverUrl}/feedback/update/${messageId}`)
            .catch(error => {
                if(error.response) {
                    toast.error(error.response.data.errorText);
                }
            });
            
        await store.dispatch('downloadMessages', isRead.value);
        showMessage.value = true;
    }
}

function closeMessageModal() {
    showMessage.value = false;
    store.commit('setMessage', null);
}

</script>

<template>
<div class="messages-container">
    <div>
        <div style="display: flex; align-items: center; gap:10px; justify-content: center; padding: 10px;">        
            <ToggleSwitch v-model="isRead" @change.prevent="() => { store.dispatch('downloadMessages', isRead) }">
                <template #handle="{ checked }">
                    <span :style="`${!checked ? 'padding-right:160px;color:black;font-weight:bold;' : 'display:none;'}`">Непрочитанные</span>
                    <span :style="`${checked ? 'padding-left:140px;color:black;font-weight:bold;' : 'display:none;'}`">Прочитанные</span>
                </template>
            </ToggleSwitch>
        </div>
        <DataTable :value="messages" paginator :rows="10" :rowsPerPageOptions="[5, 10, 20, 50]" v-model:filters="filters" filterDisplay="row" 
            :globalFilterFields="['text', 'name', 'contacts', 'dateSent']" stripedRows showGridlines selectionMode="single" @rowSelect="onRowSelect">
            <Column field="text" header="Сообщение">
                <template #body="{ data }">
                    {{ data.text }}         
                </template>
                <template #filter="{ filterModel, filterCallback }">
                    <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
                </template>
            </Column>
            <Column field="name" header="Автор" sortable style="width: 15%" id="name">
                <template #body="{ data }">
                    {{ data.name }}
                </template>
                <template #filter="{ filterModel, filterCallback }">
                    <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
                </template>
            </Column>
            <Column field="contacts" header="Контакты" style="white-space: pre-wrap; width: 18%;" id="contacts">
                <template #body="slotProps">
                    {{slotProps.data.contacts}}
                </template>
                <template #filter="{ filterModel, filterCallback }">
                    <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
                </template>
            </Column>
            <Column field="dateSent" header="Прислано" style="width: 15%;" sortable id="date-sent">
                <template #body="slotProps">
                    {{ helper.getDateString(slotProps.data.dateSent, true) }}
                </template>
                <template #filter="{ filterModel, filterCallback }">
                    <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
                </template>
            </Column>
        </DataTable>
    </div>
    <MessageComponent v-if="showMessage" :message="message" @message-shown="closeMessageModal"></MessageComponent>
</div>
</template>

<style scoped>
.messages-container {
    padding: 20px;
}

.messages-container:deep(td) {
    white-space: nowrap; 
    max-width: 200px;
    overflow: hidden;
    text-overflow: ellipsis;
}

@media (max-width: 800px){
    .messages-container {
        padding: 3px;
    }
}

</style>