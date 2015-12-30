/// <reference path="../../../typings/angularjs/angular.d.ts" />
var App;
(function (App) {
    angular.module("app")
        .controller("estimation", ["$scope", "userDataFactory", function ($scope, userDataFactory) {
            userDataFactory.Get().then(function (userData) {
                $scope.visible = true;
                $scope.data = userData;
            });
        }]);
})(App || (App = {}));
