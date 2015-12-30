namespace Core {
        angular.module('core')
                .directive('search', [function() {
                        return {
                                restrict: "A",
                                compile: function(elm, attrs) {
                                        return function(scope, elm, attrs) {
                                                var name = attrs.typeahead;
                                                var property = attrs.typeaheadProperty;
                                                var array = attrs.typeaheadArray;
                                                var url = attrs.typeaheadUrl;

                                                var remote = new Bloodhound({
                                                        limit: 10,
                                                        datumTokenizer: Bloodhound.tokenizers.obj.whitespace(property),
                                                        queryTokenizer: Bloodhound.tokenizers.whitespace,
                                                        remote: {
                                                                url: url,
                                                                ajax: {
                                                                        type: "POST",
                                                                        data: {
                                                                                "Query": function() { return elm.val() },
                                                                                "Limit": 10
                                                                        },
                                                                        beforeSend: function() {
                                                                                elm.siblings(".tt-hint").css('background-image', 'url("/content/images/loading/loading.svg")');
                                                                        },
                                                                        complete: function() {
                                                                                elm.siblings(".tt-hint").css('background-image', '');
                                                                        }
                                                                },
                                                                replace: function(url, uriEncodedQuery) {
                                                                        return url + "#" + uriEncodedQuery;
                                                                },
                                                                filter: function(data) {
                                                                        return data[array];
                                                                }
                                                        }
                                                });

                                                remote.initialize();

                                                elm.typeahead({
                                                        hint: true
                                                }, {
                                                                name: 'items',
                                                                displayKey: property,
                                                                source: remote.ttAdapter(),
                                                                templates: {
                                                                        suggestion: Handlebars.compile('<p>{{' + property + '}}</p>')
                                                                }
                                                        });

                                                elm.on('typeahead:selected', function(event, datum) {
                                                        console.log(datum);
                                                        scope.$evalAsync(attrs.typeaheadSelected, { datum: datum });
                                                });
                                                //elm.on('typeahead:cursorchanged', function (e, datum) {
                                                //    scope.$evalAsync(attrs.typeaheadSelected, { datum: datum });
                                                //});

                                                scope.$watch(attrs.typeaheadModel, function(value) {
                                                        elm.typeahead('val', value);
                                                });
                                        };
                                }
                        };
                }]);
}
