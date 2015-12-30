var Core;
(function (Core) {
    angular.module('core')
        .directive('touchSpin', [function () {
            return {
                restrict: "A",
                require: '?ngModel',
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs, ngModel) {
                        var decimals = angular.isUndefined(attrs.decimal) ? 0 : attrs.decimal;
                        var step = angular.isUndefined(attrs.step) ? 1 : attrs.step;
                        var postfix = angular.isUndefined(attrs.postfix) ? "" : attrs.postfix;
                        elm.TouchSpin({
                            max: 999999999,
                            decimals: decimals,
                            step: step,
                            postfix: postfix,
                            initval: 0
                        });
                    };
                }
            };
        }]);
})(Core || (Core = {}));
