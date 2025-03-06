import './assets/main.css';
import 'primeicons/primeicons.css';
import 'vue-toastification/dist/index.css';
import router from './router/router.js';
import App from './App.vue';
import VueCookies from 'vue3-cookies';
import store from './vuex/store.js';
import Toast, { POSITION } from 'vue-toastification';
import { createApp } from 'vue';
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura'

createApp(App)
.use(PrimeVue, {
    theme: {
        preset: Aura
    }
})
.use(VueCookies)
.use(router)
.use(Toast, { timeout: 2000, position: POSITION.TOP_CENTER })
.use(store)
.mount('#app');
