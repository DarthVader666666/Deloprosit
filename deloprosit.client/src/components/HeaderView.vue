<script setup>
import axios from 'axios';
import { useToast } from 'vue-toastification';
import { ref } from 'vue';
import { onMounted } from 'vue';
import { RouterLink } from 'vue-router';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const nickname = ref(null);
const toast = useToast();
const baseUrl = ref(null);
const environment = ref(null);

onMounted(() => {
    axios.defaults.withCredentials = true;
    baseUrl.value = import.meta.env.VITE_API_SERVER_URL;
    environment.value = import.meta.env.VITE_API_ENVIRONMENT;
})

function getUnicodeByteArray(text) {
    const utf8Encode = new TextEncoder();
    return Object.values(utf8Encode.encode(text));
}

function validateEmail (email) {
    return email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
}

const handleLogin = () => {
    const nicknameValue = validateEmail(loginRequestForm.value.nicknameOrEmail) ? '' : loginRequestForm.value.nicknameOrEmail
    const emailValue = validateEmail(loginRequestForm.value.nicknameOrEmail) ? loginRequestForm.value.nicknameOrEmail : null;

    axios.defaults.withCredentials = true;
    axios.post(`${baseUrl.value}/authorization/login?nickname=${nicknameValue}`, null,
    {
        headers: 
        {
            'Content-Type': 'application/json',
            'Authentication': JSON.stringify({
                email: emailValue,
                password: getUnicodeByteArray(loginRequestForm.value.password)
            })
        }
    })
    .then(response => {
        if(response.status === 200) {
            loginRequestForm.value.nicknameOrEmail = null;
            loginRequestForm.value.password = null;
            nickname.value = response.data.nickname;
            toast.success(`Вы вошли, как ${response.data.nickname}`);
        }
    })
    .catch(error => {
        const status = error.response.status;

        if(status === 404 || status === 400) {
            const errorText = error.response.data.errorText;
            if(errorText) {
                toast.error(errorText);
            }
            else {
                toast.error('Сервер не доступен');
            }            
        }
        else if (status === 500) {
            toast.error('Ошибка сервера');
        }

        loginRequestForm.value.nicknameOrEmail = null;
        loginRequestForm.value.password = null;
    });
}

const handleLogout = () => {
    if(window.confirm('Вы уверены, что хотите выйти?'))
    {
        axios.post(`${baseUrl.value}/authorization/logout/`, {
        headers: {
            'Content-Type': 'application/json'
        }})
        .then(response => {
            if(response.status === 200) {
                nickname.value = null;
            }
        })
        .catch(error => {
            toast.error(error.response.message);
        });
    }    
}

</script>

<template>
    <div class="header-container">        
        <div v-if="nickname" class="message">Добро пожаловать, <span>{{ nickname }}!</span>
            <button @click="handleLogout">Выйти</button>
        </div>
        <form v-else class="form-container" @submit.prevent="handleLogin">
            <div class="login-inputs" @keydown.enter.prevent="handleLogin">
                <div>
                    <label>Логин: </label>
                    <input v-model="loginRequestForm.nicknameOrEmail" type="text" placeholder="Почта или никнэйм" required>
                </div>
                <div>
                    <label>Пароль: </label>
                    <input v-model="loginRequestForm.password" type="password" placeholder="Пароль" required>
                </div>
                <button type="submit">Войти</button>
            </div>
            <div class="login-anchors">
                <RouterLink to="/register">Регистрация</RouterLink> | <a>Забыл(а) пароль</a> |
                <label for="remember">
                    <input type="checkbox" id="remember">Запомнить
                </label>
            </div>
        </form>
    </div>
</template>

<style scoped>
    .form-container {
        display: flex;
        flex-direction: column;
        justify-content: start;
        padding-right: 15px;
    }

    .login-anchors {
        align-content: center;
        padding-top: 5px;
    }

    .message {
      font-size: large;
      padding-right: 15px;
    }

    .message span {
      font-weight: bold;
      padding-right: 15px;
    }

    .header-container {
      display: flex;
      flex-direction: row;
      justify-content: end;
      padding-top: 1rem;
      padding-bottom: 1rem;
      background-image: linear-gradient(to right,rgb(165, 218, 165),rgb(72, 163, 72));
      align-content: center;
      box-shadow: 0 7px 15px -3px black;
      border-radius: 0 0 5px 5px;
    }

    a {
        color: black;
        font-size: small;
    }

    a:hover {
        color: whitesmoke;
    }

    label {
        font-weight: bold;
    }
</style>