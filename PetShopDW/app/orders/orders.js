var Orders = function () {
    var orderGrid;
    var notActive = false;
    var selectedID;
    ajaxBlocking = false;
    var id;


    var rebindEvents = function () {

        $("#show-active").on("click", function () {
            notActive = !notActive;
            $(this).find("i").toggleClass("fa-eye-slash").toggleClass("fa-eye");
        });

        $("#show-active").unbind(".Job").on("switchChange.bootstrapSwitch.Category", function () {
            orderGrid.settings.sourceFilters = {
                showActive: $("#show-active").is(":checked")
            };
            orderGrid.dataGrid.refresh();
        });

    };

    var initTable = function () {
        orderGrid = new Grid.init({
            gridPlaceholder: "#order",
            url: "/Order/Get",
            sourceFilters: {
                showActive: true
            },
            hideConfigurator: true,
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
                dataField: "DeliveryDetailsID",
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
                dataField: "Country",
                caption: "Country",
                dataType: 'string'
            },
            {
                dataField: "City",
                caption: "City"
            },
            {
                dataField: "Address",
                caption: "Address"
            },
            {
                dataField: "PaidAmount",
                caption: "PaidAmount"
            },
            {
                dataField: "OrderID",
                caption: "OrderID"
            },
            {
                dataField: "IsSent",
                caption: "Is Sent"
            }
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
