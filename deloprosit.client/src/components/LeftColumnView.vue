<script setup>
import { useStore } from 'vuex';
import { computed, onMounted } from 'vue';
import { RouterLink } from 'vue-router';

onMounted(() => {
    store.commit('downloadChapters');
})

const store = useStore();

const isAdmin = computed(() => store.getters.isAdmin);
const chapters = computed(() => store.state.chapters);

function handleChapterClick(event) {
    const chapterId = event.target.value;
    store.commit('downloadChapter', chapterId);
}

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
                <li :value="chapter.chapterId" @click.prevent="handleChapterClick">
                    <i class="pi pi-bookmark-fill"></i>
                    {{ chapter.chapterTitle }}
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
        box-shadow: rgba(158, 158, 158, 0.8) 0 0 15px 7px;
        background-color:rgba(192, 188, 188, 0.98);
    }

    ul {
        list-style-type: none;
        padding: 0;
        margin: 2px 0 0 0;
    }

    li {
        font-size: small;
        margin-bottom: 8px;
    }

    li:hover {
        color: var(--TEXT-GLOW-COLOR);
        text-decoration: underline;
        cursor: pointer;
    }
</style>