import './assets/main.css'
import router from './router';
import { createApp } from 'vue'
import App from './App.vue'
import VueCookies from 'vue3-cookies'
import Toast, { POSITION } from 'vue-toastification';
import 'vue-toastification/dist/index.css';
import store from './vuex'

createApp(App)
.use(VueCookies)
.use(router)
.use(Toast, { timeout: 2000, position: POSITION.TOP_CENTER })
.use(store)
.mount('#app');
