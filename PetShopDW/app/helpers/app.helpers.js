var userPermissions = null;
var Helpers = function () {

    var GetParentID = function (el) {
        var id = null;
        var count = 0;

        el = el.parent();

        while (id == null && count < 10) {
            id = el.data("id");
            el = el.parent();
            count++;
        }

        return id;
    };

    var Loading = function (el) {
        el.addClass('m-loader m-loader--light m-loader--right');
        el.attr('disabled', 'disabled');
    }
    var LoadingRemove = function (el) {
        el.removeClass('m-loader m-loader--light m-loader--right');
        el.removeAttr('disabled')

    }
    var GetParent = function (el) {
        var id = null;
        var count = 0;

        el = el.parent();

        while (id == null && count < 10) {
            id = el.data("id");

            if (id == null)
                el = el.parent();

            count++;
        }

        return el;
    };

    var ElSelect2 = function (el, data) {
        el.select2({
            data: data,
            refresh: true,
            placeholder: 'Select value'
        });
    };

    var ElSelect2AllowClear = function (el, data) {
        el.select2({
            data: data,
            allowClear: true,
            placeholder: "Select a value",
            refresh: true,
        });
    };
    var ElSelect2FormatSelection = function (el, data, enableClear, formatSelection) {
        if (typeof formatSelection == "undefined")
            formatSelection = function (selectedValue) {
                if (!selectedValue.id) { return selectedValue.text; }
                var $s = $("<span id='selected-value'>" + selectedValue.text + "</span>");
                return $s;
            };

        el.select2({
            data: data,
            allowClear: enableClear,
            placeholder: "Select a value",
            refresh: true,
            templateSelection: formatSelection,
        });
    };

    var CreateGuid = function () {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    };

    var DatePicker = function (el) {
        el.datepicker({
            autoclose: true,
            todayHighlight: true
        });
    };

    var ParseFloat = function (val) {
        if ($.type(val) === "number")
            return val;

        val = val.replace(/[^0-9.-]/g, '');

        var result = 0;

        if (val != null && val !== "" && !isNaN(val))
            result = parseFloat(val);

        return result;
    };

    var ParseInt = function (val) {
        if ($.type(val) === "number")
            return val;

        if (val != null) {
            val = val.replace(/[^0-9.-]/g, '');

            var result = 0;

            if (val != null && val !== "" && !isNaN(val))
                result = parseInt(val);

            return result;
        }
    };

    var CheckNumeric = function (field) {
        if ($.trim(field.val()) < 0 || !$.isNumeric(field.val()))
            return false;
        else
            return true;
    };

    var DoValidateFields = function (obj, vStatus) {
        var countFields = $(obj).length;
        var validated = 0;
        $(obj).each(function () {
            var self = $(this);

            var name = self.attr("data-id");
            if (typeof name === "undefined")
                name = self.attr("name");

            var validateTitle = name;

            var valid = self.attr("data-validate");
            if (typeof valid !== "undefined")
                validateTitle = valid;

            if (typeof validateTitle === "undefined") {
                validateTitle = self.attr("id");
            }

            var numericCheck = true;
            if (self.attr("data-inputmask") == "decimal" || self.attr("data-inputmask") == "integer")
                numericCheck = CheckNumeric(self);

            var dropdown = false;
            if (self.hasClass("s2me") || self.hasClass("s2static"))
                dropdown = true;

            if (vStatus == null || vStatus == "")
                vStatus = "validate";

            var validateStatus = self.attr(vStatus);
            if (typeof validateStatus === "undefined")
                validateStatus = null;

            //if disabled = validated;
            var disabled = self.attr("disabled");

            if (disabled == "disabled") {
                self.css("border", "");
                validated++;
            }
            else if (validateStatus && ($.trim(self.val()) == null || $.trim(self.val()) == "" || !numericCheck || (dropdown && ($.trim(self.val()) <= 0 || $.trim(self.val()) == null)))) {
                self.css("border", "1px solid #ff844a");
                self.parent().find(".select2-selection").css("border", "1px solid #ff844a");
                toastr.warning(validateTitle + " is Required", "Warning");
            } else {
                self.css("border", "");
                validated++;
            }
        });

        if (validated == countFields)
            return true;
        else
            return false;
    };

    var SetMaxLength = function () {
        $('.form-control[maxlength]').maxlength({
            alwaysShow: false,
            warningClass: "m-badge m-badge--primary m-badge--rounded m-badge--wide",
            limitReachedClass: "m-badge m-badge--danger m-badge--rounded m-badge--wide",
            validate: true
        });
    };

    var UniformFix = function (el) {
        setTimeout(function () {
            el.uniform();
        }, 100);
    };

    var SetInputMask = function (el) {
        el.inputmask({ alias: el.attr("data-mask") });
    };

    var SetAllInputMasks = function () {
        $("input[data-mask]").each(function () {
            SetInputMask($(this));
        })
    };

    var LoadData = function (obj, content) {
        if (content.length > 0) {
            //var regex = /{{\w+}}/gmi;
            var regex = /{{[A-z._\-]+}}/gmi;
            var regexReplaceFront = /{{/;
            var regexReplaceEnd = /}}/;
            var result = content.html();

            $.each(result.match(regex), function (index, value) {
                var tempVal = value.replace(regexReplaceFront, "").replace(regexReplaceEnd, "")

                var tempArr = tempVal.split(".");

                var x = obj[tempArr[0]];
                if (tempArr.length > 1) {
                    if (x != null)
                        x = x[tempArr[1]];
                }

                result = result.replace(value, EmptyIfNull(x));
            });

            var tempParsedResult = $(result);
            tempParsedResult.find(":input[type='checkbox']").each(function () {
                $(this).attr("checked", $(this).val() == "true");
                $(this).removeAttr("value");
            });

            return tempParsedResult.wrap("<tbody>").parent().html();
        }
    };

    var EmptyIfNull = function (val) {
        if (val || val == 0)
            return val;
        else
            return "";
    };

    function ParentS2MeValue(parent, field) {
        var val = null;
        var el = parent.find("[data-id='" + field + "']");

        if (el.select2("val") !== "null")
            val = el.select2("val");

        return val;
    }
    function SelectInSelect2(el, data) {
        el.val(data).trigger('change');
    }
    function ReinitS2Me(dropdowns) {
        $(".s2me").each(function (e) {
            var self = $(this);
            var dataSource = self.attr("data-list");
            var tVal = self.attr("data-value");

            Helpers.elSelect2(self, dropdowns[dataSource]);
            Helpers.SelectInSelect2(self, tVal);
        });
    }
    function ParentFieldValueAtId(parent, field) {
        return parent.find("input[data-id='" + field + "']").val();
    }

    function ParentCheckboxValueAtId(parent, field) {
        return parent.find("input[data-id='" + field + "']").is(":checked");
    }

    function ParentSetFieldValueAtId(parent, field, value) {
        return parent.find("input[data-id='" + field + "']").val(value);
    }

    function ParentSetFieldAttrAtId(parent, field, attrName, attrValue) {
        return parent.find("input[data-id='" + field + "']").attr(attrName, attrValue);
    }

    function ParentDeleteFieldAttrAtId(parent, field, attrName) {
        return parent.find("input[data-id='" + field + "']").removeAttr(attrName);
    }

    function SetButtonColorClass(buttonName, colorStyle) {
        var btn = $('#' + buttonName);
        if (btn.length == 1)
            btn[0].className += ' ' + colorStyle;
    };

    function ToggleDisabled(el) {
        if (el.attr("disabled") == "disabled") {
            el.removeAttr("disabled");
        } else {
            el.attr("disabled", "disabled");
        }
    };

    function Base64ToBlob(base64, mime) {
        mime = mime || '';
        var sliceSize = 1024;
        var byteChars = window.atob(base64);
        var byteArrays = [];

        for (var offset = 0, len = byteChars.length; offset < len; offset += sliceSize) {
            var slice = byteChars.slice(offset, offset + sliceSize);

            var byteNumbers = new Array(slice.length);
            for (var i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            var byteArray = new Uint8Array(byteNumbers);

            byteArrays.push(byteArray);
        }

        return new Blob(byteArrays, { type: mime });
    }

    return {
        getParentID: function (el) {
            return GetParentID(el);
        },
        reinitS2Me: function (dropdowns) {
            return ReinitS2Me(dropdowns);
        },

        getParent: function (el) {
            return GetParent(el);
        },
        loading: function (el) {
            return Loading(el);
        },
        loadingRemove: function (el) {
            return LoadingRemove(el);
        },
        elSelect2: function (el, data) {
            return ElSelect2(el, data);
        },
        elSelect2AllowClear: function (el, data) {
            ElSelect2AllowClear(el, data);
        },
        createGuid: function () {
            return CreateGuid();
        },
        datePicker: function (el) {
            DatePicker(el);
        },
        parseFloat: function (val) {
            return ParseFloat(val);
        },
        parseInt: function (val) {
            return ParseInt(val);
        },
        validateFields: function (attr, vAttr) {
            return DoValidateFields("[validate]", vAttr);
        },
        checkNumeric: function (field) {
            return CheckNumberic(field);
        },
        setMaxLength: function () {
            SetMaxLength();
        },
        uniformFix: function (el) {
            UniformFix(el);
        },
        elSelect2FormatSelection: function (el, data, enableClear, formatSelection) {
            ElSelect2FormatSelection(el, data, enableClear, formatSelection);
        },
        setInputMask: function () {
            SetAllInputMasks();
        },
        loadData: function (obj, content) {
            return LoadData(obj, content);
        },
        ParentS2MeValue: function (parent, field) {
            return ParentS2MeValue(parent, field);
        },
        ParentFieldValueAtId: function (parent, field) {
            return ParentFieldValueAtId(parent, field);
        },
        SelectInSelect2: function (parent, field) {
            return SelectInSelect2(parent, field);
        },
        ParentCheckboxValueAtId: function (parent, field) {
            return ParentCheckboxValueAtId(parent, field);
        },
        ParentSetFieldValueAtId: function (parent, field, value) {
            return ParentSetFieldValueAtId(parent, field, value);
        },
        ParentSetFieldAttrAtId: function (parent, field, attrName, attrValue) {
            return ParentSetFieldAttrAtId(parent, field, attrName, attrValue);
        }
        ,
        ParentDeleteFieldAttrAtId: function (parent, field, attrName) {
            return ParentDeleteFieldAttrAtId(parent, field, attrName);
        },
        SetButtonColorClass: function (buttonName, colorStyle) {
            SetButtonColorClass(buttonName, colorStyle);
        },
        toggleDisabled: function (element) {
            ToggleDisabled(element);
        },
        RemoveFromArray: function (array, element) {
            const index = array.indexOf(element);

            if (index !== -1) {
                array.splice(index, 1);
            }

            return array;
        },
        IsInArray: function (array, element) {
            const index = array.indexOf(element);

            if (index !== -1) {
                return true;
            } else {
                return false;
            }
        },
        base64ToBlob: function (base64, mime) {
            return Base64ToBlob(base64, mime);
        }


    }
}();

function GetForm(el, parent) {
    var form = {};
    var searchEl = $(el);

    if (parent != null) {
        searchEl = $(parent).find(el);
    }

    searchEl.each(function (e) {
        var self = $(this);
        var name = self.attr("name");
        if (self.hasClass("select2me"))
            form[name] = self.select2("val");
        else if (self.hasClass("check-box"))
            form[name] = self.is(":checked");
        else
            form[name] = self.val();
    });

    return form;
}

jQuery(document).ready(function () {
    if ($.fn.date != undefined) {
        $.fn.datepicker.defaults.format = "dd/mm/yyyy";
    }
    /**

* Trigger callback on done typing.

*/

    (function ($) {

        $.fn.extend({

            donetyping: function (callback, timeout) {

                timeout = timeout || 500; // 500 second default timeout

                var timeoutReference,

                    doneTyping = function (el) {

                        if (!timeoutReference) return;

                        timeoutReference = null;

                        callback.call(el);

                    };

                return this.each(function (i, el) {

                    var $el = $(el);

                    $el.is(':input') && $el.on('keyup keypress', function (e) {

                        if (e.type == 'keyup' && e.keyCode != 8) return;

                        if (timeoutReference) clearTimeout(timeoutReference);

                        timeoutReference = setTimeout(function () {

                            doneTyping(el);

                        }, timeout);

                    }).on('blur', function () {

                        doneTyping(el);

                    });

                });

            }

        });

    })(jQuery);
});

function ParseFilters(params, optionFilters) {
    var headerFilters = [];
    var filters = [];

    if (typeof optionFilters[0] === "string") {
        filters = optionFilters;
    } else {
        var isMainFilterOr = false;
        $.each(optionFilters, function (i, v) {
            if (typeof v === "string" && v === "or") {
                isMainFilterOr = true;
            }
        });

        if (isMainFilterOr) {
            headerFilters.push(optionFilters);
        } else {
            $.each(optionFilters, function (index, value) {
                if (typeof value !== "string") {
                    if (typeof value[0] === "string") {
                        filters.push(value);
                    } else {
                        var isOr = false;
                        $.each(value, function (i, v) {
                            if (typeof v === "string" && v === "or") {
                                isOr = true;
                            }
                        });
                        if (isOr)
                            headerFilters.push(value);
                        else
                            filters.push(value);
                    }
                }
            });
        }
    }
    if (headerFilters.length > 0)
        params.headerFilter = JSON.stringify(headerFilters);
    else
        params.headerFilter = null;

    if (filters.length > 0)
        params.filter = JSON.stringify(filters);
    else
        params.filter = null;

    return params;
}

function GetUserPermissions(sections) {
    var data = {
        SectionNames: sections
    };
    return $.ajax({
        url: "/AU/Base/GetUserPermissions",
        type: "POST",
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            if (data.Response == "OK") {
                userPermissions = data.Result;
                SetPermissions(data.Result);
            } else {
                toastr.error(data.Result);
            }
        },
        error: function (data) {

        }
    });
}

function SetPermissions(data) {
    if (data != null) {
        $.each(data, function (i, v) {
            if (!v)
                $("[data-permission='" + i + "']").addClass("hdn-permission permission-done");
        });
    } else {
        GetUserPermissions();
    }
}

$(document).ready(function () {
    SetPermissions();
});