import './assets/main.css'
import 'primeicons/primeicons.css'
import 'vue-toastification/dist/index.css';
import router from './router';
import App from './App.vue'
import VueCookies from 'vue3-cookies'
import store from './vuex/store.js'
import Toast, { POSITION } from 'vue-toastification';
import { createApp } from 'vue'

createApp(App)
.use(VueCookies)
.use(router)
.use(Toast, { timeout: 2000, position: POSITION.TOP_CENTER })
.use(store)
.mount('#app');
