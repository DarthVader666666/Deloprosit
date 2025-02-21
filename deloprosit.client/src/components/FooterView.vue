<script setup>
import { ref } from 'vue';
import { onMounted } from 'vue';

const environment = ref(null);

onMounted(() =>  {
    environment.value = import.meta.env.VITE_API_ENVIRONMENT;
});

</script>
<template>
    <div class="footer-container">
        <h1>{{ environment }}</h1>
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