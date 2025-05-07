<script setup>
import { helper } from '@/helper/helper';
import Button from 'primevue/button';

const props = defineProps({
    user: {
        typeof: Object,
        default: null
    }
})

const emit = defineEmits(['user-shown']);

</script>

<template>    
<div class="user">
    <div style="display: flex; justify-content: end; padding-bottom: 10px;">
        <Button @click.prevent="emit('user-shown')" style="width: 25px; height: 25px; margin-top: 10px;" severity="secondary" raised rounded icon="pi pi-times"/>
    </div>
    
</div>
</template>

<style scoped>
.user {
    max-height: 100%;
    width: 50%;
    box-shadow: var(--COMPONENT-BOX-SHADOW);
    position: fixed;
    z-index: 3;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    padding: 10px 20px 30px 40px;
    background: white;
}

@media (max-width: 1100px) {
    .user {
        font-size: small;
        height: 90%;
        width: 90%;
    }
}

@media (max-width: 800px) {
    .user {
        font-size: x-small;
        padding: 10px;
    }
}
</style>