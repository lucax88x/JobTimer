/// <reference path="../../../typings/angularjs/angular.d.ts" />
var Core;
(function (Core) {
    angular.module('core')
        .directive('toFocus', ["$timeout", function ($timeout) {
            return {
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs, ngModel) {
                        if (attrs.toFocus) {
                            scope.$watch(attrs.toFocus, function (val) {
                                $timeout(function () {
                                    elm[0].focus();
                                });
                            });
                        }
                    };
                }
            };
        }]);
})(Core || (Core = {}));
