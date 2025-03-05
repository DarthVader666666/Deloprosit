<script setup>
import { useStore } from 'vuex';
import { computed } from 'vue';
import { RouterLink } from 'vue-router';

const store = useStore();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapters = computed(() => store.state.chapters);

</script>
<template>
    <div class="left-container">
        <div class="chapters">
            <div class="chapters-header">
                <RouterLink to="/"><strong>Разделы:</strong></RouterLink>                
                <RouterLink v-if="isAdmin || isOwner" to="/chapter/create"><i class="pi pi-folder-plus"></i> Создать </RouterLink>
            </div>
            <hr/>
            <ul v-for="(chapter, index) in chapters" :key="index">
                <li>
                    <RouterLink :to="`/chapters/${chapter.chapterId}`">
                        <i class="pi pi-bookmark-fill"></i>{{ chapter.chapterTitle }}
                    </RouterLink>
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
        box-shadow: var(--GLOW-BOX-SHADOW);
        background-color:var(--TEXT-BCKGND-CLR);
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

    li a {
        text-decoration: none;
        color:black;
    }

    li a i {
        margin-right: 3px;
    }

    li a:hover {
        color: var(--TEXT-GLOW-COLOR);
        text-decoration: underline;
        cursor: pointer;
    }
</style>