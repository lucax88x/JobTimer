/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />
/// <reference path="../../../typings/underscore/underscore.d.ts" />
var JobTimer;
(function (JobTimer) {
    var ShortcutModel = (function () {
        function ShortcutModel() {
        }
        return ShortcutModel;
    })();
    angular.module("app")
        .controller("home", ["$scope", "ajaxer", "notifier", function ($scope, ajaxer, notifier) {
            function saveShorcuts(arr) {
                var ids = _.map(arr, function (item) {
                    return item.Id;
                });
                ajaxer.post("/api/home/saveshortcuts", { Shortcuts: ids });
            }
            $scope.refresh = function () {
                $scope.shortcutLocker = true;
                ajaxer.get("/api/home/getshortcuts")
                    .then(function (data) {
                    $scope.shortcuts = data.Items;
                }, function (data) {
                    notifier.Error(data);
                }).finally(function () {
                    $scope.shortcutLocker = false;
                });
            };
            // console make the add non sortable
            $scope.shortcutSortableConfig = {
                group: "item",
                animation: 150,
                ghostClass: "ghost",
                onSort: function (evt) {
                    saveShorcuts(evt.models);
                }
            };
            $scope.$on("shortcuts-added", function (evt, data) {
                var alreadyPresent = _.find($scope.shortcuts, function (x) { return x.Id === data.Id; });
                if (angular.isUndefined(alreadyPresent)) {
                    $scope.shortcuts.push(data);
                    saveShorcuts($scope.shortcuts);
                }
            });
            $scope.shortcutsEditable = function () {
                $scope.editable = !$scope.editable;
                $scope.$emit("shortcuts-enabled", $scope.editable);
            };
            $scope.removeShortcut = function (shortcut) {
                var idx = $scope.shortcuts.indexOf(shortcut);
                if (idx !== -1) {
                    $scope.shortcuts.splice(idx, 1);
                    saveShorcuts($scope.shortcuts);
                }
            };
            $scope.addFromMenu = function () {
                $scope.$emit("menu-open");
                $scope.editable = true;
                $scope.$emit("shortcuts-enabled", $scope.editable);
            };
            $scope.refresh();
        }]);
})(JobTimer || (JobTimer = {}));
