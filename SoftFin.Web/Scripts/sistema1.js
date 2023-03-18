$(function () {
    $.datepicker.setDefaults({ dateFormat: 'dd/mm/yy' });
    $("#datepicker").datepicker();
});

$(function () {
    $.datepicker.setDefaults({ dateFormat: 'dd/mm/yy' });
    $(".datepicker").datepicker();
});

$(function () {
    $('.gridRow td').dblclick(function () {
        $.ajax({
            url: '@Url.Action("Contrato", "Contrato")',
            type: 'POST',
            success: function (result) {
                // do something with the result from your AJAX call
            }
        });
    });
});

$(function () {
    $("#dialog").dialog();
});

