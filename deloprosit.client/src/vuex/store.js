import { createStore } from "vuex";

const store = createStore({
    state: {
        serverUrl: import.meta.env.VITE_API_SERVER_URL,
        environment: import.meta.env.VITE_API_ENVIRONMENT
    },
    getters: {
        serverUrl(state) {
            return state.serverUrl;
        },
        environment(state) {
            return state.environment;
        }
    },
    mutations: {
        }
    }
);

export default store;