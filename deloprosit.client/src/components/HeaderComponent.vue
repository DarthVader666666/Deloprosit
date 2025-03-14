<script setup>
import axios from 'axios';
import { useToast } from 'vue-toastification';
import { ref, computed } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import Button from 'primevue/button';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const store = useStore();
const toast = useToast();
const router = useRouter();

const nickname = computed(() => store.state.nickname);
const remember = ref (false);

const handleLogin = () => {
    const nicknameValue = helper.validateEmail(loginRequestForm.value.nicknameOrEmail) ? '' : loginRequestForm.value.nicknameOrEmail
    const emailValue = helper.validateEmail(loginRequestForm.value.nicknameOrEmail) ? loginRequestForm.value.nicknameOrEmail : null;

    axios.defaults.withCredentials = true;
    axios.post(`${store.getters.serverUrl}/authentication/login?nickname=${nicknameValue.trimEnd()}&remember=${remember.value}`, null,
    {
        headers: 
        {
            'Content-Type': 'application/json',
            'Authentication': JSON.stringify({
                email: emailValue,
                password: helper.getUnicodeByteArray(loginRequestForm.value.password)
            })
        }
    })
    .then(response => {
        if(response.status === 200) {
            loginRequestForm.value.nicknameOrEmail = null;
            loginRequestForm.value.password = null;

            if(response.data.remember === true) {
                const cookie = document.cookie.split('=');
            
                if(cookie && cookie.length === 2) {
                    localStorage.setItem(cookie[0], cookie[1]);
                }  
            }

            store.commit('setRoles', response.data.roles);
            store.commit('setNickname', response.data.nickname);
            toast.success(`Вы вошли, как ${response.data.nickname}`);
            router.push('/');
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
        axios.post(`${store.getters.serverUrl}/authentication/logout/`, {
        headers: {
            'Content-Type': 'application/json'
        }})
        .then(response => {
            if(response.status === 200) {
                localStorage.removeItem('Deloprosit_Cookies');
                store.commit('setNickname', null);
                store.commit('setRoles', []);
                store.commit('setNickname', null);
                router.push('/');
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
        <div class="logo">
            <RouterLink to="/"><h1>DP</h1></RouterLink>            
        </div>
        <div v-if="nickname" class="message"><span>{{ nickname }}</span>
            <Button @click="handleLogout" severity="secondary" label="Выйти"/>
        </div>
        <form v-else class="authentication-form" @submit.prevent="handleLogin">
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
                <label for="remember-checkbox">Запомнить</label>
                <input v-model="remember" type="checkbox" id="remember-checkbox">
            </div>
        </form>
    </div>
</template>

<style scoped>
    .header-container {
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      padding-top: 1rem;
      padding-bottom: 1rem;
      background-image: var(--BCKGND-GRADIENT);
      align-content: center;
      box-shadow: var(--COMPONENT-BOX-SHADOW);
      border-radius: 0 0 5px 5px;
      height: var(--HEADER-HEIGHT);
      font-size: small;
    }

    .authentication-form {
        display: flex;
        flex-direction: column;
        justify-content: start;
        padding-right: 15px;
        max-width: 430px;
        gap: 8px;
    }

    .login-anchors {
        align-content: center;
        padding-top: 5px;
        width: 289px;
    }

    .login-anchors a {
        color: black;
        text-decoration: none;
        font-size: small;
    }

    .login-anchors a:hover {
        cursor: pointer;
        color: var(--COLUMNS-BCKGND-CLR);;
    }

    .login-anchors input {
        height: 15px;
        width: 15px;
        position:absolute;
        margin-top:0px;
    }

    .login-anchors input, label {
        cursor: pointer;
    }

    .message {
      font-size: large;
      padding-right: 15px;
      align-content: center;
    }

    .message span {
      font-weight: bold;
      padding-right: 15px;
    }

    .logo {
        text-shadow: 3px 3px rgba(22, 22, 22, 0.651);
        height: 18px;
        margin-top: -24px;
        margin-left: 10px;
        width: 10px;
    }

    .logo a {
        text-decoration: none;
        color: var(--LOGO-COLOR);
        font-size: 18px;
    }

    .logo a:hover {
        color: var(--LOGO-COLOR);
    }

    label {
        font-weight: bold;
    }
</style>