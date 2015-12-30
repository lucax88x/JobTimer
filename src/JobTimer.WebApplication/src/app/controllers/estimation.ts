/// <reference path="../../../typings/angularjs/angular.d.ts" />

namespace App {

    interface IEstimationScope extends ng.IScope {
        data: UserData;
        hasEstimatedExitTime: boolean;
        visible: boolean;
    }

    angular.module("app")
        .controller("estimation", ["$scope", "userDataFactory", ($scope: IEstimationScope, userDataFactory: UserDataFactory) => {

            userDataFactory.Get().then((userData) => {
                $scope.visible = true;

                $scope.data = userData;
            });
        }]);
}