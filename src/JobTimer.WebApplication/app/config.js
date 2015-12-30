var JobTimer;
(function (JobTimer) {
    // find an implementation of dictionary on typescript with generic
    var Controllers = (function () {
        function Controllers() {
            this.dict = {};
        }
        Controllers.prototype.Add = function (property, ctrl) {
            if (!this.dict[property]) {
                this.dict[property] = ctrl;
                return true;
            }
            return false;
        };
        Controllers.prototype.Get = function (property) {
            if (this.dict[property]) {
                return this.dict[property];
            }
            return null;
        };
        return Controllers;
    })();
    JobTimer.Controllers = Controllers;
    var Config = (function () {
        function Config() {
            this.Modules = ['app', 'core'];
            this.Packages = {
                'registry-core': [
                    'form-core', 'grid-core'
                ],
                'registry': [
                    'form', 'grid'
                ]
            };
            this.Controllers = new Controllers();
            this.Controllers.Add("home", {
                Dependencies: [
                    'sortable', 'home'
                ]
            });
            this.Controllers.Add("admin-user", {
                Dependencies: [
                    'registry-core', 'registry', "admin-user"
                ]
            });
            this.Controllers.Add("timer", {
                Modules: ["timer"],
                Dependencies: [
                    'timer-core', 'timer'
                ]
            });
            this.Controllers.Add("charts", {
                Modules: ["charts"],
                Dependencies: [
                    'chart-core', 'charts'
                ]
            });
        }
        Object.defineProperty(Config, "Instance", {
            get: function () {
                return this._instance;
            },
            enumerable: true,
            configurable: true
        });
        Config._instance = new Config();
        return Config;
    })();
    JobTimer.Config = Config;
    new Config();
})(JobTimer || (JobTimer = {}));
