/// <reference path="../../../typings/progress-button/progress-button.d.ts" />
/// <reference path="../../../typings/slider/slider.d.ts" />
var JobTimer;
(function (JobTimer) {
    var Timer;
    (function (Timer) {
        angular.module("timer")
            .directive("slider", ["$window", function ($window) {
                return {
                    restrict: "E",
                    template: "<input type='text'/>",
                    replace: true,
                    scope: {
                        model: "=sliderModel"
                    },
                    compile: function (elm, attrs) {
                        var differenceInMinutesFromNextDay = moment.duration(moment().add(1, "day").startOf("day").diff(moment())).asMinutes();
                        differenceInMinutesFromNextDay -= differenceInMinutesFromNextDay % 15;
                        var differenceInMinutesFromPrevDay = moment.duration(moment().diff(moment().startOf("day"))).asMinutes();
                        differenceInMinutesFromNextDay -= differenceInMinutesFromNextDay % 15;
                        var slider = elm.slider({
                            tooltip: "hide",
                            value: 0,
                            min: -differenceInMinutesFromPrevDay,
                            max: differenceInMinutesFromNextDay,
                            step: 15,
                            formatter: function (value) {
                                var negative = value < 0;
                                value = Math.abs(value);
                                var hours = Math.floor(value / 60);
                                if (hours > 0) {
                                    var minutes = value - (hours * 60);
                                    if (minutes === 0) {
                                        return ((negative) ? "- " : "") + hours + "hr ";
                                    }
                                    else {
                                        return ((negative) ? "- " : "") + hours + "hr " + minutes + "m";
                                    }
                                }
                                else {
                                    return ((negative) ? "- " : "") + value + "m";
                                }
                            }
                        });
                        return function (scope, elm, attrs, ngModel) {
                            scope.model = 0;
                            slider.on("slide", function (evt) {
                                scope.$evalAsync(function () {
                                    scope.model = evt.value;
                                });
                            });
                        };
                    }
                };
            }]);
    })(Timer = JobTimer.Timer || (JobTimer.Timer = {}));
})(JobTimer || (JobTimer = {}));
