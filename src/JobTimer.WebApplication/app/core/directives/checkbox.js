var Core;
(function (Core) {
    angular.module('core')
        .directive('checkbox', ["$parse", function ($parse) {
            return {
                restrict: "A",
                require: '?ngModel',
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs, ngModel) {
                        elm.bootstrapSwitch({
                            onText: '<i class="fa fa-check"></i>',
                            offText: '<i class="fa fa-times"></i>',
                            onColor: 'success',
                            offColor: 'danger'
                        });
                        elm.on('switchChange.bootstrapSwitch', function (event, state) {
                            ngModel.$setViewValue(state);
                        });
                        scope.$watch(attrs.ngModel, function (v) {
                            elm.bootstrapSwitch('state', v, false);
                        });
                    };
                }
            };
        }]);
})(Core || (Core = {}));
