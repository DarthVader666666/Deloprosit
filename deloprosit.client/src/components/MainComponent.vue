<script setup>
import LeftColumnView from './LeftColumn.vue';
import RightColumnView from './RightColumn.vue'
import Button from 'primevue/button';
import { RouterView } from 'vue-router';
import { useStore } from 'vuex';
import { computed, onMounted, ref } from 'vue';

const store = useStore();
const title = computed(() => store.state.title);
const showRightColumn = ref(false);

onMounted(() => {
    window.addEventListener('resize', () => {
        if(window.innerWidth > 1100 || (window.innerWidth < 1100 && !showRightColumn.value)) {
            hideDocuments();
        }
    });
})

function showDocuments() {
    showRightColumn.value = true;

    const centralComponent = document.getElementById('central-container');
    centralComponent.style.position = 'absolute';

    const rightColumn = document.getElementById('right-container');
    rightColumn.style.position = 'fixed';
    rightColumn.style.right = '0';
    rightColumn.style.display = 'block';
    rightColumn.style.width = window.innerWidth < 360 ? `${window.innerWidth}px` : '360px';
}

function hideDocuments() {
    showRightColumn.value = false;

    const centralComponent = document.getElementById('central-container');
    centralComponent.style.display = 'block';
    centralComponent.style.removeProperty('position');

    const rightColumn = document.getElementById('right-container');
    rightColumn.style.position = 'sticky';
    rightColumn.style.height = '100vh';
    rightColumn.style.width = '17%';
    rightColumn.style.top = 0;

    if(window.innerWidth > 1100) {
        rightColumn.style.display = 'block'
    }
    else {
        rightColumn.style.display = 'none'
    }    
}

</script>

<template>
    <h2 v-if="title" class="title">{{ title }}</h2>
    <div class="main-container">
        <LeftColumnView/>
        <RouterView id="central-container"/>
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

#central-container {
  width: var(--CENTRAL-COLUMN_WIDTH);
  background-color: var(--CENTRAL-BCKGND-CLR);
  padding: 10px;
}

#right-container {
  width: var(--RIGHT-COLUMN-WIDTH);
  background-color: var(--COLUMNS-BCKGND-CLR);
  position: sticky;
  height: 100vh;
  top: 0;
}

@media (max-width: 1100px) {
    .document-button {
        display: block;
    }

    #central-container {
      width: 100%;
      padding: 10px 0 0 0;
    }

    #right-container {
      display: none;
    }
}

</style>
