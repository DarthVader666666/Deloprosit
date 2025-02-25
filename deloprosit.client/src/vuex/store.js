import { createStore } from "vuex";
import axios from "axios";

const store = createStore({
    state: {
        serverUrl: import.meta.env.VITE_API_SERVER_URL,
        environment: import.meta.env.VITE_API_ENVIRONMENT,
        roles: [],
        nickname: null,
        chapters: []
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
        async setChapters(state) {
            state.chapters = (await axios.get(`${state.serverUrl}/chapters/getall`).then(response => response).then(data => data)).data;
        }
    }
});

export default store;