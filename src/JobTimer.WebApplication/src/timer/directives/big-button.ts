/// <reference path='../../../typings/progress-button/progress-button.d.ts' />

namespace JobTimer.Timer {
    interface IBigButtonScope extends ng.IScope {
        disabled: boolean;
    }
    interface IBigButtonAttrs extends ng.IAttributes {
        onFinish: string;
    }
    angular.module("timer")
        .directive("bigButton", [() => {
            return {
                restrict: "E",
                template: "<button class='progress-button btn-block' data-horizontal ng-class='{ \"active\": !disabled, \"disabled\": disabled }'><span ng-transclude></span></span></button>",
                replace: true,
                transclude: true,
                scope: {
                    disabled: "=ngDisabled"
                },
                compile: (elm: JQuery, attrs: IBigButtonAttrs) => {
                    let progressButton = new ProgressButton(elm[0])

                    return (scope: IBigButtonScope, elm, attrs, ngModel) => {
                        scope.$on(attrs.onFinish, () => {
                            progressButton._stop(1);
                        });
                    };
                }
            };
        }]);
}
