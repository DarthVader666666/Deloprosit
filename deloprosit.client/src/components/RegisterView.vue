<script setup>
import LeftColumnView from './LeftColumnView.vue'
import { ref } from 'vue';
import { onMounted } from 'vue';
import axios from 'axios';

const baseUrl = ref(null);
const showNicknameError = ref(false);
const showEmailError = ref(false);

const registerModel = ref({
    nickname: null,
    email: null,
    firstName: null,
    lastName: null,
    password: null,
    birthDate: null,
    country: null,
    city: null,
    title: null,
    info: null
});

onMounted(() => {
    baseUrl.value = import.meta.env.VITE_API_SERVER_URL;
});

const handleSend = () => {
    axios.post(`${baseUrl.value}/register/`, JSON.stringify(registerModel.value),
    {
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

async function doesUserExist (nicknameOrEmail) {
    var response = await axios.get(`${baseUrl.value}/register/userExists/${nicknameOrEmail}`);

    if(response.status == 200) {
        return response.data.userExists;
    }

    return false;
}

const handleNicknameMatch = async (event) => {
    const nickname = event.target.value;
    showNicknameError.value = await doesUserExist(nickname);
}

const handleEmailMatch = async (event) => {
    const nickname = event.target.value;
    showEmailError.value = await doesUserExist(nickname);
}

</script>
<template>
    <div class="register-container">
        <LeftColumnView class="left-container"/>
        <form class="register-form" @submit.prevent="handleSend">
            <div class="register-inputs">
                <div class="spans">
                    <span>Никнэйм: <span class="red-star">*</span></span>
                    <span>Email: <span class="red-star">*</span></span>
                    <span>Имя: </span>
                    <span>Фамилия: </span>
                    <span>Пароль: <span class="red-star">*</span></span>
                    <span>Повторите пароль: <span class="red-star">*</span></span>
                    <span>Дата рождения: </span>
                    <span>Страна: </span>
                    <span>Город: </span>
                    <span>Должность: </span>                
                </div>
                <div class="inputs">
                    <input v-model="registerModel.nickname" @input.prevent="handleNicknameMatch" type="text" maxlength="30" required>
                        <span v-if="showNicknameError" class="user-exists-error">Никнэйм занят</span>
                    <input v-model="registerModel.email" @input.prevent="handleEmailMatch" type="email" maxlength="50" required>
                        <span v-if="showEmailError" class="user-exists-error">Email занят</span>
                    <input v-model="registerModel.firstName" type="text" maxlength="30">
                    <input v-model="registerModel.lastName" type="text" maxlength="30">
                    <input v-model="registerModel.password" type="password" maxlength="30" required>
                    <input type="password" maxlength="30" required>
                    <input v-model="registerModel.birthDate" type="date">
                    <input v-model="registerModel.country" type="text" maxlength="30">
                    <input v-model="registerModel.city" type="text" maxlength="30">
                    <input v-model="registerModel.title" type="text" maxlength="30">                
                </div>
            </div>            
            <hr/>
            <div class="info">
                <span>Дополнительно: </span>
                <textarea v-model="registerModel.info" maxlength="800"></textarea>
            </div>
            <hr/>
            <div class="buttons">
                <button type="submit">Отправить</button>
                <button>Отменить</button>
            </div>
        </form>
    </div>
</template>
<style scoped>
.left-container {
    width: 20%;
}

.register-inputs {
    display: flex;
    flex-direction: row;
    font-weight: bold;
}

.spans {
    display: flex;
    flex-direction: column;
    align-content: center;
    text-align: end;
    gap: 10px;
    margin: 3px;
}

.inputs {
    display: flex;
    flex-direction: column;
    gap: 4px;
    margin: 1px;
}

.info {
    display: flex;
    flex-direction: column;
    margin: 10px;
}

.info span {
    font-weight: bold;
}

.info textarea {
    max-width: 60%;
    min-width: 300px;
    height: 150px;
}

.buttons {
    display: flex;
    flex-direction: row;
    gap: 5px;
    padding-left: 15px;
}

.red-star {
    color: red;
}

.user-exists-error {
    color: red;
    font-weight: lighter;
    font-size: x-small;
}
</style>
