(function ($) {
    /***
     * A sample AJAX data store implementation.
     * Right now, it's hooked up to load Hackernews stories, but can
     * easily be extended to support any JSONP-compatible backend that accepts paging parameters.
     */
    function RemoteModel(url, filters) {
        // private
        var PAGESIZE = 50;
        var data = { length: 0 };
        var sortcol = null;
        var sortdir = 1;
        var h_request = null;
        var req = null; // ajax request

        // events
        var onDataLoading = new Slick.Event();
        var onDataLoaded = new Slick.Event();
        var onDataLoadError = new Slick.Event();


        function init() {
        }


        function isDataLoaded(from, to) {
            for (var i = from; i <= to; i++) {
                if (data[i] == undefined || data[i] == null) {
                    return false;
                }
            }

            return true;
        }


        function clear() {
            for (var key in data) {
                delete data[key];
            }
            data.length = 0;
        }


        function ensureData(from, to) {
            if (req) {
                req.abort();
                for (var i = req.fromPage; i <= req.toPage; i++)
                    data[i * PAGESIZE] = undefined;
            }

            if (from < 0) {
                from = 0;
            }

            if (data.length > 0) {
                to = Math.min(to, data.length - 1);
            }

            var fromPage = Math.floor(from / PAGESIZE);
            var toPage = Math.floor(to / PAGESIZE);

            while (data[fromPage * PAGESIZE] !== undefined && fromPage < toPage)
                fromPage++;

            while (data[toPage * PAGESIZE] !== undefined && fromPage < toPage)
                toPage--;

            if (fromPage > toPage || ((fromPage == toPage) && data[fromPage * PAGESIZE] !== undefined)) {
                // TODO:  look-ahead
                onDataLoaded.notify({ from: from, to: to });
                return;
            }

            //var url = "http://api.thriftdb.com/api.hnsearch.com/items/_search?filter[fields][type][]=submission&q=" + searchstr + "&start=" + (fromPage * PAGESIZE) + "&limit=" + (((toPage - fromPage) * PAGESIZE) + PAGESIZE);
            var postData = {};
            if (filters && typeof (filters) == "function") {
                postData.search = filters();
            }
            
            postData.start = fromPage * PAGESIZE;
            postData.limit = ((toPage - fromPage) * PAGESIZE) + PAGESIZE;

            if (sortcol != null) {
                //url += ("&sortby=" + sortcol + ((sortdir > 0) ? "+asc" : "+desc"));
                postData.sortby = sortcol + ((sortdir > 0) ? "+asc" : "+desc");
            }

            if (h_request != null) {
                clearTimeout(h_request);
            }

            h_request = setTimeout(function () {
                for (var i = fromPage; i <= toPage; i++)
                    data[i * PAGESIZE] = null; // null indicates a 'requested but not available yet'

                onDataLoading.notify({ from: from, to: to });

                req = $.ajax({
                    url: url,
                    dataType: 'json',
                    contentType: 'application/json',
                    method: 'post',
                    data: JSON.stringify(postData),
                    cache: true,
                    success: onSuccess,
                    error: function () {
                        onError(fromPage, toPage)
                    }
                });
                req.fromPage = fromPage;
                req.toPage = toPage;
            }, 50);
        }


        function onError(fromPage, toPage) {
            onDataLoadError.notify({ from: fromPage, to: toPage });
        }

        function onSuccess(resp) {
            resp = resp.data;
            var from = resp.request.start, to = from + resp.results.length;
            data.length = Math.min(parseInt(resp.hits), 1000); // limitation of the API

            for (var i = 0; i < resp.results.length; i++) {
                var item = resp.results[i];

                // Old IE versions can't parse ISO dates, so change to universally-supported format.
                //item.create_ts = item.create_ts.replace(/^(\d+)-(\d+)-(\d+)T(\d+:\d+:\d+)Z$/, "$2/$3/$1 $4 UTC"); 
                //item.create_ts = new Date(item.create_ts);

                data[from + i] = item;
                data[from + i].id = from + i;
            }

            req = null;

            var items = [];
            var i = 0;
            for (var key in data) {
                if (key != "length") {
                    if (data[key] !== undefined && data[key] !== null) {
                        items[i] = data[key];
                        i++;
                    }
                    else {
                        delete data[key];
                    }
                }
            }

            onDataLoaded.notify({ from: from, to: to, data: items });
        }


        function reloadData(from, to) {
            for (var i = from; i <= to; i++)
                delete data[i];

            ensureData(from, to);
        }


        function setSort(column, dir) {
            sortcol = column;
            sortdir = dir;
            clear();
        }


        init();

        return {
            // properties
            "data": data,

            // methods
            "clear": clear,
            "isDataLoaded": isDataLoaded,
            "ensureData": ensureData,
            "reloadData": reloadData,
            "setSort": setSort,

            // events
            "onDataLoading": onDataLoading,
            "onDataLoaded": onDataLoaded,
            "onDataLoadError": onDataLoadError
        };
    }

    // Slick.Serie.RemoteModel
    $.extend(true, window, { Slick: { Data: { RemoteModel: RemoteModel } } });
})(jQuery);
