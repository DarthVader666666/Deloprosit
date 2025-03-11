<script setup>
import { useStore } from 'vuex';
import { computed } from 'vue';
import { useRouter, RouterLink } from 'vue-router';
import Button from 'primevue/button';

const store = useStore();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapters = computed(() => store.getters.getChapters);

function handleClick(chapterId, index) {
    var links = document.getElementsByClassName('link active');

    for (let item of links) {
        item.classList.remove('active');
    }

    document.getElementById(`${index}`).classList.add('active');
    router.push(`/chapters/${chapterId}`);
}

</script>
<template>
    <div class="left-container">
        <div class="chapters">
            <div class="chapters-header">
                <strong>Разделы:</strong>
                <Button v-if="isAdmin || isOwner" text raised severity="contrast">
                    <RouterLink to="/chapters/create"><i class="pi pi-folder-plus"></i> <span>Создать</span> </RouterLink>
                </Button>
            </div>
            <hr/>
            <div class="link" v-for="(chapter, index) in chapters" :key="index" @click="handleClick(chapter.chapterId, index)" :id="index">
                <i class="pi pi-bookmark-fill"></i>{{ chapter.chapterTitle }}
           </div>
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
        align-items: center;
    }

    .chapters-header button {
        padding: 6px;
    }

    .chapters-header a {
        text-decoration: none;
        color: black;
    }

    .link {
        font-size: small;
        padding: 3px 0 3px 0;
        margin: 3px 0 0 0;
    }

    .link i {
        font-size: small;
        margin-right: 3px;
    }

    .link:hover {
        background: var(--SELECTED-LINK-BCKGND-CLR);
        cursor: pointer;
        color:white;
    }

    .active {
        background: var(--SELECTED-LINK-BCKGND-CLR);
        cursor: pointer;
        color:white;
    }

    @media (max-width: 1500px) {
        .chapters-header a span {
            display: none;
        }
    }
</style>