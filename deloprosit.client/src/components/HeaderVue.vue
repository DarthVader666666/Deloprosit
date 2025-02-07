<script>
</script>
<template>
    <div class="header-container">
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
    </div>
</template>

<style scoped>
    .header-container {
        background-image: linear-gradient(to right,rgb(165, 218, 165),rgb(72, 163, 72));
        height: 4rem;
        align-content: center;
        padding-right: 1rem;        
    }

    .form-container {
        flex-direction: column;
        justify-content: start;
        padding-left: 47rem;
    }

    .anchors {
        align-content: center;
        padding-right: 12rem;
        padding-top: 5px;
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
</style>