import RegisterView from "@/components/RegisterView.vue";
import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
    history: createWebHistory(import.meta.env.VITE_API_SERVER_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: RegisterView
        },
        {
            path: '/authentication/register',
            name: 'register',
            component: RegisterView
        }
    ]
});

export default router;