/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/jquery.scrollup/jquery.scrollup.d.ts" />
var Core;
(function (Core) {
    angular.module('core')
        .directive('scrollUp', [function () {
            return {
                replace: true,
                compile: function (elm, attrs) {
                    $.scrollUp({
                        scrollName: "scroll-up",
                        scrollText: '<span class="fa fa-chevron-up"></span>',
                        topDistance: '300',
                        topSpeed: 300,
                        animation: 'fade',
                        animationInSpeed: 200,
                        animationOutSpeed: 200,
                        activeOverlay: false,
                    });
                }
            };
        }]);
})(Core || (Core = {}));
