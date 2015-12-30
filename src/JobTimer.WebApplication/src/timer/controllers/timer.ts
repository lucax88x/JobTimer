/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />
/// <reference path="../../../typings/underscore/underscore.d.ts" />

namespace JobTimer.Timer {

    interface ITimerScope extends ng.IScope {
        lock: boolean;
        offset: number;
        date: Date;
        time: string;

        enterDisabled: boolean;
        enterLunchDisabled: boolean;
        exitLunchDisabled: boolean;
        exitDisabled: boolean;

        saveEnter();
        saveEnterLunch();
        saveExitLunch();
        saveExit();
        openDate();
    }

    angular.module("timer")
        .controller("timer", ["$rootScope", "$scope", "$timeout", "ajaxer", "locker", "notifier", "userDataFactory", ($rootScope: ng.IRootScopeService, $scope: ITimerScope, $timeout: ng.ITimeoutService, ajaxer: Core.Ajaxer, locker: Core.Locker, notifier: Core.Notifier, userDataFactory: App.UserDataFactory) => {

            userDataFactory.Get().then((userData: App.UserData) => {
                $scope.enterDisabled = true;
                $scope.enterLunchDisabled = true;
                $scope.exitLunchDisabled = true;
                $scope.exitDisabled = true;

                function updateStatus(status: ViewModels.Timer.TimerStatus) {
                    $scope.enterDisabled = !status.Enter;
                    $scope.enterLunchDisabled = !status.EnterLunch;
                    $scope.exitLunchDisabled = !status.ExitLunch;
                    $scope.exitDisabled = !status.Exit;
                }

                $scope.date = new Date();

                $scope.saveEnter = () => {
                    $scope.lock = true;

                    ajaxer.post<BindingModels.Timer.SaveEnterBindingModel, ViewModels.Timer.SaveEnterViewModel>("api/timer/saveenter", { Offset: $scope.offset, Date: $scope.date }).then((d) => {
                        userDataFactory.Update().then(() => {
                            $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You arrived");
                            $timeout(() => {
                                $scope.$broadcast("enter-finished");
                            }, 500);
                            updateStatus(d.Status);
                        }, (d) => {
                            notifier.Error(d);
                            $scope.$broadcast("enter-finished");
                        }).finally(() => {
                            $scope.lock = false;
                        });
                    }, (d) => {
                        notifier.Error(d);
                        $scope.$broadcast("enter-finished");
                    }).finally(() => {
                        $scope.lock = false;
                    });
                };

                $scope.saveEnterLunch = () => {
                    $scope.lock = true;
                    ajaxer.post<BindingModels.Timer.SaveEnterLunchBindingModel, ViewModels.Timer.SaveEnterLunchViewModel>("api/timer/saveenterlunch", { Offset: $scope.offset, Date: $scope.date }).then((d) => {
                        userDataFactory.Update().then(() => {
                            $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You're going to lunch");
                            $timeout(() => {
                                $scope.$broadcast("enter-lunch-finished");
                            }, 500);
                            updateStatus(d.Status);
                        }, (d) => {
                            notifier.Error(d);
                            $scope.$broadcast("enter-lunch-finished");
                        }).finally(() => {
                            $scope.lock = false;
                        });
                    }, (d) => {
                        notifier.Error(d);
                        $scope.$broadcast("enter-lunch-finished");
                    }).finally(() => {
                        $scope.lock = false;
                    });
                };

                $scope.saveExitLunch = () => {
                    $scope.lock = true;
                    ajaxer.post<BindingModels.Timer.SaveExitLunchBindingModel, ViewModels.Timer.SaveExitLunchViewModel>("api/timer/saveexitlunch", { Offset: $scope.offset, Date: $scope.date }).then((d) => {
                        userDataFactory.Update().then(() => {
                            $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You're back from lunch");
                            $timeout(() => {
                                $scope.$broadcast("exit-lunch-finished");
                            }, 500);
                            updateStatus(d.Status);
                        }, (d) => {
                            notifier.Error(d);
                            $scope.$broadcast("exit-lunch-finished");
                        }).finally(() => {
                            $scope.lock = false;
                        });
                    }, (d) => {
                        notifier.Error(d);
                        $scope.$broadcast("exit-lunch-finished");
                    }).finally(() => {
                        $scope.lock = false;
                    });
                };

                $scope.saveExit = () => {
                    $scope.lock = true;
                    ajaxer.post<BindingModels.Timer.SaveExitBindingModel, ViewModels.Timer.SaveExitViewModel>("api/timer/saveexit", { Offset: $scope.offset, Date: $scope.date }).then((d) => {
                        userDataFactory.Update().then(() => {
                            $rootScope.$broadcast("notify", moment(d.Date).format("HH:mm:ss"), userData.email, "You are going out");
                            $timeout(() => {
                                $scope.$broadcast("exit-finished");
                            }, 500);
                            updateStatus(d.Status);
                        }, (d) => {
                            notifier.Error(d);
                            $scope.$broadcast("exit-finished");
                        }).finally(() => {
                            $scope.lock = false;
                        });
                    }, (d) => {
                        notifier.Error(d);
                        $scope.$broadcast("exit-finished");
                    }).finally(() => {
                        $scope.lock = false;
                    });
                };

                function updateTime() {
                    let today = moment();
                    let date = moment().add($scope.offset, "minute");

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

                $scope.$watch("offset", (e, d) => {
                    updateTime();
                });

                $scope.openDate = () => {
                    $rootScope.$broadcast("openTrueDatePicker", true);
                };
                $scope.$watch("date", (d: Date) => {
                    let m = moment(d);

                    if (m.isValid()) {
                        console.log(m.format());
                        $scope.lock = true;
                        ajaxer.post<BindingModels.Timer.GetStatusBindingModel, ViewModels.Timer.GetStatusViewModel>("api/timer/getstatus", { Date: d }).then((d) => {
                            updateStatus(d.Status);
                        }, (d) => {
                            notifier.Error(d);
                        }).finally(() => {
                            $scope.lock = false;
                        });

                    }
                });
            });
        }]);
}