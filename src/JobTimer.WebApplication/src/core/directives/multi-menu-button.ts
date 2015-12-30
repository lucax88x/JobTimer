/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/jquery.multilevelpushmenu/jquery.multilevelpushmenu.d.ts" />

namespace Core {
    angular.module('core')
        .directive('multiMenuButton', [function() {
            return {
                compile: function(elm, attrs) {
                    var $menu = $('#menu');
                    if ($menu.length > 0) {
                        var $ul = $menu.find("ul:first");
                        elm.on("click", function() {
                            var menuExpanded = $ul.is(":visible");
                            if (menuExpanded) {
                                $menu.multilevelpushmenu('collapse');
                            }
                            else {
                                $menu.multilevelpushmenu('expand');
                            }
                        });
                        if (elm && elm.show) {
                            elm.show();
                        }
                    }
                }
            };
        }]);
}