<script setup>
import LeftColumnView from './LeftColumn.vue';
import RightColumnView from './RightColumn.vue'
import Button from 'primevue/button';
import { RouterView } from 'vue-router';
import { useStore } from 'vuex';
import { computed, ref } from 'vue';

const store = useStore();
const title = computed(() => store.state.title);
const showRightColumn = ref(false);

function showDocuments() {
    showRightColumn.value = true;

    const centralComponent = document.getElementById('central-component');
    centralComponent.style.position = 'absolute';

    const rightColumn = document.getElementById('right-column');
    rightColumn.style.position = 'fixed';
    rightColumn.style.right = '0';
    rightColumn.style.display = 'block';
    rightColumn.style.width = '360px';
}

function hideDocuments() {
    showRightColumn.value = false;

    const centralComponent = document.getElementById('central-component');
    centralComponent.style.display = 'block';
    centralComponent.style.removeProperty('position');

    const rightColumn = document.getElementById('right-column');
    rightColumn.style.display = 'none';
}

</script>

<template>
    <h2 v-if="title" class="title">{{ title }}</h2>
    <div class="main-container">
        <LeftColumnView/>
        <RouterView class="central-container" id="central-component"/>
        <RightColumnView/>
        <div class="document-button">
            <Button v-if="!showRightColumn" @click="showDocuments"  severity="secondary" raised icon="pi pi-caret-left"></Button>
            <Button v-else @click="hideDocuments" severity="contrast" raised icon="pi pi-caret-right"></Button>
        </div>
    </div>
</template>

<style scoped>
.document-button {
    position: fixed;
    margin-top: 25vh;
    margin-bottom: auto;
    right: 0;
    display: none;
}

.document-button button {
    height: 60px;
    width: 15px;
    border-radius: 50% 0 0 50%;
}

@media (max-width: 1100px) {
    .document-button {
        display: block;
    }
}

</style>
