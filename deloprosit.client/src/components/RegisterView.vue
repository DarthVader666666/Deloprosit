<script setup>
import LeftColumnView from './LeftColumnView.vue'
import RegisterFormView from './RegisterFormView.vue';
import SpinningCircleView from './SpinningCircleView.vue';
import { ref } from 'vue';
import { useToast } from 'vue-toastification';
import { RouterLink } from 'vue-router';

const toast = useToast();

const pending = ref(false);
const showEmailNotification = ref(false);

const handlePending = async (promise) => {
    if(promise) {
        pending.value = true

        showEmailNotification.value = await promise
            .catch(error => {
                const status = error.response.status;
                
                if(status === 400) {
                    toast.error('Не удалось отправить письмо');
                }

                if (status === 500) {
                    toast.error('Ошибка сервера');
                }
        });
    }

    pending.value = false;
};

</script>

<template>
    <h2 class="title" v-if="!showEmailNotification && !pending">Заполните форму регистрации</h2>
    <div class="create-form-container">
        <LeftColumnView/>
        <div class="email-sent-notification" v-if="showEmailNotification">
            <h3>Письмо успешно отправлено</h3>
            <h3>Проверьте свой Email</h3>
            <img src="/src/assets/email-sent.jpg" alt="email-sent.jpg">
            <button><RouterLink to="/">Понятно</RouterLink></button>
        </div>
        <SpinningCircleView v-else-if="pending"/>
        <RegisterFormView v-else :pending="pending" @email-sent="handlePending"/>
    </div>
</template>

<style scoped>
.left-container {
    width: var(--SIDE-COLUMN-WIDTH);
}
</style>
