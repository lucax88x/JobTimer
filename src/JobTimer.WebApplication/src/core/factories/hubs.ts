/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />

namespace Core {

    export class Hubs {

        private callbacks: Array<ng.IDeferred<void>> = [];
        private connected: boolean = false;
        private connection: SignalR;
        private hubs: Object = {};

        constructor(private $q: ng.IQService, private $timeout: ng.ITimeoutService) {
            this.connection = $.connection;
        }

        Connect() {

            let hub = this.connection.hub;

            let notificationHub = this.Get<JobTimer.Hubs.INotificationHubProxy>("notificationHub");

            this.connection.hub.disconnected(() => {
                this.$timeout(() => {
                    console.log("trying to reconnect..");
                    hub.start();
                }, 10000);
            });

            // this is for getting the notification registered
            notificationHub.client.starter = () => {
                console.log("starter");
            };

            console.log("Connecting..");

            hub.start()
                .done(() => {
                    console.log('Now connected, connection ID=' + hub.id);
                    // console.log("Transport = " + hub.transport.name);

                    // notificationHub.server.starter();

                    this.connected = true;
                    for (var i = 0; i < this.callbacks.length; i++) {
                        var q: ng.IDeferred<void> = this.callbacks[i];
                        q.resolve();
                    }
                })
                .fail(() => {
                    console.log('Could not Connect!');
                });

            hub.error(function(error) {
                console.log('SignalR error: ' + error)
            });
        }

        Get<T>(hub: string): T {
            if (angular.isUndefined(this.hubs[hub])) {
                this.hubs[hub] = this.connection[hub];
            }

            return this.hubs[hub];
        }

        Connected(): ng.IPromise<void> {
            var q: ng.IDeferred<void> = this.$q.defer<void>();

            if (!this.connected) {
                this.callbacks.push(q);
            }
            else {
                q.resolve();
            }

            return q.promise;
        }



        public get Id(): string {
            return $.connection.hub.id;
        }
    }

    angular.module('core')
        .factory('hubs', ["$q", "$timeout", ($q: ng.IQService, $timeout: ng.ITimeoutService) => {
            return new Hubs($q, $timeout);
        }]);
}