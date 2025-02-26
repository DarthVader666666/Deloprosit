<script setup>
import LeftColumnView from './LeftColumnView.vue';
import { RouterView } from 'vue-router';
import { useStore } from 'vuex';
import { computed } from 'vue';

const store = useStore();

const title = computed(() => 
    store.state.title
);
</script>

<template>
    <h2 v-if="title" class="title">{{ title }}</h2>
    <div class="main-container">
        <LeftColumnView/>
        <RouterView/>
    </div>
</template>

<style scoped>
.main-container {
    display: flex;
    flex-direction: row;
    height: 100%;
    box-shadow: 0 7px 15px -3px black;
}
    
.title {
  text-align: center;
  align-content: center;
  height: 2rem;
}
</style>
