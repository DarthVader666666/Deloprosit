<script setup>
import { useStore } from 'vuex';
import { computed } from 'vue';
import Button from 'primevue/button';

const store = useStore();
const emit = defineEmits(['removeTheme']);

const downloadedThemes = computed(() => store.getters.getThemes );

const props = defineProps({
    themes: {
        typeof: Array,
        default: []
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

const themes = computed(() => props.themes.length ? props.themes : downloadedThemes.value);

</script>

<template>
<div class="theme">
    <div v-for="(theme, index) in themes" :key="index" :id="`theme_${theme.themeId}`">
        <div class="theme-header">
            <span>{{ theme.themeTitle }}</span>

            <Button v-if="useDeleteButtons && (store.getters.isAdmin || store.getters.isOwner)" icon="pi pi-times" text severity="danger"
                title="Удалить раздел" rounded @click="() => emit('removeTheme', theme.themeId)"></Button>
        </div>
        <div v-if="!props.useShortMode" v-html="theme.content" class="theme-content"></div>
    </div>
</div>
</template>
<style scoped>
.theme {
    display: flex;
    flex-direction: column;
    justify-content: start;
    gap: 15px;
}

.theme-header {
    display: flex;
    flex: row;
    justify-content: space-between;
    align-items: center;
    font-size: large;
    color: black;
    background: var(--THEME-HEADER-BCKGND-GRADIENT);
    padding: 6px;
    height: 34px;
}

.theme-header a {
    margin-left: 5px;
    color:  var(--THEME-HEADER-COLOR);
}

.theme-content {
    padding: 18px 20px 20px 20px;
    background: white;
}

@media (max-width: 1500px) {
    .theme-content:deep(img) {
        max-width: 500px;
    }
}

@media (max-width: 800px) {
    .theme-content:deep(img) {
        max-width: 300px;
    }
}

</style>