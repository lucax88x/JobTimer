/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/bootstrap.datepicker/bootstrap.datepicker.d.ts" />
var Core;
(function (Core) {
    var DatePickerOptions = (function () {
        function DatePickerOptions() {
            this.calendarWeeks = true;
            this.autoclose = true;
            this.todayHighlight = true;
            this.todayBtn = true;
            this.endDate = "0d";
        }
        return DatePickerOptions;
    })();
    angular.module("core")
        .directive("datePicker", [function () {
            return {
                require: "?ngModel",
                restrict: "E",
                templateUrl: "/templates/date-picker.html",
                replace: true,
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs, ngModel) {
                        var input = elm.find("input:text");
                        var opts = new DatePickerOptions();
                        var datePickerType = "normal";
                        if (!angular.isUndefined(attrs.datePickerType)) {
                            datePickerType = attrs.datePickerType.toLowerCase();
                        }
                        var $date;
                        switch (datePickerType) {
                            case "year":
                                {
                                    opts.minViewMode = 2;
                                    opts.format = "yyyy";
                                    $date = input.datepicker(opts);
                                    break;
                                }
                            case "month":
                                {
                                    opts.format = "dd/mm/yyyy";
                                    opts.minViewMode = 1;
                                    $date = input.datepicker(opts);
                                    break;
                                }
                            default:
                                {
                                    opts.format = "dd/mm/yyyy";
                                    $date = input.datepicker(opts);
                                    break;
                                }
                        }
                        $date.on("changeDate", function (e, v) {
                            var newDate = $date.datepicker("getDate");
                            var m = moment(newDate);
                            if (m.isValid()) {
                                scope.$evalAsync(function () {
                                    ngModel.$setViewValue(m.format("YYYY-MM-DDT00:00:00"));
                                });
                            }
                        });
                        if (attrs.datePickerOpenChannel) {
                            scope.$on(attrs.datePickerOpenChannel, function () {
                                $date.datepicker("show");
                            });
                        }
                    };
                }
            };
        }]);
})(Core || (Core = {}));
