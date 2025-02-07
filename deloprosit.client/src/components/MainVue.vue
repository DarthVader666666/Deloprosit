<script setup>
import LeftColumnVue from './LeftColumnVue.vue';
import RightColumnVue from './RightColumnVue.vue';
import CentralColumnVue from './CentralColumnVue.vue'
</script>

<template>
    <div class="main-container">
        <LeftColumnVue/>
        <CentralColumnVue/>
        <RightColumnVue/>
    </div>
</template>

<style scoped>
    .main-container {
        flex-direction: row;
        height: 1000px;
    }
</style>