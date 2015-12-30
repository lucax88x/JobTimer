/// <reference path='../../../typings/progress-button/progress-button.d.ts' />
var JobTimer;
(function (JobTimer) {
    var Timer;
    (function (Timer) {
        angular.module("timer")
            .directive("bigButton", [function () {
                return {
                    restrict: "E",
                    template: "<button class='progress-button btn-block' data-horizontal ng-class='{ \"active\": !disabled, \"disabled\": disabled }'><span ng-transclude></span></span></button>",
                    replace: true,
                    transclude: true,
                    scope: {
                        disabled: "=ngDisabled"
                    },
                    compile: function (elm, attrs) {
                        var progressButton = new ProgressButton(elm[0]);
                        return function (scope, elm, attrs, ngModel) {
                            scope.$on(attrs.onFinish, function () {
                                progressButton._stop(1);
                            });
                        };
                    }
                };
            }]);
    })(Timer = JobTimer.Timer || (JobTimer.Timer = {}));
})(JobTimer || (JobTimer = {}));
