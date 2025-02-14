import './assets/main.css'
import { createApp } from 'vue'
import App from './App.vue'
import VueCookies from 'vue3-cookies'

createApp(App).use(VueCookies).mount('#app')
