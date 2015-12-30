/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/amplifyjs/amplifyjs.d.ts" />
var Core;
(function (Core) {
    var Store = (function () {
        function Store() {
        }
        Object.defineProperty(Store, "Help", {
            get: function () { return "Help"; },
            enumerable: true,
            configurable: true
        });
        Store.prototype.Set = function (key, data) {
            amplify.store(key, data);
        };
        ;
        Store.prototype.Exists = function (key) {
            return !angular.isUndefined(amplify.store(key));
        };
        ;
        Store.prototype.Get = function (key) {
            return amplify.store(key);
        };
        ;
        Store.prototype.Clear = function (key) {
            amplify.store(key, null);
        };
        ;
        return Store;
    })();
    Core.Store = Store;
    angular.module("core")
        .factory("store", [function () {
            return new Store();
        }]);
})(Core || (Core = {}));
