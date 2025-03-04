<script setup>
import { useStore } from 'vuex';
import { onUpdated, ref } from 'vue';

const store = useStore();

const emit = defineEmits(['deleteButtonStatusChanged']);

const selectedThemeIds = ref([]);

const props = defineProps({
    chapter: {
        typeof: Object,
        default: null,
        required: true
    },
    useCheckboxes: {
        typeof: Boolean,
        default: false
    }
});

onUpdated(() => {
    const checkBoxes = document.querySelectorAll('input[type=checkbox]');
    checkBoxes.forEach(x => x.checked = false);
})

function handleCheckboxChange(event, themeId) {
    const isChecked = event.target.checked;

    if(isChecked) {
        selectedThemeIds.value.push(themeId);
    }
    else {
        const index = selectedThemeIds.value.indexOf(themeId);
        selectedThemeIds.value.splice(index, 1);
    }

    emit('deleteButtonStatusChanged', selectedThemeIds.value.length > 0, selectedThemeIds.value)
}

</script>
<template>
<div class="theme">
    <div v-for="(theme, index) in props.chapter.themes" :key="index">
        <div class="theme-header">
            <span>{{ theme.themeTitle }}</span>

            <input v-if="useCheckboxes && (store.getters.isAdmin || store.getters.isOwner)" type="checkbox"
                @change.prevent="handleCheckboxChange($event, theme.themeId)">            
        </div>
        <div class="theme-content">
            <span v-html="theme.content"></span>
        </div>
    </div>
</div>
</template>
<style scoped>
.theme {
    text-align: start;    
    display: flex;
    flex-direction: column;
    justify-content: start;
    margin: 0 15px 15px 15px;
    gap: 15px;
}

.theme-header {
    display: flex;
    flex: row;
    justify-content: space-between;
    align-items: center;
    font-size: medium;
    color: var(--THEME-HEADER-COLOR);
    background: var(--THEME-HEADER-BCKGND-GRADIENT);
    padding: 3px;
    height: 20px;
}

.theme-header a {
    margin-left: 5px;
    color:  var(--THEME-HEADER-COLOR);
}

.theme-header span {
    margin-right: 5px;
}

.theme-content {
    padding: 8px 0 8px 0;
    background: lightgray;
}

.theme-content span{
    word-break: break-all;
}

.theme-content a {
    color: black;
    padding: 8px;
}

.theme-content input {
    float: inline-end;
    margin: 0 8px 8px 0;
}

.theme i {
    color: var(--TEXT-GLOW-COLOR);
    margin-right: 5px;
}
</style>