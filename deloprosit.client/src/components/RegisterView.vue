<script setup>
import LeftColumnView from './LeftColumnView.vue'
import RegisterFormView from './RegisterFormView.vue';
import SpinningCircleView from './SpinningCircleView.vue';
import { ref } from 'vue';

const pending = ref(false);
const showEmailNotification = ref(false);

const handlePending = async (promise, isPending) => {
    if(isPending) {
        pending.value = isPending;
    }
    else if(promise) {
        await promise;
        pending.value = false;
    }
    else {
        pending.value = false;
    }
};

</script>

<template>
    <h2>Заполните форму регистрации</h2>
    <div class="register-container">        
        <LeftColumnView/>
        <SpinningCircleView v-if="pending"/>
        <RegisterFormView v-else :pending="pending" @email-sent="handlePending"/>
    </div>
</template>

<style scoped>
.left-container {
    width: 20%;
}

h2 {
    text-align: center;
}
</style>
