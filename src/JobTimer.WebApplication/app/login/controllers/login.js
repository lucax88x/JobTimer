var Login;
(function (Login) {
    var LoginData = (function () {
        function LoginData() {
        }
        return LoginData;
    })();
    var RegisterData = (function () {
        function RegisterData() {
        }
        return RegisterData;
    })();
    var ExternalNecessaryData = (function () {
        function ExternalNecessaryData() {
        }
        return ExternalNecessaryData;
    })();
    angular.module("app")
        .controller("login", ["$scope", "$window", "hubs", "ajaxer", "notifier", function ($scope, $window, hubs, ajaxer, notifier) {
            $scope.login = new LoginData();
            $scope.register = new RegisterData();
            $scope.necessaryData = new ExternalNecessaryData();
            hubs.Connect();
            $scope.doLogin = function () {
                $scope.locker = true;
                $scope.disable = true;
                ajaxer.post("/api/account/login", { Email: $scope.login.email, Password: $scope.login.password })
                    .then(function (data) {
                    if (data.Result === true) {
                        notifier.Success("Signed In!");
                        $window.location.href = "/";
                    }
                    else {
                        notifier.Error(data.Message);
                    }
                }, function (data) {
                    notifier.Error(data);
                    $scope.disable = false;
                    $scope.locker = false;
                });
            };
            $scope.doRegister = function () {
                $scope.locker = true;
                $scope.disable = true;
                ajaxer.post("/api/account/register", { Email: $scope.register.email, Password: $scope.register.password, ConfirmPassword: $scope.register.confirmPassword })
                    .then(function (data) {
                    if (data.Result === true) {
                        notifier.Success("Registered!");
                        $window.location.href = "/";
                    }
                    else {
                        notifier.Error(data.Message);
                    }
                }, function (data) {
                    $scope.locker = false;
                    $scope.disable = false;
                    notifier.Error(data);
                });
            };
            $scope.authExternalProvider = function (provider) {
                var redirectUri = $window.location.origin + "/authcomplete.html";
                var clientId = "jobTimerDebug";
                var externalProviderUrl = $window.location.origin + "/api/Account/ExternalLogin?provider=" + provider
                    + "&response_type=token&client_id=" + clientId
                    + "&redirect_uri=" + redirectUri;
                window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
            };
            function registerExternal(email, provider, token) {
                var externalData = { Email: email, Provider: provider, ExternalAccessToken: token };
                $scope.locker = true;
                ajaxer.post("/api/account/registerexternal", externalData).then(function (d) {
                    console.log('finished');
                    notifier.Success("Signed In!");
                    $window.location.href = "/";
                }, function (d) {
                    notifier.Error(d);
                    $scope.disable = false;
                    $scope.locker = false;
                });
            }
            $scope.necessaryDataClose = function () {
                $scope.locker = true;
                registerExternal($scope.necessaryData.email, $scope.necessaryData.provider, $scope.necessaryData.access_token);
            };
            window["authCompletedCB"] = function (fragment) {
                $scope.$evalAsync(function () {
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
                        var externalData = { Email: fragment.external_email, Provider: fragment.provider, ExternalAccessToken: fragment.external_access_token };
                        $scope.locker = true;
                        ajaxer.post("/api/account/VerifyExternalLogin", externalData).then(function (d) {
                            notifier.Success("Signed In!");
                            $window.location.href = "/";
                        }, function (d) {
                            notifier.Error(d);
                            $scope.disable = false;
                            $scope.locker = false;
                        });
                    }
                });
            };
        }]);
})(Login || (Login = {}));
