<script setup>
import { useStore } from 'vuex';
import Button from 'primevue/button';
import { RouterLink, useRouter } from 'vue-router';
import { helper } from '@/helper/helper';
import { computed } from 'vue';
import SpinningCircle from './SpinningCircle.vue';

const store = useStore();
const router = useRouter();
const emit = defineEmits(['removeTheme']);

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const pending = computed(() => store.getters.getPending);

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
            <div>
                <RouterLink :class="!useShortMode && `disabled`" :to="`/chapters/${store.state.chapter.chapterId}/${props.theme.themeId}`" :disabled="true">
                    {{ props.theme.themeTitle }}
                </RouterLink>
                <Button v-if="!props.useShortMode && (isAdmin || isOwner)" rounded text icon="pi pi-pencil" severity="contrast" 
                    title="Редактировать" @click="router.push(`/themes/${theme.themeId}/edit`)"/>
            </div>
            <span v-if="!useShortMode" class="date">{{ helper.getDateString(props.theme.dateCreated) }}</span>

            <Button v-if="useDeleteButtons && (store.getters.isAdmin || store.getters.isOwner)" icon="pi pi-times" text severity="danger"
                title="Удалить раздел" rounded @click="() => emit('removeTheme', props.theme.themeId)"></Button>
        </div>
        <div v-if="pending" style="display: flex; flex-direction: column; align-items: center;">
            <h3>Загрузка...</h3>
            <SpinningCircle></SpinningCircle>
        </div>
        <div v-else-if="!props.useShortMode" v-html="theme.content" class="theme-content"></div>
    </div>
</template>
<style scoped>
.theme-header {
    display: flex;
    flex: row;
    justify-content: space-between;
    align-items: center;
    font-size: large;
    background: var(--THEME-HEADER-BCKGND-GRADIENT);
    padding: 6px;
    min-height: 34px;
}

.theme-header a {
    text-decoration: none;
    margin-left: 5px;
    color:  var(--TEXT-COLOR);
}

.theme-header a:hover {
    text-decoration: underline;
}

.disabled {
    pointer-events: none;
}

.disabled:hover {
    text-decoration: none;
}

.theme-content {
    padding: 18px 20px 20px 20px;
    background: white;
}

.theme-content:deep(img) {
    max-width: 920px;
    height: auto;
}

.date {
    font-size: small;
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

    .theme-header span {
        display: none;
    }
}

</style>