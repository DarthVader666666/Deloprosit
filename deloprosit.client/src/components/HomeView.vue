<script setup>
import RightColumnView from './RightColumnView.vue';
import CentralColumnView from './CentralColumnView.vue';
import { useStore } from 'vuex';
import { onMounted } from 'vue';

const store = useStore();

onMounted(() => {
    store.commit('renderSearchBar', true);
});

</script>

<template>
    <CentralColumnView/>
    <RightColumnView/>
</template>

<style scoped>

</style>