/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/toastr/toastr.d.ts" />
var Core;
(function (Core) {
    angular.module('core')
        .directive('dropdown', [function () {
            return {
                restrict: "A",
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs) {
                        if (attrs.dropdownHidden) {
                            elm.on("hidden.bs.dropdown", function () {
                                scope.$emit(attrs.dropdownHidden, true);
                            });
                        }
                    };
                }
            };
        }]);
})(Core || (Core = {}));
