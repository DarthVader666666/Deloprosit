<script setup>
import { useStore } from 'vuex';
import { computed } from 'vue';
import { useRouter, RouterLink } from 'vue-router';
import Button from 'primevue/button';
import { helper } from '@/helper/helper';

const store = useStore();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapters = computed(() => store.getters.getChapters);
const chapter = computed(() => store.getters.getChapter);
const showChapterList = computed(() => store.getters.getShowChapterList);

</script>
<template>
    <div class="left-container">
        <div v-if="showChapterList" class="items">
            <div class="items-header">
                <strong>Разделы:</strong>
                <Button v-if="isAdmin || isOwner" text raised severity="contrast">
                    <RouterLink to="/chapters/create"><i class="pi pi-folder-plus"></i> <span>Создать</span> </RouterLink>
                </Button>
            </div>
            <hr/>
            <div class="link" v-for="(chapter, index) in chapters" :key="index" @click="router.push(`/chapters/${chapter.chapterId}`)">
                <i class="pi pi-bookmark-fill"></i>{{ chapter.chapterTitle }}
           </div>
        </div>
        <div v-else class="items">
            <div class="items-header">
                <strong>{{ chapter.chapterTitle }}:</strong>
            </div>
            <hr/>
            <div class="link" v-for="(theme, index) in chapter.themes" :key="index" @click="helper.scrollToTheme(theme.themeId)" :id="`listItem_${theme.themeId}`">
                <i class="pi pi-bookmark"></i>{{ theme.themeTitle }}
           </div>
        </div>
    </div>
</template>

<style scoped>
    .items {        
        text-align: start;
        padding: 1rem;
    }

    .items-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        min-height: 30px;
    }

    .items-header button {
        padding: 5px;
    }

    .items-header a {
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
        .items-header a span {
            display: none;
        }
    }
</style>