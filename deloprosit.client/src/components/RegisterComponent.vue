<script setup>
import { ref, computed } from 'vue';
import axios from 'axios';
import { useToast } from 'vue-toastification';
import { helper } from '@/helper/helper.js';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import CaptchaComponent from '@/components/CaptchaComponent.vue'

const props = defineProps({
    pending: {
        type: Boolean,
        default: false
    }
});

const emit = defineEmits(['email-sent']);
const store = useStore();
const toast = useToast();
const router = useRouter();

const isCaptchaMatch = ref(false);
const isAgreementChecked = ref(false);
const showNicknameError = ref(false);
const showEmailError = ref(false);
const repeatPassword = ref(null);
const registerModel = ref({
    nickname: null,
    email: null,
    firstName: null,
    password: null
});

const isDisabledSendButton = computed(() => {    
    return !(registerModel.value.nickname && registerModel.value.email && registerModel.value.password && isMatchPassword.value && isCaptchaMatch.value && isAgreementChecked.value)
    || showNicknameError.value || showEmailError.value || showPasswordsError.value || props.pending;
});

const isMatchPassword = computed(() => {
    return registerModel.value.password === repeatPassword.value;
});

const showPasswordsError = computed(() => {
    if(!repeatPassword.value) {
        return false;
    }
    else {
        return !isMatchPassword.value;
    }
});

const handleSend = () => {
    const promise = axios.post(`${store.getters.serverUrl}/register/`, null,
    {
        headers: {
            'Content-Type': 'application/json',
            'Registration': JSON.stringify(
                {
                    nickname: helper.getUnicodeByteArray(registerModel.value.nickname),
                    email: registerModel.value.email,
                    firstName: helper.getUnicodeByteArray(registerModel.value.firstName),
                    password: helper.getUnicodeByteArray(registerModel.value.password),
                    registerDate: helper.getCurrentDate()
                })
        }
    })
    .then(response => {
        const status = response.status;

        if(status === 200) {
            toast.success(response.data.okText);
            return true;
        }

        return false;
    });

    emit('email-sent', promise);
}

async function doesUserExist (nickname, email) {
    await helper.timeoutAsync(500);

    let url = `${store.getters.serverUrl}/register/userExists?` + (nickname ? `nickname=${nickname}` : `email=${email}`);
    let response = await axios.get(url);

    if(response.status === 200) {
        return response.data.userExists;
    }

    return false;
}

const handleNicknameMatch = async (event) => {
    const nickname = event.target.value;
    showNicknameError.value = await doesUserExist(nickname, null);
}

const handleEmailMatch = async (event) => {
    const email = event.target.value;

    if(helper.validateEmail(email)) {
        showEmailError.value = await doesUserExist(null, email);
    }
}

function setCaptchaMatch(isMatch) {
    isCaptchaMatch.value = isMatch;
}
</script>

<template>
    <form @submit.prevent="handleSend">
        <div class="register-inputs">
            <div class="register-input">
                <span>Имя: </span>
                <InputText v-model="registerModel.firstName" type="text" maxlength="30"/>
            </div>
            <div class="register-input">
                <span>Никнэйм: <span class="red-star">* <span v-if="showNicknameError">Никнэйм занят</span></span></span>
                <InputText v-model="registerModel.nickname" @input.prevent="handleNicknameMatch" type="text" maxlength="30" required/>
            </div>
            <div class="register-input">
                <span>Email: <span class="red-star">* <span v-if="showEmailError">Email занят</span></span></span>
                <InputText v-model="registerModel.email" @input.prevent="handleEmailMatch" type="email" maxlength="50" required/>
            </div>
            <div class="register-input">
                <span>Пароль: <span class="red-star">* <span v-if="showPasswordsError">Пароли не совпадают</span></span></span>
                <InputText v-model="registerModel.password" type="password" maxlength="30" required/>
            </div>
            <div class="register-input">
                <span>Повторите пароль: <span class="red-star">*</span></span>
                <InputText v-model="repeatPassword" type="password" maxlength="30" required/>
            </div>
            <CaptchaComponent @captcha-match="setCaptchaMatch"></CaptchaComponent>
            <div>
                <input type="checkbox" v-model="isAgreementChecked" style="margin-right: 10px">
                <RouterLink to="personal-data-agreement">Соглашаюсь с правилами хранения и обработки персональных данных</RouterLink>
            </div>
        </div>
        <hr/>
        <div class="buttons">
            <Button severity="secondary" type="submit" :disabled="isDisabledSendButton">Отправить</Button>
            <Button severity="contrast" type="button" @click="router.push('/')">Отменить</Button>
        </div>
    </form>
</template>

<style scoped>

.register-inputs {
    padding: 10px;
    display: flex;
    flex-direction: column;
    align-items: start;
    gap: 10px;
}

.register-input {
    display: flex;
    flex-direction: column;
    min-width: 50%;
}

.buttons {
    display: flex;
    flex-direction: row;
    gap: 15px;
    padding-left: 10px;
}

.error-message {
    position:absolute;
    margin-top: 20px;
    color: red;
    font-weight: lighter;
    font-size: x-small;
}

</style>
