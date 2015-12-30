/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../typings/slickgrid/slickgrid.d.ts" />
/// <reference path="../../../typings/slickgrid/slickgrid.data.remotemodel.d.ts" />
/// <reference path="../../../typings/slickgrid/slickgrid.autotooltips.d.ts" />
/// <reference path="../../../typings/slickgrid/SlickGrid.RowSelectionModel.d.ts" />
var Core;
(function (Core) {
    angular.module('core')
        .directive('slickGrid', ["$window", "$timeout", "$stateParams", "ajaxer", "locker", "notifier", function ($window, $timeout, $stateParams, ajaxer, locker, notifier) {
            return {
                restrict: "A",
                link: function (scope, element, attrs) {
                    var grid;
                    var loader;
                    var dataView;
                    var columns = scope.$eval(attrs.columns);
                    var columnFilters = {};
                    var filterHasChanged = false;
                    var options = {
                        showHeaderRow: true,
                        headerRowHeight: 45,
                        enableCellNavigation: true,
                        enableColumnReorder: false,
                        rowHeight: 45,
                        editable: true,
                        multiSelect: false,
                        explicitInitialization: true
                    };
                    function filter() {
                        return columnFilters;
                    }
                    scope.disableModify = true;
                    var $el = element.find(".grid");
                    $el.height(screen.height - 300);
                    var gridjs = $el.slickgrid({
                        columns: columns,
                        slickGridOptions: options,
                        handleCreate: function () {
                            loader = new Slick.Data.RemoteModel(attrs.getDataUrl, filter);
                            var tooltip = new Slick.AutoTooltips({ enableForHeaderCells: true });
                            var o = this.wrapperOptions;
                            dataView = new Slick.Data.DataView();
                            grid = new Slick.Grid(this.element, dataView, o.columns, o.slickGridOptions);
                            //grid = new Slick.Grid(this.element, loader.data, o.columns, o.slickGridOptions);
                            //happens that width when initialized is not correct, so need to resize canvas
                            var previousWidth = $el.width();
                            setTimeout(function () {
                                if (previousWidth !== $el.width()) {
                                    previousWidth = $el.width();
                                    grid.resizeCanvas();
                                }
                            }, 300);
                            grid.registerPlugin(tooltip);
                            var selectionModel = new Slick.RowSelectionModel();
                            grid.setSelectionModel(selectionModel);
                            selectionModel.onSelectedRangesChanged.subscribe(function (range) {
                                scope.$evalAsync(function () {
                                    scope.disableModify = grid.getSelectedRows().length === 0;
                                });
                            });
                            grid.onDblClick.subscribe(function (e, args) {
                                scope.$evalAsync(function () {
                                    scope.modify();
                                });
                            });
                            //grid.onClick.subscribe(function (e, args) {
                            //    var $target = $(e.target);
                            //    if ($target.hasClass("glyphicons")) {
                            //        $target = $target.parent();
                            //    }
                            //    if ($target.hasClass("btn")) {
                            //        var item = dataView.getItem(args.row);
                            //        var site = item.SiteName;
                            //        var url = item.Url;
                            //        var $row = $target.parents(".slick-row");
                            //        url = url.replace("://", "");
                            //        url = url.substr(url.indexOf("/"))
                            //        raise("OnRowClick", args, $target, $row, item, site, url);
                            //    }
                            //    e.stopImmediatePropagation();
                            //});
                            //dataView.onRowsChanged.subscribe(function (e, args) {
                            //    grid.invalidateRows(args.rows);
                            //    grid.render();
                            //});
                            //// wire up model events to drive the grid
                            //dataView.onRowCountChanged.subscribe(function (e, args) {
                            //    grid.updateRowCount();
                            //    grid.render();
                            //});
                            grid.onViewportChanged.subscribe(function (e, args) {
                                var vp = grid.getViewport();
                                loader.ensureData(vp.top, vp.bottom);
                            });
                            grid.onSort.subscribe(function (e, args) {
                                loader.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
                                var vp = grid.getViewport();
                                loader.ensureData(vp.top, vp.bottom);
                            });
                            grid.onHeaderRowCellRendered.subscribe(function (e, args) {
                                if (args.column && args.column.field) {
                                    var col = _.find(columns, function (val) { return val.field === args.column.field; });
                                    if (col.filter === true) {
                                        $(args.node).empty();
                                        $("<input class='form-control' type='text'>")
                                            .data("columnId", args.column.id)
                                            .val(columnFilters[args.column.id])
                                            .appendTo(args.node);
                                    }
                                }
                            });
                            var oldTimeout;
                            $(grid.getHeaderRow()).delegate(":input", "change keyup", function (e) {
                                var columnId = $(this).data("columnId");
                                if (columnId != null) {
                                    var val = $.trim($(this).val());
                                    if (val) {
                                        if (columnFilters[columnId] !== val) {
                                            filterHasChanged = true;
                                            columnFilters[columnId] = val;
                                        }
                                    }
                                    else {
                                        if (columnFilters[columnId]) {
                                            filterHasChanged = true;
                                            delete columnFilters[columnId];
                                        }
                                    }
                                    if (filterHasChanged) {
                                        filterHasChanged = false;
                                        if (oldTimeout)
                                            clearTimeout(oldTimeout);
                                        oldTimeout = setTimeout(function () {
                                            scope.refresh();
                                        }, 350);
                                    }
                                }
                            });
                            grid.init();
                            grid.setHeaderRowVisibility(false);
                            loader.onDataLoading.subscribe(function () {
                                locker.Lock(element);
                            });
                            loader.onDataLoadError.subscribe(function () {
                                notifier.Error("Error");
                                locker.Unlock(element);
                            });
                            loader.onDataLoaded.subscribe(function (e, args) {
                                if (args.data) {
                                    dataView.beginUpdate();
                                    //if (attrs.filter) {
                                    //dataView.setFilterArgs(attrs.filterArgs);
                                    //dataView.setFilter(attrs.filter);                                
                                    //}
                                    dataView.setItems(args.data);
                                    dataView.endUpdate();
                                    for (var i = args.from; i <= args.to; i++) {
                                        grid.invalidateRow(i);
                                    }
                                    grid.updateRowCount();
                                    grid.render();
                                }
                                locker.Unlock(element);
                            });
                            // load the first page
                            //grid.onViewportChanged.notify();
                            var vp = grid.getViewport();
                            loader.ensureData(vp.top, vp.bottom);
                        }
                    });
                    scope.refresh = function () {
                        loader.clear();
                        var vp = grid.getViewport();
                        loader.reloadData(vp.top, vp.bottom);
                    };
                    scope.toggleFilters = function () {
                        if (grid.getOptions().showHeaderRow) {
                            grid.setHeaderRowVisibility(false);
                        }
                        else {
                            grid.setHeaderRowVisibility(true);
                        }
                    };
                    //t.UpdateFilter = function () {
                    //    dataView.setFilterArgs(opts.filterArgs);
                    //    dataView.refresh();
                    //}
                    scope.add = function () {
                        scope.item = {};
                        scope.itemModalShow = true;
                    };
                    $($window).bind('keydown', function (event) {
                        if (event.ctrlKey || event.metaKey) {
                            switch (String.fromCharCode(event.which).toLowerCase()) {
                                case 's':
                                    event.preventDefault();
                                    if (scope.itemModalShow === true) {
                                        if (scope.itemModalLocker !== true) {
                                            scope.confirmAddOrUpdate();
                                        }
                                    }
                                    break;
                            }
                        }
                    });
                    scope.onItemModalShown = function () {
                        scope.modalButtonShow = true;
                        if (scope.item && scope.item.ID && scope.item.ID !== 0) {
                            scope.itemModalLocker = true;
                            ajaxer.post(attrs.readDataUrl, { Data: { ID: scope.item.ID } })
                                .then(function (data) {
                                scope.item = data.Data;
                            }, function (data) {
                                notifier.Error(data);
                                $timeout(function () {
                                    scope.itemModalShow = false;
                                }, 2000);
                            }).finally(function () {
                                scope.itemModalLocker = false;
                            });
                        }
                    };
                    scope.onItemModalHide = function () {
                        scope.modalButtonShow = false;
                    };
                    scope.modify = function () {
                        var rows = grid.getSelectedRows();
                        var id = rows[0];
                        scope.item = dataView.getItemById(id);
                        scope.itemModalShow = true;
                    };
                    scope.delete = function () {
                        var rows = grid.getSelectedRows();
                        var id = rows[0];
                        scope.itemRemoveModalShow = true;
                        scope.item = angular.copy(dataView.getItemById(id));
                    };
                    scope.closeModal = function () {
                        scope.itemModalShow = false;
                    };
                    scope.confirmAddOrUpdate = function () {
                        scope.itemModalLocker = true;
                        delete scope.item.id;
                        ajaxer.post(attrs.saveDataUrl, { Data: scope.item })
                            .then(function (data) {
                            scope.itemModalShow = false;
                            delete scope.item;
                            scope.refresh();
                        }, function (data) {
                            notifier.Error(data);
                        }).finally(function () {
                            scope.itemModalLocker = false;
                        });
                    };
                    scope.confirmDelete = function () {
                        scope.itemRemoveModalLocker = true;
                        delete scope.item.id;
                        ajaxer.post(attrs.deleteDataUrl, { Data: scope.item })
                            .then(function (data) {
                            scope.itemRemoveModalShow = false;
                            delete scope.item;
                            scope.refresh();
                        }, function (data) {
                            notifier.Error(data);
                        }).finally(function () {
                            scope.itemRemoveModalLocker = false;
                        });
                    };
                    if (!angular.isUndefined($stateParams.id)) {
                        scope.item = { ID: $stateParams.id };
                        scope.itemModalShow = true;
                    }
                }
            };
        }]);
})(Core || (Core = {}));
