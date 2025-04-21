<script setup>
    import { RouterLink } from 'vue-router'

    const emit = defineEmits(['agreement-checked']);

    const props = defineProps(
        {
            isAgreementChecked: {
                typeof: Boolean,
                require: true
            }
        }
    )
</script>
<template>
    <div style="display: flex; gap: 10px; align-items: center;">
        <input type="checkbox" @change="emit('agreement-checked')" :checked="props.isAgreementChecked"
            style="margin: 0; height: 15px; width: 15px;">
        <span>
            Соглашаюсь с <RouterLink to="personal-data-agreement"> правилами хранения и обработки персональных данных</RouterLink>
        </span>
        
    </div>
</template>
