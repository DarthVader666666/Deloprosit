<script setup>
import { helper } from '@/helper/helper';
import { computed, ref } from 'vue';
import { useStore } from 'vuex';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import Column from 'primevue/column';
import { FilterMatchMode } from '@primevue/core/api';


const store = useStore();
const messages = computed(() => store.getters.getMessages);

const filters = ref({
    text: { value: null, matchMode: FilterMatchMode.CONTAINS },
    name: { value: null, matchMode: FilterMatchMode.CONTAINS },
    contacts: { value: null, matchMode: FilterMatchMode.CONTAINS },
    dateSent: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

</script>

<template>
<div class="messages-container">
    <DataTable :value="messages" paginator :rows="10" :rowsPerPageOptions="[5, 10, 20, 50]" v-model:filters="filters" filterDisplay="row" :globalFilterFields="['text', 'name', 'contacts', 'dateSent']" stripedRows showGridlines >
        <Column field="text" header="Сообщение" style="width: 50%;">
            <template #body="{ data }">
                {{ data.text }}
            </template>
            <template #filter="{ filterModel, filterCallback }">
                <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
            </template>
        </Column>
        <Column field="name" header="Автор" sortable style="width: 18%" id="name">
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
        <Column field="dateSent" header="Прислано" sortable id="date-sent">
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

@media (max-width: 800px){
    .messages-container {
        padding: 3px;
    }
}

</style>