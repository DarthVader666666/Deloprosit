<script setup>
import LeftColumnView from './LeftColumn.vue';
import RightColumnView from './RightColumn.vue'
import Button from 'primevue/button';
import { RouterView } from 'vue-router';
import { useStore } from 'vuex';
import { computed } from 'vue';

const store = useStore();
const title = computed(() => store.state.title);

function showDocuments() {
    const rightColumn = document.getElementById('right-column');
    rightColumn.style.display = 'block';
    rightColumn.style.width = '250px';
    rightColumn.style.zIndex = '1';
}

</script>

<template>
    <h2 v-if="title" class="title">{{ title }}</h2>
    <div class="main-container">
        <LeftColumnView/>
        <RouterView class="central-container"/>
        <RightColumnView/>
        <Button @click="showDocuments" class="document-button" severity="secondary" raised icon="pi pi-caret-left"></Button>
    </div>
</template>

<style scoped>
.document-button {
    position: fixed;
    height: 60px;
    width: 18px;
    margin-top: 10%;
    margin-bottom: auto;
    right: 0;
    border-radius: 50% 0 0 50%;
}

</style>
