namespace Core {
    export class Guid {
        Generate() {
            return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(c) {
                let r = Math.random() * 16 | 0, v = c == "x" ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }
    }

    angular.module("core")
        .factory("guid", [() => {
            return new Guid();
        }]);
}