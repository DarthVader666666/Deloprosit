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
const topPosition = ref(0);

function showDocuments() {
    showRightColumn.value = true;

    const centralComponent = document.getElementById('central-component');
    topPosition.value = document.getElementById('app').offsetTop;
    console.log(topPosition.value)
    centralComponent.style.display = 'none';

    const rightColumn = document.getElementById('right-column');
    rightColumn.style.display = 'block';
    rightColumn.style.width = '100%';

    window.scrollTo(0, 0);
}

function hideDocuments() {
    showRightColumn.value = false;

    const centralComponent = document.getElementById('central-component');
    centralComponent.style.display = 'block';

    const rightColumn = document.getElementById('right-column');
    rightColumn.style.display = 'none';

    window.scrollTo(0, topPosition.value);
}

</script>

<template>
    <h2 v-if="title" class="title">{{ title }}</h2>
    <div class="main-container">
        <LeftColumnView/>
        <RouterView class="central-container" id="central-component"/>
        <RightColumnView/>
        <Button v-if="!showRightColumn" @click="showDocuments" class="document-button" severity="secondary" raised icon="pi pi-caret-left"></Button>
        <Button v-else @click="hideDocuments" class="document-button" severity="contrast" raised icon="pi pi-caret-right"></Button>
    </div>
</template>

<style scoped>
.document-button {
    position: fixed;
    height: 60px;
    width: 15px;
    margin-top: 25vh;
    margin-bottom: auto;
    right: 0;
    border-radius: 50% 0 0 50%;
}

</style>
