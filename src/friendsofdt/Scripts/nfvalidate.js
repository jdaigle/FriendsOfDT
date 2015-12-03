if (typeof (nfvalidate) == "undefined") nfvalidate = {};

// An array of validators to push your validators to
nfvalidate.validators = new Array();

nfvalidate.validators.push({
    name: "validation-required",
    doValidate: function (e) { return !$(e).data("validation-required") || $(e).val() != ''; },
    errorMessage: function (e) { return "Field is required"; }
});

nfvalidate.validators.push({
    name: "validation-maxlength",
    doValidate: function (e) {
        return !($(e).val().length > this.getMaxLength(e));
    },
    errorMessage: function (e) { return "Field cannot be more than " + this.getMaxLength(e) + " characters"; },
    getMaxLength: function (e) { return parseInt($(e).data("validation-maxlength")); }
});

// Finds an Error Message data value or undefined
nfvalidate.findErrorMessage = function (element, validator) {
    return $(element).data(validator.name + "-message");
}

// Returns an array of validation errors for the specific element
nfvalidate.validateElement = function (element) {
    return $.map(nfvalidate.validators, function (validator, i) {
        if ($(element).data(validator.name) === undefined) return null;
        if (validator.doValidate(element) == false) {
            var errorMessage = nfvalidate.findErrorMessage(element, validator);
            if (errorMessage === undefined) errorMessage = validator.errorMessage(element);
            return { element: element, message: errorMessage };
        } else { return null; }
    });
};

// Finds all elements that need validation within the selected element (such as a form) and
// returns an array of all validation errors
nfvalidate.validate = function (selector) {
    var validationSelectors = $.map(nfvalidate.validators, function (e, i) { return "[data-" + e.name + "]"; }).join(",");
    var elementsToValidate = $(selector).find(validationSelectors);
    return $.map(elementsToValidate, function (e, i) { return nfvalidate.validateElement(e); });
};