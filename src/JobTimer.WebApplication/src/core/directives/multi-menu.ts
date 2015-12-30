/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/jquery.multilevelpushmenu/jquery.multilevelpushmenu.d.ts" />

namespace Core {
    angular.module('core')
        .directive('multiMenu', function() {
            return {
                compile: function(elm, attrs) {
                    elm.show();
                    elm.css("top", "55px");

                    elm.multilevelpushmenu({
                        collapsed: true,
                        //mode: 'cover',
                        fullCollapse: true,
                        menuHeight: window.outerHeight,
                        preventItemClick: false,
                        onItemClick: function() {
                            elm.multilevelpushmenu("collapse");
                        }
                    });


                    //if you apply this by css it will wrong width calculation
                    var $div = elm.find("div.levelHolderClass.ltr:first");
                    $div.css("position", "relative");

                    $(document).mouseup(function(e) {
                        if (!elm.is(e.target)
                            && elm.has(e.target).length === 0) {
                            elm.multilevelpushmenu("collapse");
                        }
                    });

                    return function(scope, elm, attrs) {
                        scope.$on("menu-open", function(evt, data) {
                            elm.multilevelpushmenu("expand");
                        });

                        scope.$on("shortcuts-enabled", function(evt, data) {
                            scope.shortcuts = data;
                        });

                        scope.addToShortcuts = function(data) {
                            scope.$broadcast("shortcuts-added", data);
                        }
                    };
                }
            };
        });
}