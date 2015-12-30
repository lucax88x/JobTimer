/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />
var Core;
(function (Core) {
    var Hubs = (function () {
        function Hubs($q, $timeout) {
            this.$q = $q;
            this.$timeout = $timeout;
            this.callbacks = [];
            this.connected = false;
            this.hubs = {};
            this.connection = $.connection;
        }
        Hubs.prototype.Connect = function () {
            var _this = this;
            var hub = this.connection.hub;
            var notificationHub = this.Get("notificationHub");
            this.connection.hub.disconnected(function () {
                _this.$timeout(function () {
                    console.log("trying to reconnect..");
                    hub.start();
                }, 10000);
            });
            // this is for getting the notification registered
            notificationHub.client.starter = function () {
                console.log("starter");
            };
            console.log("Connecting..");
            hub.start()
                .done(function () {
                console.log('Now connected, connection ID=' + hub.id);
                // console.log("Transport = " + hub.transport.name);
                // notificationHub.server.starter();
                _this.connected = true;
                for (var i = 0; i < _this.callbacks.length; i++) {
                    var q = _this.callbacks[i];
                    q.resolve();
                }
            })
                .fail(function () {
                console.log('Could not Connect!');
            });
            hub.error(function (error) {
                console.log('SignalR error: ' + error);
            });
        };
        Hubs.prototype.Get = function (hub) {
            if (angular.isUndefined(this.hubs[hub])) {
                this.hubs[hub] = this.connection[hub];
            }
            return this.hubs[hub];
        };
        Hubs.prototype.Connected = function () {
            var q = this.$q.defer();
            if (!this.connected) {
                this.callbacks.push(q);
            }
            else {
                q.resolve();
            }
            return q.promise;
        };
        Object.defineProperty(Hubs.prototype, "Id", {
            get: function () {
                return $.connection.hub.id;
            },
            enumerable: true,
            configurable: true
        });
        return Hubs;
    })();
    Core.Hubs = Hubs;
    angular.module('core')
        .factory('hubs', ["$q", "$timeout", function ($q, $timeout) {
            return new Hubs($q, $timeout);
        }]);
})(Core || (Core = {}));
