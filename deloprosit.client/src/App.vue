<script setup>
import HeaderView from './components/HeaderView.vue';
import SearchView from './components/SearchView.vue';
import FooterView from './components/FooterView.vue'
import { RouterView } from 'vue-router';
</script>

<template>  
  <HeaderView/>
  <RouterView/>
  <FooterView/>
</template>

<style scoped>
</style>
