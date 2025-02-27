<script setup>
import { useStore } from 'vuex';
import { onMounted, computed } from 'vue';

const store = useStore();
const themes = computed(() => store.state.themes);

onMounted(() => {
    store.commit('renderSearchBar', true);
});

</script>

<template>
    <div class="central-container">
        <ul v-for="(theme, index) in themes" :key="index">
            <li>{{theme.description}}</li>
        </ul>
    </div>
</template>

<style scoped>

</style>