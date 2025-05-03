<script setup>
import RegisterComponent from '@/components/RegisterComponent.vue';
import SpinningCircleView from '@/components/SpinningCircle.vue';
import { ref } from 'vue';
import { useToast } from 'vue-toastification';
import { useRouter } from 'vue-router';
import { useStore } from 'vuex';
import Button from 'primevue/button';

const store = useStore();
const toast = useToast();
const router = useRouter();

const pending = ref(false);
const showEmailNotification = ref(false);

const handlePending = async (promise) => {
    if(promise) {
        pending.value = true
        store.commit('setTitle', null)

        showEmailNotification.value = await promise
        .catch(error => {
            if(error.response) {
                toast.error(error.response.data.errorText);
            }
        });
    }

    pending.value = false;
};

</script>

<template>
    <div class="email-sent-notification" v-if="showEmailNotification">
        <h3>Письмо успешно отправлено</h3>
        <h3>Проверьте свой Email</h3>
        <img src="/src/assets/email-sent.jpg" alt="email-sent.jpg">
        <Button severity="secondary" raised @click="router.push('/')">Понятно</Button>
    </div>
    <SpinningCircleView v-else-if="pending" title='Письмо отправляется...'/>
    <RegisterComponent v-else :pending="pending" @email-sent="handlePending"/>
</template>
