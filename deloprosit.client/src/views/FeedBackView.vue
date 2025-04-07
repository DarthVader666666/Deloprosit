<script setup>
import InputText from 'primevue/inputtext';
import Textarea from 'primevue/textarea';
import Button from 'primevue/button'
import axios from 'axios';
import { useStore } from 'vuex';
import { computed, ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import { helper } from '@/helper/helper';
import SpinningCircle from '@/components/SpinningCircle.vue';

const placeholder = 'Должен быть указан Email и/или Номер телефона';

const store = useStore();
const toast = useToast();
const router = useRouter();

const captcha = computed(() => store.state.captcha);

const isCaptchaMatch = ref(false);
const captchaNumber = ref(Math.floor(Math.random() * 10) + 1);
const pending = ref(false);
const invalid = ref(false);
const messageForm = ref(
    {
        name: null,
        email: null,
        phone: null,
        text: null,
        dateSent: null
    });

watch(messageForm.value, (oldValue, newValue) => {
    if(newValue.email || newValue.phone) {
        invalid.value = false;
        const email = document.getElementById('email');
        const phone = document.getElementById('phone');
        email.setAttribute('placeholder', '');
        phone.setAttribute('placeholder', '');
    }
});

async function handleSendProcess(promise) {
    if(promise) {
        pending.value = true

        await promise
            .then(response => {
                if(response.status === 200) {
                    toast.success('Сообщение успешно отправлено');
                    messageForm.value.name = null;
                    messageForm.value.email = null;
                    messageForm.value.phone = null;
                    messageForm.value.text = null;
                    messageForm.value.dateSent = null;
                    router.push('/');
                }
            })
            .catch(error => {
                const status = error.response.status;
                
                if(status === 400) {
                    toast.error(error.response.data.errorText);
                }

                if (status === 500) {
                    toast.error(error.response.data.errorText);
                }
        });
    }

    pending.value = false;
}

function sendMessage() {
    if(!(messageForm.value.email || messageForm.value.phone)) {
        invalid.value = true;
        const email = document.getElementById('email');
        const phone = document.getElementById('phone');

        email.setAttribute('placeholder', placeholder);
        phone.setAttribute('placeholder', placeholder);
        
        return;
    }

    var formData = new FormData();
    formData.append('name', messageForm.value.name);
    formData.append('email', messageForm.value.email);
    formData.append('phone', messageForm.value.phone);
    formData.append('text', messageForm.value.text);
    formData.append('dateSent', helper.getCurrentDate());

    const promise = axios.post(`${store.state.serverUrl}/feedback/send`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });

    handleSendProcess(promise);
}

function checkCaptchaMatch(event) {
    if(event.target.value === store.state.captcha[captchaNumber.value]) {
        isCaptchaMatch.value = true;
    }
    else {
        isCaptchaMatch.value = false;
    }
}

</script>

<template>
<div class="feedback-container">
    <div v-if="!pending">
        <form @submit.prevent="sendMessage" class="send-message-form">
            <div class="send-message-input">
                <span>Ваше имя:</span>
                <InputText required v-model="messageForm.name"></InputText>
            </div>
            <div class="send-message-input">
                <span>Ваш Email:</span>        
                <InputText type="email" :invalid="invalid" v-model="messageForm.email" id="email"></InputText>
            </div>
            <div class="send-message-input">
                <span>Ваш номер телефона:</span>
                <InputText type="tel" :invalid="invalid" v-model="messageForm.phone" id="phone"></InputText>
            </div>
            <div class="send-message-input">
                <span>Ваше сообщение:</span>
                <Textarea v-model="messageForm.text" required></Textarea>
            </div>        
            <div style="display: flex; gap: 10px; align-content: center;">
                <img :src="`/src/assets/captcha/${captchaNumber}.png`">
                <InputText style="width: 100px" type="text" @input="checkCaptchaMatch"></InputText>
                <i v-if="isCaptchaMatch" class="pi pi-check" style="color:green; margin-top:8px;font-size: large;"></i>
            </div>
            <div>
                <Button severity="secondary" :disabled="!isCaptchaMatch" type="submit" raised>Отправить</Button>
                <Button severity="contrast" raised @click="router.push('/')">Отменить</Button>
            </div>
        </form>
    </div>
    <SpinningCircle v-else title="Сообщение отправляется..."></SpinningCircle>
</div>

</template>

<style scoped>
.feedback-container form {
    padding: 10px;
}

.feedback-container h1 {
    margin-top: 0;
}

.send-message-form {
    display: flex;
    flex-direction: column;
    gap: 10px;
    align-items: start;
}

.send-message-input {
    display: flex;
    flex-direction: column;
    min-width: 70%;
}

.send-message-input textarea {
    min-height: 200px;
    max-height: 250px;
}

button {
    margin: 5px;
}

@media(max-width: 800px) {
    .feedback-container {
        padding: 15px;
    }

    .send-message-input {
        width: 100%;
    }
}

</style>