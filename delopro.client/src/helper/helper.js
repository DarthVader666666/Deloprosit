import store from "@/vuex/store";

export const helper = {
    getUnicodeByteArray: (text) => {
        const utf8Encode = new TextEncoder();
        return Object.values(utf8Encode.encode(text));
    },
    validateEmail: (email) => email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/),
    timeoutAsync: (ms) => new Promise(resolve => setTimeout(resolve, ms)),
    getCurrentDate: () => {
        const today = new Date();
        const day = String(today.getDate()).padStart(2, '0');
        const month = String(today.getMonth() + 1).padStart(2, '0');
        const year = today.getFullYear();
        const hours = withLeadingZero(today.getHours());
        const minutes = withLeadingZero(today.getMinutes());
        const seconds = withLeadingZero(today.getSeconds());

        function withLeadingZero(value) {
            if(String(value).length < 2) {
                return `0${value}`;
            };

            return value;
        };
    
        return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes + ':' + seconds;
    },
    getDateString(dateValue, short = false) {
        const date = new Date(dateValue);

        if(short) {
            return date.toLocaleDateString('ru-RU', {day: 'numeric', month: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit'});
        }
        else {
            return date.toLocaleDateString('ru-RU', {day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit'});
        }        
    },
    getQueryString(array, key) {
        const queryString = array.map(value => `${key}=${value}&`).join('').slice(0, -1);
        return '?' + queryString;
    },
    getImagePath() {
        return store.getters.environment === 'development' ? '/src/assets/chapter-' : '/chapter-';
    },
    scrollToTheme(themeId) {
        if(themeId) {
            let links = document.getElementsByClassName('link active');

            for (let item of links) {
                item.classList.remove('active');
            }
        
            document.getElementById(`listItem_${themeId}`).classList.add('active');
        }
    },
    closeMenu(event, ids, hasSelect = false) {
        let isValidClick = false;

        if(hasSelect) {
            let select = document.getElementsByClassName('p-select-list-container')[0];

            if(anyChildren(event, select)) 
                isValidClick = true;
        }
        
        ids.forEach(id => {
            if(anyChildren(event, document.getElementById(id))) 
                isValidClick = true;
        });

        return isValidClick;

        function anyChildren(event, element) {
            if(element && element.children.length) {
                if(event.target === element) {
                    return true;
                }
        
                for (let i = 0; i < element.children.length; i++) {
                    if(event.target === element.children[i]) {
                        return true;
                    }
                    else {
                        if(anyChildren(event, element.children[i])) {
                            return true;
                        }
                    }
                }        
            }
        
            return false;
        }
    }
}