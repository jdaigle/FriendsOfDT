if (typeof (fodt) == "undefined") fodt = {};

$(function () {
    "placeholder" in document.createElement("input") || $("textarea[placeholder], input[placeholder]").addClass("gray").each(function () {
        var a = $(this), b = a.attr("placeholder");
        a.val() == "" && a.val(b)
    });
    $("textarea.gray, input.gray").focus(function () {
        $(this).attr("rows", 7).filter(".gray").removeClass("gray").val("")
    });
});