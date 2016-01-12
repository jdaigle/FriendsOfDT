﻿if (typeof (fodt) == "undefined") fodt = {};

/* Quick and Dirty Form Posting */
fodt.doPost = function (url, params) {
    var form = document.createElement("form");
    form.action = url;
    form.method = "POST";
    form.style.display = "none";
    for (var x in params) {
        var opt = document.createElement("textarea");
        opt.name = x;
        opt.value = params[x];
        form.appendChild(opt);
    }
    document.body.appendChild(form);
    form.submit();
    return form;
};

fodt.postLink = function (event, params) {
    fodt.doPost($(event.target || event.srcElement).closest("a").attr("href"), params);
    return false;
}

// global ajax event handlers
$(document)
    .bind('ajaxStart', function () {
        $('#ajax_spinner').fadeIn(100);
    })
    .bind('ajaxStop', function () {
        $('#ajax_spinner').fadeOut(500);
    })
    .bind("ajaxError", function (event, jqXHR, ajaxSettings, thrownError) {
        // TODO global ajax error handler?
        console.log("textStatus=" + textStatus.status);
        console.log("textStatus=" + textStatus.statusText);
    });

$(function () {

    // handles any <button> or <a> with  the class "js-edit-modal-link"
    // it grabs the resource at the associated href or url, and loads
    // the HTML into a modal dialog
    $(document).on('click', '.js-edit-modal-link', function (e) {
        e.preventDefault();
        var url = $(this).attr('href') || $(this).data('url');
        $.get(url, function (html) {
            $(html).modal();
        }, "html");
    });

    // changes the parent <span> to be active on hover and unhover
    $('.delete-button').hover(function () {
        $(this).parent('span').toggleClass('delete-button-span-active');
    });

    // does a quick post to the href of the button and reloads the page if successful
    $(document).on('click', '.delete-button, .js-delete-button', function (e) {
        e.preventDefault();
        var _confirmationText = $(this).data('delete-confirmation-text') || "Are you sure you want to delete this item?";
        if (!confirm(_confirmationText))
            return;
        var url = $(this).attr('href') || $(this).data('url');
        $.post(url, function (data) {
            if (data.message && data.message.length > 0)
                alert(data.message);
            document.location.reload(true);
        }, "json");
    });
});