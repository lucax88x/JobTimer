/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../generated/webapi.d.ts" />
/// <reference path="../../generated/constants.ts" />
/// <reference path="../../../typings/jstz/jstz.d.ts" />

namespace Core {

    export class Ajaxer {
        constructor(private $http: ng.IHttpService, private $q: ng.IQService, private hubs: Hubs) {

        }
        private handleError(def: ng.IDeferred<Object>, msg: any, code: number = 200) {

            let message = "Error!";

            if (!angular.isUndefined(msg)) {
                if (!angular.isUndefined(msg.Message)) {
                    if (msg.Message) {
                        message = msg.Message;
                    }
                }
                else {
                    message = msg;
                }
            }

            switch (code) {
                case 200:
                    def.reject(message);
                    break;
                case 404:
                    def.reject("Not Found!");
                    break;
                case 500:
                    def.reject(message);
                    break;
                default:
                    def.reject(message);
                    break;
            }
        };

        private handleSuccess<V extends ViewModels.BaseViewModel>(def: ng.IDeferred<V>, data: V) {
            if (data.Result === true) {
                def.resolve(data);
            }
            else {
                this.handleError(def, data);
            }
        }

        private buildConfig(url: string, method: string, opts?: ng.IRequestShortcutConfig): ng.IPromise<ng.IRequestConfig> {
            let def = this.$q.defer<ng.IRequestConfig>();

            this.hubs.Connected().then(() => {

                if (url[0] !== "/") {
                    url = "/" + url;
                }

                let config: ng.IRequestConfig = {
                    url: url,
                    method: method,
                    headers: {}
                };

                config.headers[Constants.HttpHeaders.Request.SignalRConnectionId] = this.hubs.Id;
                config.headers[Constants.HttpHeaders.Request.CurrentTimeZone] = jstz.determine().name();

                if (!angular.isUndefined(opts)) {
                    angular.extend(config, opts);
                }

                def.resolve(config);
            },
                () => {
                    def.reject();
                });


            return def.promise;
        }

        get<V extends ViewModels.BaseViewModel>(url: string, opts?: ng.IRequestShortcutConfig): ng.IPromise<V> {
            let def = this.$q.defer<V>();

            this.buildConfig(url, "GET", opts).then((config) => {
                this.$http(config)
                    .success((data: V) => {
                        this.handleSuccess(def, data);
                    }).error((msg, code) => {
                        this.handleError(def, msg, code);
                    });
            }, () => {
                def.reject("problem building config");
            });

            return def.promise;
        };
        post<B, V extends ViewModels.BaseViewModel>(url: string, d: B, opts?: ng.IRequestShortcutConfig): ng.IPromise<V> {

            var def = this.$q.defer<V>();

            this.buildConfig(url, "POST", opts).then((config) => {
                config.data = d;

                this.$http(config)
                    .success((data: V) => {
                        this.handleSuccess(def, data);
                    }).error((msg, code) => {
                        this.handleError(def, msg, code);
                    });
            }, () => {
                def.reject("problem building config");
            });

            return def.promise;
        };
        postEmpty<V extends ViewModels.BaseViewModel>(url: string, opts?: ng.IRequestShortcutConfig): ng.IPromise<V> {

            let def = this.$q.defer<V>();

            this.buildConfig(url, "POST", opts).then((config) => {
                this.$http(config)
                    .success((data: V) => {
                        this.handleSuccess(def, data);
                    }).error((msg, code) => {
                        this.handleError(def, msg, code);
                    });
            }, () => {
                def.reject("problem building config");
            });

            return def.promise;
        };
    }

    angular.module("core")
        .factory("ajaxer", ["$http", "$q", "hubs", ($http: ng.IHttpService, $q: ng.IQService, hubs: Hubs) => {
            return new Ajaxer($http, $q, hubs);
        }]);

}