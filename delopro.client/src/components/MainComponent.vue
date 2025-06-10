<script setup>
import LeftColumnView from './LeftColumn.vue';
import RightColumnView from './RightColumn.vue'
import Button from 'primevue/button';
import { RouterView } from 'vue-router';
import { useStore } from 'vuex';
import { computed, onMounted, watch } from 'vue';

const store = useStore();
const title = computed(() => store.state.title);
const showRightColumn = computed(() => store.getters.getShowRightColumn);

onMounted(() => {
    window.addEventListener('resize', () => {
        if(window.innerWidth > 1100 || (window.innerWidth < 1100 && !showRightColumn.value)) {
            hideDocuments();
            store.commit('setShowRightColumn', false);
        }
    });
})

watch(showRightColumn, (oldValue, newValue) => {
    if(!newValue) {
        showDocuments();
    }
    else {
        hideDocuments();
    }
})

function showDocuments() {
    const rightColumn = document.getElementById('right-container');
    rightColumn.style.position = 'fixed';
    rightColumn.style.zIndex = '1';
    rightColumn.style.right = '0';
    rightColumn.style.display = 'block';
    rightColumn.style.boxShadow = '0px 0px 10px 0px black';
    rightColumn.style.width = window.innerWidth < 360 ? `${window.innerWidth}px` : '360px';
}

function hideDocuments() {
    const rightColumn = document.getElementById('right-container');
    rightColumn.style.removeProperty('box-shadow');
    rightColumn.style.position = 'sticky';
    rightColumn.style.width = '17%';
    rightColumn.style.zIndex = '0';

    if(window.innerWidth > 1100) {
        rightColumn.style.display = 'block'
    }
    else {
        rightColumn.style.display = 'none'
    }
}

</script>

<template>
    <div v-if="title" class="title">
        <h2 style="margin: 0">{{ title }}</h2>
    </div>    
    <div class="main-container">
        <LeftColumnView/>
        <RouterView id="central-container"/>
        <RightColumnView/>
        <div class="document-button">
            <Button v-if="!showRightColumn" @click="store.commit('setShowRightColumn', true)" severity="secondary" raised icon="pi pi-caret-left"></Button>
            <Button v-else @click="store.commit('setShowRightColumn', false)" severity="contrast" raised icon="pi pi-caret-right"></Button>
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
    z-index: 2;
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
  overflow-wrap: break-word;
}

#right-container {
  width: var(--RIGHT-COLUMN-WIDTH);
  background-color: var(--COLUMNS-BCKGND-CLR);
  position: sticky;
  height: 100vh;
  top: 0;
  animation-name: slide;
  animation-duration: 0.2s;
  transform: translateX(0%);
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
