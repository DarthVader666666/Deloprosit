import { createStore } from "vuex";
import axios from "axios";

const store = createStore({
    state: {
        serverUrl: import.meta.env.VITE_API_SERVER_URL,
        environment: import.meta.env.VITE_API_ENVIRONMENT,
        roles: [],
        nickname: null,
        chapter: null,
        chapters: [],
        themes: [],
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
        getThemes(state) {
            return state.themes;
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
        setThemes(state, themes) {
            state.themes = themes;
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
        }
    }
});

export default store;