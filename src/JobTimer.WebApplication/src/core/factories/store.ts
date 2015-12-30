/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/amplifyjs/amplifyjs.d.ts" />

namespace Core {
    export class Store {

        public static get Help(): string { return "Help"; }


        Set(key: string, data) {
            amplify.store(key, data);
        };
        Exists(key: string): boolean {
            return !angular.isUndefined(amplify.store(key));
        };
        Get<T>(key: string): T {
            return amplify.store(key) as T;
        };
        Clear(key: string) {
            amplify.store(key, null);
        };

    }

    angular.module("core")
        .factory("store", [() => {
            return new Store();
        }]);
}