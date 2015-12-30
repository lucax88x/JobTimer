/// <reference path="../../../typings/tabs/tabs.d.ts" />
var Core;
(function (Core) {
    angular.module('core')
        .directive('tabs', [function () {
            return {
                restrict: "A",
                compile: function (elm, attrs) {
                    new CBPFWTabs(elm[0]);
                    return function (scope, elm, attrs) {
                    };
                }
            };
        }]);
})(Core || (Core = {}));
