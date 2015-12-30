namespace Core {
    angular.module('core')
        .factory('cache', [function() {

            var str = {
                Cached: {},
                Data: {},
                IsCached: function(cacheKey) {
                    var result = false;
                    if (this.Cached.hasOwnProperty(cacheKey)) {
                        result = this.Cached[cacheKey] === true;
                    }
                    return result;
                },
                Cache: function() {

                    if (arguments.length === 0) {
                        return null;
                    }

                    if (arguments.length === 1) {
                        if (this.Data.hasOwnProperty(arguments[0])) {
                            return this.Data[arguments[0]];
                        } else {
                            return null;
                        }
                    } else {
                        this.Data[arguments[0]] = arguments[1];
                        this.Cached[arguments[0]] = true;
                    }

                }
            }

            return str;
        }]);
}