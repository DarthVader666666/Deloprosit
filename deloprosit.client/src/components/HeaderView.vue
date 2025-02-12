<script setup>
import axios from 'axios';
import { ref } from 'vue';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const handleLogin = () => {
    axios.post('deloprosit/authorization/login/', JSON.stringify(loginRequestForm), 
        {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'multipart/form-data'
            }
        }
    );
}
</script>

<template>
    <div class="header-container">
        <div class="form-container">
            <form class="login-inputs" @submit="handleLogin">
                <div>
                    <label>Логин: </label>
                    <input v-model="loginRequestForm.nicknameOrEmail" name="NicknameOrEmail" type="text">
                </div>
                <div>
                    <label>Пароль: </label>
                    <input v-model="password" name="Password" type="password">
                </div>
                <button type="submit">Войти</button>
            </form>
            <div class="login-anchors">
                <a>Регистрация</a> | <a>Забыл(а) пароль</a> |
                <label for="remember">
                    <input class="checkbox" type="checkbox" id="remember">Запомнить
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