<script setup>
import LeftColumnView from './LeftColumnView.vue';
import RightColumnView from './RightColumnView.vue'
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
        <RightColumnView/>
    </div>
</template>

<style scoped>

</style>
