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
        showSearchBar: true,
        title: null,
        isEditMode: false
    },
    getters: {
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
        async downloadChapters(state) {
            state.chapters = (await axios.get(`${state.serverUrl}/chapters/getlist`).then(response => response).then(data => data)).data;
        },
        async downloadChapter(state, chapterId) {
            const url = `${state.serverUrl}/chapters/get/${chapterId}`;
            const data = (await axios.get(url).then(response => response).then(data => data)).data;
            state.chapter = data;
            state.themes = data.themes;
        },
        setIsEditMode(state, value) {
            state.isEditMode = value;
        }
    }
});

export default store;