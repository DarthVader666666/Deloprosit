<script setup>
import HeaderView from './components/HeaderView.vue';
import FooterView from './components/FooterView.vue';
import SearchBarView from './components/SearchBarView.vue';
import MainView from './components/MainView.vue';
import axios from 'axios';
import { useStore } from 'vuex';
import { computed, onMounted } from 'vue';
import { useCookies } from 'vue3-cookies';

const store = useStore();
const cookieManager = useCookies();
const showSearchBar = computed(() => store.state.showSearchBar);

const coockieName ='Deloprosit_Cookies';

onMounted(async () => {
    axios.defaults.withCredentials = true;

    const activeCookies = cookieManager.cookies.get(coockieName);
    const localCookies = localStorage.getItem(coockieName);

    if (!activeCookies && localCookies) {
        cookieManager.cookies.set(coockieName, localCookies);
    }    

    const response = await axios.get(`${store.getters.serverUrl}/authentication/cookiecredentials`);

    if(response.data.isAuthenticated === true && response.data.nickname) {
        store.commit('setNickname', response.data.nickname);
        store.commit('setRoles', response.data.roles);
    }

    store.commit('downloadThemes');
})

</script>

<template>
  <HeaderView/>
  <SearchBarView v-if="showSearchBar"/>
  <MainView/>
  <FooterView/>
</template>

<style scoped>
</style>
