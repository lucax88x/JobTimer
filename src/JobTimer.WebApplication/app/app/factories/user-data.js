/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../generated/webapi.d.ts" />
/// <reference path="../../generated/constants.ts" />
var App;
(function (App) {
    var UserData = (function () {
        function UserData() {
        }
        return UserData;
    })();
    App.UserData = UserData;
    var UserDataFactory = (function () {
        function UserDataFactory($q, ajaxer) {
            this.$q = $q;
            this.ajaxer = ajaxer;
        }
        UserDataFactory.prototype.updateUserData = function (d) {
            this.userData.hasEstimatedExitTime = d.HasEstimatedExitTime;
            this.userData.estimatedExitTime = d.EstimatedExitTime;
            this.userData.timeOffset = d.TimeOffset;
            this.userData.timeOffsetType = d.TimeOffsetType;
        };
        UserDataFactory.prototype.Get = function () {
            var _this = this;
            if (angular.isUndefined(this.userData)) {
                // already requesting userdata
                if (!angular.isUndefined(this.def)) {
                    return this.def.promise;
                }
                this.def = this.$q.defer();
                this.ajaxer.get("/api/master/getuserdata").then(function (d) {
                    _this.userData = new UserData();
                    _this.userData.userName = d.UserName;
                    _this.userData.email = d.Email;
                    _this.updateUserData(d);
                    _this.def.resolve(_this.userData);
                }, function (d) {
                    _this.def.reject(d);
                });
            }
            else {
                this.def.resolve(this.userData);
            }
            return this.def.promise;
        };
        UserDataFactory.prototype.Update = function () {
            var _this = this;
            if (angular.isUndefined(this.userData)) {
                return this.Get();
            }
            else {
                var def = this.$q.defer();
                this.ajaxer.get("/api/master/updateuserdata").then(function (d) {
                    _this.updateUserData(d);
                    def.resolve(_this.userData);
                }, function (d) {
                    def.reject(d);
                });
                return def.promise;
            }
        };
        return UserDataFactory;
    })();
    App.UserDataFactory = UserDataFactory;
    angular.module("app")
        .factory("userDataFactory", ["$q", "ajaxer", function ($q, ajaxer) {
            return new UserDataFactory($q, ajaxer);
        }]);
})(App || (App = {}));
