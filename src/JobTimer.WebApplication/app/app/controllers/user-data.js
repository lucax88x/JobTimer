/// <reference path="../../../typings/angularjs/angular.d.ts" />
var App;
(function (App) {
    var Notification = (function () {
        function Notification(Date, Username, Action, isNew) {
            if (isNew === void 0) { isNew = true; }
            this.Date = Date;
            this.Username = Username;
            this.Action = Action;
            this.isNew = isNew;
        }
        return Notification;
    })();
    angular.module("app")
        .controller("userData", ["$scope", "$window", "hubs", "ajaxer", "notifier", "userDataFactory", function ($scope, $window, hubs, ajaxer, notifier, userDataFactory) {
            $scope.notifications = [];
            userDataFactory.Get().then(function (userData) {
                $scope.userData = userData;
            });
            var notificationHub = hubs.Get("notificationHub");
            function notify(date, username, action) {
                $scope.notifications.push(new Notification(date, username, action));
                notifier.Info("<b>" + username + "</b>:&nbsp;" + action);
            }
            ;
            notificationHub.client.updateModel = function (model) {
                if (model) {
                    $scope.$evalAsync(function () {
                        notify(model.Date, model.Username, model.Action);
                    });
                }
            };
            $scope.$on("notify", function (evt, date, username, action) {
                console.log(date);
                notify(date, username, action);
            });
            $scope.getNewNotificationsCount = function () {
                var onlyNews = _.filter($scope.notifications, function (n) {
                    return n.isNew;
                });
                return onlyNews.length;
            };
            $scope.hasNewNotifications = function () {
                var resultIndex = _.findIndex($scope.notifications, function (n) {
                    return n.isNew;
                });
                return resultIndex !== -1;
            };
            // $scope.notifications.push(new Notification(Date.now().toString(), "testa vuota", "ha fatto danni"));
            // $scope.notifications.push(new Notification(Date.now().toString(), "testapiena", "ha fatto salcazzo voleva", false));            
            hubs.Connect();
            hubs.Connected().then(function () {
                // notificationHub.server.updateModel({ Username: "da client", Date: "01092015" })
            });
            $scope.$on("alert-dropdown-closed", function (evt, d) {
                if (d) {
                    $scope.$evalAsync(function () {
                        $scope.notifications.forEach(function (not, i) {
                            not.isNew = false;
                        });
                    });
                }
            });
            $scope.logout = function () {
                $scope.logoutLocker = true;
                ajaxer.postEmpty("/api/account/logout")
                    .then(function (data) {
                    $window.location.href = "/login";
                });
            };
        }]);
})(App || (App = {}));
