fodt.ajaxFormController = function (formSelector) {
    var controller = this;
    $(formSelector).on("submit", function () {
        controller.submit($(this));
        return false;
    });
}

$.extend(fodt.ajaxFormController.prototype, {
    onSuccess: function (form, data) { return; },
    onError: function (form, jqXHR) { return; },
    onBefore: function (form) { return; },
    onAfter: function (form) { return; },
    submit: function (form) {
        var controller = this;
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            beforeSend: function () {
                var messages = form.find("div.messages");
                messages.empty().hide();
                form.find("div.working").show();
                controller.onBefore(form);
            },
            complete: function () {
                form.find("div.working").hide();
                controller.onAfter(form);
            },
            success: function (data) {
                if (data.redirect) {
                    window.location.href = data.redirect;
                } else {
                    controller.onSuccess(form, data);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var messages = form.find("div.messages");
                messages.empty().html("An Error Occured, Please Try Again").show();
                controller.onError(form, jqXHR);
            }
        });
    }
});