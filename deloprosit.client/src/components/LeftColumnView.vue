<script setup>
import { useStore } from 'vuex';
import { computed, onMounted } from 'vue';
import { RouterLink } from 'vue-router';

onMounted(() => {
    store.commit('setChapters');
})

const store = useStore();

const isAdmin = computed(() => store.getters.isAdmin);
const chapters = computed(() => store.state.chapters);

</script>
<template>
    <div class="left-container">
        <div class="chapters">
            <div class="chapters-header">
                <strong>Разделы:</strong>
                <RouterLink v-if="isAdmin" to="/create-chapter"><i class="pi pi-folder-plus"></i> Создать </RouterLink>
            </div>
            <hr/>
            <ul v-for="(chapter, index) in chapters" :key="index">
                <li>
                    <i class="pi pi-bookmark-fill"></i>
                    <RouterLink :to="`/get-chapters/${chapter.chapterId}`">{{ chapter.chapterTitle }}</RouterLink>
                </li>
            </ul>
        </div>        
    </div>
</template>

<style scoped>
    .chapters {        
        text-align: start;
        padding: 1rem;
    }

    .chapters-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
    }

    .chapters-header a {
        text-decoration: none;
        color: black;
    }

    .chapters-header a:hover {
        text-decoration: underline;
    }

    ul {
        list-style-type: none;
        padding: 0;
        margin: 2px 0 0 0;
    }

    li {
        font-size: x-small;
    }

    li a {
        margin-left: 3px;
        text-decoration: none;
        color: black;
        font-size: small;
    }

    li a:hover {
        color: var(--TEXT-GLOW-COLOR);
        text-decoration: underline;
    }
</style>