namespace Core {
    angular.module('core')
        .directive('popover', [function() {
            return {
                restrict: "A",
                compile: function(elm, attrs) {
                    return function(scope, elm, attrs, ngModel) {
                        if (attrs.popover) {
                            var $content = $(attrs.popover);
                            var placement = "right";
                            if ($content.attr("popover-placement")) {
                                placement = $content.attr("popover-placement");
                            }

                            elm.popover({
                                container: 'body',
                                placement: placement,
                                trigger: 'focus',
                                html: true,
                                content: function() {
                                    return $content.html();
                                }
                            }).click(function(e) {
                                e.preventDefault();
                            });

                            elm.on("shown.bs.popover", function(e) {
                                scope.$evalAsync();
                                console.log(scope);
                            });
                        }
                    }
                }
            }
        }]);
}
