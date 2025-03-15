import { createStore } from "vuex";
import axios from "axios";
import { useToast } from "vue-toastification";

const toast = useToast();

const store = createStore({
    state: {
        serverUrl: import.meta.env.VITE_API_SERVER_URL,
        environment: import.meta.env.VITE_API_ENVIRONMENT,
        roles: [],
        nickname: null,
        chapter: null,
        chapters: [],
        theme: null,
        themes: [],
        documents: [],
        showSearchBar: true,
        title: null,
        imagePaths: 
            [
                'archive-1.png',
                'case-files-1.png',
                'folders-1.png',
                'laptop-1.png',
                'laptop-2.png',
            ],
        showChapterList: true
    },
    getters: {
        getChapter(state) {
            return state.chapter;
        },
        getChapters(state) {
            return state.chapters;
        },
        getTheme(state) {
            return state.theme;
        },
        getThemes(state) {
            return state.themes;
        },
        getDocuments(state) {
            return state.documents;
        },
        serverUrl(state) {
            return state.serverUrl;
        },
        environment(state) {
            return state.environment;
        },
        isAdmin(state) {
            return state.roles.includes('Admin');
        },
        isOwner(state) {
            return state.roles.includes('Owner');
        },
        getShowChapterList(state) {
            return state.showChapterList;
        }
    },
    mutations: {
        setRoles(state, userRoles) {
            state.roles = userRoles;
        },
        setNickname(state, userNickname) {
            state.nickname = userNickname;
        },
        renderSearchBar(state) {
            state.title = null;
            state.showSearchBar = true;
        },
        setTitle(state, value) {
            state.title = value;
            state.showSearchBar = false;
        },
        setChapters(state, chapters) {
            state.chapters = chapters;
        },
        setChapter(state, chapter) {            
            state.chapter = chapter;
        },
        setTheme(state, theme) {
            state.theme = theme;
        },
        setThemes(state, themes) {
            state.themes = themes;
        },
        setDocuments(state, documents) {
            state.documents = documents;
        },
        setShowChapterList(state, value) {
            state.showChapterList = value;
        }
    },
    actions: {
        async downloadChapters({commit, state}) {
            const chapters = (await axios.get(`${state.serverUrl}/chapters/getlist`)).data;
            commit('setChapters', chapters);
        },
        async downloadChapter({commit, state}, chapterId ) {
            const url = `${state.serverUrl}/chapters/get/${chapterId}`;
            const chapter = (await axios.get(url)).data;
            commit('setChapter', chapter);
            commit('setThemes', chapter.themes);
        },
        async downloadTheme({commit, state}, themeId ) {
            if (themeId) {
                const url = `${state.serverUrl}/themes/get/${themeId}`;
                const theme = (await axios.get(url)).data;
                commit('setTheme', theme);
            }
            else {                
                commit('setTheme', state.chapter.themes[0]);
            }
        },
        async downloadDocuments({commit, state}) {
            const documents = (await axios.get(`${state.serverUrl}/documents/getlist`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response.status === 500) {
                        toast.error(error.response.data.message)
                    }
                }));

            commit('setDocuments', documents);
        },
    }
});

export default store;