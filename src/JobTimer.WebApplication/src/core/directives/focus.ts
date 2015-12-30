/// <reference path="../../../typings/angularjs/angular.d.ts" />

namespace Core {
    angular.module('core')
        .directive('toFocus', ["$timeout", ($timeout: ng.ITimeoutService) => {
            return {
                compile: (elm, attrs) => {
                    return (scope: ng.IScope, elm, attrs, ngModel) => {
                        if (attrs.toFocus) {
                            scope.$watch(attrs.toFocus, (val) => {
                                $timeout(() => {
                                    elm[0].focus();
                                });
                            });
                        }
                    }
                }
            };
        }]);
}