/// <reference path="../../../typings/introjs/introjs.d.ts" />
var App;
(function (App) {
    var HelpStorage = (function () {
        function HelpStorage() {
            this.FirstTime = true;
            this.Pages = {};
        }
        return HelpStorage;
    })();
    var OptionsFactory = (function () {
        function OptionsFactory() {
        }
        OptionsFactory.GetIndexHelpOptions = function () {
            return {
                steps: [
                    {
                        element: $("#cPages")[0],
                        intro: "Those are your actually available shortcuts",
                        position: "bottom"
                    },
                    {
                        element: $("#btn-editable-shortcuts")[0],
                        intro: "If you want to add, move or remove your shortcuts, use this button",
                        position: "bottom-left-aligned"
                    },
                    {
                        element: $("#menu-btn")[0],
                        intro: "This is the button which opens the menu",
                        position: "bottom"
                    },
                    {
                        element: $("#btn-notifications")[0],
                        intro: "These are your notifications",
                        position: "bottom"
                    },
                    {
                        element: $("#btn-logout")[0],
                        intro: "If you want to logout, use this button",
                        position: "bottom"
                    },
                    {
                        element: $("#tile-timer")[0],
                        intro: "For the home, it's over, now you should move to the timer page, click on the shortcut!",
                        position: "bottom"
                    }
                ]
            };
        };
        OptionsFactory.GetTimerHelpOptions = function () {
            return {
                steps: [
                    {
                        element: $("#btn-enter")[0],
                        intro: "When you come to job, you press this button, and you start the time tracking",
                        position: "bottom"
                    },
                    {
                        element: $("#btn-enter-lunch")[0],
                        intro: "When you go to lunch or in pause, you press this button, and you stop the time tracking",
                        position: "right"
                    },
                    {
                        element: $("#btn-exit-lunch")[0],
                        intro: "When you are back from your lunch or pause, you press this button, and you resume the time tracking",
                        position: "left"
                    },
                    {
                        element: $("#btn-exit")[0],
                        intro: "When you are ready to leave office, you press this button, it will end the time tracking. However, if you may need to resume the time tracking, you can press Enter again.",
                        position: "top"
                    },
                    {
                        element: $("#date-picker")[0],
                        intro: "If you forgot to press the button in time, you can use this date-picker in order to adjust the enter/exit date",
                        position: "bottom"
                    },
                    {
                        element: $("#hour-slider").siblings(".slider-horizontal")[0],
                        intro: "If you forgot to press the button in time, you can use this slider in order to adjust the enter/exit time",
                        position: "top"
                    },
                    {
                        element: ($("#btn-estimatedExitTime").length !== 0 ? $("#btn-estimatedExitTime")[0] : $("#btn-noEstimatedExitTime")[0]),
                        intro: "When you're in job time, it will tell you when's your exit time",
                        position: "bottom"
                    },
                    {
                        element: $("#btn-offset")[0],
                        intro: "It will tell you your overtime offset, green if you are a stakanovist, red if you need to recover hours",
                        position: "bottom"
                    }
                ]
            };
        };
        return OptionsFactory;
    })();
    angular.module("app")
        .controller("help", ["$rootScope", "$scope", "$timeout", "store", function ($rootScope, $scope, $timeout, store) {
            var helpStorage;
            if (store.Exists(Core.Store.Help)) {
                helpStorage = store.Get(Core.Store.Help);
            }
            else {
                helpStorage = new HelpStorage();
            }
            var intro = introJs();
            intro.onexit(function () {
                $scope.$evalAsync(function () {
                    $scope.isHavingHelp = false;
                });
            });
            intro.oncomplete(function () {
                $scope.$evalAsync(function () {
                    $scope.isHavingHelp = false;
                });
            });
            var currentState;
            $rootScope.$on("$stateChangeStart", function (event, toState) {
                intro.exit();
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState) {
                currentState = toState.name;
                switch (currentState) {
                    case "index":
                        $scope.hasHelp = true;
                        if (helpStorage.FirstTime) {
                            helpStorage.FirstTime = false;
                            store.Set(Core.Store.Help, helpStorage);
                            $timeout(function () {
                                $scope.isHavingHelp = true;
                                intro.setOptions({
                                    steps: [
                                        {
                                            intro: "Welcome to JobTimer!"
                                        },
                                        {
                                            element: $(".btn-help")[0],
                                            intro: "If you are a new user of the site, feel free to use this button on each page, it will explain you how to properly do your time tracking",
                                            position: "left"
                                        }
                                    ]
                                });
                                intro.start();
                            });
                        }
                        break;
                    case "timer":
                        $scope.hasHelp = true;
                        break;
                    default:
                        $scope.hasHelp = false;
                        break;
                }
                if ($scope.hasHelp) {
                    if (!helpStorage.Pages[currentState]) {
                        helpStorage.Pages[currentState] = true;
                        store.Set(Core.Store.Help, helpStorage);
                        $scope.newPage = true;
                    }
                    else {
                        $scope.newPage = false;
                    }
                }
            });
            $scope.startHelp = function () {
                $scope.isHavingHelp = false;
                intro.exit();
                var options;
                switch (currentState) {
                    case "index":
                        {
                            intro.setOptions(OptionsFactory.GetIndexHelpOptions());
                            intro.start();
                        }
                        break;
                    case "timer":
                        {
                            intro.setOptions(OptionsFactory.GetTimerHelpOptions());
                            intro.start();
                        }
                        break;
                }
            };
        }]);
})(App || (App = {}));
