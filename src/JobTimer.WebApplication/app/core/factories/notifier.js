/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/toastr/toastr.d.ts" />
var Core;
(function (Core) {
    var Notifier = (function () {
        function Notifier() {
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": true,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "onclick": null,
                "showDuration": 300,
                "hideDuration": 1000,
                "timeOut": 12000,
                "extendedTimeOut": 1000,
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            };
        }
        Notifier.prototype.Info = function (message) {
            toastr.info(message);
        };
        Notifier.prototype.Success = function (message) {
            toastr.success(message);
        };
        Notifier.prototype.Error = function (message) {
            toastr.error(message);
        };
        return Notifier;
    })();
    Core.Notifier = Notifier;
    angular
        .module("core")
        .factory("notifier", [function () {
            return new Notifier();
        }]);
})(Core || (Core = {}));
