fodt.ajaxFormController = function (formSelector) {
    var controller = this;
    $(formSelector).on("submit", function () {
        controller.submit($(this));
        return false;
    });
    $(formSelector).append($("<div>").addClass("working").html("Please Wait"));
    $(formSelector).append($("<div>").addClass("messages").html("Please Wait"));
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
                form.find("div.messages").empty().hide();
                form.find("div.working").hide();
                form.find("span.error").remove();
                var errors = nfvalidate.validate(form);
                if (errors.length > 0) {
                    for (var i = 0; i < errors.length; i++) {
                        var error = errors[i];
                        $(error.element).after($("<span>").addClass("error").html(error.message));
                    }
                    return false;
                }
                controller.onBefore(form);
                form.find("div.working").show();
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
                form.find("div.messages").empty().html("An Error Occured, Please Try Again").show();
                controller.onError(form, jqXHR);
            }
        });
    }
});