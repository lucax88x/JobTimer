/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/bootstrap/bootstrap.d.ts" />
var Core;
(function (Core) {
    angular
        .module("core")
        .directive("modalVisible", ["$parse", function ($parse) {
            return {
                restrict: "A",
                link: function (scope, element, attrs) {
                    // Hide or show the modal                    
                    function showModal(visible, elem) {
                        if (!elem)
                            elem = element;
                        if (visible)
                            $(elem).modal("show");
                        else
                            $(elem).modal("hide");
                    }
                    if (attrs.modalVisibleType === "emit") {
                        scope.$on(attrs.modalVisible, function (evt, data, bindData) {
                            if (bindData) {
                                // don"t use angular copy
                                for (var key in bindData) {
                                    scope[key] = bindData[key];
                                }
                            }
                            showModal(data, element);
                        });
                    }
                    else {
                        var parsed = $parse(attrs.modalVisible);
                        scope.$watch(attrs.modalVisible, function (newValue, oldValue) {
                            showModal(newValue, element);
                        });
                        element.bind("hide.bs.modal", function () {
                            if (attrs.modalHide) {
                                scope.$eval(attrs.modalHide);
                            }
                            scope.$evalAsync(function () {
                                parsed.assign(scope, false);
                            });
                        });
                        if (attrs.modalShown) {
                            element.on("shown.bs.modal", function (e) {
                                scope.$eval(attrs.modalShown);
                            });
                        }
                    }
                }
            };
        }]);
})(Core || (Core = {}));
