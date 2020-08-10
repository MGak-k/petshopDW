var Grid = (function () {
    function Grid(options) {
        var tplProfile = $("#grid-profile").clone();
        this.storageKey = "";
        this.stateProfiles = [];
        this.profileSelector;
        this.settings = options || {
            gridPlaceholder: "",
            gridId: "",
            gridColumns: [],
            url: "",
            toolbarItems: [],
            sourceFilters: {},
            addURL: "",
            updateURL: "",
            removeURL: ""
        };

        this.gridState = {};

        this.dataGrid = null;
        this.dataSource = null;

        var parent = this;
        this.getGridProfiles = function () {
            //return $.ajax({
            //    url: "/Grid/GetGridProfiles/" + parent.settings.gridId,
            //    method: "GET",
            //    success: function (response) {
            //        if (response.success) {
            //            parent.stateProfiles = response.data["Items"];
            //            parent.storageKey = response.data["SelectedKey"];
            //        }
            //        else
            //            toastr.error(response.errorMessage);
            //    },
            //    fail: function (response) {
            //        toastr.error(response.errorMessage);
            //    }
            //});
        };

        this.getGridProfile = function (data) {
            //var deferred = new $.Deferred;
            //var storageRequestSettings = {
            //    url: "/Grid/GetGridState/" + parent.storageKey,
            //    method: "GET",
            //    dataType: "json",
            //    success: function (response) {
            //        if (response.success) {
            //            deferred.resolve(response.data);

            //        }
            //        else
            //            deferred.resolve();
            //    },
            //    fail: function (error) {
            //        deferred.reject();
            //    }
            //};
            //if (data) {
            //    storageRequestSettings.data = JSON.stringify(data);
            //}
            //$.ajax(storageRequestSettings);
            //return deferred.promise();
        };

        this.sendGridProfile = function (data) {
            //if (parent.storageKey != null && parent.storageKey != "") {
            //    data.gridID = parent.storageKey;
            //    data.GridCode = parent.settings.gridId;

            //    var deferred = new $.Deferred;
            //    $.ajax({
            //        url: "/Grid/SaveGridState/",
            //        data: JSON.stringify(data),
            //        contentType: 'application/json; charset=utf-8',
            //        dataType: 'json',
            //        method: "POST",
            //        success: function (response) {
            //            if (response.success) {
            //                deferred.resolve(response.data);
            //                toastr.success("State saved.");

            //                parent.stateProfiles = response.data["Items"];
            //                parent.storageKey = response.data["SelectedKey"];
            //                parent.profileSelector.option("items", parent.stateProfiles);
            //                parent.profileSelector.option("value", parent.storageKey);

            //                parent.dataGrid.refresh();
            //            }
            //            else
            //                toastr.error(response.errorMessage);
            //        },
            //        fail: function (error) {
            //            deferred.reject();
            //        }
            //    });

            //    return deferred.promise();
            //}
        },

            //this.sendGridProfile = function (data) {
            //    if (parent.storageKey != null && parent.storageKey != "") {
            //        var deferred = new $.Deferred;
            //        var storageRequestSettings = {
            //            url: "/Settings/SaveGridState/",
            //            contentType: 'application/json; charset=utf-8',
            //            dataType: 'json',
            //            method: "POST",
            //            success: function (response) {
            //                if (response.success) {
            //                    deferred.resolve(response.data);
            //                    toastr.success("State saved.");
            //                    parent.saveSemaphore = false;
            //                    parent.dataGrid.refresh();
            //                }
            //                else
            //                    toastr.error(response.errorMessage);
            //            },
            //            fail: function (error) {
            //                deferred.reject();
            //            }
            //        };

            //        if (data) {
            //            data.gridID = parent.storageKey;
            //            storageRequestSettings.data = JSON.stringify(data);
            //        }
            //        $.ajax(storageRequestSettings);
            //        return deferred.promise();
            //    }
            //};

            this.loadData = function () {
                parent.dataSource = new DevExpress.data.DataSource({
                    load: function (loadOptions) {
                        var d = new $.Deferred();
                        var params = {};
                        //Getting filter options
                        if (loadOptions.filter) {
                            params = ParseFilters(params, loadOptions.filter);
                        }
                        //Getting sort options
                        if (loadOptions.sort) {
                            params.sort = JSON.stringify(loadOptions.sort);
                        }
                        //Getting dataField option
                        if (loadOptions.dataField) {
                            params.dataField = loadOptions.dataField;
                        }
                        //skip and take are used for paging
                        params.skip = loadOptions.skip; //A number of records that should be skipped
                        params.take = loadOptions.take; //A number of records that should be taken

                        if (!params.skip)
                            params.skip = 0;
                        if (!params.take)
                            params.take = 20;

                        if (!loadOptions.requireTotalCount) {
                            params.take = 0;
                            params.take = 1000000;
                        }

                        if (loadOptions.searchValue == null && parent.settings.sourceFilters != null)
                            parent.settings.sourceFilters.filter = null;

                        $.ajax({
                            url: parent.settings.url,
                            type: "POST",
                            data: JSON.stringify($.extend(parent.settings.sourceFilters, params)),
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            success: function (response, textStatus, jqXHR) {
                                if (response.success)
                                    d.resolve(response.data, { totalCount: response.data.totalCount });
                                else
                                    d.resolve([], { totalCount: 0 });
                            }
                        });
                        return d.promise();
                    },
                    update: function (key, values) {
                        $.each(values, function (i, n) {
                            key[i] = n;
                        });


                        return $.ajax({
                            url: parent.settings.updateURL,
                            method: "POST",
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: JSON.stringify(key),
                            success: function (response) {

                            }
                        });
                    },
                    insert: function (values) {
                        var filter = parent.settings.sourceFilters;

                        for (var prop in filter) {
                            values[prop] = filter[prop];
                        }

                        return $.ajax({
                            url: parent.settings.addURL,
                            method: "POST",
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: JSON.stringify(values),
                            success: function (response) {

                            }
                        });
                    },
                    remove: function (values) {
                        return $.ajax({
                            url: parent.settings.removeURL,
                            method: "POST",
                            data: values,
                            success: function (response) {

                            }
                        });
                    },
                });
                parent.InitDataTable();
            }

        this.InitDataTable = function () {
            var defaultSettings = {
                dataSource: parent.dataSource,
                allowColumnReordering: true,
                allowColumnResizing: true,
                showColumnLines: true,
                rowAlternationEnabled: true,
                wordWrapEnabled: true,
                columnHidingEnabled: false,
                columnChooser: {
                    enabled: true
                },
                filterRow: {
                    visible: true,
                    applyFilter: "auto"
                },
                pager: {
                    showPageSizeSelector: true,
                    allowedPageSizes: [5, 10, 20],
                    showInfo: true
                },
                remoteOperations: {
                    filtering: true,
                    sorting: true,
                    paging: true
                },

                stateStoring: {
                    enabled: true,
                    type: "custom",
                    customLoad: function () {
                        if (parent.storageKey != "00000000-0000-0000-0000-000000000000")
                            return parent.getGridProfile(parent.storageKey);
                    },
                    customSave: function (state) {
                        //if (parent.saveSemaphore) {
                        parent.gridState = state;
                        //parent.sendGridProfile(state);
                        //}
                        //else {
                        //    parent.saveSemaphore = true;
                        //}
                    }

                },

                onToolbarPreparing: function (e) {
                    var grid = e.component;

                    if (!parent.settings.hideConfigurator) {
                        parent.profileSelector = {
                            location: "after",
                            widget: "dxSelectBox",
                            options: {
                                width: 200,
                                items: parent.stateProfiles,
                                displayExpr: "text",
                                valueExpr: "id",
                                value: parent.storageKey,
                                onValueChanged: function (e) {
                                    parent.storageKey = e.value;

                                    if (parent.storageKey != "00000000-0000-0000-0000-000000000000") {
                                        $.ajax({
                                            url: "/Grid/GetGridState/" + parent.storageKey,
                                            method: "GET",
                                            dataType: "json",
                                            success: function (response) {
                                                if (response.success)
                                                    parent.dataGrid.state(response.data);

                                            },
                                            fail: function (error) {

                                            }
                                        });
                                    }
                                },
                                onInitialized: function (e) {
                                    parent.profileSelector = e.component;
                                }
                            }
                        };

                        e.toolbarOptions.items.push(
                            parent.profileSelector, {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    icon: "save",
                                    onClick: function () {
                                        if (parent.storageKey == "00000000-0000-0000-0000-000000000000") {
                                            bootbox.confirm({
                                                message: tplProfile.html(),
                                                buttons: {
                                                    confirm: {
                                                        label: 'Save profile',
                                                        className: 'btn-success'
                                                    },
                                                    cancel: {
                                                        label: 'Cancel',
                                                        className: 'btn-danger'
                                                    }
                                                },
                                                callback: function (result) {
                                                    if (result) {
                                                        var profil = $("#profile_name").val();
                                                        var profilType = $("input[name='profile_type']:checked").val();

                                                        if (profil == "")
                                                            toastr.error("Please enter profile name.");

                                                        if (profilType == "")
                                                            toastr.error("Please select profile category.");

                                                        var data = parent.gridState;
                                                        data.ProfileName = profil;
                                                        data.ProfileCategory = profilType;
                                                        parent.sendGridProfile(data);

                                                    }
                                                }
                                            });
                                        } else {
                                            var data = parent.gridState;
                                            parent.sendGridProfile(data);
                                        }
                                    }
                                }
                            }
                        )
                    }


                    if (parent.settings.toolbarItems != null) {
                        $.each(parent.settings.toolbarItems, function (i, n) {
                            e.toolbarOptions.items.push(n);
                        });
                    }
                },
                columns: parent.settings.gridColumns
            };

            if (parent.settings.gridSettings != null) {
                $.each(parent.settings.gridSettings, function (i, n) {
                    defaultSettings[i] = n;
                });
            }
            parent.dataGrid = $(parent.settings.gridPlaceholder).dxDataGrid(defaultSettings).dxDataGrid("instance");
            return parent.dataGrid;
        }

        var a1 = this.getGridProfiles();
        $.when(a1).done(function () {
            parent.loadData();
        });

    }

    return {
        init: function (options) {
            return new Grid(options);
        }
    };

}());