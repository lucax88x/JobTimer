var Core;
(function (Core) {
    angular
        .module("core")
        .directive("locker", [
        "locker", function (locker) {
            return {
                restrict: "A",
                controller: ["$scope", "$element", "$attrs", function ($scope, $element, $attrs) {
                        function handle(val) {
                            if (!angular.isUndefined(val)) {
                                if (typeof (val) === "boolean") {
                                    if (val === true) {
                                        locker.Lock($element);
                                    }
                                    else {
                                        locker.Unlock($element);
                                    }
                                }
                                else {
                                    console.error("not a boolean!");
                                }
                            }
                        }
                        if ($attrs.lockerType === "emit") {
                            $scope.$on($attrs.locker, function (evt, data) {
                                handle(data);
                            });
                        }
                        else {
                            $scope.$watch($attrs.locker, handle);
                        }
                    }]
            };
        }
    ]);
})(Core || (Core = {}));
