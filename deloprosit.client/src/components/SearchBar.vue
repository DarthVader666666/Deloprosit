<script setup>
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';

</script>
<template>
    <div class="search-bar">
        <label>Поиск<span> в разделах:</span></label>
        <InputText placeholder="Введите, что хотите найти..."/>
        <Button raised severity="secondary" reised><i class="pi pi-search"></i><span>Искать</span></Button>
    </div>
</template>

<style scoped>
    .search-bar {
        height: var(--SEARCHBAR-HEIGHT);
        flex-direction: row;
        text-align: center;
        align-content: center;
        background-color: var(--COLUMNS-BCKGND-CLR);;
        margin-top: 0.5rem;
        margin-bottom: 0.5rem;
        box-shadow: 0 7px 15px -3px black;
        border-radius: 5px;
    }

    .search-bar label {
        margin-right: 10px;
    }

    .search-bar button {
        box-shadow: var(--INPUT-BOX-SHADOW);
        height: 38px;
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