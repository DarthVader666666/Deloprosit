import RegisterView from "@/views/RegisterView.vue";
import HomeView from "@/views/HomeView.vue";
import ChapterCreateView from "@/views/ChapterCreateView.vue";
import ChapterDetailsView from "@/views/ChapterDetailsView.vue";
import ChapterEditView from "@/views/ChapterEditView.vue";
import ThemeEditView from "@/views/ThemeEditView.vue";
import FeedBackView from "@/views/FeedBackView.vue";
import { createRouter, createWebHistory } from "vue-router";
import store from '@/vuex/store.js';
import MessagesView from "@/views/MessagesView.vue";
import SearchResultView from "@/views/SearchResultView.vue";

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
            path: '/chapters/:chapterId/:themeId?',
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
        },
        {
            path: '/themes/:themeId/edit',
            name: 'edit-theme',
            component: ThemeEditView        
        },
        {
            path: '/feedback',
            name: 'feedback',
            component: FeedBackView
        },
        {
            path: '/messages',
            name: 'messages',
            component: MessagesView
        },
        {
            path: '/search-result',
            name: 'search-result',
            component: SearchResultView
        }
    ]
});

router.afterEach(async (to) => {
    if(to.name === 'chapter-details') {
        await store.dispatch('downloadChapter', to.params['chapterId']);
        await store.dispatch('downloadTheme', to.params['themeId']);
        store.commit('renderSearchBar');
        store.commit('setShowChapterList', false);        
    }
    else {
        store.commit('setShowChapterList', true);
        store.commit('setTheme', null);
    }

    if(to.name === 'edit-theme') {
        await store.dispatch('downloadTheme', to.params['themeId']);
        store.commit('setTitle', 'Редактирование темы');
    }

    if(to.name === 'edit-chapter') {
        await store.dispatch('downloadChapter', to.params['chapterId']);
        store.commit('setTitle', 'Редактирование раздела');
    }

    if(to.name === 'create-chapter') {
        store.commit('setTitle', 'Создание нового раздела');
    }

    if(to.name === 'register') {
        store.commit('setTitle', 'Заполните форму регистрации');        
        const captchaInput =  document.getElementById('captcha-input');

        if(captchaInput) {
            captchaInput.value = null;
        }

        await store.dispatch('downloadCaptcha');
    }

    if(to.name === 'home') {
        store.commit('renderSearchBar');
    }

    if(to.name === 'feedback') {
        store.commit('setTitle', 'Напишите ваше сообщение');
        const captchaInput =  document.getElementById('captcha-input');

        if(captchaInput) {
            captchaInput.value = null;
        }

        await store.dispatch('downloadCaptcha');
    }

    if(to.name === 'messages') {
        await store.dispatch('downloadMessages', false);
        store.commit('setTitle', 'Сообщения');
     }

    await store.dispatch('downloadChapters');
    await store.dispatch('downloadChapterNodes');
    await store.dispatch('downloadDocuments');
    await store.dispatch('downloadDocumentNodes');
    window.scrollTo(0, 0);
});

export default router;