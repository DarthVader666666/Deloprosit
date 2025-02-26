<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const chapter = computed(() => store.state.chapter);
const themes = computed(() => store.state.themes);

</script>
<template>
    <div class="central-container">
        <div v-if="chapter && chapter.chapterTitle">
            <h3>Темы раздела:</h3>
            <h4>"{{chapter.chapterTitle}}"<i v-if="store.getters.isAdmin || store.getters.isOwner" class="pi pi-pen-to-square edit-chapter-button"></i></h4>
            <hr/>
        </div>
        <div v-if="themes">
            <ul v-for="(theme, index) in themes" :key="index">
                <li>{{theme.description}}</li>
            </ul>
        </div>
    </div>
</template>

<style scoped>
h4 i {
    margin-left: 8px;
}

.edit-chapter-button:hover {
    box-shadow: rgba(151, 151, 151, 0.8) 0 0 16px 8px;
    cursor: pointer;
}
</style>