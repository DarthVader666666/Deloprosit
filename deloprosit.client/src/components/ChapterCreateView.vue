<script setup>
import ChapterCreateFormView from './ChapterCreateFormView.vue';
import { useStore } from 'vuex';
import { onMounted } from 'vue';

const store = useStore();

onMounted(() => {
    store.commit('setTitle', 'Создание нового раздела');
});

</script>

<template>
    <div class="create-form-container">
        <ChapterCreateFormView/>
    </div>
</template>

<style scoped>

</style>
