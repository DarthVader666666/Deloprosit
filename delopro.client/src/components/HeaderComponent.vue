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
const unreadMessagesCount = computed(() => store.getters.getUnreadMessagesCount);
const darkenBackground = computed(() => showLogin.value || showMenu.value || showAccountSettings.value);

const remember = ref(false);
const showLogin = ref(false);
const showMenu = ref(false);
const showAccountSettings = ref(false);
const header = ref(null);

onMounted(async () => {
    window.addEventListener('click', (event) => { if(!helper.closeMenu(event, ['login-form', 'login-button'])) showLogin.value = false });
    window.addEventListener('click', (event) => { if(!helper.closeMenu(event, ['menu', 'burger-button'])) showMenu.value = false });
    window.addEventListener('click', (event) => { if(!helper.closeMenu(event, ['account-settings', 'account-button'])) showAccountSettings.value = false });
    window.addEventListener('resize', handleScreenSizeChange);
});

watch(showMenu, (oldValue, newValue) => {
    const menu = document.getElementById('menu');

    if(newValue) {
        menu.classList.remove('slide-container');
        menu.classList.add('menu');
    }
    else {
        menu.classList.remove('menu');
        menu.classList.add('slide-container');
    }
});

watch(darkenBackground, (oldValue, newValue) => {
    const containers = [
        document.getElementsByClassName('main-container'),
        document.getElementsByClassName('search-bar'),
        document.getElementsByClassName('title')
    ]

    if(!newValue) {
        containers.forEach(items => {
            for(let item of items) {            
                item.style.opacity = 0.8;
                item.style.filter = 'brightness(50%)';
            }
        })
    }
    else {
        containers.forEach(items => {
            for(let item of items) {
                item.style.removeProperty('opacity');
                item.style.removeProperty('filter');
            }
        })
    }
});

const handleScreenSizeChange = () => {
    if(document.documentElement.clientWidth > 800) {
        showMenu.value = false;
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
        if(error.response) {
            toast.error(error.response.data.errorText)
        }

        loginRequestForm.value.nicknameOrEmail = null;
        loginRequestForm.value.password = null;
    })
};

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
                showAccountSettings.value = false;
                router.push('/');
            }
        })
        .catch(error => {
            if(error.response) {
                toast.error(error.response.data.errorText)
            }
        });
    }
}

function handleBurgerClick() {
    showMenu.value = !showMenu.value;
}

</script>

<template>
    <div class="header-container" ref="header">
        <div class="logo">
            <RouterLink to="/"><h1>DeloPro</h1></RouterLink>            
        </div>
        <div class="account-and-menu">
            <div class="menu-burger" >
                <span v-if="nickname" style="font-weight: bold;">{{ nickname }}</span>
                <Button
                    @click="handleBurgerClick" 
                    security="contrast" rounded text
                    id="burger-button"
                >
                    <i class="pi pi-bars"></i>
                </Button>
            </div>
            <div class="menu" id="menu">
                <div v-if="nickname" class="account">
                    <Button 
                        @click="() => { showAccountSettings = !showAccountSettings; showMenu = false }"
                        severity="secondary" rounded
                        id="account-button"
                    >
                    <i class="pi pi-user" style="font-size: x-large;"></i>
                    </Button>
                    <span>{{ nickname }}</span>
                </div>
                <Button
                    @click="() => { showMenu = false; router.push('/'); }" 
                    severity="contrast" text label="Главная" 
                    id="home-button" style="border-radius: 0"
                />
                <Button v-if="!(isAdmin || isOwner)"
                    @click="() => { showMenu = false; router.push('/feedback'); }" 
                    severity="contrast" text label="Обратная связь"
                    id="feedback-button" style="border-radius: 0"
                />
                <Button v-if="isAdmin || isOwner"
                    @click="() => { showMenu = false; router.push('/chapters/create'); }"
                    severity="contrast" text label="Создать раздел"
                    id="create-chapter-button" style="border-radius: 0"
                />
                <Button v-if="isOwner" @click="() => { showMenu = false; router.push('/messages'); }" 
                    severity="contrast" text
                    id="messages-button" style="border-radius: 0"
                >
                    <span>Сообщения</span>
                    <span class="unread-messages-count" :style="unreadMessagesCount ? '' : 'display: none;'">{{ unreadMessagesCount }}</span>
                </Button>
                <Button v-if="!nickname" @click="() => { showMenu = false; showLogin = false; router.push('/register'); }" 
                    severity="contrast" text label="Регистрация"
                    id="register-button" style="border-radius: 0"
                />
                <Button v-if="!nickname" @click="() => { showMenu = false; showLogin = !showLogin; }" 
                    severity="contrast" text label="Войти" icon="pi pi-sign-in"
                    id="login-button" style="border-radius: 0"
                />
            </div>            
            <div v-if="nickname && showAccountSettings" class="slide-container" id="account-settings">
                <div>
                    <span style="font-size: large;">
                        {{ nickname }}
                    </span>
                    <Button @click="showAccountSettings = false" severity="contrast" rounded text icon="pi pi-times" style="position: absolute; right: 5px; top: 5px; height: 25px; width: 25px"></Button>
                </div>                
                <Button 
                    @click="handleLogout" 
                    severity="secondary" label="Выйти"
                    icon="pi pi-sign-out"
                    id="logout-button"
                />
            </div>
        </div>
    </div>

    <form v-if="showLogin" class="slide-container" @submit.prevent="handleLogin" @keydown.enter.prevent="handleLogin" id="login-form">
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
        <RouterLink to="recover-password" @click="() => { showMenu = false; showLogin = !showLogin; }">Забыли пароль?</RouterLink>
    </form>
</template>

<style scoped>
    .header-container {
      display: flex;
      flex-direction: row;
      justify-content: center;
      background-image: var(--BCKGND-GRADIENT);
      box-shadow: var(--COMPONENT-BOX-SHADOW);
      border-radius: 0 0 5px 5px;
      height: var(--HEADER-HEIGHT);
    }

    .account-and-menu {
        display: flex;
        flex-direction: row;
    }

    .account {
        position: absolute;
        right: 20px;
        padding-bottom: 8px;
        display: flex;
        flex-direction: column;
        font-size: medium;
        align-items: center;
    }

    .account button {
        height: 50px;
        width: 50px;
        border-radius: 50%;
    }

    .slide-container {
        position: fixed;
        top: var(--HEADER-HEIGHT);
        right: 0;
        z-index: 1;
        background-color: var(--MENU-BCKGND-CLR);
        display: flex;
        flex-direction: column;
        padding: 15px;
        border-radius: 3px;
        box-shadow: var(--MENU-BOX-SHADOW);
        align-items: start;
        animation-name: slide;
        animation-duration: 0.2s;
        transform: translateX(0%);
        gap: 12px;
        min-width: 220px;
    }

    .menu {
        display: flex;
        flex-direction: row;
        align-items: end;
        gap: 10px;
    }

    .slide-container:deep(button) {
        width: 100%;
    }

    .slide-container button:deep(span) {
        font-weight: bold;
        color: var(--TEXT-COLOR);
    }

    .menu-burger {
        display: none;
        align-content: center;
    }

    .menu-burger button {
        margin: 0 10px 0 10px;
        padding: 12px;
    }

    .menu-burger button {
        border-width: 1px;
        border-color: rgba(0, 0, 0, 0.332);
    }

    .menu-burger i {
        color: var(--TEXT-COLOR);
        font-size: x-large;
    }

    .menu button:not(.account button)  {
        border-radius: 0;
    }

    .menu button:deep(span) {
        font-weight:bold;
        color: var(--TEXT-COLOR);
    }

    .slide-container input[type="text"], input[type="password"] {
        font-size: medium;
        height: 30px;
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
        font-size: medium;
        height: 30px;
        padding: 8px;
        margin-left: 22px;
        border-radius: 4px;
    }

    .unread-messages-count {
        right: 0;
        background:red;
        color:white !important;
        font-size: small;
        font-weight:normal !important; 
        padding:3px 0 0 0;
        border-radius:50%;
        height:20px;
        width:20px;
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

    .logo {
        position: absolute;
        left: 0;
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
        .header-container {
            justify-content: end;
        }

        .menu {
            display: none;
        }

        .menu-burger {
            display: flex;
            align-items: center;
        }

        .account {
            position: relative;
            padding: 0;
            right: 0;
            margin: auto;
        }

        .account-and-menu {
            flex-direction: row-reverse;
        }
    }
</style>