<script setup>
import ChapterCreateFormView from '@/components/ChapterCreateForm.vue';
import { useStore } from 'vuex';
import { onMounted } from 'vue';

const store = useStore();

onMounted(() => {
    store.commit('setTitle', 'Создание нового раздела');
});

</script>

<template>
    <div class="central-container">
        <ChapterCreateFormView/>
    </div>
</template>

<style scoped>

</style>
