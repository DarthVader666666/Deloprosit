<script setup>
import LeftColumnView from './LeftColumnView.vue'
import { ref } from 'vue';
import { onMounted } from 'vue';
import { computed } from 'vue';
import { useToast } from 'vue-toastification';
import axios from 'axios';

const baseUrl = ref(null);
const showNicknameError = ref(false);
const showEmailError = ref(false);
const toast = useToast();

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

const repeatPassword = ref(null);

onMounted(() => {
    baseUrl.value = import.meta.env.VITE_API_SERVER_URL;
});

const sendButtonDisabled = computed(() => {    
    return !(registerModel.value.nickname && registerModel.value.email && registerModel.value.password && passwordsMatch.value)
    || showNicknameError.value || showEmailError.value || showPasswordsError.value;
});

const passwordsMatch = computed(() => {
    return registerModel.value.password === repeatPassword.value;
});

const showPasswordsError = computed(() => {
    if(!repeatPassword.value) {
        return false;
    }
    else {
        return !passwordsMatch.value;
    }
});

const handleSend = () => {
    axios.post(`${baseUrl.value}/register/`, null,
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
    .then(response => 
    {
        if(response.status === 200) {
            toast.success('Письмо отправлено');

            registerModel.value.nickname = null;
            registerModel.value.email = null;
            registerModel.value.firstName = null;
            registerModel.value.lastName = null;
            registerModel.value.password = null;
            registerModel.value.birthDate = null;
            registerModel.value.country = null;
            registerModel.value.city = null;
            registerModel.value.title = null;
            registerModel.value.info = null;

            repeatPassword.value = null;
        }
    })
    .catch(error => {
        const status = error.response.status;
        if(status === 400) {
            toast.error('Не удалось отправить письмо');       
        }
        else if (status === 500) {
            toast.error('Ошибка сервера');
        }
    });
}

async function doesUserExist (nicknameOrEmail) {
    var url = `${baseUrl.value}/register/userExists/${nicknameOrEmail}`;
    var response = await axios.get(url);

    if(response.status == 200) {
        return response.data.userExists;
    }

    return false;
}

function validateEmail (email) {
    return email.match(
        /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
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

    return year + '-' + month + '-' + day;
}

const handleNicknameMatch = async (event) => {
    const nicknameOrEmail = event.target.value;
    showNicknameError.value = await doesUserExist(nicknameOrEmail);
}

const handleEmailMatch = async (event) => {
    const nicknameOrEmail = event.target.value;

    if(validateEmail(nicknameOrEmail)) {
        showEmailError.value = await doesUserExist(nicknameOrEmail);
    }
}

</script>
<template>
    <h2>Заполните форму регистрации</h2>
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
                <button type="submit" :disabled="sendButtonDisabled">Отправить</button>
                <button type="button"><a href="/">Отменить</a></button>
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

h2 {
    text-align: center;
}
</style>
