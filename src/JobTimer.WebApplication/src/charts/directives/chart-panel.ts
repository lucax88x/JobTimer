namespace JobTimer.Charts {
    interface IChartPanelScope extends ng.IScope {
        locker: boolean;
        refresh(evt: ng.IAngularEvent);
    }
    angular.module("charts")
        .directive("chartPanel", ["ajaxer", (ajaxer: Core.Ajaxer) => {
            return {
                restrict: "E",
                templateUrl: "/templates/chart-panel.html",
                replace: true,
                scope: {
                    title: "@title",
                    options: "@options"
                },
                compile: (elm: JQuery, attrs) => {
                    return (scope: IChartPanelScope, elm: JQuery, attrs, ngModel) => {
                        scope.locker = false;

                        scope.$on("chart-panel-lock", (evt: ng.IAngularEvent, data: boolean) => {
                            scope.locker = data;
                        });

                        scope.refresh = (evt: ng.IAngularEvent) => {
                            evt.stopPropagation();
                            scope.$broadcast("chart-refresh");
                        }

                        elm.on("shown.bs.collapse", () => {
                            scope.$broadcast("chart-refresh");
                        });
                    }
                }
            };
        }]);
}
