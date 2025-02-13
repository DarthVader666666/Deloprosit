<script setup>
import axios from 'axios';
import { ref } from 'vue';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const handleLogin = () => {
    axios.defaults.withCredentials = true;
    axios.post('https://localhost:7250/authorization/login/', JSON.stringify(loginRequestForm.value), {
        headers: {
            'Content-Type': 'application/json'    
        }}).then(response => {
            if(response.status === 200) {
                loginRequestForm.value.nicknameOrEmail = null;
                loginRequestForm.value.password = null;
            }
        });        
}

const handleLala = () => {
    axios.get('https://localhost:7250/authorization/lala/');
}

</script>

<template>
    <div class="header-container">
        <div class="form-container">
            <div class="login-inputs">
                <div>
                    <label>Логин: </label>
                    <input v-model="loginRequestForm.nicknameOrEmail" type="text" required @keydown.enter.prevent="handleLogin">
                </div>
                <div>
                    <label>Пароль: </label>
                    <input v-model="loginRequestForm.password" type="password" required @keydown.enter.prevent="handleLogin">
                </div>
                <button @click.prevent="handleLogin">Войти</button>
            </div>
            <div class="login-anchors">
                <a href="#" @click.prevent="handleLala">Регистрация</a> | <a>Забыл(а) пароль</a> |
                <label for="remember">
                    <input type="checkbox" id="remember">Запомнить
                </label>
            </div>
        </div>
    </div>
</template>

<style scoped>
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