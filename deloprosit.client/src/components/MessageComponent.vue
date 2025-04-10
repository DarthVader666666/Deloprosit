<script setup>
import { helper } from '@/helper/helper';
import Button from 'primevue/button';

const props = defineProps({
    message: {
        typeof: Object,
        default: null
    }
})

const emit = defineEmits(['message-shown']);

</script>

<template>    
<div class="message">
    <div style="display: flex; justify-content: end; padding-bottom: 10px;">
        <Button @click.prevent="emit('message-shown')" style="width: 25px; height: 25px; margin-top: 10px;" severity="secondary" raised rounded icon="pi pi-times"/>
    </div>
    <div>
        {{ props.message.text }}
    </div>
    <div style="display: flex; justify-content: space-between; padding: 20px 20px 0 20px;">
        <span>{{ props.message.name }}</span>
        <span>{{ helper.getDateString(props.message.dateSent) }}</span>
    </div>
    <div style="white-space: pre-wrap; padding: 20px 0 0 20px;">
        {{ props.message.contacts }}
    </div>
</div>
</template>

<style scoped>
.message {
    max-height: 100%;
    width: 70%;
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
    .message {
        font-size: small;
        height: 100%;
        width: 90%;
    }
}

@media (max-width: 800px) {
    .message {
        font-size: x-small;
        padding: 10px;
        width: 100%;
    }
}
</style>