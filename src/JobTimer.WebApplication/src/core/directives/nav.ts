namespace Core {
        angular.module('core')
                .directive('nav', function() {
                        return {
                                restrict: "E",
                                compile: function(elm, attrs) {
                                        return function(scope, elm, attrs) {
                                                var $menu = angular.element("#menu");
                                                $menu.css({ "top": "50px" });
                                                var $containers = angular.element(".cd-panel-container");
                                                elm.autoHidingNavbar({
                                                        animationDuration: 200,
                                                        showOnBottom: false,
                                                        showOnUpscroll: false,
                                                        hideOffset: 50,
                                                        onShow: function() {
                                                                angular.element("body").removeClass("without-top");
                                                                
                                                                $menu.animate({ "top": "50px" }, 200);
                                                                $containers.animate({ "top": "55px" }, 200);
                                                        },
                                                        onHide: function() {
                                                                angular.element("body").addClass("without-top");
                                                                
                                                                $menu.animate({ "top": "0" }, 200);
                                                                $containers.animate({ "top": "0" }, 200);
                                                        }
                                                });
                                        };
                                }
                        };
                });
}
