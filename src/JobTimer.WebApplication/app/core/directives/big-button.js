var Core;
(function (Core) {
    angular.module('core')
        .directive('bigButton', ["$parse", function ($parse) {
            return {
                restrict: "e",
                template: '<button class="btn btn-success btn-xl">Enter</button>',
                replace: true,
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs, ngModel) {
                    };
                }
            };
        }]);
})(Core || (Core = {}));
