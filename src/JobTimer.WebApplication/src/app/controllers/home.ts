/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />
/// <reference path="../../../typings/underscore/underscore.d.ts" />

namespace JobTimer {

    class ShortcutModel {
        Id: string;
    }

    angular.module("app")
        .controller("home", ["$scope", "ajaxer", "notifier", function($scope, ajaxer: Core.Ajaxer, notifier: Core.Notifier) {

            function saveShorcuts(arr) {
                let ids = _.map(arr, function(item: ShortcutModel) {
                    return item.Id;
                });


                ajaxer.post<BindingModels.Home.SaveShortcutsBindingModel, ViewModels.Home.SaveShortcutsViewModel>(
                    "/api/home/saveshortcuts",
                    { Shortcuts: ids });
            }

            $scope.refresh = function() {
                $scope.shortcutLocker = true;
                ajaxer.get<ViewModels.Home.GetShortcutsViewModel>(
                    "/api/home/getshortcuts")
                    .then(
                    (data) => {
                        $scope.shortcuts = data.Items;
                    },
                    (data) => {
                        notifier.Error(data);
                    }).finally(() => {
                        $scope.shortcutLocker = false;
                    });
            };

            // console make the add non sortable
            $scope.shortcutSortableConfig = {
                group: "item",
                animation: 150,
                ghostClass: "ghost",
                onSort: function(evt) {
                    saveShorcuts(evt.models);
                }
            };

            $scope.$on("shortcuts-added", (evt, data) => {
                let alreadyPresent = _.find($scope.shortcuts, (x: ShortcutModel) => { return x.Id === data.Id });

                if (angular.isUndefined(alreadyPresent)) {
                    $scope.shortcuts.push(data);
                    saveShorcuts($scope.shortcuts);
                }
            });

            $scope.shortcutsEditable = (evt: ng.IAngularEvent) => {
                evt.stopPropagation();

                $scope.editable = !$scope.editable;
                $scope.$emit("shortcuts-enabled", $scope.editable);
            };

            $scope.removeShortcut = (shortcut) => {
                let idx = $scope.shortcuts.indexOf(shortcut);
                if (idx !== -1) {
                    $scope.shortcuts.splice(idx, 1);
                    saveShorcuts($scope.shortcuts);
                }
            }

            $scope.addFromMenu = function() {
                $scope.$emit("menu-open");

                $scope.editable = true;
                $scope.$emit("shortcuts-enabled", $scope.editable);
            }

            $scope.refresh();
        }]);
}