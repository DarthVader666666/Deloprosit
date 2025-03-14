<script setup>
import { RouterLink } from 'vue-router';
import Button from 'primevue/button';
import { computed } from 'vue';
import { useStore } from 'vuex';
import axios from 'axios';

const store = useStore();
const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const documents = computed(() => store.getters.getDocuments);

function download(url, label) {
    console.log(url)
    console.log(label)
    axios.get(url, { responseType: 'blob' })
      .then(response => {
        const blob = new Blob([response.data], { type: 'image/png' })
        const link = document.createElement('a')
        link.href = URL.createObjectURL(blob)
        link.download = label
        link.click()
        URL.revokeObjectURL(link.href)
      }).catch(console.error)
}

</script>
<template>
    <div class="right-container">
        <div class="items">
            <div class="items-header">
                <strong>Документы:</strong>
                <Button v-if="isAdmin || isOwner" text raised severity="contrast">
                    <RouterLink to="/documents/settings"><i class="pi pi-upload"></i> <span>Загрузить</span> </RouterLink>
                </Button>
            </div>
            <hr/>
            <div class="link" v-for="(document, index) in documents" :key="index" @click="download(document.path, document.name)">
                <i class="pi pi-file"></i>
                {{ document.name }}
                <Button v-if="isAdmin || isOwner" severity="danger" text rounded><i class="pi pi-times"></i></Button>
           </div>
        </div>
    </div>        
</template>

<style scoped>
    .items {        
        text-align: start;
        padding: 1rem;
    }

    .items-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        min-height: 30px;
    }

    .items-header button {
        padding: 5px;
    }

    .items-header a {
        text-decoration: none;
        color: black;
    }

    .link {
        display: flex;
        flex-direction: row;
        align-items: center;
        text-decoration: none;
        color: black;
        font-size: small;
        padding: 3px 0 3px 0;
        margin: 3px 0 0 0;
    }

    .link i {
        font-size: small;
        margin-right: 3px;
    }

    .link button {
        height: 20px;
        width: 20px;
        margin-left: 5px;
        padding: 0;
    }

    .link button i {
        font-size: xx-small;
        padding-left: 3px;
    }

    .link:hover {
        background: var(--SELECTED-LINK-BCKGND-CLR);
        cursor: pointer;
        color:white;
    }

    .active {
        background: var(--SELECTED-LINK-BCKGND-CLR);
        cursor: pointer;
        color:white;
    }

    @media (max-width: 1500px) {
        .items-header a span {
            display: none;
        }
    }
</style>