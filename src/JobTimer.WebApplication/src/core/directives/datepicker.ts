/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/bootstrap.datepicker/bootstrap.datepicker.d.ts" />

namespace Core {

    interface IDatePickerAttr extends ng.IAttributes {
        datePickerType: string;
        datePickerOpenChannel: string;
    }

    class DatePickerOptions implements DatepickerOptions {
        calendarWeeks: boolean = true;
        autoclose: boolean = true;
        todayHighlight: boolean = true;
        todayBtn: boolean = true;
        // orientation: string = "top right";
        minViewMode: any;
        format: string;
        endDate: string = "0d";
    }

    angular.module("core")
        .directive("datePicker", [() => {
            return {
                require: "?ngModel",
                restrict: "E",
                templateUrl: "/templates/date-picker.html",
                replace: true,
                compile: (elm, attrs) => {
                    return (scope: ng.IScope, elm, attrs: IDatePickerAttr, ngModel) => {

                        let input = elm.find("input:text");

                        let opts: DatePickerOptions = new DatePickerOptions();

                        let datePickerType = "normal";
                        if (!angular.isUndefined(attrs.datePickerType)) {
                            datePickerType = attrs.datePickerType.toLowerCase();
                        }

                        let $date;
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

                        $date.on("changeDate", (e, v) => {
                            let newDate = $date.datepicker("getDate");
                            let m = moment(newDate);
                            if (m.isValid()) {
                                scope.$evalAsync(() => {
                                    ngModel.$setViewValue(m.format("YYYY-MM-DDT00:00:00"));
                                });
                            }
                        });

                        if (attrs.datePickerOpenChannel) {
                            scope.$on(attrs.datePickerOpenChannel, () => {
                                $date.datepicker("show");
                            });
                        }
                    };
                }
            };
        }]);
}
