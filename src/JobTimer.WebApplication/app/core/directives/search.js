/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/typeahead/typeahead.d.ts" />
/// <reference path="../../../typings/handlebars/handlebars.d.ts" />
var Core;
(function (Core) {
    angular.module('core')
        .directive('search', ["$state", function ($state) {
            return {
                restrict: "A",
                compile: function (elm, attrs) {
                    return function (scope, elm, attrs) {
                        function generateEngine(url, arrayProperty) {
                            var engine = new Bloodhound({
                                limit: 10,
                                datumTokenizer: Bloodhound.tokenizers.obj.whitespace("Text"),
                                queryTokenizer: Bloodhound.tokenizers.whitespace,
                                remote: {
                                    url: url,
                                    ajax: {
                                        type: "POST",
                                        data: {
                                            "Query": function () { return elm.val(); },
                                            "Limit": 5
                                        },
                                        beforeSend: function () {
                                            elm.siblings(".tt-hint").css('background-image', 'url("/content/images/loading/loading.svg")');
                                        },
                                        complete: function () {
                                            elm.siblings(".tt-hint").css('background-image', '');
                                        }
                                    },
                                    replace: function (url, uriEncodedQuery) {
                                        return url + "#" + uriEncodedQuery;
                                    },
                                    filter: function (data) {
                                        return data[arrayProperty];
                                    }
                                }
                            });
                            engine.initialize();
                            return engine;
                        }
                        var pages = generateEngine("/api/data/searchPages", "Pages");
                        var instance = elm.typeahead({
                            hint: true
                        }, {
                            name: 'pages',
                            displayKey: "Text",
                            source: pages.ttAdapter(),
                            templates: {
                                header: '<h3><i class="fa fa-file-text-o"></i> Pagine</h3>',
                                suggestion: Handlebars.compile('<a href="{{Url}}">{{Text}}</a>')
                            }
                        });
                        // elm.on('typeahead:selected', function(event, datum) {
                        //         handleSelected(datum);
                        // });
                        //elm.on('typeahead:cursorchanged', function (e, datum) {
                        //    //handleSelected(datum);
                        //});
                    };
                }
            };
        }]);
})(Core || (Core = {}));
