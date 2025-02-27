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
        const hours = today.getHours();
        const minutes = today.getMinutes();
        const seconds = today.getSeconds();
    
        return year + '-' + month + '-' + day + ' ' + hours + ':' + minutes + ':' + seconds;
    }
}