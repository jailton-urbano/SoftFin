/*
* Localized default methods for the jQuery validation plugin.
* Locale: DE
*/
jQuery.extend(jQuery.validator.methods, {
    date: function (value, element) {
        return this.optional(element) || /^\d\d?\.\d\d?\.\d\d\d?\d?$/.test(value);
    },
    number: function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    }
});


jQuery(function ($) {
    $('#inputReais').maskMoney({ showSymbol: true, symbol: "R$", decimal: ",", thousands: "." });
});