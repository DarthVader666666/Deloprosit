<script setup>
import { helper } from '@/helper/helper';
import { computed, onBeforeMount, ref } from 'vue';
import { useStore } from 'vuex';
import { FilterMatchMode } from '@primevue/core/api';
import DataTable from 'primevue/datatable';
import InputText from 'primevue/inputtext';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import UserComponent from '@/components/UserComponent.vue';
import { useToast } from  'vue-toastification';
import axios from 'axios';

const store = useStore();
const users = computed(() => store.getters.getUsers);

const filters = ref({
    nickname: { value: null, matchMode: FilterMatchMode.CONTAINS },
    registerDate: { value: null, matchMode: FilterMatchMode.CONTAINS },
});

const toast = useToast();
const showUser = ref(false);

onBeforeMount(() => {
    showUser.value = false;
    store.commit('setUsers', []);
    store.commit('setUser', null);
})

async function onRowSelect(event) {
    const userId = event.data.userId;
    await store.dispatch('downloadUser', userId);
    showUser.value = true;
}

function closeUserModal() {
    showUser.value = false;
    store.commit('setUser', null);
}
</script>

<template>
<div class="users-container">
    <DataTable :value="users" paginator :rows="10" :rowsPerPageOptions="[5, 10, 20, 50]" v-model:filters="filters" filterDisplay="row" 
        :globalFilterFields="['nickname', 'registerDate', 'roles', 'status']" stripedRows showGridlines selectionMode="single" @rowSelect="onRowSelect">
        <Column field="nickname" header="Никнэйм" sortable>
            <template #body="{ data }">
                <div style="display: flex; align-items: center; gap: 20px;">
                    <img :src="`data:image;base64,${data.avatar}`" style="border-radius: 50%;height: 50px; width: 50px;" >
                    <span>{{ data.nickname }}</span>
                </div>
            </template>
            <template #filter="{ filterModel, filterCallback }">
                <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
            </template>
        </Column>
        <Column field="registerDate" header="Дата регистрации" sortable>
            <template #body="{ data }">
                {{ helper.getDateString(data.registerDate, true) }}
            </template>
            <template #filter="{ filterModel, filterCallback }">
                <InputText v-model="filterModel.value" type="text" @input="filterCallback()" style="width:100%" placeholder="Поиск" />
            </template>
        </Column>
        <Column field="roles" header="Роли" sortable>
            <template #body="{ data }">
                {{data.roles}}
            </template>
        </Column>
        <Column field="status" header="Статус" sortable>
            <template #body="{ data }">
                <Tag :value="helper.userStatuses[data.status]" :severity="helper.getUserTagSeverity(data.status)"></Tag>
            </template>
        </Column>
    </DataTable>
    <UserComponent v-if="showUser" @user-shown="closeUserModal"></UserComponent>
</div>
</template>

<style scoped>
.users-container {
    padding: 20px;
}

.users-container:deep(td) {
    white-space: nowrap; 
    max-width: 200px;
    overflow: hidden;
    text-overflow: ellipsis;
}

@media (max-width: 800px){
    .users-container {
        padding: 3px;
    }
}
</style>