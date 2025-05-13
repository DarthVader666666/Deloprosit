<script setup>
import { computed, ref } from 'vue';
import { helper } from '@/helper/helper';
import Button from 'primevue/button';
import { useStore } from 'vuex';
import { useToast } from  'vue-toastification';
import Tag from 'primevue/tag';
import Select from 'primevue/select';
import MultiSelect from 'primevue/multiselect';
import axios from 'axios';

const store = useStore();
const toast = useToast();

const updatedUser = ref({
    userId: null,
    deletionDate: null,
    status: null,
    roles: []
});
const user = computed(() => {
    const user = store.getters.getUser;
    defineUpdatedUserFileds(user)
    return user;
});

const selectedRoles = ref([]);
const emit = defineEmits(['user-shown']);

function defineUpdatedUserFileds(user = null) {
    updatedUser.value.userId = user.userId;
    updatedUser.value.deletionDate = user.deletionDate;
    updatedUser.value.status = user.status;
    updatedUser.value.roles = user.roles;

    selectedRoles.value = helper.userRoles.filter(x => user.roles.includes(helper.userRoles.indexOf(x)));
}

function handleUpdateStatus(status) {
    updatedUser.value.status = helper.userStatuses.indexOf(status);

    if(status === 'Удален') {
        let deletionDate = helper.getFutureDate(30);
        updatedUser.value.deletionDate = deletionDate;
    }
    else {
        updatedUser.value.deletionDate = null;
    }
}

async function handleUpdateRoles(roles) {
    updatedUser.value.roles = roles.map(x => helper.userRoles.indexOf(x));
}

function handleCancel() {
    emit('user-shown');
}

async function updateUser() {
    const url = store.state.serverUrl;

    await axios.put(`${url}/administration/updateuser`,  updatedUser.value,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success(response.data.okText);
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    });

    await store.dispatch('downloadUsers');
    emit('user-shown');
}

</script>

<template>    

<div class="user">
    <div style="display: flex; justify-content: space-between; align-items: center; padding-bottom: 30px;">
        <span style="color: red">{{ updatedUser.deletionDate && 'Пользователь будет удален ' + helper.getDateString(updatedUser.deletionDate) }}</span>
        <Button @click.prevent="handleCancel" style="width: 25px; height: 25px; margin-top: 10px;" severity="secondary" raised rounded icon="pi pi-times"/>
    </div>
    <div style="display: flex; gap: 10px; justify-content: space-between;">
        <div class="user-fields">
            <div v-if="user.nickname">
                <span style="font-weight: bold;">Никнэйм:</span>
                <span>{{ user.nickname }}</span>
            </div>
            <div v-if="user.email">
                <span style="font-weight: bold;">Email:</span>
                <span>{{ user.email }}</span>
            </div>
            <div v-if="user.phone">
                <span style="font-weight: bold;">Телефон:</span>
                <span>{{ user.phone }}</span>
            </div>
            <div v-if="user.firstName">
                <span style="font-weight: bold;">Имя:</span>
                <span>{{ user.firstName + user.lastName ? ' ' + user.lastName : '' }}</span>
            </div>
            <div v-if="user.birthDate">
                <span style="font-weight: bold;">Дата рождения:</span>
                <span>{{ user.birthDate }}</span>
            </div>
            <div v-if="user.registerDate">
                <span style="font-weight: bold;">Дата регистрации:</span>
                <span>{{ helper.getDateString(user.registerDate) }}</span>
            </div>
            <div v-if="user.status">
                <span>Статус:</span>
                <span>{{ user.status }}</span>
            </div>
        </div>
        <div style="display: flex; flex-direction: column; gap: 5px;">
            <span style="font-weight: bold;">Статус:</span>
            <Select @update:model-value="handleUpdateStatus" :options="helper.userStatuses" class="selector">
                <template #value>
                    <Tag :value="helper.userStatuses[updatedUser.status]" :severity="helper.getUserTagSeverity(updatedUser.status)"></Tag>
                </template>
                <template #option="slotProps">
                    <Tag :value="slotProps.option" :severity="helper.getUserTagSeverity(helper.userStatuses.indexOf(slotProps.option))"></Tag>
                </template>
            </Select>
            <span style="font-weight: bold;">Роли:</span>
            <MultiSelect @update:model-value="handleUpdateRoles" v-model:model-value="selectedRoles" :options="helper.userRoles" class="selector" />
        </div>
    </div>
    <div class="buttons">
        <Button type="button" @click="updateUser" raised severity="secondary" >
            <i class="pi pi-save"></i>
            <span>Сохранить</span>
        </Button>
        <Button type="button" @click="handleCancel" raised severity="contrast">
            <i class="pi pi-ban"></i>
            <span>Отменить</span>
        </Button>
    </div>  
</div>
</template>

<style scoped>
.user {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    max-height: 100%;
    width: 50%;
    box-shadow: var(--COMPONENT-BOX-SHADOW);
    position: fixed;
    z-index: 3;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    padding: 10px 20px 20px 40px;
    background: white;
    gap: 10px;
}

.user-fields {
    display: flex; 
    flex-direction: column; 
    gap: 5px;
}

.user-fields:deep(div) {
    display: flex;
    flex-direction: row;
    gap: 5px;
}

.selector {
    width: 180px;
    height: 35px;
}

.selector:deep(span) {
    padding: 3px;
    width: 100%;
    text-align: center;
}

ul li span {
    width: 100%;
    text-align: center;
}

.buttons {
    display: flex;
    flex-direction: row;
    margin-top: 20px;
    justify-content: center;
    gap: 20px;
}

@media (max-width: 1450px) {
    .user {
        width: 50%;
    }
}

@media (max-width: 1100px) {
    .user {
        width: 70%;
    }
}

@media (max-width: 800px) {
    .user {
        padding: 10px 10px 20px 10px;
        width: 100%;
    }

    .user-fields:deep(div) {
        flex-direction: column;
        gap: 0;
    }
}

</style>