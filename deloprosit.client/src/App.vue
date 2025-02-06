<script setup>
import HeaderVue from './components/HeaderVue.vue';
import SearchVue from './components/SearchVue.vue';
import MainVue from './components/MainVue.vue';
</script>

<template>
  <div class="main-container">
    <HeaderVue/>
    <SearchVue/>
    <MainVue/>
  </div>
</template>

<style scoped>
  .main-container {
    direction: flex;  
    flex-direction: column;
    min-width: 1300px;
    box-shadow: 0 7px 15px -3px black;
  }
</style>
