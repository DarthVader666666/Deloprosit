<script setup>
import axios from 'axios';
import { useToast } from 'vue-toastification';
import { ref, computed, onMounted, watch } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';

const loginRequestForm = ref({
    nicknameOrEmail: null,
    password: null
});

const store = useStore();
const toast = useToast();
const router = useRouter();

const nickname = computed(() => store.state.nickname);
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const remember = ref (false);
const showLogin = ref(false);
const header = ref(null)
const isMenuOpened = ref(false);

onMounted(() => {
    window.addEventListener('click', closeLogin)
})

watch(isMenuOpened, (oldValue, newValue) => {
    const menu = document.getElementById('menu');

    if(newValue) {
        menu.classList.remove('menu-minimized');
        menu.classList.add('menu');
    }
    else {
        menu.classList.remove('menu');
        menu.classList.add('menu-minimized');        
    }
});

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

function handleBurgerClick() {
    isMenuOpened.value = !isMenuOpened.value;
}

</script>

<template>
    <div class="header-container" ref="header">
        <div class="logo">
            <RouterLink to="/"><h1>DP</h1></RouterLink>            
        </div>        
        <div class="menu-burger" >
            <Button  security="contrast" rounded text @click="handleBurgerClick">
                <i class="pi pi-bars"></i>
            </Button>
        </div>
        <div class="menu" id="menu">
            <Button @click="() => { isMenuOpened = false; router.push('/'); }" severity="contrast" text label="Главная"/>
            <Button v-if="!(isAdmin || isOwner)" @click="() => { isMenuOpened = false; router.push('/feedback'); }" severity="contrast" text label="Обратная связь"/>
            <Button v-else @click="() => { isMenuOpened = false; router.push('/chapters/create'); }" severity="contrast" text label="Создать раздел"/>
            <Button v-if="isOwner" @click="() => { isMenuOpened = false; router.push('/messages'); }" severity="contrast" text label="Сообщения"></Button>   
            <Button v-if="!nickname" @click="() => { isMenuOpened = false; showLogin = false; router.push('/register'); }" severity="contrast" text label="Регистрация"/>
            <Button v-if="!nickname" @click="() => { isMenuOpened = false; showLogin = !showLogin; }" icon="pi pi-sign-in" severity="contrast" text label="Войти" id="login-button"/>
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

    .menu-minimized {
        position: absolute;
        right: 0;
        z-index: 1;
        background-color: var(--MENU-BCKGND-CLR);
        display: flex;
        flex-direction: column-reverse;
        padding: 10px;
        border-radius: 3px;
        box-shadow: var(--MENU-BOX-SHADOW);
        top: 80px;
        align-items: start;
        animation-name: slide;
        animation-duration: 0.2s;
        transform: translateX(0%);
    }

    .menu-minimized button:deep(span) {
        font-weight: bold;
        color: var(--TEXT-COLOR);
    }

    .menu-burger {
        display: none;
        align-content: center;
        padding: 0;
    }

    .menu-burger button {
        margin: 0 10px 0 10px;
        padding: 12px;
    }

    .menu-burger button {
        font-size: large;
        border-width: 1px;
        border-color: rgba(0, 0, 0, 0.332);
    }

    .menu-burger i {
        color: var(--TEXT-COLOR);
        font-size: x-large;
    }

    .menu button {
        border-radius: 0;
    }

    .menu button:deep(span) {
        font-weight:bold;
        color: var(--TEXT-COLOR);
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
        background-color: var(--MENU-BCKGND-CLR);
        border-radius: 5px;        
        z-index: 1;
        box-shadow: var(--MENU-BOX-SHADOW);
        font-size: small;
        animation-name: slide;
        animation-duration: 0.2s;
        transform: translateX(0%)
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

    .login-input input {
        border-radius: 4px;
    }

    .login-input:hover:deep(input) {
        cursor:text;
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
        margin-left: 15px;
        border-radius: 4px;
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

    @media(max-width: 800px) {
        .menu {
            display: none;
        }

        .menu-burger {
            display: block;
        }
    }

    @media(min-width: 800px) {
        .menu-minimized {
            display: none;
        }

        .menu {
            display: flex;
        }
    }

    @keyframes slide {
        0% {
            transform: translateX(100%);
        }
    }
</style>