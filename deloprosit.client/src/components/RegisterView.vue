<script setup>
import LeftColumnView from './LeftColumnView.vue'
</script>
<template>
    <div class="register-container">
        <LeftColumnView class="left-container"/>
        <div class="register-form">
            <div class="register-inputs">
                <div class="spans">
                    <span>Никнэйм: </span>                
                    <span>Email: </span>
                    <span>Имя: </span>
                    <span>Фамилия: </span>
                    <span>Пароль: </span>
                    <span>Повторите пароль: </span>
                    <span>Дата рождения: </span>
                    <span>Страна: </span>
                    <span>Город: </span>
                    <span>Должность: </span>                
                </div>
                <div class="inputs">
                    <input type="text">
                    <input type="email">
                    <input type="text">
                    <input type="text">
                    <input type="password">
                    <input type="password">
                    <input type="date">
                    <input type="text">
                    <input type="text">
                    <input type="text">                
                </div>
            </div>            
            <hr/>
            <div class="info">
                <span>Дополнительно: </span>
                <textarea></textarea>
            </div>
            <hr/>
            <div class="buttons">
                <button>Отправить</button>
                <button>Отменить</button>
            </div>
        </div>
    </div>
</template>
<style scoped>
.left-container {
    width: 20%;
}

.register-form {
    width: 80%;
    padding: 10px;
    background-color: rgb(241, 235, 235);
}

.register-inputs {
    display: flex;
    flex-direction: row;
    font-weight: bold;
}

.spans {
    display: flex;
    flex-direction: column;
    text-align: end;
    gap: 10px;
    margin: 3px;
}

.inputs {
    display: flex;
    flex-direction: column;
    gap: 4px;
    margin: 1px;
}

.info {
    display: flex;
    flex-direction: column;
    margin: 10px;
}

.info span {
    font-weight: bold;
}

.info textarea {
    max-width: 60%;
    min-width: 300px;
    height: 150px;
}

.buttons {
    display: flex;
    flex-direction: row;
    gap: 5px;
    padding-left: 15px;
}
</style>