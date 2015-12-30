/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/signalr/signalr.d.ts" />
/// <reference path="../../../typings/underscore/underscore.d.ts" />
/// <reference path="../../../typings/highcharts/highcharts.d.ts" />
var JobTimer;
(function (JobTimer) {
    var Charts;
    (function (Charts) {
        angular.module("charts")
            .controller("charts", [function () {
            }]);
    })(Charts = JobTimer.Charts || (JobTimer.Charts = {}));
})(JobTimer || (JobTimer = {}));
