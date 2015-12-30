/// <reference path="../../../typings/angularjs/angular.d.ts" />
var App;
(function (App) {
    angular.module("app")
        .controller("master", ["$scope", "$location", function ($scope, $location) {
            $scope.$watch("lastVisited", function (v) {
                if (v) {
                    $location.path(v);
                }
            });
            $scope.$on("last-visited-added", function (evt, v) {
                if (!$scope.data) {
                    $scope.data = {};
                }
                if (!angular.isArray($scope.data.LastVisiteds)) {
                    $scope.data.LastVisiteds = [];
                }
                var indexOfV = $scope.data.LastVisiteds.indexOf(v);
                // already the first
                if (indexOfV !== 0) {
                    if (indexOfV === -1) {
                        if ($scope.data.LastVisiteds.length > 5) {
                            $scope.data.LastVisiteds.pop();
                        }
                    }
                    else {
                        $scope.data.LastVisiteds.splice(indexOfV, 1);
                    }
                    $scope.data.LastVisiteds = [v].concat($scope.data.LastVisiteds);
                }
            });
            $scope.doSearch = function () {
                $scope.searchVisible = true;
            };
        }]);
})(App || (App = {}));
