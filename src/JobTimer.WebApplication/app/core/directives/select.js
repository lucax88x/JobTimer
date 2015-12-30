var Core;
(function (Core) {
    angular.module('core')
        .directive('select', ["$parse", "$timeout", "ajaxer", "cache", function ($parse, $timeout, ajaxer, cache) {
            return {
                restrict: "E",
                require: '?ngModel',
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs, ngModel) {
                        elm.selectpicker({
                            width: "100%"
                        });
                        if (elm.is(":hidden")) {
                            elm.show();
                        }
                        var cacheDisabled = attrs.cache === "false";
                        var $sel = elm.siblings("div.bootstrap-select");
                        var parsed = $parse(attrs.ajaxData);
                        function resetLoadOnClick() {
                            $sel.on("click.firstAjax", function () {
                                $sel.off("click.firstAjax");
                                var opts = {
                                    css: { border: 'none', 'opacity': '0.9', 'background-color': 'transparent', 'width': '100%' },
                                    overlayCSS: { opacity: .6, 'border-radius': '4px' },
                                    bindEvents: true,
                                    baseZ: 1060,
                                    message: '<img width="16" height="16" alt="" src="/content/images/loading/loading.svg">',
                                    centerY: true
                                };
                                $sel.block(opts);
                                var params = null;
                                if (attrs.ajaxGetParams) {
                                    params = scope.$eval(attrs.ajaxGetParams);
                                }
                                ajaxer.post(attrs.ajaxUrl, params).then(function (d) {
                                    var arr = d[attrs.ajaxDataProperty];
                                    if (arr) {
                                        if (!cacheDisabled) {
                                            cache.Cache(attrs.ajaxData, arr);
                                        }
                                        parsed.assign(scope, arr);
                                    }
                                    $timeout(function () {
                                        elm.selectpicker('refresh');
                                    });
                                    $sel.unblock();
                                }, function () {
                                    $sel.unblock();
                                });
                            });
                        }
                        if (attrs.resetChannel) {
                            scope.$on(attrs.resetChannel, function (evt) {
                                parsed.assign(scope, []);
                                resetLoadOnClick();
                            });
                        }
                        scope.$watch($parse(attrs.ngModel), function (v) {
                            if (v) {
                                if (attrs.ajaxUrl && ((!cacheDisabled && !cache.IsCached(attrs.ajaxData)) || cacheDisabled)) {
                                    var existingArray_1 = parsed(scope);
                                    if (!angular.isArray(existingArray_1)) {
                                        existingArray_1 = [];
                                    }
                                    var alreadyPresents = _.filter(existingArray_1, function (n) {
                                        return n.ID == v.ID;
                                    });
                                    if (alreadyPresents.length === 0) {
                                        existingArray_1.push(v);
                                        parsed.assign(scope, existingArray_1);
                                    }
                                    ngModel.$setViewValue(v);
                                    $timeout(function () {
                                        elm.selectpicker('refresh');
                                    });
                                }
                                else {
                                    elm.selectpicker('refresh');
                                }
                            }
                            else {
                                var existingArray = parsed(scope);
                                if (angular.isArray(existingArray) && existingArray.length > 0) {
                                    ngModel.$setViewValue(existingArray[0]);
                                    $timeout(function () {
                                        elm.selectpicker('refresh');
                                    });
                                }
                            }
                        });
                        if (attrs.ajaxUrl) {
                            if ((!cacheDisabled && !cache.IsCached(attrs.ajaxData)) || cacheDisabled) {
                                resetLoadOnClick();
                            }
                            else {
                                $timeout(function () {
                                    elm.selectpicker('refresh');
                                });
                            }
                        }
                        if (attrs.array) {
                            scope.$watch(attrs.array, function (v) {
                                if (angular.isArray(v)) {
                                    $timeout(function () {
                                        elm.selectpicker('refresh');
                                    });
                                }
                            });
                        }
                    };
                }
            };
        }]);
})(Core || (Core = {}));
