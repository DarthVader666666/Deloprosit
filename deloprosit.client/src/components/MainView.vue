<script setup>
import SearchView from './SearchView.vue'
import LeftColumnView from './LeftColumnView.vue';
import RightColumnView from './RightColumn.vue';
import CentralColumnView from './CentralColumnView.vue'
</script>

<template>
    <SearchView/>
    <div class="main-container">
        <LeftColumnView/>
        <CentralColumnView/>
        <RightColumnView/>
    </div>
</template>

<style scoped>
    .main-container {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        background-color: white;
        box-shadow: 0 7px 15px -3px black;
    }
</style>