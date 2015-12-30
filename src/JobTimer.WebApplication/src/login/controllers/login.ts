namespace Login {

    class LoginData {
        email: string;
        password: string;
    }

    class RegisterData {
        email: string;
        password: string;
        confirmPassword: string;
    }

    class ExternalNecessaryData {
        email: string;
        access_token: string;
        provider: string;
    }

    interface IExternalLoginFragments {
        external_access_token: string;
        provider: string;
        haslocalaccount: string;
        external_email: string;
    }

    interface ILoginScope extends ng.IScope {

        login: LoginData;
        register: RegisterData;

        necessaryDataModal: boolean;
        necessaryData: ExternalNecessaryData;

        locker: boolean;
        disable: boolean;

        doLogin();
        doRegister();
        authExternalProvider(provider: string);
        necessaryDataClose();
    }

    angular.module("app")
        .controller("login", ["$scope", "$window", "hubs", "ajaxer", "notifier", ($scope: ILoginScope, $window: ng.IWindowService, hubs: Core.Hubs, ajaxer: Core.Ajaxer, notifier: Core.Notifier) => {

            $scope.login = new LoginData();
            $scope.register = new RegisterData();
            $scope.necessaryData = new ExternalNecessaryData();

            hubs.Connect();

            $scope.doLogin = () => {
                $scope.locker = true;
                $scope.disable = true;
                ajaxer.post<BindingModels.Account.LoginBindingModel, ViewModels.Account.LoginViewModel>("/api/account/login",
                    { Email: $scope.login.email, Password: $scope.login.password })
                    .then((data) => {
                        if (data.Result === true) {
                            notifier.Success("Signed In!");
                            $window.location.href = "/";
                        }
                        else {
                            notifier.Error(data.Message);
                        }
                    },
                    (data) => {
                        notifier.Error(data);
                        $scope.disable = false;
                        $scope.locker = false;
                    });
            };

            $scope.doRegister = () => {
                $scope.locker = true;
                $scope.disable = true;
                ajaxer.post<BindingModels.Account.RegisterBindingModel, ViewModels.Account.RegisterViewModel>("/api/account/register",
                    { Email: $scope.register.email, Password: $scope.register.password, ConfirmPassword: $scope.register.confirmPassword })
                    .then(
                    (data) => {
                        if (data.Result === true) {
                            notifier.Success("Registered!");
                            $window.location.href = "/";
                        }
                        else {
                            notifier.Error(data.Message);
                        }
                    },
                    (data) => {
                        $scope.locker = false;
                        $scope.disable = false;
                        notifier.Error(data);
                    });
            };

            $scope.authExternalProvider = (provider: string) => {

                let redirectUri = $window.location.origin + "/authcomplete.html";
                let clientId = "jobTimerDebug";

                let externalProviderUrl = $window.location.origin + "/api/Account/ExternalLogin?provider=" + provider
                    + "&response_type=token&client_id=" + clientId
                    + "&redirect_uri=" + redirectUri;

                window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
            };

            function registerExternal(email: string, provider: string, token: string) {
                let externalData = { Email: email, Provider: provider, ExternalAccessToken: token };
                $scope.locker = true;
                ajaxer.post<BindingModels.Account.RegisterExternalBindingModel, ViewModels.Account.RegisterExternalViewModel>("/api/account/registerexternal", externalData).then((d) => {
                    console.log('finished');
                    notifier.Success("Signed In!");
                    $window.location.href = "/";
                }, (d) => {
                    notifier.Error(d);
                    $scope.disable = false;
                    $scope.locker = false;
                });
            }

            $scope.necessaryDataClose = () => {
                $scope.locker = true;
                registerExternal($scope.necessaryData.email, $scope.necessaryData.provider, $scope.necessaryData.access_token);
            };

            window["authCompletedCB"] = (fragment: IExternalLoginFragments) => {

                $scope.$evalAsync(() => {

                    if (fragment.haslocalaccount === "False") {
                        if (!fragment.external_email) {
                            $scope.locker = false;
                            $scope.necessaryData.access_token = fragment.external_access_token;
                            $scope.necessaryData.provider = fragment.provider;
                            $scope.necessaryDataModal = true;
                        }
                        else {
                            registerExternal(fragment.external_email, fragment.provider, fragment.external_access_token);
                        }
                    }
                    else {
                        let externalData = { Email: fragment.external_email, Provider: fragment.provider, ExternalAccessToken: fragment.external_access_token };
                        $scope.locker = true;
                        ajaxer.post<BindingModels.Account.VerifyExternalLoginBindingModel, ViewModels.Account.VerifyExternalLoginViewModel>("/api/account/VerifyExternalLogin", externalData).then((d) => {
                            notifier.Success("Signed In!");
                            $window.location.href = "/";
                        }, (d) => {
                            notifier.Error(d);
                            $scope.disable = false;
                            $scope.locker = false;
                        });
                    }
                });
            };
        }]);
}