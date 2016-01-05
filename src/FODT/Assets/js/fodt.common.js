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


$(function () {
    $(document).on('click', '.js-edit-modal-link', function (e) {
        e.preventDefault();
        var url = $(this).attr('href') || $(this).data('url');
        $.get(url, function (html) {
            $(html).modal();
        });
    });
});