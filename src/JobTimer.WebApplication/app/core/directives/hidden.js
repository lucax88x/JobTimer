var Core;
(function (Core) {
    angular.module('core')
        .directive('hidden', [function () {
            return {
                restrict: "A",
                compile: function (elm, attrs) {
                    elm.show();
                }
            };
        }]);
})(Core || (Core = {}));
