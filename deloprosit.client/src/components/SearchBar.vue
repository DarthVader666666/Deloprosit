<script setup>
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useStore } from 'vuex';

const store = useStore();
const router = useRouter();
const searchLine = ref(null);

async function handleSearch() {
    await store.dispatch('downloadChapterSearchResult', searchLine.value);
    searchLine.value = null;
    router.push('/search-result');
}

</script>
<template>
    <div class="search-bar">
        <label>Поиск<span> в разделах</span>:</label>
        <InputText v-model="searchLine" @keydown.enter="handleSearch" placeholder="Введите, что хотите найти..."/>
        <Button @click="handleSearch" raised severity="secondary" reised><i class="pi pi-search"></i><span>Искать</span></Button>
    </div>
</template>

<style scoped>
    .search-bar {
        position: relative;
        height: var(--SEARCHBAR-HEIGHT);
        flex-direction: row;
        text-align: center;
        align-content: center;
        background-color: var(--COLUMNS-BCKGND-CLR);;
        margin-top: 0.5rem;
        margin-bottom: 0.5rem;
        box-shadow: var(--COMPONENT-BOX-SHADOW);
        border-radius: 5px;
        padding-right: 60px;
    }

    .search-bar label {
        margin-right: 10px;
    }

    .search-bar button {
        position: absolute;
        height: 36px;
        bottom: 12px;
    }

    .search-bar button i {
        margin-right: 5px;
    }

    .search-bar input {
        width: 40%;
        margin-right: 5px;
        box-shadow: var(--INPUT-BOX-SHADOW);
    }

    @media (max-width: 800px) {
        .search-bar input{
            width: 70%;
        }

        .search-bar button span {
            display: none;
        }

        .search-bar label span {
            display: none;
        }

        .search-bar button {
            width: 40px;
        }
    }
</style>