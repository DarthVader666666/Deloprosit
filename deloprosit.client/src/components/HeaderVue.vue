<script>
</script>
<template>
    <div class="form-container">
        <div class="inputs">
            <form action="deloprosit/user/login/" method="post" name="UserLogin" id="UserLogin">
                <label>Логин:</label>
                <input type="text">
                <label>Пароль:</label>
                <input type="password">
                <button type="submit">Войти</button>
            </form>    
        </div>
        <div class="anchors">
            <a href="deloprosit/user/register/">Регистрация</a> | <a href="deloprosit/user/password/">Забыл(а) пароль</a> |
            <label for="remember">
                <input class="checkbox" type="checkbox" id="remember">Запомнить
            </label>
        </div>
    </div>
</template>

<style scoped>
    .form-container {
        background-image: linear-gradient(to right,rgb(165, 218, 165),rgb(72, 163, 72));
        height: 4rem;
        align-content: center;
        flex-direction: column;
        padding-right: 1rem;        
    }

    input {
        margin-right: 5px;
    }

    a {
        color: black;
        font-size: small;
    }

    a:hover {
        color: whitesmoke;
    }

    label {
        font-weight: bold;
    }

    .inputs {
        text-align: end;
    }

    .anchors {
        text-align: end;
        align-content: center;
        padding-right: 12rem;
        padding-top: 5px;
    }
</style>