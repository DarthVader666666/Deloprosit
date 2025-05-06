<script setup>
import InputText from 'primevue/inputtext';
import Button from 'primevue/button'
import axios from 'axios';
import { useStore } from 'vuex';
import { ref } from 'vue';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import SpinningCircle from '@/components/SpinningCircle.vue';
import CaptchaComponent from '@/components/CaptchaComponent.vue';

const store = useStore();
const toast = useToast();
const router = useRouter();

const isCaptchaMatch = ref(false);
const pending = ref(false);
const email = ref(null);

async function handleSendProcess(promise) {
    if(promise) {
        pending.value = true

        await promise
            .then(response => {
                if(response.status === 200) {
                    toast.success('Сообщение успешно отправлено');
                    email.value = null;
                    router.push('/');
                }
            })
            .catch(error => {
                if(error.response) {
                    toast.error(error.response.data.errorText);
                    isCaptchaMatch.value = false;                    
                }
        });
    }

    pending.value = false;
}

function sendMessage() {
    console.log(email.value)

    const promise = axios.post(`${store.state.serverUrl}/authentication/recoverpassword`, null, 
    {
        headers: {
            'Content-Type': 'application/json',
            'Email': `${email.value}`
        }
    });

    handleSendProcess(promise);
}

function setCaptchaMatch(isMatch) {
    isCaptchaMatch.value = isMatch;
}
</script>

<template>
<div class="recover-password-container">
    <div v-if="!pending">
        <h3 style="padding: 11px;">Новый пароль будет отправлен на ваш email</h3>
        <form @submit.prevent="sendMessage" class="send-message-form">
            <div class="send-message-input">
                <span>Ваш Email:</span>        
                <InputText type="email" v-model="email" required></InputText>
            </div>            
            <CaptchaComponent @captcha-match="setCaptchaMatch"></CaptchaComponent>
            <div>
                <Button severity="secondary" :disabled="!(isCaptchaMatch && email)" type="submit" raised>Отправить</Button>
                <Button severity="contrast" raised @click="router.push('/')">Отменить</Button>
            </div>
        </form>
    </div>
    <SpinningCircle v-else title="Сообщение отправляется..."></SpinningCircle>
</div>

</template>

<style scoped>
.recover-password-container form {
    padding: 10px;
}

.recover-password-container h1 {
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
    max-width: 400px;
}

button {
    margin: 5px;
}

@media(max-width: 800px) {
    .recover-password-container {
        padding: 15px;
    }

    .send-message-input {
        width: 100%;
    }
}

</style>