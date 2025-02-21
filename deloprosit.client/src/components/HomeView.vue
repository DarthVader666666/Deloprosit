<script setup>
import SearchView from './SearchView.vue'
import RightColumnView from './RightColumnView.vue';
import MainView from './MainView.vue';
</script>

<template>
    <SearchView/>
    <div class="home-container">
        <MainView/>
        <RightColumnView/>
    </div>
</template>

<style scoped>
    .home-container {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        background-color: var(--COLUMNS-BCKGND-CLR);;
        box-shadow: 0 7px 15px -3px black;
    }
</style>