<script setup>
import InputText from 'primevue/inputtext';
import Textarea from 'primevue/textarea';
import Button from 'primevue/button'
import axios from 'axios';
import { useStore } from 'vuex';
import { ref } from 'vue';
import { useToast } from 'vue-toastification';

const store = useStore();
const toast = useToast();

const messageForm = ref(
    {
        name: null,
        email: null,
        phone: null,
        message: null
    });

function sendMessage() {
    console.log(messageForm.value)

    if(!(messageForm.value.email && messageForm.value.phone)) {
        toast.error('Поле "Ваш Email" или "Ваш номер телефона" должны быть заполнены');
        return;
    }

    var formData = new FormData();
    formData.append('name', messageForm.value.name);
    formData.append('email', messageForm.value.email);
    formData.append('phone', messageForm.value.phone);
    formData.append('message', messageForm.value.message);

    axios.put(`${store.state.serverUrl}/feedback`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
}

</script>

<template>
<div class="feedback-container">
    <h1>Напишите ваше сообщение</h1>
    <form @submit.prevent="sendMessage" class="send-message-form">
        <div class="send-message-input">
            <span>Ваше имя:</span>
            <InputText required v-model="messageForm.name"></InputText>
        </div>
        <div class="send-message-input">
            <span>Ваш Email:</span>        
            <InputText type="email" v-model="messageForm.email"></InputText>
        </div>
        <div class="send-message-input">
            <span>Ваш номер телефона:</span>
            <InputText type="tel" v-model="messageForm.phone"></InputText>
        </div>
        <div class="send-message-input">
            <span>Ваше сообщение:</span>
            <Textarea v-model="messageForm.message" required></Textarea>
        </div>        
        <div>
            <Button severity="secondary" type="submit" raised>Отправить</Button>
            <Button severity="contrast" raised>Отменить</Button>
        </div>        
    </form>

</div>

</template>

<style scoped>
.feedback-container {
    padding: 30px;
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
    min-height: 300px;
}

button {
    margin: 5px;
}

@media(max-width: 800px) {
    .send-message-input {
        width: 100%;
    }
}

</style>