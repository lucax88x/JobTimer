/***
 * Contains basic SlickGrid formatters.
 * 
 * NOTE:  These are merely examples.  You will most likely need to implement something more
 *        robust/extensible/localizable/etc. for your use!
 * 
 * @module Formatters
 * @namespace Slick
 */

(function ($) {
    // register namespace
    $.extend(true, window, {
        "Slick": {
            "Formatters": {
                "Checkmark": CheckmarkFormatter,
                "Date": DateFormatter,
                "Year": YearFormatter,
                "Object": ObjectFormatter
            }
        }
    });



    function CheckmarkFormatter(row, cell, value, columnDef, dataContext) {
        return value ? "<i class='fa green med fa-check'></i>" : "<i class='fa red med fa-times'></i>";
    }

    function DateFormatter(row, cell, value, columnDef, dataContext) {
        var result = "";
        if (value) {
            var date = moment(value);
            if (date.isValid()) {
                result = date.format("DD/MM/YYYY");
            }
            else {
                date = moment(value, "DD/MM/YYYY");
                if (date.isValid()) {
                    result = date.format("DD/MM/YYYY");
                }
            }
        }
        return result;
    }

    function YearFormatter(row, cell, value, columnDef, dataContext) {
        var result = "";
        if (value) {
            var date = moment(value);
            if (date.isValid()) {
                result = date.format("YYYY");
            }
            else {
                date = moment(value, "DD/MM/YYYY");
                if (date.isValid()) {
                    result = date.format("YYYY");
                }
            }
        }
        return result;
    }

    function ObjectFormatter(row, cell, value, columnDef, dataContext) {
        var result = "";
        if (value) {
            if (columnDef["object-property"]) {
                if (value[columnDef["object-property"]]) {
                    result = value[columnDef["object-property"]]
                }
            }
        }
        return result;
    }

})(jQuery);
