// ready: initial the language selection
$(document).ready(function () {
    var url = window.location.href.toString();
    if (url.indexOf("en_US") > -1) {
        $("#langSelect").val("en_US")
    } else if (url.indexOf("zh_CN") > -1) {
        $("#langSelect").val("zh_CN")
    } else if (url.indexOf("zh_HK") > -1) {
        $("#langSelect").val("zh_HK")
    } else if (url.indexOf("th_TH") > -1) {
        $("#langSelect").val("th_TH")
    }
})

// onchange of switch language selection
function switchLang(lang) {
    var url = window.location.href.toString();
    if (url.indexOf("en_US") > -1) {
        url = url.replace("en_US", lang);
    } else if (url.indexOf("zh_CN") > -1) {
        url = url.replace("zh_CN", lang);
    } else if (url.indexOf("zh_HK") > -1) {
        url = url.replace("zh_HK", lang);
    } else if (url.indexOf("th_TH") > -1) {
        url = url.replace("th_TH", lang);
    } else {
        url += lang + "/";
    }
    window.location.href = url;
}