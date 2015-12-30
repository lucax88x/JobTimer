/// <reference path="../../../typings/progress-button/progress-button.d.ts" />
/// <reference path="../../../typings/slider/slider.d.ts" />

namespace JobTimer.Timer {
    interface ISliderScope extends ng.IScope {
        model: number;
    }
    interface ISliderAttrs extends ng.IAttributes {
    }
    angular.module("timer")
        .directive("slider", ["$window", ($window: ng.IWindowService) => {
            return {
                restrict: "E",
                template: "<input type='text'/>",
                replace: true,
                scope: {
                    model: "=sliderModel"
                },
                compile: (elm: JQuery, attrs: ISliderAttrs) => {

                    let differenceInMinutesFromNextDay = moment.duration(moment().add(1, "day").startOf("day").diff(moment())).asMinutes();
                    differenceInMinutesFromNextDay -= differenceInMinutesFromNextDay % 15;

                    let differenceInMinutesFromPrevDay = moment.duration(moment().diff(moment().startOf("day"))).asMinutes()
                    differenceInMinutesFromNextDay -= differenceInMinutesFromNextDay % 15;

                    let slider = elm.slider({
                        tooltip: "hide",
                        value: 0,
                        min: -differenceInMinutesFromPrevDay,
                        max: differenceInMinutesFromNextDay,
                        step: 15,

                        formatter: (value: number): string => {
                            let negative = value < 0;
                            value = Math.abs(value);
                            let hours = Math.floor(value / 60);

                            if (hours > 0) {
                                let minutes = value - (hours * 60);
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

                    return (scope: ISliderScope, elm, attrs, ngModel) => {
                        scope.model = 0;

                        slider.on("slide", (evt: ISliderSlideEvent) => {
                            scope.$evalAsync(() => {
                                scope.model = evt.value;
                            });
                        });
                    };
                }
            };
        }]);
}
