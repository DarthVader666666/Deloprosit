<script setup>
import HeaderView from './components/HeaderView.vue';
import FooterView from './components/FooterView.vue'
import SearchBarView from './components/SearchBarView.vue'
import { useStore } from 'vuex';
import { computed } from 'vue';
import MainView from './components/MainView.vue';

const store = useStore();
const showSearchBar = computed(() => store.state.showSearchBar);

</script>

<template>  
  <HeaderView/>
  <SearchBarView v-if="showSearchBar"/>
  <MainView/>
  <FooterView/>
</template>

<style scoped>
</style>
