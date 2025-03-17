<script setup>
import axios from 'axios';
import { useToast } from 'vue-toastification';
import { ref, computed, onMounted } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import Menu from 'primevue/menu';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const store = useStore();
const toast = useToast();
const router = useRouter();

const nickname = computed(() => store.state.nickname);
const remember = ref (false);
const showLogin = ref(false);
const header = ref(null)

onMounted(() => {
    window.addEventListener('click', closeLogin)
})

function anyChildren(event, element) {
    if(element && element.children.length) {
        if(event.target === element) {
            return true;
        }

        for (let i = 0; i < element.children.length; i++) {
            if(event.target === element.children[i]) {
                return true;
            }
            else {
                if(anyChildren(event, element.children[i])) {
                    return true;
                }
            }
        }        
    }

    return false;
}

const closeLogin = (event) => {
    var isButton = anyChildren(event, document.getElementById('login-button'));
    var isForm = anyChildren(event, document.getElementById('login-form'));

    if (!isForm && !isButton) {
        showLogin.value = false;
    }
};

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
            showLogin.value = false;
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
    <div class="header-container" ref="header">
        <div class="logo">
            <RouterLink to="/"><h1>DP</h1></RouterLink>            
        </div>        

        <div class="menu">
            <Button v-if="!nickname" @click="() => { showLogin = false; router.push('/register'); }" severity="contrast" text label="Регистрация"/>
            <Button v-if="!nickname" @click="() => showLogin = !showLogin" icon="pi pi-sign-in" severity="contrast" text label="Войти" id="login-button"/>
            <div v-else class="message"><span>{{ nickname }}</span>
                <Button @click="handleLogout" severity="secondary" label="Выйти"/> 
            </div>
        </div>
    </div>

    <form v-if="showLogin" class="authentication-form" @submit.prevent="handleLogin" @keydown.enter.prevent="handleLogin" id="login-form">
        <div class="login-input">
            <label>Логин: </label>
            <InputText v-model="loginRequestForm.nicknameOrEmail" type="text" placeholder="Почта или никнэйм" required/>
        </div>
        <div class="login-input">
            <label>Пароль: </label>
            <InputText v-model="loginRequestForm.password" type="password" placeholder="Пароль" required/>
        </div>
        <div class="bottom-part">
            <div class="remember">
                <label for="remember-checkbox">Запомнить</label>
                <input v-model="remember" type="checkbox" id="remember-checkbox"/>
            </div>
            <Button type="submit" severity="secondary" icon="pi pi-sign-in" label="Войти" raised form="login-form"></Button>
        </div>
    </form>
</template>

<style scoped>
    .header-container {
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      background-image: var(--BCKGND-GRADIENT);
      box-shadow: var(--COMPONENT-BOX-SHADOW);
      border-radius: 0 0 5px 5px;
      height: var(--HEADER-HEIGHT);
      font-size: small;
    }

    .menu {
        display: flex;
        flex-direction: row;
        justify-content: end;
        align-items: end;
        padding: 8px;
    }

    .menu button {
        border-radius: 0;
    }

    .authentication-form {
        position: absolute;
        right: 0;
        display: flex;
        flex-direction: column;
        justify-content: start;
        padding: 15px;
        align-items: end;
        gap: 12px;
        background-color: var(--SELECTED-LINK-BCKGND-CLR);
        border-radius: 5px;        
        z-index: 1;
        box-shadow: var(--COMPONENT-BOX-SHADOW);
        font-size: small;
    }

    .authentication-form input[type="text"], input[type="password"] {
        font-size: small;
        height: 22px;
    }

    .login-input {
        display: flex;
        flex-direction: column;
        gap: 2px;
    }

    .bottom-part {
        display: flex;
        flex-direction: row;
        align-items: center;
    }

    .bottom-part button {
        font-size: small;
        height: 24px;
        padding: 5px;
        margin-left: 30px;
    }

    .remember {
        display: flex;
        flex-direction: row;
        align-items: center;
        gap: 5px;
    }

    .remember label:hover, input:hover {
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