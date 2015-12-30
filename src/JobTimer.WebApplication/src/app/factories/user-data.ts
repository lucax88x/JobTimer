/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../generated/webapi.d.ts" />
/// <reference path="../../generated/constants.ts" />

namespace App {
    export class UserData {
        userName: string;
        email: string;
        hasEstimatedExitTime: boolean;
        estimatedExitTime: Date;

        timeOffset: string;
        timeOffsetType: ViewModels.Master.OffsetTypes;
    }

    export class UserDataFactory {
        constructor(private $q: ng.IQService, private ajaxer: Core.Ajaxer) {

        }

        userData: UserData;

        private updateUserData(d: ViewModels.Master.UserDataViewModel) {
            this.userData.hasEstimatedExitTime = d.HasEstimatedExitTime;
            this.userData.estimatedExitTime = d.EstimatedExitTime;
            this.userData.timeOffset = d.TimeOffset;
            this.userData.timeOffsetType = d.TimeOffsetType;
        }

        def: ng.IDeferred<UserData>;
        Get(): ng.IPromise<UserData> {
            if (angular.isUndefined(this.userData)) {

                // already requesting userdata
                if (!angular.isUndefined(this.def)) {
                    return this.def.promise;
                }

                this.def = this.$q.defer<UserData>();

                this.ajaxer.get<ViewModels.Master.GetUserDataViewModel>("/api/master/getuserdata").then((d) => {
                    this.userData = new UserData();

                    this.userData.userName = d.UserName;
                    this.userData.email = d.Email;

                    this.updateUserData(d);

                    this.def.resolve(this.userData);
                }, (d) => {
                    this.def.reject(d);
                });
            }
            else {
                this.def.resolve(this.userData);
            }
            return this.def.promise;
        }

        Update(): ng.IPromise<UserData> {
            if (angular.isUndefined(this.userData)) {
                return this.Get();
            }
            else {
                let def = this.$q.defer<UserData>();

                this.ajaxer.get<ViewModels.Master.UpdateUserDataViewModel>("/api/master/updateuserdata").then((d) => {
                    this.updateUserData(d);
                    def.resolve(this.userData);
                }, (d) => {
                    def.reject(d);
                });

                return def.promise;
            }
        }
    }

    angular.module("app")
        .factory("userDataFactory", ["$q", "ajaxer", ($q: ng.IQService, ajaxer: Core.Ajaxer) => {
            return new UserDataFactory($q, ajaxer);
        }]);

}