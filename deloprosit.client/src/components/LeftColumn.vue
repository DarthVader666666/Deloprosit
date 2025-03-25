<script setup>
import { useStore } from 'vuex';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import Tree from 'primevue/tree';

const store = useStore();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapterNodes = computed(() => store.getters.getChapterNodes);

const selectedKey = ref(null);

function handleThemeClick(node) {
    if(!node.data) {
        return
    }

    router.push(node.data);
}

</script>
<template>
    <div class="left-container">
        <div class="items">
            <div class="items-header">
                <strong>Разделы:</strong>
                <Button v-if="isAdmin || isOwner" raised severity="secondary" @click="router.push('/chapters/create')">
                    <i class="pi pi-plus"></i> <span>Создать</span>
                </Button>
            </div>
            <hr/>
            <Tree :value="chapterNodes" class="tree" v-model:selectionKeys="selectedKey" selectionMode="single" @nodeSelect="handleThemeClick">
                <template #url="{ node }">
                    <span>{{ node.label }}</span>
                </template>
            </Tree>
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

    .tree {
        padding: 0;
        font-size: small;
        background: var(--COLUMNS-BCKGND-CLR);
    }

    .tree:deep(div) {
        padding: 1px;
    }

    .tree:deep(li) {
        font-weight: bold;
    }

    .tree:deep(li) {
        font-weight: bold;
    }

    .tree:deep(span span) {
        font-weight: normal;
    }

    .tree:deep(*) {
        padding: 0;
        margin: 0;
        font-size: small;
    }

    @media (max-width: 1400px) {
        .items-header button span {
            display: none;
        }
    }
</style>