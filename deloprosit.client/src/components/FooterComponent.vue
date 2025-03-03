<script setup>
import { useStore } from 'vuex';

const store = useStore();

</script>
<template>
    <div class="footer-container">
        <h1>{{ store.getters.environment }}</h1>
    </div>
</template>

<style scoped>
    .footer-container {
        background-image: var(--BCKGND-GRADIENT);
        height: 4rem;
        align-content: center;
        padding-right: 1rem;        
        box-shadow: 0 7px 15px -3px black;
    }
</style>