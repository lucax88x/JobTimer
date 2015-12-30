/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/jquery.blockUI/jquery.blockUI.d.ts" />
/// <reference path="../../../typings/nprogress/nprogress.d.ts" />
var Core;
(function (Core) {
    var Locker = (function () {
        function Locker() {
            this.locks = 0;
        }
        Locker.prototype.Lock = function (element, extOpts) {
            this.locks++;
            // console.log("L:" + this.locks);
            var opts = {
                css: { border: "none", "opacity": "0.9", "background-color": "transparent" },
                overlayCSS: { opacity: 0, "border-radius": "4px" },
                bindEvents: true,
                baseZ: 1060,
                message: "",
                centerY: true
            };
            if (extOpts) {
                $.extend(opts, extOpts);
            }
            NProgress.start();
            if (element) {
                $(element).block(opts);
            }
            else {
                $.blockUI(opts);
            }
        };
        Locker.prototype.Unlock = function (element) {
            this.locks--;
            // console.log("U:" + this.locks);
            if (this.locks < 0) {
                this.locks = 0;
                return;
            }
            if (this.locks === 0) {
                NProgress.done();
                if (element)
                    $(element).unblock();
                else
                    $.unblockUI();
            }
        };
        return Locker;
    })();
    Core.Locker = Locker;
    angular
        .module("core")
        .factory("locker", [function () {
            return new Locker();
        }]);
})(Core || (Core = {}));
