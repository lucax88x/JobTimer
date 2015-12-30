namespace JobTimer.Charts {
    export interface IChartOptions {
        options: HighchartsOptions;
        url: string;
    }
    interface IChartScope extends ng.IScope {
    }
    interface IChartAttrs extends ng.IAttributes {
        options: string;
    }

    angular.module("charts")
        .directive("chart", ["$injector", "$timeout", "ajaxer", "notifier", ($injector, $timeout: ng.ITimeoutService, ajaxer: Core.Ajaxer, notifier: Core.Notifier) => {
            return {
                restrict: "E",
                templateUrl: "/templates/chart.html",
                replace: true,
                scope: true,
                compile: (elm: JQuery, attrs) => {
                    return (scope: IChartScope, elm: JQuery, attrs: IChartAttrs) => {
                        if (attrs.options) {
                            let options: IChartOptions = $injector.get(attrs.options);
                            let chart: HighchartsChartObject;

                            function refresh() {
                                scope.$emit("chart-panel-lock", true);
                                ajaxer.postEmpty<ViewModels.Chart.ChartViewModel<any>>(options.url).then((d) => {

                                    for (let i = 0; i < d.Data.series.length; i++) {
                                        chart.series[i].setData(d.Data.series[i].data);
                                    }

                                    // chart.redraw();

                                }, (d) => {
                                    notifier.Error(d);
                                }).finally(() => {
                                    scope.$emit("chart-panel-lock", false);
                                });
                            }

                            let initialized = false;
                            scope.$on("chart-refresh", () => {
                                refresh();

                                if (!initialized) {
                                    elm.highcharts(options.options);
                                    chart = elm.highcharts();
                                    $(window).resize(function() {
                                        chart.reflow();
                                    });
                                }
                            });
                        };
                    }
                }
            };
        }]);
}
