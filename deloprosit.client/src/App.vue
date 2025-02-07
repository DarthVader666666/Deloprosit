<script setup>
import HeaderView from './components/HeaderView.vue';
import SearchView from './components/SearchView.vue';
import MainView from './components/MainView.vue';
import FooterView from './components/FooterView.vue'
</script>

<template>
  <div class="app-container">
    <HeaderView/>
    <SearchView/>
    <MainView/>
    <FooterView/>
  </div>
</template>

<style scoped>
  .app-container {
    direction: flex;  
    flex-direction: column;
    min-width: 1380px;    
  }
</style>
