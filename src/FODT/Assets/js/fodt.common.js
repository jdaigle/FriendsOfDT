if (typeof (fodt) == "undefined") fodt = {};

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
    .bind("ajaxSuccess", function (data, textStatus, jqXHR) {
        // TODO check to see if data is a JSON object with a successs message or a redirect
    })
    .bind("ajaxError", function (jqXHR, textStatus, errorThrown) {
        // TODO global ajax error handler
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
        });
    });

    // changes the parent <span> to be active on hover and unhover
    $('.delete-button').hover(function () {
        $(this).parent('span').toggleClass('delete-button-span-active');
    });

    // does a quick post to the href of the button and reloads the page if successful
    $(document).on('click', '.delete-button', function (e) {
        e.preventDefault();
        if (!confirm("Are you sure you want to delete this tag?"))
            return;
        var url = $(this).attr('href');
        var _parentSpan = $(this).parent('span');
        $.post(url, function (data) {
            document.location.reload(true);
        });
    });
});