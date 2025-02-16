import RegisterView from "@/components/RegisterView.vue";
import MainView from "@/components/MainView.vue";
import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'main',
            component: MainView
        },
        {
            path: '/authentication/register',
            name: 'register',
            component: RegisterView
        }
    ]
});

export default router;