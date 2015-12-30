/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/toastr/toastr.d.ts" />

namespace Core {

    export class Notifier {
        constructor() {
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

        Info(message: string) {
            toastr.info(message);
        }
        Success(message: string) {
            toastr.success(message);
        }
        Error(message: string) {
            toastr.error(message);
        }
    }

    angular
        .module("core")
        .factory("notifier", [() => {
            return new Notifier();
        }]);
}
