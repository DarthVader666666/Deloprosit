<script setup>
import HeaderView from './components/HeaderView.vue';
import SearchView from './components/SearchView.vue';
import MainView from './components/MainView.vue';
import FooterView from './components/FooterView.vue'
import { RouterView } from 'vue-router';
</script>

<template>
  <RouterView/>
  <HeaderView/>
  <SearchView/>
  <MainView/>
  <FooterView/>
</template>

<style scoped>
</style>
