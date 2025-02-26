import RegisterView from "@/components/RegisterView.vue";
import HomeView from "@/components/HomeView.vue";
import ChapterCreateView from "@/components/ChapterCreateView.vue";
import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView
        },
        {
            path: '/register',
            name: 'register',
            component: RegisterView
        },
        {
            path: '/create-chapter',
            name: 'create-chapter',
            component: ChapterCreateView            
        },
        {
            path: '/:catchAll(.*)', // any resource which doesn't exist
            name: 'home',
            component: HomeView
        }
    ]
});

export default router;