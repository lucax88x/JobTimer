var JobTimer;
(function (JobTimer) {
    var Charts;
    (function (Charts) {
        angular.module("charts")
            .directive("chart", ["$injector", "$timeout", "ajaxer", "notifier", function ($injector, $timeout, ajaxer, notifier) {
                return {
                    restrict: "E",
                    templateUrl: "/templates/chart.html",
                    replace: true,
                    scope: true,
                    compile: function (elm, attrs) {
                        return function (scope, elm, attrs) {
                            if (attrs.options) {
                                var options = $injector.get(attrs.options);
                                var chart;
                                function refresh() {
                                    scope.$emit("chart-panel-lock", true);
                                    ajaxer.postEmpty(options.url).then(function (d) {
                                        for (var i = 0; i < d.Data.series.length; i++) {
                                            chart.series[i].setData(d.Data.series[i].data);
                                        }
                                        // chart.redraw();
                                    }, function (d) {
                                        notifier.Error(d);
                                    }).finally(function () {
                                        scope.$emit("chart-panel-lock", false);
                                    });
                                }
                                var initialized = false;
                                scope.$on("chart-refresh", function () {
                                    refresh();
                                    if (!initialized) {
                                        elm.highcharts(options.options);
                                        chart = elm.highcharts();
                                        $(window).resize(function () {
                                            chart.reflow();
                                        });
                                    }
                                });
                            }
                            ;
                        };
                    }
                };
            }]);
    })(Charts = JobTimer.Charts || (JobTimer.Charts = {}));
})(JobTimer || (JobTimer = {}));
