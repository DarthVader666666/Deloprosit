<script setup>
import { defineProps } from 'vue';

const props = defineProps({
    themes: {
        typeof: Array,
        default: []
    }
});

</script>

<template>
<div v-for="(theme, index) in themes" :key="index">
    {{ theme.description }}
</div>
</template>

<style scoped>

</style>