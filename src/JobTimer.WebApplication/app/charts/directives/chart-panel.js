var JobTimer;
(function (JobTimer) {
    var Charts;
    (function (Charts) {
        angular.module("charts")
            .directive("chartPanel", ["ajaxer", function (ajaxer) {
                return {
                    restrict: "E",
                    templateUrl: "/templates/chart-panel.html",
                    replace: true,
                    scope: {
                        title: "@title",
                        options: "@options"
                    },
                    compile: function (elm, attrs) {
                        return function (scope, elm, attrs, ngModel) {
                            scope.locker = false;
                            scope.$on("chart-panel-lock", function (evt, data) {
                                scope.locker = data;
                            });
                            scope.refresh = function (evt) {
                                evt.stopPropagation();
                                scope.$broadcast("chart-refresh");
                            };
                            elm.on("shown.bs.collapse", function () {
                                scope.$broadcast("chart-refresh");
                            });
                        };
                    }
                };
            }]);
    })(Charts = JobTimer.Charts || (JobTimer.Charts = {}));
})(JobTimer || (JobTimer = {}));
