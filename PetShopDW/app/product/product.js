var Product = function () {
    var productGrid;
    var notActive = false;
    var selectedID;
    ajaxBlocking = false;
    var id;
    var dropdowns = [];

    var loadDropdowns = function () {
        return $.ajax({
            url:"/Product/GetDropdowns",
            type: "GET",
            success: function (data) {
                dropdowns = data.data;
                //Helpers.reinitS2Me(dropdowns);
            },
            error: function (data) {
            }
        });
    };

    var rebindEvents = function () {

        $("#show-active").on("click", function () {
            notActive = !notActive;
            //loadData();
            $(this).find("i").toggleClass("fa-eye-slash").toggleClass("fa-eye");
        });

        $("#show-active").unbind(".Job").on("switchChange.bootstrapSwitch.Category", function () {
            productGrid.settings.sourceFilters = {
                showActive: $("#show-active").is(":checked")
            };
            productGrid.dataGrid.refresh();
        });

    };

    var initTable = function () {
        productGrid = new Grid.init({
            gridPlaceholder: "#product",
            url: "/Product/Get",
            sourceFilters: {
                showActive: true
            },
            gridId: "BodyLocation",
            hideConfigurator: true,
            addURL: "/Product/AddProduct",
            updateURL: "/Product/UpdateProduct",
            //removeURL:  "/Category/RemoveCategory",
            gridSettings: {
                onContentReady: function () {
                    rebindEvents();
                },
                editing: {
                    mode: "batch",
                    allowUpdating: true,
                    allowAdding: true,
                    allowDeleting: false
                }
            },
            paging: {
                enabled: true,
                pageSize: 5
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [5, 10, 15]
            },
            filterRow: {
                visible: false
            },

            gridColumns: [{
                dataField: "ProductID",
                caption: "Actions",
                visible: false,
                cellTemplate: function (container, options) {
                    var row = options.row.data;

                },
                allowFiltering: false,
                allowSorting: false,
                dataType: 'string'
            },
            {
                dataField: "ProductName",
                caption: "Product",
                dataType: 'string'
                },
                {
                    dataField: "CategoryID",
                    caption: "Category",
                    validationRules: [{ type: 'required' }],
                    width: 125,
                    lookup: {
                        dataSource: dropdowns.Users,
                        displayExpr: "text",
                        valueExpr: "id"
                    }
                },
            //{
            //    dataField: "CategoryID",
            //    caption: "CategoryID"
            //},
            {
                dataField: "Description",
                caption: "Description"
                },
                {
                    dataField: "ProductCount",
                    caption: "On Count"
                },
                {
                    dataField: "Price",
                    caption: "Price"
                },
                {
                    dataField: "CreatedDate",
                    dataType:"date"
                },
                {
                    dataField: "UpdatedDate",
                    caption: "Updated Date",
                    dataType:"date"
                },
                {
                    dataField: "IsActive",
                    caption: "Active"
                },
                {
                    dataField: "IsDeleted",
                    caption: "Deleted"
                }
            ]
        });
    };

    return {

        init: function () {
            $("[data-switch=true]").bootstrapSwitch();
            var a1 = loadDropdowns();
            $.when(a1).done(function () {
                initTable();
            });
        }
    };
}();
