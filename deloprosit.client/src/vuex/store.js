import { createStore } from "vuex";
import axios from "axios";
import { useToast } from "vue-toastification";

const toast = useToast();

const store = createStore({
    state: {
        captcha: null,
        serverUrl: import.meta.env.VITE_API_SERVER_URL,
        environment: import.meta.env.VITE_API_ENVIRONMENT,
        roles: [],
        nickname: null,
        chapter: null,
        chapters: [],
        chapterNodes: [],
        theme: null,
        themes: [],
        documents: [],
        documentNodes: [],
        messages: [],
        message: null,
        unreadMessagesCount: 0,
        chapterSearchResult: [],
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
        getChapterNodes(state) {
            return state.chapterNodes;
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
        getDocumentNodes(state) {
            return state.documentNodes;
        },
        getMessages(state) {
            return state.messages;
        },
        getMessage(state) {
            return state.message;
        },
        getUnreadMessagesCount(state) {
            return state.unreadMessagesCount;
        },
        getChapterSearchResult(state) {
            return state.chapterSearchResult;
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
        },
        getCaptcha(state) {
            return state.captcha;
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
        setChapter(state, chapter) {            
            state.chapter = chapter;
        },
        setChapters(state, chapters) {
            state.chapters = chapters;
        },
        setChapterNodes(state, chapterNodes) {
            state.chapterNodes = chapterNodes;
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
        setDocumentNodes(state, documentNodes) {
            state.documentNodes = documentNodes;
        },
        setMessages(state, messages) {
            state.messages = messages;
        },
        setMessage(state, message) {
            state.message = message;
        },
        setUnreadMessagesCount(state, count) {
            state.unreadMessagesCount = count;
        },
        setMessageById(state, messageId) {
            state.message = state.messages.find(x => x.messageId === messageId);
        },
        setChapterSearchResult(state, chapterSearchResult) {
            state.chapterSearchResult = chapterSearchResult;
        },
        setShowChapterList(state, value) {
            state.showChapterList = value;
        },
        setCaptcha(state, value) {
            state.captcha = value;
        }
    },
    actions: {
        async downloadChapter({commit, state}, chapterId ) {
            const url = `${state.serverUrl}/chapters/get/${chapterId}`;
            const chapter = (await axios.get(url)).data;
            commit('setChapter', chapter);
            commit('setThemes', chapter.themes);
        },
        async downloadChapters({commit, state}) {
            const chapters = (await axios.get(`${state.serverUrl}/chapters/getlist`)).data;
            commit('setChapters', chapters);
        },
        async downloadChapterNodes({commit, state}) {
            const chapterNodes = (await axios.get(`${state.serverUrl}/chapters/getnodes`)).data;
            commit('setChapterNodes', chapterNodes);
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
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setDocuments', documents);
        },
        async downloadDocumentNodes({commit, state}) {
            const documentNodes = (await axios.get(`${state.serverUrl}/documents/getnodes`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setDocumentNodes', documentNodes);
        },
        async downloadMessages({commit, state}, isRead) {
            const messages = (await axios.get(`${state.serverUrl}/feedback/getlist/${isRead}`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setMessages', messages);
        },
        async downloadMessage({commit, state}, messageId) {
            const message = await axios.get(`${state.serverUrl}/feedback/get/${messageId}`)
                .then(response => {
                    if(response.status == 200) {
                        return response.data;
                    }
                })
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                });

            commit('setMessage', message);
        },
        async downloadUnreadMessagesCount({commit, state}) {
            const count = await axios.get(`${state.serverUrl}/feedback/getunreadmessagescount`)
                .then(response => {
                    if(response.status == 200) {
                        return response.data;
                    }
                });

            commit('setUnreadMessagesCount', count);
        },
        async downloadChapterSearchResult({commit, state}, searchLine) {
            const chapterSearchResult = (await axios.post(`${state.serverUrl}/chapters/search`,
                {
                    searchLine: searchLine
                }
            )
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setChapterSearchResult', chapterSearchResult);
        },
        async downloadCaptcha({commit, state}) {
            const captcha = (await axios.get(`${state.serverUrl}/captcha/get`)
            .then(response => response.data)
            .catch(error => {
                if(error.response) {
                    toast.error(error.response.data.errorText)
                }
            }));

            commit('setCaptcha', captcha);
        }
    }
});

export default store;