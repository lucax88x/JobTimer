/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../generated/webapi.d.ts" />
/// <reference path="../../generated/constants.ts" />
/// <reference path="../../../typings/jstz/jstz.d.ts" />
var Core;
(function (Core) {
    var Ajaxer = (function () {
        function Ajaxer($http, $q, hubs) {
            this.$http = $http;
            this.$q = $q;
            this.hubs = hubs;
        }
        Ajaxer.prototype.handleError = function (def, msg, code) {
            if (code === void 0) { code = 200; }
            var message = "Error!";
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
        ;
        Ajaxer.prototype.handleSuccess = function (def, data) {
            if (data.Result === true) {
                def.resolve(data);
            }
            else {
                this.handleError(def, data);
            }
        };
        Ajaxer.prototype.buildConfig = function (url, method, opts) {
            var _this = this;
            var def = this.$q.defer();
            this.hubs.Connected().then(function () {
                if (url[0] !== "/") {
                    url = "/" + url;
                }
                var config = {
                    url: url,
                    method: method,
                    headers: {}
                };
                config.headers[Constants.HttpHeaders.Request.SignalRConnectionId] = _this.hubs.Id;
                config.headers[Constants.HttpHeaders.Request.CurrentTimeZone] = jstz.determine().name();
                if (!angular.isUndefined(opts)) {
                    angular.extend(config, opts);
                }
                def.resolve(config);
            }, function () {
                def.reject();
            });
            return def.promise;
        };
        Ajaxer.prototype.get = function (url, opts) {
            var _this = this;
            var def = this.$q.defer();
            this.buildConfig(url, "GET", opts).then(function (config) {
                _this.$http(config)
                    .success(function (data) {
                    _this.handleSuccess(def, data);
                }).error(function (msg, code) {
                    _this.handleError(def, msg, code);
                });
            }, function () {
                def.reject("problem building config");
            });
            return def.promise;
        };
        ;
        Ajaxer.prototype.post = function (url, d, opts) {
            var _this = this;
            var def = this.$q.defer();
            this.buildConfig(url, "POST", opts).then(function (config) {
                config.data = d;
                _this.$http(config)
                    .success(function (data) {
                    _this.handleSuccess(def, data);
                }).error(function (msg, code) {
                    _this.handleError(def, msg, code);
                });
            }, function () {
                def.reject("problem building config");
            });
            return def.promise;
        };
        ;
        Ajaxer.prototype.postEmpty = function (url, opts) {
            var _this = this;
            var def = this.$q.defer();
            this.buildConfig(url, "POST", opts).then(function (config) {
                _this.$http(config)
                    .success(function (data) {
                    _this.handleSuccess(def, data);
                }).error(function (msg, code) {
                    _this.handleError(def, msg, code);
                });
            }, function () {
                def.reject("problem building config");
            });
            return def.promise;
        };
        ;
        return Ajaxer;
    })();
    Core.Ajaxer = Ajaxer;
    angular.module("core")
        .factory("ajaxer", ["$http", "$q", "hubs", function ($http, $q, hubs) {
            return new Ajaxer($http, $q, hubs);
        }]);
})(Core || (Core = {}));
