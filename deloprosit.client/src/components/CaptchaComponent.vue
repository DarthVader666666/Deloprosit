<script setup>
import { useStore } from 'vuex';
import { computed, ref } from 'vue';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';

const emit = defineEmits(['captcha-match']);

const store = useStore();
const captcha = computed(() => store.state.captcha);

const isCaptchaMatch = ref(false);
const captchaNumber = ref(Math.floor(Math.random() * 10) + 1);

function checkCaptchaMatch(event) {
    if(event.target.value === captcha.value[captchaNumber.value]) {
        isCaptchaMatch.value = true;
    }
    else {
        isCaptchaMatch.value = false;
    }
    emit('captcha-match', isCaptchaMatch.value);
}

function refreshCaptcha() {
    captchaNumber.value = Math.floor(Math.random() * 10) + 1;
    document.getElementById('captcha-input').value = null;
    isCaptchaMatch.value = false;
    emit('captcha-match', isCaptchaMatch.value);
}
</script>
<template>
    <div style="display: flex; gap: 10px; align-content: center;">
        <img :src="`/src/assets/captcha/${captchaNumber}.png`">
        <InputText style="width: 100px" type="text" @input="checkCaptchaMatch" id="captcha-input"></InputText>
        <Button v-if="!isCaptchaMatch" @click="refreshCaptcha" title="Другую картинку"
            text severity="contrast" icon="pi pi-refresh" rounded style="width:25px;height:25px;top:4px;">
        </Button>
        <i v-if="isCaptchaMatch" class="pi pi-check" style="color:green;margin-top:8px;font-size:large;font-weight:bold;"></i>
    </div>
</template>