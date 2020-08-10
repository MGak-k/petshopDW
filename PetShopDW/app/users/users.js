var Users = function () {
    var usersGrid;
    var showActive = true;
    var currentLocationCode;
    ajaxBlocking = false;
    
    var rebindEvents = function () {
        $("#show-active").on("click", function () {
            $(this).find("i").toggleClass("fa-eye-slash").toggleClass("fa-eye");
        });

        $("#show-active").unbind(".Users").on("switchChange.bootstrapSwitch.Users", function () {

            usersGrid.settings.sourceFilters = {
                showInactive: $("#show-active").is(":checked")
            };
            showActive = !showActive;

            usersGrid.dataGrid.refresh();
        });

        $(".cmd-delete-user").unbind().on("click", function () {
            var userID = $(this).data("id");
            $.ajax({
                url: "/Users/DeleteUser?UserID=" + userID,
                type: "POST",
                success: function (data) {
                    if (data.success === true) {
                        initTable();
                    }
                    else {
                        toastr.error("Error: " + data.Result);
                    }
                }
            });
        });

        $(".cmd-restore-user").unbind().on("click", function () {
            var userID = $(this).data("id");
            $.ajax({
                url:  "/Users/Restore?UserID=" + userID,
                type: "POST",
                success: function (data) {
                    if (data.success === true) {
                        showActive = !showActive;
                        initTable();
                    }
                    else {
                        toastr.error("Error: " + data.Result);
                    }
                }
            });
        });

    };

    var initTable = function () {
        usersGrid = new Grid.init({
            gridPlaceholder: "#users",
            url: "/Users/Get",
            sourceFilters: {
                showInactive: false
            },
            gridId: "Users",
            hideConfigurator: true,

            gridSettings: {
                onContentReady: function () {
                    rebindEvents();
                },
                editing: {
                    mode: "batch",
                    allowUpdating: false,
                    allowAdding: false,
                    allowDeleting: false
                }
            },

            gridColumns: [{
                dataField: "ID",
                width: 110,
                cssClass: "alignElements",
                cellTemplate: function (container, options) {
                    var row = options.row.data;

                    $("<a href='/Users/Edit/" + row.ID + "'  title='Edit User' class='btn btn-success btn-sm cmd-unlock cmd-edit-user mright5 gridButton'><i class='fa fa-edit'></i></a>").appendTo(container);
                    if (showActive) {
                        $("<button title='Remove User' class='btn btn-danger btn-sm cmd-unlock cmd-delete-user mright5 gridButton' data-id='" + row.ID + "'><i class='fa fa-trash'></i></button>").appendTo(container);
                    } else {
                        $("<button title='Restore User' class='btn btn-info btn-sm cmd-unlock cmd-restore-user mright5 gridButton' data-id='" + row.ID + "'><i class='fa fa-refresh'></i></button>").appendTo(container);
                    }
                }
            },
            {
                dataField: "FirstName",
            },
            {
                dataField: "LastName",
            },
            {
                dataField: "UserName",
            },
            {
                dataField: "Email",
            },
            {
                dataField: "Active",
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

//jQuery(document).ready(function () {
//    Users.init();
//});