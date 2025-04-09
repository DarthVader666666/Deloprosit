<script setup>
import { helper } from '@/helper/helper';
import { computed, reactive, ref, watch } from 'vue';
import { useStore } from 'vuex';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import Column from 'primevue/column';
import ToggleSwitch from 'primevue/toggleswitch'
import { FilterMatchMode } from '@primevue/core/api';


const store = useStore();
const messages = computed(() => store.getters.getMessages);
const checkedMessages = reactive(messages.value.map(x => 
    {
        if(!x.isRead) {
            return x.value;
        }
    }))
    
const checked = ref(true);

watch(checked, (oldValue, newValue) => {
    let r = [];
    if(newValue) {
        r = messages.value.map(x => {
            if(x.isRead) {
                console.log(x.value)
                return x.value;
            }
        })
    }
    else {
        r = messages.value.map(x => {
            if(!x.isRead) {
                return x;
            }
        })
    }

    console.log(r)
});

const filters = ref({
    text: { value: null, matchMode: FilterMatchMode.CONTAINS },
    name: { value: null, matchMode: FilterMatchMode.CONTAINS },
    contacts: { value: null, matchMode: FilterMatchMode.CONTAINS },
    dateSent: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

function onRowSelect(event) {
    const messageId = event.data.messageId;

}

</script>

<template>
<div class="messages-container">
    <div style="display: flex;align-items: center; gap:10px; justify-content: center; padding: 10px;">
        <span :style="!checked ? 'font-weight: bold;' : ''">Прочитанные</span>
        <ToggleSwitch v-model="checked"/>
        <span :style="checked ? 'font-weight: bold;' : ''">Непрочитанные</span>
    </div>
    <DataTable :value="checkedMessages" paginator :rows="10" :rowsPerPageOptions="[5, 10, 20, 50]" v-model:filters="filters" filterDisplay="row" 
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