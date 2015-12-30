/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />
/// <reference path="../../../typings/underscore/underscore.d.ts" />
var JobTimer;
(function (JobTimer) {
    var Timer;
    (function (Timer) {
        angular.module("timer")
            .controller("timer", ["$rootScope", "$scope", "$timeout", "ajaxer", "locker", "notifier", "userDataFactory", function ($rootScope, $scope, $timeout, ajaxer, locker, notifier, userDataFactory) {
                userDataFactory.Get().then(function (userData) {
                    $scope.enterDisabled = true;
                    $scope.enterLunchDisabled = true;
                    $scope.exitLunchDisabled = true;
                    $scope.exitDisabled = true;
                    function updateStatus(status) {
                        $scope.enterDisabled = !status.Enter;
                        $scope.enterLunchDisabled = !status.EnterLunch;
                        $scope.exitLunchDisabled = !status.ExitLunch;
                        $scope.exitDisabled = !status.Exit;
                    }
                    $scope.date = new Date();
                    $scope.saveEnter = function () {
                        $scope.lock = true;
                        ajaxer.post("api/timer/saveenter", { Offset: $scope.offset, Date: $scope.date }).then(function (d) {
                            userDataFactory.Update().then(function () {
                                $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You arrived");
                                $timeout(function () {
                                    $scope.$broadcast("enter-finished");
                                }, 500);
                                updateStatus(d.Status);
                            }, function (d) {
                                notifier.Error(d);
                                $scope.$broadcast("enter-finished");
                            }).finally(function () {
                                $scope.lock = false;
                            });
                        }, function (d) {
                            notifier.Error(d);
                            $scope.$broadcast("enter-finished");
                        }).finally(function () {
                            $scope.lock = false;
                        });
                    };
                    $scope.saveEnterLunch = function () {
                        $scope.lock = true;
                        ajaxer.post("api/timer/saveenterlunch", { Offset: $scope.offset, Date: $scope.date }).then(function (d) {
                            userDataFactory.Update().then(function () {
                                $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You're going to lunch");
                                $timeout(function () {
                                    $scope.$broadcast("enter-lunch-finished");
                                }, 500);
                                updateStatus(d.Status);
                            }, function (d) {
                                notifier.Error(d);
                                $scope.$broadcast("enter-lunch-finished");
                            }).finally(function () {
                                $scope.lock = false;
                            });
                        }, function (d) {
                            notifier.Error(d);
                            $scope.$broadcast("enter-lunch-finished");
                        }).finally(function () {
                            $scope.lock = false;
                        });
                    };
                    $scope.saveExitLunch = function () {
                        $scope.lock = true;
                        ajaxer.post("api/timer/saveexitlunch", { Offset: $scope.offset, Date: $scope.date }).then(function (d) {
                            userDataFactory.Update().then(function () {
                                $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You're back from lunch");
                                $timeout(function () {
                                    $scope.$broadcast("exit-lunch-finished");
                                }, 500);
                                updateStatus(d.Status);
                            }, function (d) {
                                notifier.Error(d);
                                $scope.$broadcast("exit-lunch-finished");
                            }).finally(function () {
                                $scope.lock = false;
                            });
                        }, function (d) {
                            notifier.Error(d);
                            $scope.$broadcast("exit-lunch-finished");
                        }).finally(function () {
                            $scope.lock = false;
                        });
                    };
                    $scope.saveExit = function () {
                        $scope.lock = true;
                        ajaxer.post("api/timer/saveexit", { Offset: $scope.offset, Date: $scope.date }).then(function (d) {
                            userDataFactory.Update().then(function () {
                                $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You are going out");
                                $timeout(function () {
                                    $scope.$broadcast("exit-finished");
                                }, 500);
                                updateStatus(d.Status);
                            }, function (d) {
                                notifier.Error(d);
                                $scope.$broadcast("exit-finished");
                            }).finally(function () {
                                $scope.lock = false;
                            });
                        }, function (d) {
                            notifier.Error(d);
                            $scope.$broadcast("exit-finished");
                        }).finally(function () {
                            $scope.lock = false;
                        });
                    };
                    function updateTime() {
                        var today = moment();
                        var date = moment().add($scope.offset, "minute");
                        if (today.dayOfYear() !== date.dayOfYear()) {
                            if ($scope.offset > 0) {
                                date = today.endOf("day");
                            }
                            else if ($scope.offset < 0) {
                                date = today.startOf("day");
                            }
                        }
                        $scope.time = date.format("HH:mm");
                    }
                    $scope.$watch("offset", function (e, d) {
                        updateTime();
                    });
                    $scope.openDate = function () {
                        $rootScope.$broadcast("openTrueDatePicker", true);
                    };
                    $scope.$watch("date", function (d) {
                        var m = moment(d);
                        if (m.isValid()) {
                            console.log(m.format());
                            $scope.lock = true;
                            ajaxer.post("api/timer/getstatus", { Date: d }).then(function (d) {
                                updateStatus(d.Status);
                            }, function (d) {
                                notifier.Error(d);
                            }).finally(function () {
                                $scope.lock = false;
                            });
                        }
                    });
                });
            }]);
    })(Timer = JobTimer.Timer || (JobTimer.Timer = {}));
})(JobTimer || (JobTimer = {}));
