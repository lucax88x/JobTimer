namespace Core {
    angular
        .module("core")
        .directive("locker", [
            "locker", (locker: Core.Locker) => {
                return {
                    restrict: "A",
                    controller: ["$scope", "$element", "$attrs", ($scope, $element: JQuery, $attrs) => {
                        function handle(val: boolean) {
                            if (!angular.isUndefined(val)) {
                                if (typeof (val) === "boolean") {
                                    if (val === true) {
                                        locker.Lock($element);
                                    } else {
                                        locker.Unlock($element);
                                    }
                                }
                                else {
                                    console.error("not a boolean!");
                                }
                            }
                        }

                        if ($attrs.lockerType === "emit") {
                            $scope.$on($attrs.locker, (evt, data) => {
                                handle(data);
                            });
                        }
                        else {
                            $scope.$watch($attrs.locker, handle);
                        }
                    }]
                }
            }
        ]);
}
