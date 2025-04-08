<script setup>
import { useStore } from 'vuex';
import { computed, ref } from 'vue';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';

const emit = defineEmits(['captcha-match']);
const store = useStore();
const captcha = computed(() => store.getters.getCaptcha);
const isCaptchaMatch = ref(false);

function checkCaptchaMatch(event) {
    if(event.target.value === captcha.value.code) {
        isCaptchaMatch.value = true;
    }
    else {
        isCaptchaMatch.value = false;
    }
    emit('captcha-match', isCaptchaMatch.value);
}

function refreshCaptcha() {
    store.dispatch('downloadCaptcha');
    document.getElementById('captcha-input').value = null;
    isCaptchaMatch.value = false;
    emit('captcha-match', isCaptchaMatch.value);
}
</script>
<template>
    <div style="display: flex; gap: 10px; align-content: center;">
        <img :src="`data:image/png;base64,${captcha ? captcha.image : ''}`" >
        <InputText style="width: 100px" type="text" @input="checkCaptchaMatch" id="captcha-input"></InputText>
        <Button v-if="!isCaptchaMatch" @click="refreshCaptcha" title="Другую картинку"
            text severity="contrast" icon="pi pi-refresh" rounded style="width:25px;height:25px;top:4px;">
        </Button>
        <i v-if="isCaptchaMatch" class="pi pi-check" style="color:green;margin-top:8px;font-size:large;font-weight:bold;"></i>
    </div>
</template>