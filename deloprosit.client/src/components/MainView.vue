<script setup>
import LeftColumnView from './LeftColumn.vue';
import RightColumnView from './RightColumn.vue';
import CentralColumnView from './CentralColumn.vue'
</script>

<template>
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
        max-height: 1300px;        
        background-color: white;
        box-shadow: 0 7px 15px -3px black;
    }
</style>