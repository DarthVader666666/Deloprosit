<script setup>
import { useStore } from 'vuex';
import Button from 'primevue/button';
import { RouterLink } from 'vue-router';

const store = useStore();
const emit = defineEmits(['removeTheme']);

const props = defineProps({
    theme: {
        typeof: Object,
        default: null
    },
    useDeleteButtons: {
        typeof: Boolean,
        default: false
    },
    useShortMode: {
        typeof: Boolean,
        default: false
    }
});

</script>

<template>
    <div :id="`theme_${props.theme.themeId}`" :ref="`theme_${props.theme.themeId}`">
        <div class="theme-header">
            <RouterLink :to="`/chapters/${store.state.chapter.chapterId}/${props.theme.themeId}`">
                {{ props.theme.themeTitle }}
            </RouterLink>

            <Button v-if="useDeleteButtons && (store.getters.isAdmin || store.getters.isOwner)" icon="pi pi-times" text severity="danger"
                title="Удалить раздел" rounded @click="() => emit('removeTheme', props.theme.themeId)"></Button>
        </div>
        <div v-if="!props.useShortMode" v-html="theme.content" class="theme-content"></div>
    </div>
</template>
<style scoped>
.theme-header {
    display: flex;
    flex: row;
    justify-content: space-between;
    align-items: center;
    font-size: large;
    color: black;
    background: var(--THEME-HEADER-BCKGND-GRADIENT);
    padding: 6px;
    min-height: 34px;
}

.theme-header a {
    margin-left: 5px;
    color:  var(--THEME-HEADER-COLOR);
}

.theme-content {
    padding: 18px 20px 20px 20px;
    background: white;
}

.theme-content:deep(img) {
    max-width: 1000px;
    height: auto;
}

@media (max-width: 1500px) {
    .theme-content:deep(img) {
        max-width: 500px;
        height: auto;
    }
}

@media (max-width: 800px) {
    .theme-content:deep(img) {
        max-width: 300px;
        height: auto;
    }
}

</style>