if (typeof (fodt) == "undefined") fodt = {};

fodt.getData = function (url, data, onSuccess, onError) {
    $.ajax({
        url: url,
        data: data,
        cache: false,
        success: onSuccess,
        error: onError
    });
};

fodt.rootUrl = "/";

fodt.buildUrl = function (url) {
    if (url.charAt(0) != '/') {
        return fodt.rootUrl + url;
    } else {
        return fodt.rootUrl + url.substring(1);
    }
};

$(function () {
    "placeholder" in document.createElement("input") || $("textarea[placeholder], input[placeholder]").addClass("gray").each(function () {
        var a = $(this), b = a.attr("placeholder");
        a.val() == "" && a.val(b)
    });
    $("body").on("focus", "textarea[placeholder].gray, input[placeholder].gray", function () {
        $(this).attr("rows", 7).filter(".gray").removeClass("gray").val("")
    });
    $("form").on("submit", function () {
        $(this).find("textarea[placeholder].gray, input[placeholder].gray").removeClass("gray").val("");
    });
});