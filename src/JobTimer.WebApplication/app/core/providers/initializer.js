/// <reference path="../../../typings/basket/basket.d.ts" />
var Core;
(function (Core) {
    angular.module("core")
        .provider("initializer", ["$controllerProvider", "$compileProvider", "$provide", function ($controllerProvider, $compileProvider, $provide) {
            var debug = false;
            var cache = false;
            var providers = {
                $controllerProvider: $controllerProvider,
                $compileProvider: $compileProvider,
                $provide: $provide
            };
            var queueLens = {};
            var registered = {};
            var downloaded = {};
            function initModuleLazy(moduleName) {
                try {
                    if (!queueLens[moduleName]) {
                        queueLens[moduleName] = angular.module(moduleName)._invokeQueue.length;
                        registered[moduleName] = {};
                    }
                }
                catch (error) {
                    if (debug)
                        console.log("initialize error " + error);
                }
            }
            function registerModuleLazy(moduleName) {
                if (debug)
                    console.log("trying to register " + moduleName);
                // new module from downloaded, need to initialize
                if (!queueLens[moduleName]) {
                    queueLens[moduleName] = 0;
                    registered[moduleName] = {};
                }
                var queue = angular.module(moduleName)._invokeQueue;
                for (var i = queueLens[moduleName]; i < queue.length; i++) {
                    var call = queue[i];
                    if (debug)
                        console.log("registering " + call[1] + ": " + call[2][0] + " in " + moduleName);
                    if (!registered[moduleName][call[1]])
                        registered[moduleName][call[1]] = {};
                    if (!registered[moduleName][call[1]][call[2][0]]) {
                        registered[moduleName][call[1]][call[2][0]] = true;
                        var provider = providers[call[0]];
                        if (provider) {
                            provider[call[1]].apply(provider, call[2]);
                            if (debug)
                                console.log("registered!");
                        }
                        else {
                            if (debug)
                                console.log("no provider defined!");
                        }
                    }
                    else {
                        if (debug)
                            console.log("already defined!");
                    }
                }
                queueLens[moduleName] = queue.length;
            }
            function getScriptUrl(url) {
                if (cache === false) {
                    return url + "?bust=" + (new Date()).getTime();
                }
                else {
                    return url;
                }
            }
            function require(url) {
                if (angular.isArray(url)) {
                    return basket.require.apply(basket, url);
                }
                else {
                    return basket.require(url);
                }
            }
            function serial($q, urls) {
                var deferred = $q.defer();
                function go(urls) {
                    if (urls[0]) {
                        require(urls[0]).then(function () {
                            go(urls.slice(1));
                        });
                    }
                    else {
                        deferred.resolve();
                    }
                }
                go(urls);
                return deferred.promise;
            }
            var downloadedUrls = {};
            function canDownload(url) {
                var result = false;
                if (!downloadedUrls.hasOwnProperty(url)) {
                    downloadedUrls[url] = true;
                    result = true;
                }
                return result;
            }
            function recursiveGetUrls(urls, pkgName) {
                var pkgUrls = [];
                var pkg = JobTimer.Config.Instance.Packages[pkgName];
                var url;
                if (pkg) {
                    for (var y = 0; y < pkg.length; y++) {
                        url = pkg[y];
                        if (url[0] === "/" || url[0] === "~") {
                            if (canDownload(url)) {
                                pkgUrls.push({ url: getScriptUrl(url), skipCache: !cache });
                            }
                        }
                        else {
                            recursiveGetUrls(urls, url);
                        }
                    }
                    if (pkgUrls.length !== 0) {
                        urls.push(pkgUrls);
                    }
                }
                else {
                    if (canDownload(pkgName)) {
                        urls.push({ url: getScriptUrl(pkgName), skipCache: !cache });
                    }
                }
            }
            function getScripts($q, ctrl) {
                var deferred = $q.defer();
                if (!downloaded[ctrl]) {
                    downloaded[ctrl] = true;
                    var controller = JobTimer.Config.Instance.Controllers.Get(ctrl);
                    if (controller) {
                        var urls = [];
                        for (var i = 0; i < controller.Dependencies.length; i++) {
                            recursiveGetUrls(urls, controller.Dependencies[i]);
                        }
                        if (urls.length > 0) {
                            serial($q, urls).then(function () {
                                deferred.resolve();
                            });
                        }
                        else {
                            deferred.resolve();
                        }
                    }
                    else {
                        deferred.resolve();
                    }
                }
                else {
                    if (debug)
                        console.log("already downloaded!");
                    deferred.resolve();
                }
                return deferred.promise;
            }
            var bundlesAlreadyDownloaded = false;
            function tryGetBundles($http, $q) {
                var deferred = $q.defer();
                if (!bundlesAlreadyDownloaded) {
                    $http.get("/api/master/getbundles").then(function (response) {
                        bundlesAlreadyDownloaded = true;
                        if (response.data && response.data.Bundles && angular.isArray(response.data.Bundles)) {
                            for (var j = 0; j < response.data.Bundles.length; j++) {
                                var bundle = response.data.Bundles[j];
                                if (!angular.isUndefined(JobTimer.Config.Instance.Packages[bundle.Bundle])) {
                                    if (debug)
                                        console.log("bundle already in package");
                                    continue;
                                }
                                JobTimer.Config.Instance.Packages[bundle.Bundle] = bundle.Scripts;
                            }
                            deferred.resolve();
                        }
                        else {
                            deferred.reject();
                        }
                    }, function () {
                        deferred.reject();
                    });
                }
                else {
                    deferred.resolve();
                }
                return deferred.promise;
            }
            function getScriptsAndPage($http, $templateCache, $q, ctrl, url) {
                return tryGetBundles($http, $q).then(function () {
                    return getScripts($q, ctrl).then(function () {
                        if (debug)
                            console.log("downloaded " + JSON.stringify(downloadedUrls));
                        for (var i = 0; i < JobTimer.Config.Instance.Modules.length; i++) {
                            registerModuleLazy(JobTimer.Config.Instance.Modules[i]);
                        }
                        var controller = JobTimer.Config.Instance.Controllers.Get(ctrl);
                        if (controller && controller.Modules) {
                            for (var i = 0; i < controller.Modules.length; i++) {
                                registerModuleLazy(controller.Modules[i]);
                            }
                        }
                        return $http.get(url, { cache: $templateCache }).then(function (response) {
                            return response.data;
                        });
                    });
                });
            }
            return {
                InitializeModules: function () {
                    for (var i = 0; i < JobTimer.Config.Instance.Modules.length; i++) {
                        if (debug)
                            console.log("trying to initialize " + JobTimer.Config.Instance.Modules[i]);
                        initModuleLazy(JobTimer.Config.Instance.Modules[i]);
                    }
                },
                Get: getScriptsAndPage,
                $get: function (url, opts) {
                    return {};
                }
            };
        }]);
})(Core || (Core = {}));
