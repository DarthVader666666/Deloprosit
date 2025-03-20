<script setup>
import { helper } from '@/helper/helper';
import { computed } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const messages = computed(() => store.getters.getMessages);

</script>

<template>
<div class="messages-container">
    <table>
        <thead>
            <tr>
                <th>Сообщение</th>
                <th>Автор</th>
                <th>Прислано</th>
            </tr>            
        </thead>
        <tbody>
            <tr v-for="(message, index) in messages" :key="index">
                <td>{{ message.text }}</td>
                <td>{{ message.name }}</td>
                <td>{{ helper.getDateString(message.dateSent) }}</td>
            </tr>
        </tbody>
    </table>
</div>
</template>

<style scoped>
.messages-container {
    padding: 20px;
}

table, th, td {
  border: 1px solid black;
}

</style>