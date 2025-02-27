import RegisterView from "@/components/RegisterView.vue";
import HomeView from "@/components/HomeView.vue";
import ChapterCreateView from "@/components/ChapterCreateView.vue";
import ChapterDetailsView from "@/components/ChapterDetailsView.vue";
import { createRouter, createWebHistory } from "vue-router";
import store from '@/vuex/store.js';

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
            path: '/chapter-details/:chapterId',
            name: 'chapter-details',
            component: ChapterDetailsView            
        },
        {
            path: '/:catchAll(.*)', // any resource which doesn't exist
            name: 'home',
            component: HomeView
        }
    ]
});

router.afterEach((to) => {
    if(to.name === 'home') {
        store.commit('downloadThemes');
    }

    if(to.name === 'chapter-details') {
        store.commit('downloadChapter', to.params.chapterId);
    }
});

export default router;