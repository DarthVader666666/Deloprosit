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
        height: 2rem;
        flex-direction: row;
        text-align: center;
        align-content: center;
        background-color: white;
    }

    input {
        width: 400px;
        margin-right: 5px;
    }
</style>