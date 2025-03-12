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
    getDateString(dateValue) {
        const date = new Date(dateValue);
        return date.toLocaleDateString('ru-RU', {day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit'});
    },
    getQueryString(array, key) {
        const queryString = array.map(value => `${key}=${value}&`).join('').slice(0, -1);
        return '?' + queryString;
    },
    getImagePath(imagePath) {
        return store.state.environment === 'development' ? '/src/assets/images/' + imagePath : '/' + imagePath
    },
    scrollToTheme(themeId) {
        var links = document.getElementsByClassName('link active');

        for (let item of links) {
            item.classList.remove('active');
        }
    
        document.getElementById(`listItem_${themeId}`).classList.add('active');
        
        var top = document.getElementById(`theme_${themeId}`).offsetTop;
        window.scrollTo(0, top);
    }
}