<script></script>
<template>
    <div class="search-bar">
        <label>Поиск на форуме:  </label>
        <input class="search-input" type="text" placeholder="Введите, что хотите найти...">
        <button><i class="pi pi-search"></i>Искать</button>
    </div>
</template>

<style scoped>
    .search-bar {
        height: 3rem;
        flex-direction: row;
        text-align: center;
        align-content: center;
        background-color: var(--COLUMNS-BCKGND-CLR);;
        margin-top: 0.5rem;
        margin-bottom: 0.5rem;
        box-shadow: 0 7px 15px -3px black;
        border-radius: 5px;
    }

    .search-bar button {
        height: 50%;
    }

    .search-bar button i {
        margin-right: 5px;
    }

    .search-bar input {
        height: 38%;
        width: 40%;
        margin-right: 5px;
        box-shadow: var(--INPUT-BOX-SHADOW);
    }
</style>