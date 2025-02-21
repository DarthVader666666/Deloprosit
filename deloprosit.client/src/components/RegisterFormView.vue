<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import { useToast } from 'vue-toastification';

const props = defineProps({
    pending: {
        type: Boolean,
        default: false
    }
});

const emit = defineEmits(['email-sent','show-notification']);

const toast = useToast();

const baseUrl = ref(null);
const showNicknameError = ref(false);
const showEmailError = ref(false);

const registerModel = ref({
    nickname: '',
    email: '',
    firstName: '',
    lastName: '',
    password: '',
    birthDate: '',
    country: '',
    city: '',
    title: '',
    info: ''
});

const repeatPassword = ref(null);

onMounted(() => {
    baseUrl.value = import.meta.env.VITE_API_SERVER_URL;
});

const isDisabledSendButton = computed(() => {    
    return !(registerModel.value.nickname && registerModel.value.email && registerModel.value.password && isMatchPassword.value)
    || showNicknameError.value || showEmailError.value || showPasswordsError.value || props.pending;
});

const isMatchPassword = computed(() => {
    return registerModel.value.password === repeatPassword.value;
});

const showPasswordsError = computed(() => {
    if(!repeatPassword.value) {
        return false;
    }
    else {
        return !isMatchPassword.value;
    }
});

const handleSend = () => {
    const promise = axios.post(`${baseUrl.value}/register/`, null,
    {
        headers: {
            'Content-Type': 'application/json',
            'Registration': JSON.stringify(
                {
                    nickname: getUnicodeByteArray(registerModel.value.nickname),
                    email: registerModel.value.email,
                    firstName: getUnicodeByteArray(registerModel.value.firstName),
                    lastName: getUnicodeByteArray(registerModel.value.lastName),
                    password: getUnicodeByteArray(registerModel.value.password),
                    birthDate: registerModel.value.birthDate,
                    registerDate: getCurrentDate(),
                    country: getUnicodeByteArray(registerModel.value.country),
                    city: getUnicodeByteArray(registerModel.value.city),
                    title: getUnicodeByteArray(registerModel.value.title),
                    info: getUnicodeByteArray(registerModel.value.info)
                })
        }
    })
    .then(response => {
        const status = response.status;

        if(status === 200) {
            toast.success('Письмо отправлено');
            return true;
        }

        return false;
    });

    emit('email-sent', promise);
}

function timeoutAsync(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function doesUserExist (nickname, email) {
    await timeoutAsync(200);

    var url = `${baseUrl.value}/register/userExists?` + (nickname ? `nickname=${nickname}` : `email=${email}`);
    var response = await axios.get(url);

    if(response.status == 200) {
        return response.data.userExists;
    }

    return false;
}

function validateEmail (email) {
    return email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
}

function getUnicodeByteArray(text) {
    const utf8Encode = new TextEncoder();
    return Object.values(utf8Encode.encode(text));
}

function getCurrentDate() {
    const today = new Date();
    const day = String(today.getDate()).padStart(2, '0');
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const year = today.getFullYear();
    const hours = today.getHours();
    const minutes = today.getMinutes();
    const seconds = today.getSeconds();

    return year + '-' + month + '-' + day + ' ' + hours + ':' + minutes + ':' + seconds;
}

const handleNicknameMatch = async (event) => {
    const nickname = event.target.value;
    showNicknameError.value = await doesUserExist(nickname, null);
}

const handleEmailMatch = async (event) => {
    const email = event.target.value;

    if(validateEmail(email)) {
        showEmailError.value = await doesUserExist(null, email);
    }
}
</script>

<template>
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
                <div class="input-with-error-message">
                    <input v-model="registerModel.nickname" @input.prevent="handleNicknameMatch" type="text" maxlength="30" required>
                    <span v-if="showNicknameError" class="error-message">Никнэйм занят</span>
                </div>
                <div class="input-with-error-message">
                    <input v-model="registerModel.email" @input.prevent="handleEmailMatch" type="email" maxlength="50" required>
                    <span v-if="showEmailError" class="error-message">Email занят</span>
                </div>                    
                <input v-model="registerModel.firstName" type="text" maxlength="30">
                <input v-model="registerModel.lastName" type="text" maxlength="30">
                <input v-model="registerModel.password" type="password" maxlength="30" required>
                <div class="input-with-error-message">
                    <input v-model="repeatPassword" type="password" maxlength="30" required>
                    <span v-if="showPasswordsError" class="error-message">Пароли не совпадают</span>
                </div>
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
            <button type="submit" :disabled="isDisabledSendButton">Отправить</button>
            <button type="button"><a href="/">Отменить</a></button>
        </div>
    </form>
</template>

<style scoped>
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
    gap: 20px;
    margin: 3px;
}

.inputs {
    display: flex;
    flex-direction: column;
    gap: 14px;
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
    height: 110px;
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

.error-message {
    position:absolute;
    margin-top: 20px;
    color: red;
    font-weight: lighter;
    font-size: x-small;
}

.input-with-error-message {
    display: flex;
    flex-direction: column;
}

button a {
    text-decoration: none;
    color: black;
}
</style>
