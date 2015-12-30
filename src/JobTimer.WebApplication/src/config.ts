
namespace JobTimer {
    export interface IController {
        // new modules to register
        Modules?: Array<string>,

        // packages needed and defined by packages
        Dependencies: Array<string>,
    }

    // find an implementation of dictionary on typescript with generic
    export class Controllers {
        private dict: Object;
        constructor() {
            this.dict = {}
        }

        public Add(property: string, ctrl: IController): boolean {
            if (!this.dict[property]) {
                this.dict[property] = ctrl;
                return true;
            }
            return false;
        }
        public Get(property: string): IController {
            if (this.dict[property]) {
                return this.dict[property];
            }
            return null;
        }
    }
    export class Config {
        Modules: Array<string> = ['app', 'core'];
        Packages: Object = {
            'registry-core':
            [
                'form-core', 'grid-core'
            ],
            'registry':
            [
                'form', 'grid'
            ]
        }
        Controllers: Controllers;

        constructor() {

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

        private static _instance: Config = new Config();
        static get Instance(): Config {
            return this._instance;
        }
    }

    new Config();
}