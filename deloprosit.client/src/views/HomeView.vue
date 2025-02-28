<script setup>
import { useStore } from 'vuex';
import { onMounted, computed } from 'vue';
import ThemeList from '@/components/ThemeList.vue';

const store = useStore();
const chapters = computed(() => store.state.chapters);

onMounted(() => {
    store.commit('renderSearchBar', true);
});

</script>

<template>
    <div class="central-container">
        <div class="title">            
            <h3><i class="pi pi-stopwatch"></i>Темы с последними сообщениями</h3>
        </div>
        <hr/>
        <div v-for="(chapter, index) in chapters" :key="index">
            <ThemeList :chapter="chapter"></ThemeList>
        </div>
    </div>
</template>

<style scoped>
.title {
    display: flex;
    flex-direction: row;
    padding-left: 15px;
    padding-right: 15px;
    justify-content: center;
}

h3 i {
    margin-left: 8px;
    margin-right: 5px;
    color:brown;
}

</style>