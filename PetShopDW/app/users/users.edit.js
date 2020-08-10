var UsersEdit = function () {
    var usersGrid;
    var notSent = true;
    ajaxBlocking = false;
    var UserId;
    var userLocations = [];
    var dropdowns = [];
    var userNameChanged = false;

    var loadDropdowns = function () {
        return $.ajax({
            url: "/Users/GetDropdowns",
            type: "GET",
            success: function (data) {
                dropdowns = data.data;
                Helpers.reinitS2Me(dropdowns);
            },
            error: function (data) {
            }
        });
    };

    var rebindEvents = function () {

        $("#save").on("click", function () {
            var ugs = $('#Sites').select2('val');

            $.ajax({
                url: "/Users/AddUserToGroups?userGroups=" + ugs + "&UserID=" + UserId + "&SendApprovalMail=" + false,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    if (data.Response === "OK") {
                        toastr.success('Saved.');
                        window.setTimeout(function () {
                            window.location.replace('/' + currentLocationCode + '/Users/');
                        }, 1000);
                    }
                    else
                        toastr.error("Error: " + data);
                },
                error: function (jqXHR, exception) {
                    toastr.error("Error while saving");
                }
            });

        });


        $("#UserName").change(function () {
            userNameChanged = true;
        });

        $("#save-edit-user").on("click", function () {
            var saveUserInfo = {};
            $(".ff").each(function () {
                var self = $(this);
                var name = self.attr("name");
                //var pName = name.substring(9, name.length);
                if (self.hasClass("s2me"))
                    saveUserInfo[self.attr('id')] = self.select2("val");
                else if (self.hasClass("checkbox"))
                    saveUserInfo[name] = self.is(":checked");
                else
                    saveUserInfo[name] = self.val();
            });

            saveUserInfo.ID = UserId;
            saveUserInfo.userNameChanged = userNameChanged;
            if (Helpers.validateFields()) {
                $.ajax({
                    url: "/Users/SaveUser/",
                    type: "POST",
                    data: saveUserInfo,
                    success: function (data) {
                        if (data.Response === "OK") {
                            toastr.success("Profile saved!");
                            window.setTimeout(function () { location.reload() }, 1500)
                        }
                        else
                            toastr.error("Error: " + data.Result);

                    }
                });
            }
        })

        $("#reset-user-password").on("click", function () {
            var confirmpassword = $("#repet-password").val();
            var newpassword = $("#new-password").val();

            if (newpassword === confirmpassword && newpassword.length >= 6) {

                var userForm = {
                    NewPassword: newpassword,
                    UserID: UserId
                };

                $.ajax({
                    url: "/UserResetPassword/",
                    type: 'POST',
                    data: JSON.stringify(userForm),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {

                        if (data.Response === "OK") {
                            toastr.success('Password Reset!');
                            $("#repet-password").val("");
                            $("#new-password").val("");
                        }
                        else {
                            toastr.error("Error - " + data.Result);
                            $("#repet-password").val("");
                            $("#new-password").val("");
                        }
                    },
                    error: function (jqXHR, exception) {
                        toastr.error("Error while saving");
                    }
                });
            }
            else {
                toastr.error("Check your password");
            }
        });

        $("#add-remove-button").unbind().on("click", function () {
            var a1 = renderAddRemoveLocations(false);
            $.when(a1).done(function () {
                $("#AddRemoveLocationModal").modal('show');
            })
        });

        $(".cmd-edit-user").unbind().on("click", function () {
            selectedID = $(this).data("id");
            //TODO
        });

        $(".u-nav-link").unbind().on("click", function () {
            var id = $(this).data("id");
            $('.tab-view').hide();
            $('.' + id).show();
        });

        $(".set-default").unbind().on("click", function () {
            var locationId = $(this).attr("data-id");

            $.ajax({
                url: "/" + currentLocationCode + "/Users/SetDefaultLocation?UserId=" + UserId + "&LocationId=" + locationId,
                type: "GET",
                success: function (data) {
                    loadUserLocations();
                    rebindEvents();
                },
                error: function (data) {

                }
            });
        });
    };
    $("#submit").on("click", function (event) {
        var formData = new FormData();
        var File = [];

        var filesInput = $('#upload')[0].files;


        if (filesInput.length > 0) {
            //for (var i = 0; i < File.length; i++)
            formData.append("imageFile", filesInput[0]);


            $.ajax({
                type: "POST",
                url: "/Users/Upload?Id=" + UserId,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response === "OK")
                        location.reload();

                },
                error: function (error) {
                }
            });
        } else
            toastr.warning('Please select file to upload!');
    });
    return {
        init: function (userid) {
            $(".js-example-basic-multiple").select2();
            UserId = userid;

            var a1 = loadDropdowns();
            $.when(a1).done(function () {
                rebindEvents();
            });
        }
    };
}();
