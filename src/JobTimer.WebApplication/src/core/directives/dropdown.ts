/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/toastr/toastr.d.ts" />

namespace Core {
    angular.module('core')
        .directive('dropdown', [() => {
            return {
                restrict: "A",
                compile: function(elm, attrs) {
                    return function(scope, elm, attrs) {
                        if (attrs.dropdownHidden) {
                            elm.on("hidden.bs.dropdown", () => {
                                scope.$emit(attrs.dropdownHidden, true);
                            });
                        }
                    };
                }
            };
        }]);
}
