import RegisterView from "@/views/RegisterView.vue";
import HomeView from "@/views/HomeView.vue";
import ChapterCreateView from "@/views/ChapterCreateView.vue";
import ChapterDetailsView from "@/views/ChapterDetailsView.vue";
import ChapterEditView from "@/views/ChapterEditView.vue";
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
            path: '/chapters/create',
            name: 'create-chapter',
            component: ChapterCreateView            
        },
        {
            path: '/chapters/:chapterId',
            name: 'chapter-details',
            component: ChapterDetailsView            
        },
        {
            path: '/chapters/:chapterId/edit',
            name: 'edit-chapter',
            component: ChapterEditView        
        },
        {
            path: '/:catchAll(.*)', // any resource which doesn't exist
            name: 'home',
            component: HomeView
        }
    ]
});

router.afterEach(async (to, from) => {
    // if(from.name === 'chapter-details' || from.name === 'create-chapter') {
    //     store.commit('renderSearchBar');
    // }

    // if(to.name === 'chapter-details' || to.name === 'edit-chapter') {
    //     await store.dispatch('downloadChapter', to.params['chapterId']);
    // }
});

export default router;