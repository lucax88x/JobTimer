var Core;
(function (Core) {
    var Guid = (function () {
        function Guid() {
        }
        Guid.prototype.Generate = function () {
            return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == "x" ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };
        return Guid;
    })();
    Core.Guid = Guid;
    angular.module("core")
        .factory("guid", [function () {
            return new Guid();
        }]);
})(Core || (Core = {}));
