<script setup>
import ChapterCreateForm from '@/components/ChapterCreateForm.vue';
import { useStore } from 'vuex';
import { onMounted } from 'vue';

const store = useStore();

onMounted(() => {
    store.commit('setTitle', 'Создание нового раздела');
});

</script>

<template>
    <ChapterCreateForm/>
</template>

<style scoped>

</style>
