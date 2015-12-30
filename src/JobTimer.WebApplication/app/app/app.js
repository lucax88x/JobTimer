/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../typings/angular-ui-router/angular-ui-router.d.ts" />
var App;
(function (App) {
    angular.module("app", ["core", "ngAnimate", "ui.router"])
        .config(["$stateProvider", "$locationProvider", "$urlRouterProvider", "initializerProvider", function ($stateProvider, $locationProvider, $urlRouterProvider, initializer) {
            initializer.InitializeModules();
            function template(ctrl, url) {
                return [
                    "$http", "$templateCache", "$q", function ($http, $templateCache, $q) {
                        return initializer.Get($http, $templateCache, $q, ctrl, url);
                    }
                ];
            }
            $stateProvider
                .state("401", {
                url: "/401",
                templateUrl: "/errors/401-error.html"
            })
                .state("404", {
                url: "/404",
                templateUrl: "/errors/404-error.html"
            })
                .state("index", {
                url: "/",
                templateProvider: template("home", "/index?SPA=1"),
                controller: "home"
            })
                .state("timer", {
                url: "/timer",
                templateProvider: template("timer", "/timer?SPA=1"),
                controller: "timer"
            })
                .state("charts", {
                url: "/timer/charts",
                templateProvider: template("charts", "/timer/charts?SPA=1"),
                controller: "charts"
            })
                .state("admin-user", {
                url: "/admin/user",
                templateProvider: template("admin-user", "/admin/user?SPA=1"),
                controller: "admin-user"
            });
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false
            });
        }])
        .run(["$rootScope", "$state", "$stateParams", "ajaxer", "$location", function ($rootScope, $state, $stateParams, ajaxer, $location) {
            var speed = 100;
            $rootScope.$on("$stateChangeStart", function (event, toState) {
                var $view = $("#ui-view-hidden");
                $view.css("visibility", "hidden");
                $view.hide();
                $("#ui-loading").fadeIn(speed);
                NProgress.start();
            });
            $rootScope.$on("$stateChangeCancel", function (event, to, toParams, from, fromParams, error) {
            });
            $rootScope.$on("$stateNotFound", function (event, to, toParams, from, fromParams, error) {
                $state.go("404");
            });
            $rootScope.$on("$stateChangeError", function (event, to, toParams, from, fromParams, error) {
                if (error && error.status) {
                    switch (error.status) {
                        case 404:
                            $state.go("404");
                            break;
                        case 401:
                            $state.go("401");
                            break;
                    }
                }
                NProgress.done();
                $("#ui-loading").fadeOut(speed);
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState) {
                angular.element(".modal-backdrop.fade.in").remove();
                var path = $location.path();
                // don"t add the home            
                if (path !== "/") {
                    ajaxer.post("/api/master/saveAsLastVisited", { Url: path });
                    $rootScope.$broadcast("last-visited-added", path);
                }
                NProgress.done();
                $("#ui-loading").fadeOut(speed, function () {
                    var $view = $("#ui-view-hidden");
                    $view.hide();
                    $view.css("visibility", "visible");
                    $view.slideDown(speed);
                });
                // Sets the layout name, which can be used to display different layouts (header, footer etc.)
                // based on which page the user is located
                $rootScope.layout = toState.layout;
                $("html, body").animate({ scrollTop: 0 }, "slow");
            });
        }])
        .config(["$provide", function ($provide) {
            $provide.decorator("$exceptionHandler", ["$delegate", function ($delegate) {
                    return function (exception, cause) {
                        console.log(exception.message);
                        $delegate(exception, cause);
                    };
                }]);
        }]);
})(App || (App = {}));
