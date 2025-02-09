<script></script>
<template>
    <div class="search-bar">
        <label>Поиск на форуме:  </label>
        <input class="search-input" type="text" placeholder="Введите, что хотите найти...">
        <button>Искать</button>
    </div>
</template>

<style scoped>
    .search-bar {
        height: 3rem;
        flex-direction: row;
        text-align: center;
        align-content: center;
        background-color: white;
        margin-top: 1rem;
        margin-bottom: 1rem;
        box-shadow: 0 7px 15px -3px black;
    }

    input {
        width: 300px;
        margin-right: 5px;
    }
</style>