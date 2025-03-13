<script setup>
import { useStore } from 'vuex';
import { computed } from 'vue';
import ThemeComponent from './ThemeComponent.vue';

const store = useStore();
const downloadedThemes = computed(() => store.getters.getThemes );

const props = defineProps({
    themes: {
        typeof: Array,
        default: []
    },
    removeTheme: {
        typeof: Function
    }
});

const themes = computed(() => props.themes.length ? props.themes : downloadedThemes.value);

</script>

<template>
<div class="theme">
    <ThemeComponent v-for="(theme, index) in themes" :key="index" @removeTheme="props.removeTheme"
        :theme="theme" 
        :useDeleteButtons="true" 
        :useShortMode="true"
        >
    </ThemeComponent>
</div>
</template>
<style scoped>
.theme {
    display: flex;
    flex-direction: column;
    justify-content: start;
    gap: 15px;
}
</style>