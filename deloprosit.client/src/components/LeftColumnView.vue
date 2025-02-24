<script setup>
function isInRole(role) {
    const roles = sessionStorage.getItem('roles');
    console.log(roles);

    return roles === role;
}

</script>
<template>
    <div class="left-container">
        <div class="chapters">
            <div class="chapters-header">
                <strong>Разделы:</strong><a v-if="isInRole('Admin')" href="#"><i class="pi pi-plus-circle"></i> Создать</a>
            </div>
            <hr/>
            <ul>
                <li><a href="">SDFSDF</a></li>
                <li><a href="">SFGSFHGDGH</a></li>
                <li><a href="">SFGSFSGFSGF</a></li>
            </ul>
        </div>
        
    </div>
</template>

<style scoped>
    .chapters {        
        text-align: start;
        padding: 1rem;
    }

    .chapters-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
    }

    .chapters-header a{
        text-decoration: none;
        color: black;
    }

    .chapters-header a:hover{
        text-decoration: underline;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        padding: 0 0 6px 0;
        font-size: x-small;
    }
</style>