/// <reference path="../../../typings/tabs/tabs.d.ts" />

namespace Core {
    angular.module('core')
        .directive('tabs', [() => {
            return {
                restrict: "A",
                compile: (elm: JQuery, attrs) => {
                    
                    new CBPFWTabs(elm[0]);
                    return function(scope, elm, attrs) {
                    };
                }
            };
        }]);
}
