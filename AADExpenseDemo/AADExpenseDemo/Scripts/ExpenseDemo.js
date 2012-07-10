/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.5.1-vsdoc.js " />
/// <reference path="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.11/jquery-ui.js" />

$(document).ready(function() {

    $(":input[data-autocomplete]").each(function () {

        $(this).autocomplete({ source: $(this).attr("data-autocomplete") });
    });
    $(":input[data-datepicker]").datepicker();
})