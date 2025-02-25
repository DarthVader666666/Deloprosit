import RegisterView from "@/components/RegisterView.vue";
import HomeView from "@/components/HomeView.vue";
import ChapterCreateView from "@/components/ChapterCreateView.vue";
import ChapterDetailsView from "@/components/ChapterDetailsView.vue";
import { createRouter, createWebHistory } from "vue-router";
import store from "@/vuex/store";

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
            path: '/chapters/create',
            name: 'chapters-create',
            component: ChapterCreateView,
            meta:{ conditionalRoute: store.getters.isOwner || store.getters.isAdmin }
        },
        {
            path: '/chapters/:chapterId',
            name: 'chapter-details',
            component: ChapterDetailsView,
        },
        {
            path: '/:catchAll(.*)', // any resource which doesn't exist
            name: 'home',
            component: HomeView
        }
    ]
});

export default router;