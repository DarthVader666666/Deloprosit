<script setup>
import RegisterFormView from '@/components/RegisterComponent.vue';
import SpinningCircleView from '@/components/SpinningCircle.vue';
import { ref } from 'vue';
import { useToast } from 'vue-toastification';
import { RouterLink } from 'vue-router';
import { useStore } from 'vuex';

const store = useStore();
const toast = useToast();

const pending = ref(false);
const showEmailNotification = ref(false);

const handlePending = async (promise) => {
    if(promise) {
        pending.value = true
        store.commit('setTitle', null)

        showEmailNotification.value = await promise
            .catch(error => {
                const status = error.response.status;
                
                if(status === 400) {
                    toast.error('Не удалось отправить письмо');
                }

                if (status === 500) {
                    toast.error('Ошибка сервера');
                }

                store.commit('setTitle', 'Заполните форму регистрации')
        });
    }

    pending.value = false;
};

</script>

<template>
    <div class="email-sent-notification" v-if="showEmailNotification">
        <h3>Письмо успешно отправлено</h3>
        <h3>Проверьте свой Email</h3>
        <img src="/src/assets/images/email-sent.jpg" alt="email-sent.jpg">
        <button><RouterLink to="/">Понятно</RouterLink></button>
    </div>
    <SpinningCircleView v-else-if="pending"/>
    <RegisterFormView v-else :pending="pending" @email-sent="handlePending"/>
</template>
