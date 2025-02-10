<script>
</script>
<template>
    <div class="header-container">
        <div class="form-container">
            <form class="login-inputs" action="deloprosit/user/login/" method="post">
                <div>
                    <label>Логин: </label>
                    <input type="text">
                </div>
                <div>
                    <label>Пароль: </label>
                    <input type="password">
                </div>
                <button type="submit">Войти</button>
            </form>
            <div class="login-anchors">
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
        display: flex;
        flex-direction: row;
        justify-content: end;
        padding-top: 1rem;
        padding-bottom: 1rem;
        background-image: linear-gradient(to right,rgb(165, 218, 165),rgb(72, 163, 72));
        align-content: center;
        box-shadow: 0 7px 15px -3px black;
    }

    .form-container {
        display: flex;
        flex-direction: column;
        justify-content: start;
        padding-right: 15px;
    }

    .login-anchors {
        align-content: center;
        padding-top: 5px;
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