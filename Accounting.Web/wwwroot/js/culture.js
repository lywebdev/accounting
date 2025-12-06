window.culture = {
    set: function (cookieValue) {
        document.cookie = ".AspNetCore.Culture=" + cookieValue + "; path=/";
    }
};
