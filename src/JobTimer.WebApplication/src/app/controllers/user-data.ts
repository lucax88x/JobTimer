/// <reference path="../../../typings/angularjs/angular.d.ts" />

namespace App {

    class Notification {
        isNew: boolean;
        constructor(private Date: string, private Username: string, private Action: string, isNew: boolean = true) {
            this.isNew = isNew;
        }
    }

    interface IUserDataScope extends ng.IScope {
        logoutLocker: boolean;
        userData: UserData;

        notifications: Array<Notification>;

        logout();
        hasNewNotifications(): boolean;
        getNewNotificationsCount(): number;
    }

    angular.module("app")
        .controller("userData", ["$scope", "$window", "hubs", "ajaxer", "notifier", "userDataFactory", ($scope: IUserDataScope, $window: ng.IWindowService, hubs: Core.Hubs, ajaxer: Core.Ajaxer, notifier: Core.Notifier, userDataFactory: UserDataFactory) => {

            $scope.notifications = [];

            userDataFactory.Get().then((userData) => {
                $scope.userData = userData;
            });

            let notificationHub = hubs.Get<JobTimer.Hubs.INotificationHubProxy>("notificationHub");

            function notify(date: string, username: string, action: string) {
                $scope.notifications.push(new Notification(date, username, action));
                notifier.Info("<b>" + username + "</b>:&nbsp;" + action);
            };

            notificationHub.client.updateModel = (model) => {
                if (model) {
                    $scope.$evalAsync(() => {
                        notify(model.Date, model.Username, model.Action);
                    });
                }
            };

            $scope.$on("notify", (evt, date: string, username: string, action: string) => {
                console.log(date);
                notify(date, username, action);
            });

            $scope.getNewNotificationsCount = (): number => {
                var onlyNews = _.filter<Notification>($scope.notifications, (n: Notification): boolean => {
                    return n.isNew;
                });
                return onlyNews.length;
            }
            $scope.hasNewNotifications = (): boolean => {
                var resultIndex = _.findIndex<Notification>($scope.notifications, (n: Notification): boolean => {
                    return n.isNew;
                });
                return resultIndex !== -1;
            };

            // $scope.notifications.push(new Notification(Date.now().toString(), "testa vuota", "ha fatto danni"));
            // $scope.notifications.push(new Notification(Date.now().toString(), "testapiena", "ha fatto salcazzo voleva", false));            
            hubs.Connect();
            hubs.Connected().then(() => {                
                // notificationHub.server.updateModel({ Username: "da client", Date: "01092015" })
            });

            $scope.$on("alert-dropdown-closed", (evt, d) => {
                if (d) {
                    $scope.$evalAsync(() => {
                        $scope.notifications.forEach((not, i) => {
                            not.isNew = false;
                        });
                    });
                }
            });

            $scope.logout = function() {
                $scope.logoutLocker = true;
                ajaxer.postEmpty<ViewModels.Account.LogoutViewModel>(
                    "/api/account/logout")
                    .then(
                    (data) => {
                        $window.location.href = "/login";
                    });
            }
        }]);
}