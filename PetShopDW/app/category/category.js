var Category = function () {
    var categoryGrid;
    var notActive = false;
    var selectedID;
    ajaxBlocking = false;
    var id;


    var rebindEvents = function () {

        $("#show-active").on("click", function () {
            notActive = !notActive;
            //loadData();
            $(this).find("i").toggleClass("fa-eye-slash").toggleClass("fa-eye");
        });

        $("#show-active").unbind(".Job").on("switchChange.bootstrapSwitch.Category", function () {
            categoryGrid.settings.sourceFilters = {
                showActive: $("#show-active").is(":checked")
            };
            categoryGrid.dataGrid.refresh();
        });

    };

    var initTable = function () {
        categoryGrid = new Grid.init({
            gridPlaceholder: "#category",
            url: "/Category/Get",
            sourceFilters: {
                showActive: true
            },
            gridId: "BodyLocation",
            hideConfigurator: true,
            addURL: "/Category/AddCategory",
            updateURL: "/Category/UpdateCategory",
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
                dataField: "CategoryID",
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
                dataField: "CategoryName",
                caption: "Category",
                dataType: 'string'
            },
            {
                dataField: "IsActive",
                caption: "Active"
            },
            {
                dataField: "IsDeleted",
                captipn: "Deleted"
            },
            ]
        });
    };

    return {

        init: function () {
            $("[data-switch=true]").bootstrapSwitch();
                initTable();
        }
    };
}();
