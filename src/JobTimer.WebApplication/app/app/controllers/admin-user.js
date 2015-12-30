var App;
(function (App) {
    angular.module("app")
        .controller("admin-user", ["$scope", "ajaxer", function ($scope, ajaxer) {
            $scope.columns = [
                { id: "ID", name: "ID", field: "ID", width: 0, minWidth: 0, maxWidth: 0, cssClass: "ng-hide", headerCssClass: "ng-hide" },
                { id: "UserName", name: "UserName", field: "UserName", width: 250 },
                { id: "Email", name: "Email", field: "Email", width: 250 }
            ];
            $scope.isNew = function (id) {
                return angular.isUndefined(id) || (!angular.isUndefined(id) && id.length === 0);
            };
            $scope.canAddToRole = function (role, user) {
                if (role && user) {
                    if (!angular.isArray(user.Roles)) {
                        user.Roles = [];
                    }
                    if (user.Roles.indexOf(role.Name) === -1) {
                        return true;
                    }
                }
                return false;
            };
            $scope.addToRole = function (role, user) {
                if (role && user) {
                    if (user.Roles.indexOf(role.Name) === -1) {
                        user.Roles.push(role.Name);
                    }
                }
            };
            $scope.removeFromRole = function (role, user) {
                if (role && user) {
                    var idx = user.Roles.indexOf(role.Name);
                    if (idx !== -1) {
                        user.Roles.splice(idx, 1);
                    }
                }
            };
            function getRoles() {
                $scope.gridLocker = true;
                ajaxer.postEmpty("/api/adminuser/getroles")
                    .then(function (data) {
                    $scope.gridLocker = false;
                    $scope.data.Roles = data.Roles;
                }, function () {
                    $scope.gridLocker = false;
                });
            }
            getRoles();
        }]);
})(App || (App = {}));
