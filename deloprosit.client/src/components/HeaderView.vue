<script setup>
import axios from 'axios';
import { ref } from 'vue';
import { onMounted } from 'vue';
import { RouterLink } from 'vue-router';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const nickname = ref(null);
const errorText = ref(null);
const baseUrl = ref(null);
const environment = ref(null);

onMounted(() => {
    axios.defaults.withCredentials = true;
    baseUrl.value = import.meta.env.VITE_API_SERVER_URL;
    environment.value = import.meta.env.VITE_API_ENVIRONMENT;
})

const handleLogin = () => {
    axios.defaults.withCredentials = true;
    axios.post(`${baseUrl.value}/authorization/login/`, JSON.stringify(loginRequestForm.value), {
        headers: {
            'Content-Type': 'application/json'
        }}).then(response => {
            if(response.status === 200) {
                loginRequestForm.value.nicknameOrEmail = null;
                loginRequestForm.value.password = null;

                nickname.value = response.data.nickname;
            }

            if(response.status === 404 || response.status === 404) {
                errorText.value = response.data.errorText;
                console.log(response.data.errorText)
            }
        });
}

const handleLogout = () => {
    if(window.confirm('Вы уверены, что хотите выйти?'))
    {
        axios.post(`${baseUrl.value}/authorization/logout/`, {
        headers: {
            'Content-Type': 'application/json'
        }}).then(response => {
            if(response.status === 200) {
                nickname.value = null;
            }
        });
    }    
}

</script>

<template>
    <div class="header-container">
        <label class="message">{{ environment }}</label>
        <div v-if="errorText" class="message">{{ errorText }}</div>
        <div v-else-if="nickname" class="message">Добро пожаловать, <span>{{ nickname }}!</span>
            <button @click="handleLogout">Выйти</button>
        </div>
        <div v-else class="form-container">
            <div class="login-inputs" @keydown.enter.prevent="handleLogin">
                <div>
                    <label>Логин: </label>
                    <input v-model="loginRequestForm.nicknameOrEmail" type="text" placeholder="Почта или никнэйм" required>
                </div>
                <div>
                    <label>Пароль: </label>
                    <input v-model="loginRequestForm.password" type="password" placeholder="Пароль" required>
                </div>
                <button @click.prevent="handleLogin">Войти</button>
            </div>
            <div class="login-anchors">
                <RouterLink to="/authentication/register">Регистрация</RouterLink> | <a>Забыл(а) пароль</a> |
                <label for="remember">
                    <input type="checkbox" id="remember">Запомнить
                </label>
            </div>
        </div>
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