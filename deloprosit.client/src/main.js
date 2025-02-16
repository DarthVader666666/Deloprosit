import './assets/main.css'
import router from './router';
import { createApp } from 'vue'
import App from './App.vue'
import VueCookies from 'vue3-cookies'

createApp(App)
.use(VueCookies)
.use(router)
.mount('#app')
