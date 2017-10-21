$(function () {
    $(':input:visible').first().focus();
});

var NotificationTypeNames = { Error: "error", Warning: "warning", Success: "success" };

/*
* Generic function to check whether an object is empty
*/
function isEmpty(obj) {
    if (typeof obj == 'undefined' || obj === null || obj === '') return true;
    if (typeof obj == 'number' && isNaN(obj)) return true;
    if (obj instanceof Date && isNaN(Number(obj))) return true;
    return false;
}

function saveKendoGridState(gridName) {
    var grid = $(gridName).data("kendoGrid");
    var gridOptions = JSON.parse(kendo.stringify(grid.getOptions()));
    delete gridOptions['columns']; // restoring column state will remove button event handlers
    var dataSource = gridOptions['dataSource'];
    delete dataSource['transport']; // removing transport from datasource as otherwise read is restored
    localStorage[gridName.substr(1) + "-kendo-grid-options"] = kendo.stringify(gridOptions);
}

function loadKendoGridUsingSavedState(gridName) {
    var grid = $(gridName).data("kendoGrid");
    var options = localStorage[gridName.substr(1)+"-kendo-grid-options"];
    if (options) {
        var toolBar = $(gridName + " .k-grid-toolbar").html();
        grid.setOptions(JSON.parse(options));
        $(gridName+" .k-grid-toolbar").html(toolBar);  // templates are lost once setOptions is invoked
        $(gridName+" .k-grid-toolbar").addClass("k-grid-top");
    }
}

function popupGridEditorButtonNamingOnInsert(gridName) {
    var grid = $(gridName).data("kendoGrid");
    grid.bind("edit", function (e) {
        e.container.find(".k-grid-update, .k-grid-cancel").css("margin-top", "10px");
        if (typeof isCreating !== "undefined" && isCreating) {
            e.container.find(".k-grid-update").html('<span class="k-icon k-update"></span>Add');
            isCreating = false;
        }
    });
}

function highlightRequiredFields() {
    var divs = $('.editor-field').has('[data-val-required]:not([type="checkbox"])');
    $(divs).each(function (index, div) {
        $(div).children().first().before('<span class="required-indicator">*</span>');

    });
}

function onErrorNoXhr(e) {
    var errorStatus = e.status;
    var errorHandlingInstanceId = null;
    if (e.getResponseHeader("ErrorMessage") !== undefined) {
        errorHandlingInstanceId = e.getResponseHeader("ErrorMessage");
    }
    onErrorMessage(errorStatus, errorHandlingInstanceId, false);
}

function onErrorCustom(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        kendoErrorAlert(message);
    }
}

function onError(e) {
    onErrorCore(e, true);
}

function onErrorNoCancel(e) {
    onErrorCore(e, false);
}

function onErrorCancelWithGridId(e, id) {
    onErrorCore(e, false);
    var grid = $(id).data("kendoGrid");
    grid.cancelChanges();
}

function onErrorCore(e, cancelChanges) {
    stopSpinner();
    // Error Status of 'customerror' when you set ModelStateError and return JSON result
    var errorStatus = e.status;
    if (errorStatus == "customerror") {
        if (canAddMessageToValidationSummary()) {
            addMessageToValidationSummary(getCustomErrors(e));
        } else {
            onErrorCustom(e);
        }
        if (cancelChanges && this.cancelChanges) {
            this.cancelChanges();
        }
    }
    else {
        if (e.xhr !== null && e.xhr.getResponseHeader("HandledError") !== undefined) {
            var errorMessage = e.xhr.getResponseHeader("HandledError");
            kendoErrorAlert(errorMessage);
            if (cancelChanges && this.cancelChanges) {
                this.cancelChanges();
            }
        }
        else {
            var errorHandlingInstanceId = null;
            if (e.xhr !== null && e.xhr.getResponseHeader("ErrorMessage") !== undefined) {
                errorHandlingInstanceId = e.xhr.getResponseHeader("ErrorMessage");
            }
            onErrorMessage(errorStatus, errorHandlingInstanceId, true);
        }
    }
}

function onErrorMessage(errorStatus, errorMessage, cancelChanges) {
    if (errorStatus !== "modelstateerror") {
        if (errorMessage && errorMessage.indexOf("SessionExpired") >= 0) {
            window.location.href = "/Error/SessionExpired";
        } else {
            var message = "An unexpected error has occurred the details of which have been logged. Please try again and if the problem persists, contact an administrator.";
            if (errorMessage) {
                message = "An unexpected error has occurred the details of which have been logged. Please try again and if the problem persists, contact an administrator with the Error ID [" + errorMessage + "]";
            }
            kendoErrorAlert(message);

            if (cancelChanges) {
                if (typeof (this.cancelChanges) === "function") {
                    this.cancelChanges();
                }
            }
        }
    }
}

function canAddMessageToValidationSummary() {
    var element = $("[data-valmsg-summary=true]");
    if (element && element.length > 0) {
        return true;
    }
    return false;
}

function getCustomErrors(e) {
    var messages = [];
    if (e.errors) {
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    messages.push(this);
                });
            }
        });
    }
    return messages;
}

/*
* This expects a JsonResultViewModel
*/
function addMessageToValidationSummaryFromJson(model) {
    addMessageToValidationSummary(model.Messages);
}

function addMessageToValidationSummary(messages) {
    // Try and find the validation summary.
    var element = $("[data-valmsg-summary=true]");
    if (element) {
        var ul = element.find("ul");
        if (messages && messages.length > 0) {
            // Set classes.
            element
                .removeClass("validation-summary-valid")
                .addClass("validation-summary-errors");
            messages.forEach(function (entry) {
                ul.append("<li>" + entry + "</li>");
            });
        } else {
            // Set classes.
            element
                .removeClass("validation-summary-errors")
                .addClass("validation-summary-valid");
            ul.empty();
        }
    }
}

function syncHandler(e) {
    this.read();
}

function updateError(e) {
    kendoErrorAlert();
}

function searchError(e) {
    kendoErrorAlert();
}

function diagramError() {
    kendoErrorAlert();
}

function kendoErrorAlert(message) {
    var msg = "An unexpected error has occurred the details of which have been logged. Please try again and if the problem persists, contact an administrator.";
    if (!isEmpty(message)) {
        msg = message;
    }

    kendo.alert(msg);
}

function error_handler(e) {
    if (e.errors) {
        var message = "";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        kendoErrorAlert(message);
    }
}

function addInputType() {
    var wnd = $("#input-type-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#InputTypeName").focus();
    });

    wnd.refresh().center().open();
}

function onInputTypeRefresh() {
    $("#InputTypeCreateButton").click(function () {
        var form = $(this).closest("form");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (newInputType) {
                var multiselect = $("#DeskInputTypes").data("kendoMultiSelect");
                var currentValue = multiselect.value().slice(0);
                currentValue.push(newInputType.Id);
                multiselect.dataSource.add(newInputType);
                multiselect.dataSource.filter({});
                multiselect.value(currentValue);
                multiselect.trigger("change");
                $("#input-type-window").data("kendoWindow").close();
            },
            error: function (result) {
                handleResponseTextError(result);
            },

        });
    });
}

function onOperationalProcessTypeRefresh(e) {
    $("#operationalProcessTypeCreateButton").click(function (clk) {
        var form = $(this).closest("form");
        var data;
        if ($("#OperationalProcessTypeRefDataGrid").data("kendoGrid")) {
            data = form.serialize();
        } else {
            data = {
                OperationalProcessTypeName: $('#OperationalProcessTypeName').val(),
                Visible: $("input#OperationalProcessTypeVisible").is(":checked"),
                Standard: $("input#OperationalProcessTypeStandard").is(":checked"),
                SortOrder: $("#OperationalProcessTypeSortOrder").val(),
            };
        }

        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: data,
                success: function(newProcessType) {
                    var multiSelect = $("#OperationalProcesses_OperationalProcessTypes").data("kendoMultiSelect");
                    multiSelect.dataSource.add(newProcessType);
                    var selectedValues = multiSelect
                        .value(); // strange but true, cannot push onto existing selcted values array
                    var newValues = $.merge($
                        .merge([], selectedValues),
                        [newProcessType.Id]); // have to create a new array
                    multiSelect.value(newValues);
                    $("#process-type-window").data("kendoWindow").close();
                },
                error: function(result) {
                    handleResponseTextError(result);
                },
                complete: function () {
                    stopSpinner();
                }
            });
        } else {
            clk.preventDefault();
        }
    });
}

function addDomainType(e) {
    // Has it has come from the Edit Service Domain Page
    if ($(".js-editServiceDomain").length) {
        e.preventDefault();
    }

    var wnd = $("#domain-type-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#DomainName").focus();
    });

    wnd.refresh().center().open();
}

function onDomainTypeRefresh() {
    $("#domainTypeCreateButton").click(function (clk) {
        var form = $(this).closest("form");
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (newInputType) {
                    // Has it come from the Edit Service Domain page?
                    if ($(".js-editServiceDomain").length) {
                        var dropDownList = $("#ServiceDomain_DomainTypeId").data("kendoDropDownList");
                        if (dropDownList) {
                            dropDownList.dataSource.add(newInputType);
                            dropDownList.value(newInputType.Id);
                            dropDownList.trigger("change");
                        }
                    } else {
                        // It has come from the Add Service Domain page?
                        var addServiceDomainGrid = $("#AddServiceDomainGrid").data("kendoGrid");
                        if (addServiceDomainGrid) {
                            addServiceDomainGrid.addRow();
                            var toolbar = addServiceDomainGrid.wrapper.find(".k-grid-toolbar").html();
                            var gridColumns = addServiceDomainGrid.columns;
                            var domainTypeColumn = getKendoGridColumn(addServiceDomainGrid, "DomainTypeId");
                            var domainTypeColumnIndex = getKendoGridIndex(addServiceDomainGrid, "DomainTypeId");
                            var existingDomainTypes = domainTypeColumn.values;
                            existingDomainTypes.push({ text: newInputType.DomainName, value: newInputType.Id });
                            gridColumns[domainTypeColumnIndex] = {
                                dataTextField: "Text",
                                dataValueField: "Value",
                                field: domainTypeColumn.field,
                                width: domainTypeColumn.width,
                                title: domainTypeColumn.title,
                                filter: "contains",
                                values: existingDomainTypes,
                                editor: function(container, options) {
                                    $("<input name='" + options.field + "'/>")
                                        .appendTo(container)
                                        .kendoDropDownList({
                                            dataSource: existingDomainTypes,
                                            filter: "startswith",
                                            dataTextField: "text",
                                            optionLabel: "-- Please Select --",
                                            dataValueField: "value"
                                        });
                                }
                            };
                            addServiceDomainGrid.setOptions({ columns: gridColumns });

                            addServiceDomainGrid.wrapper.find(".k-grid-toolbar").html(toolbar);

                            // Set the value of the first row to be the newly created type
                            var data = addServiceDomainGrid.dataSource.at(0);
                            data.set("DomainTypeId", newInputType.Id);

                            // Need to re-attach the hander for the Add More Rows button.
                            addServiceDomainClickHandlers();
                        }
                    }

                    $("#domain-type-window").data("kendoWindow").close();
                },
                error: function (result) {
                    handleResponseTextError(result);
                },
            });
            stopSpinner();
        } else {
            clk.preventDefault();
        }
    });
}

function handleResponseTextError(result) {
    try {
        var jsonResponseText = $.parseJSON(result.responseText);
        var message;
        $.each(jsonResponseText, function(name, val) {
            if (name === "Errors") {
                message = $.parseJSON(JSON.stringify(val));
                return false;
            }
        });

        if (message == undefined) {
            onErrorMessage(500);
        } else {
            kendoErrorAlert(message);
        }
    }
    catch (e) {
        kendoErrorAlert("An unexpected error has occurred the details of which have been logged. Please try again and if the problem persists, contact an administrator.");
    }
}

function addServiceDomainClickHandlers() {

    // Add More Rows button.
    $("#addMoreRowsToolbar").click(function (e) {
        e.preventDefault();
        var grid = $("#AddServiceDomainGrid").data("kendoGrid");
        repeat(function () { grid.addRow(); }, 4);
    });

    // Add Domain Type button.
    $("#addDomainTypeToolbar").click(function (e) {
        e.preventDefault();
        addDomainType(e);
    });
}

var moveServiceDomainId;
var moveServiceDeskId;
function moveServiceDomainType(e) {
    var wnd = $("#domain-move-window").data("kendoWindow");
    if (wnd) {
        wnd.refresh().center().open();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        moveServiceDomainId = dataItem.Id;
        moveServiceDeskId = dataItem.ServiceDeskId;
    }
}

function onMoveServiceDomainTypeRefresh() {
    var dropDownList = $("#ServiceDeskId").data("kendoDropDownList");
    if (dropDownList) {
        var currentDropDownListData = dropDownList.dataSource.data();
        if (currentDropDownListData) {
            for (var i = 0; i < currentDropDownListData.length; i++) {
                if (currentDropDownListData[i].Key == moveServiceDeskId) {
                    dropDownList.dataSource.remove(currentDropDownListData[i]);
                    break;
                }
            }
        }
        $("#moveDomainType")
            .click(function(clk) {
                var form = $(this).closest("form");
                var validator = form.kendoValidator().data("kendoValidator");
                if (validator.validate()) {
                    startSpinner();
                    $("#ServiceDomainId").val(moveServiceDomainId);
                    $.ajax({
                        url: form.attr("action"),
                        type: "POST",
                        data: form.serialize(),
                        success: function(newInputType) {
                            var wnd = $("#domain-move-window").data("kendoWindow");
                            wnd.close();
                            var grid = $("#ServiceDomainGrid");
                            if (grid) {
                                var kendoGrid = grid.data("kendoGrid");
                                kendoGrid.dataSource.read();
                                kendoGrid.refresh();
                            }
                            stopSpinner();
                        },
                        error: function(result) {
                            onErrorNoXhr(result);
                            stopSpinner();
                        }
                    });
                } else {
                    clk.preventDefault();
                }
            });
    }
}

function repeat(fn, times) {
    for (var i = 0; i < times; i++) fn();
}

function onAddCustomerPackClick() {
    var wnd = $("#add-customer-pack-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#Filename").focus();
    });

    wnd.refresh().center().open();
}

function onUploadCustomerFileClick() {
    var wnd = $("#upload-customer-file-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#CustomerFile").focus();
    });

    wnd.refresh().center().open();
}

function getKendoGridColumn(grid, fieldName) {
    for (var i = 0; i < grid.columns.length; i++) {
        var gridColumn = grid.columns[i];
        if (gridColumn.field == fieldName) {
            return gridColumn;
        }
    }
    return null;
}

function getKendoGridIndex(grid, fieldName) {
    for (var i = 0; i < grid.columns.length; i++) {
        var gridColumn = grid.columns[i];
        if (gridColumn.field == fieldName) {
            return i;
        }
    }
    return null;
}

function openSelectUserWindow() {
    var wnd = $("#user-selector-window").data("kendoWindow");
    wnd.refresh().center().open();
}

function closeSelectUserWindow() {
    var wnd = $("#user-selector-window").data("kendoWindow");
    wnd.close();
}

function getSelectUserSelected() {
    return $("#SelectedEmail").data("kendoDropDownList").value();
}

/* Fixup the Menu dropdowns when loaded by Ajax */
function fixupAjaxGridMenus() {
    $('.cell-menu-list').kendoMenu();
}

function addFunctionType(e) {
    // Has it has come from the Edit Service Function Page
    if ($(".js-editServiceFunction").length) {
        e.preventDefault();
    }

    var wnd = $("#function-type-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#FunctionName").focus();
    });

    wnd.refresh().center().open();
}

function addServiceFunctionClickHandlers() {
    // Add More Rows button.
    $("#addMoreRowsToolbar").click(function (e) {
        e.preventDefault();
        var grid = $("#AddServiceFunctionGrid").data("kendoGrid");
        repeat(function () { grid.addRow(); }, 4);
    });

    // Add Function Type button.
    $("#addFunctionTypeToolbar").click(function (e) {
        e.preventDefault();
        addFunctionType();
    });
}

var moveServiceFunctionId;
function moveServiceFunctionType(e) {
    var wnd = $("#function-move-window").data("kendoWindow");
    wnd.refresh().center().open();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    moveServiceFunctionId = dataItem.Id;
    moveServiceDomainId = dataItem.ServiceDomainId;
}

function onMoveServiceFunctionTypeRefresh() {
    var dropDownList = $("#ServiceDomainId").data("kendoDropDownList");
    var currentDropDownListData = dropDownList.dataSource.data();
    if (currentDropDownListData) {
        for (var i = 0; i < currentDropDownListData.length; i++) {
            if (currentDropDownListData[i].Key == moveServiceDomainId) {
                dropDownList.dataSource.remove(currentDropDownListData[i]);
                break;
            }
        }
    }
    $("#moveFunctionType").click(function (clk) {
        var form = $(this).closest("form");
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            $("#ServiceFunctionId").val(moveServiceFunctionId);
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (newInputType) {
                    var wnd = $("#function-move-window").data("kendoWindow");
                    wnd.close();
                    var grid = $("#ServiceFunctionGrid");
                    if (grid) {
                        var kendoGrid = grid.data("kendoGrid");
                        kendoGrid.dataSource.read();
                        kendoGrid.refresh();
                    }
                    stopSpinner();
                },
                error: function (result) {
                    onErrorNoXhr(result);
                    stopSpinner();
                }
            });
        } else {
            clk.preventDefault();
        }
    });
}

function onFunctionTypeRefresh() {
    $("#functionTypeCreateButton").click(function (clk) {
        var form = $(this).closest("form");
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (newInputType) {
                    // Has it come from the Add Service Function page?
                    var addServiceFunctionGrid = $("#AddServiceFunctionGrid").data("kendoGrid");
                    if (addServiceFunctionGrid) {
                        addServiceFunctionGrid.addRow();
                        var toolbar = addServiceFunctionGrid.wrapper.find(".k-grid-toolbar").html();

                        var gridColumns = addServiceFunctionGrid.columns;
                        var functionTypeColumn = getKendoGridColumn(addServiceFunctionGrid, "FunctionTypeId");
                        var functionTypeColumnIndex = getKendoGridIndex(addServiceFunctionGrid, "FunctionTypeId");
                        var existingFunctionTypes = functionTypeColumn.values;
                        existingFunctionTypes.push({ text: newInputType.FunctionName, value: newInputType.Id });
                        gridColumns[functionTypeColumnIndex] = {
                            dataTextField: "Text",
                            dataValueField: "Value",
                            field: functionTypeColumn.field,
                            width: functionTypeColumn.width,
                            title: functionTypeColumn.title,
                            filter: "contains",
                            values: existingFunctionTypes,
                            editor: function (container, options) {
                                $("<input name='" + options.field + "'/>").appendTo(container)
                                    .kendoDropDownList({
                                        dataSource: existingFunctionTypes,
                                        filter: "startswith",
                                        dataTextField: "text",
                                        optionLabel: "-- Please Select --",
                                        dataValueField: "value"
                                    });
                            }
                        };

                        addServiceFunctionGrid.setOptions({ columns: gridColumns });

                        addServiceFunctionGrid.wrapper.find(".k-grid-toolbar").html(toolbar);

                        // Set the value of the first row to be the newly created type
                        var data = addServiceFunctionGrid.dataSource.at(0);
                        data.set("FunctionTypeId", newInputType.Id);

                        // Need to re-attach the hander for the Add More Rows button.
                        addServiceFunctionClickHandlers();
                    } else {
                        // Has it come from the Edit Service Function page?
                        if ($(".js-editServiceFunction").length) {
                            var dropDownList = $("#ServiceFunction_FunctionTypeId").data("kendoDropDownList");
                            if (dropDownList) {
                                dropDownList.dataSource.add(newInputType);
                                dropDownList.value(newInputType.Id);
                                dropDownList.trigger("change");
                            }
                        }
                    }

                    $("#function-type-window").data("kendoWindow").close();
                },
                error: function (result) {
                    handleResponseTextError(result);
                }
            });
            stopSpinner();
        } else {
            clk.preventDefault();
        }
    });
}
var moveResolverId;
var moveResolverDeskId;
var moveServiceComponentId;
var moveServiceComponentServiceFunctionId;

function moveServiceComponentLevel1(dataItem) {
    var wnd = $("#component-level1-move-window").data("kendoWindow");
    wnd.refresh().center().open();
    moveServiceComponentId = dataItem.Id;
    moveServiceComponentServiceFunctionId = dataItem.ServiceFunctionId;
}

function moveServiceComponentLevel2(dataItem) {
    var wnd = $("#component-level2-move-window").data("kendoWindow");
    wnd.refresh().center().open();
    moveServiceComponentId = dataItem.Id;
    moveServiceComponentServiceFunctionId = dataItem.ServiceFunctionId;
}

function openMoveResolverWindow(dataItem) {
    var wnd = $("#resolver-move-window").data("kendoWindow");
    wnd.refresh().center().open();
    moveServiceComponentId = dataItem.ServiceComponentId;
}

function openMoveResolverLevelZeroWindow(dataItem) {
    var wnd = $("#resolver-move-level-zero-window").data("kendoWindow");
    wnd.refresh().center().open();
    moveResolverId = dataItem.Id;
    moveResolverDeskId = dataItem.ServiceDeskId;
}

function onMoveServiceComponentLevel1Refresh() {
    var form = $("#moveServiceComponentLevel1Form");
    var dropDownList = form.find("#DestinationServiceFunctionId").data("kendoDropDownList");
    if (dropDownList) {
        var currentDropDownListData = dropDownList.dataSource.data();
        if (currentDropDownListData) {
            for (var i = 0; i < currentDropDownListData.length; i++) {
                if (currentDropDownListData[i].Value == moveServiceComponentServiceFunctionId) {
                    dropDownList.dataSource.remove(currentDropDownListData[i]);
                    break;
                }
            }
        }
    }
    $("#moveServiceComponentLevel1").click(function (clk) {
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            form.find("#ServiceComponentId").val(moveServiceComponentId);
            $.post(form.attr("action"), form.serialize(), "json")
                .done(function () {
                    var wnd = $("#component-level1-move-window").data("kendoWindow");
                    wnd.close();
                    var grid = $("#ServiceComponentsGrid");
                    if (grid) {
                        var kendoGrid = grid.data("kendoGrid");
                        kendoGrid.dataSource.read();
                        kendoGrid.refresh();
                    }
                    stopSpinner();
                    displayNotification("Service Component moved successfully.", NotificationTypeNames.Success);
                })
                .fail(function (result) {
                    stopSpinner();
                    onErrorNoXhr(result);
                });
        } else {
            clk.preventDefault();
        }
    });
}

function onMoveServiceComponentLevel2Refresh() {
    var form = $("#moveServiceComponentLevel2Form");
    var dropDownList = form.find("#DestinationServiceComponentId").data("kendoDropDownList");
    if (dropDownList) {
        var currentDropDownListData = dropDownList.dataSource.data();
        if (currentDropDownListData) {
            for (var i = 0; i < currentDropDownListData.length; i++) {
                if (currentDropDownListData[i].Value == moveServiceComponentId) {
                    dropDownList.dataSource.remove(currentDropDownListData[i]);
                    break;
                }
            }
        }
    }
    $("#moveServiceComponentLevel2").click(function (clk) {
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            form.find("#ServiceComponentId").val(moveServiceComponentId);
            $.post(form.attr("action"), form.serialize(), "json")
                .done(function () {
                    var wnd = $("#component-level2-move-window").data("kendoWindow");
                    wnd.close();
                    var grid = $("#ServiceComponentsLevelTwoGrid");
                    if (grid) {
                        var kendoGrid = grid.data("kendoGrid");
                        kendoGrid.dataSource.read();
                        kendoGrid.refresh();
                    }
                    stopSpinner();
                    displayNotification("Service Component moved successfully.", NotificationTypeNames.Success);
                })
                .fail(function (result) {
                    stopSpinner();
                    onErrorNoXhr(result);
                });
        } else {
            clk.preventDefault();
        }
    });
}

function onMoveResolverRefresh() {
    var form = $("#moveResolverForm");
    var dropDownList = form.find("#DestinationServiceComponentId").data("kendoDropDownList");
    if (dropDownList) {
        var currentDropDownListData = dropDownList.dataSource.data();
        if (currentDropDownListData) {
            for (var i = 0; i < currentDropDownListData.length; i++) {
                if (currentDropDownListData[i].Value == moveServiceComponentServiceFunctionId) {
                    dropDownList.dataSource.remove(currentDropDownListData[i]);
                    break;
                }
            }
        }
    }
    $("#moveResolver").click(function (clk) {
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            form.find("#ServiceComponentId").val(moveServiceComponentId);
            $.post(form.attr("action"), form.serialize(), "json")
                .done(function () {
                    var wnd = $("#resolver-move-window").data("kendoWindow");
                    wnd.close();
                    var grid = $("#ResolverGrid");
                    if (grid) {
                        var kendoGrid = grid.data("kendoGrid");
                        kendoGrid.dataSource.read();
                        kendoGrid.refresh();
                    }
                    stopSpinner();
                    displayNotification("Resolver Group moved successfully.", NotificationTypeNames.Success);
                })
                .fail(function (result) {
                    stopSpinner();
                    onErrorNoXhr(result);
                });
        } else {
            clk.preventDefault();
        }
    });
}

function onMoveResolverLevelZeroRefresh() {
    var form = $("#moveResolverForm");
    var dropDownList = form.find("#DestinationDeskId").data("kendoDropDownList");
    if (dropDownList) {
        var currentDropDownListData = dropDownList.dataSource.data();
        if (currentDropDownListData) {
            for (var i = 0; i < currentDropDownListData.length; i++) {
                if (currentDropDownListData[i].Value == moveResolverDeskId) {
                    dropDownList.dataSource.remove(currentDropDownListData[i]);
                    break;
                }
            }
        }
    }
    $("#moveResolver").click(function (clk) {
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            form.find("#Id").val(moveResolverId);
            $.post(form.attr("action"), form.serialize(), "json")
                .done(function () {
                    var wnd = $("#resolver-move-level-zero-window").data("kendoWindow");
                    wnd.close();
                    var grid = $("#ResolverLevelZeroGrid");
                    if (grid) {
                        var kendoGrid = grid.data("kendoGrid");
                        kendoGrid.dataSource.read();
                        kendoGrid.refresh();
                    }
                    stopSpinner();
                    displayNotification("Resolver Group moved successfully.", NotificationTypeNames.Success);
                })
                .fail(function (result) {
                    stopSpinner();
                    onErrorNoXhr(result);
                });
        } else {
            clk.preventDefault();
        }
    });
}

function addServiceComponentClickHandlers() {
    // Add More Rows button.
    $("#addMoreRowsToolbar").click(function (e) {
        e.preventDefault();
        var grid = $("#AddServiceComponentsGrid").data("kendoGrid");
        repeat(function () { grid.addRow(); }, 4);
    });
}

function addServiceDeliveryUnitType(e) {
    var panel = $("#EditServiceComponentAddResolverPanel").data("kendoPanel");
    // Has it has come from the Edit Resolver Page
    if (panel !== null && panel !== undefined) {
        e.preventDefault();
    }

    var wnd = $("#service-delivery-unit-type-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#ServiceDeliveryUnitTypeName").focus();
    });

    wnd.refresh().center().open();
}

function addResolverGroupType(e) {
    var panel = $("#EditServiceComponentAddResolverPanel").kendoPanelBar().data("kendoPanelBar");
    // Has it has come from the Edit Resolver Page
    if (panel !== null && panel !== undefined) {
        e.preventDefault();
    }
    var wnd = $("#resolver-group-type-window").data("kendoWindow");
    wnd.bind('activate', function () {
        $("input#ResolverGroupTypeName").focus();
    });
    wnd.refresh().center().open();
}

function addResolverClickHandlers() {
    // Add More Rows button.
    $("#addMoreRowsToolbar").click(function (e) {
        e.preventDefault();
        var grid = $("#AddResolverGrid").data("kendoGrid");
        repeat(function () { grid.addRow(); }, 4);
    });

    // Add Service Delivery Unit Type button.
    $("#addServiceDeliveryUnitTypeToolbar").click(function (e) {
        e.preventDefault();
        addServiceDeliveryUnitType();
    });

    // Add Resolver Group Type button.
    $("#addResolverGroupTypeToolbar").click(function (e) {
        e.preventDefault();
        addResolverGroupType();
    });
}

function onServiceDeliveryUnitTypeRefresh() {
    $("#serviceDeliveryUnitTypeCreateButton").click(function (clk) {
        var form = $(this).closest("form");
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            var form = $(this).closest("form");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (newType) {

                    // Has it come from the Add Resolver page?
                    var addResolverGrid = $("#AddResolverGrid").data("kendoGrid");
                    if (addResolverGrid) {
                        var toolbar = addResolverGrid.wrapper.find(".k-grid-toolbar").html();

                        var gridColumns = addResolverGrid.columns;
                        var serviceDeliveryUnitTypeColumn = getKendoGridColumn(addResolverGrid, "ServiceDeliveryUnitTypeId");
                        var serviceDeliveryUnitTypeColumnIndex = getKendoGridIndex(addResolverGrid, "ServiceDeliveryUnitTypeId");
                        var existingServiceDeliveryUnitTypes = serviceDeliveryUnitTypeColumn.values;
                        existingServiceDeliveryUnitTypes.push({ text: newType.ServiceDeliveryUnitTypeName, value: newType.Id });
                        gridColumns[serviceDeliveryUnitTypeColumnIndex] = {
                            dataTextField: "Text",
                            dataValueField: "Value",
                            field: serviceDeliveryUnitTypeColumn.field,
                            width: serviceDeliveryUnitTypeColumn.width,
                            title: serviceDeliveryUnitTypeColumn.title,
                            filter: "contains",
                            values: existingServiceDeliveryUnitTypes,
                            editor: function (container, options) {
                                $("<input name='" + options.field + "'/>").appendTo(container)
                                    .kendoDropDownList({
                                        dataSource: existingServiceDeliveryUnitTypes,
                                        filter: "startswith",
                                        dataTextField: "text",
                                        optionLabel: "-- Please Select --",
                                        dataValueField: "value"
                                    });
                            }
                        };

                        addResolverGrid.setOptions({ columns: gridColumns });

                        addResolverGrid.wrapper.find(".k-grid-toolbar").html(toolbar);

                        // Set the value of the first row to be the newly created type
                        var data = addResolverGrid.dataSource.at(0);
                        data.set("ServiceDeliveryUnitTypeId", newType.Id);

                        // Need to re-attach the hander for the Add More Rows button.
                        addResolverClickHandlers();
                    } else {
                        // Has it come from the Edit Resolver page?
                        var grid = $("#AddResolverGrid").data("kendoGrid");
                        if (grid !== null && grid !== undefined) {
                            var addDropDownList = $("input#Resolver_ServiceDeliveryUnitTypeId").data("kendoDropDownList");
                            if (addDropDownList) {
                                addDropDownList.dataSource.add(newType);
                                addDropDownList.value(newType.Id);
                                addDropDownList.trigger("change");
                            }
                        } else {
                            // Has it has come from the Edit Resolver Page?
                            var panel = $("#EditServiceComponentAddResolverPanel").kendoPanelBar().data("kendoPanelBar");
                            if (panel !== null && panel !== undefined) {
                                var editDropDownList = $("input#ResolverServiceDeliveryUnit_ServiceDeliveryUnitTypeId").data("kendoDropDownList");
                                if (editDropDownList) {
                                    editDropDownList.dataSource.add(newType);
                                    editDropDownList.value(newType.Id);
                                    editDropDownList.trigger("change");
                                }
                            }
                        }
                    }

                    $("#service-delivery-unit-type-window").data("kendoWindow").close();
                },
                error: function (result) {
                    handleResponseTextError(result);
                }
            });
            stopSpinner();
        } else {
            clk.preventDefault();
        }
    });
}

function onResolverGroupTypeRefresh() {
    $("#resolverGroupTypeCreateButton").click(function (clk) {
        if ($("input#IsVisible").is(":checked")) {
            $("input#Visible").prop("checked", true);
        } else {
            $("input#Visible").prop("checked", false);
        }
        var form = $(this).closest("form");
        var validator = form.kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            startSpinner();
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (newType) {

                    // Has it come from the Add Resolver page?
                    var addResolverGrid = $("#AddResolverGrid").data("kendoGrid");
                    if (addResolverGrid) {
                        var toolbar = addResolverGrid.wrapper.find(".k-grid-toolbar").html();

                        var gridColumns = addResolverGrid.columns;
                        var resolverGroupTypeColumn = getKendoGridColumn(addResolverGrid, "ResolverGroupTypeId");
                        var resolverGroupTypeColumnIndex = getKendoGridIndex(addResolverGrid, "ResolverGroupTypeId");
                        var existingResolverGroupTypes = resolverGroupTypeColumn.values;
                        existingResolverGroupTypes.push({ text: newType.ResolverGroupTypeName, value: newType.Id });
                        gridColumns[resolverGroupTypeColumnIndex] = {
                            dataTextField: "Text",
                            dataValueField: "Value",
                            field: resolverGroupTypeColumn.field,
                            width: resolverGroupTypeColumn.width,
                            title: resolverGroupTypeColumn.title,
                            filter: "contains",
                            values: existingResolverGroupTypes,
                            editor: function (container, options) {
                                $("<input name='" + options.field + "'/>").appendTo(container)
                                    .kendoDropDownList({
                                        dataSource: existingResolverGroupTypes,
                                        filter: "startswith",
                                        dataTextField: "text",
                                        optionLabel: "-- Please Select --",
                                        dataValueField: "value"
                                    });
                            }
                        };

                        addResolverGrid.setOptions({ columns: gridColumns });

                        addResolverGrid.wrapper.find(".k-grid-toolbar").html(toolbar);

                        // Set the value of the first row to be the newly created type
                        var data = addResolverGrid.dataSource.at(0);
                        data.set("ResolverGroupTypeId", newType.Id);

                        // Need to re-attach the hander for the Add More Rows button.
                        addResolverClickHandlers();
                    } else {
                        // Has it come from the Add Resolver page?
                        var grid = $("#AddResolverGrid").data("kendoGrid");
                        if (grid !== null && grid !== undefined) {
                            var addDropDownList = $("input#Resolver_ResolverGroupTypeId").data("kendoDropDownList");
                            if (addDropDownList) {
                                addDropDownList.dataSource.add(newType);
                                addDropDownList.value(newType.Id);
                                addDropDownList.trigger("change");
                            }
                        } else {
                            // Has it has come from the Edit Resolver Page?
                            var panel = $("#EditServiceComponentAddResolverPanel").kendoPanelBar().data("kendoPanelBar");
                            if (panel !== null && panel !== undefined) {
                                var editDropDownList = $("input#ResolverGroup_ResolverGroupTypeId").data("kendoDropDownList");
                                if (editDropDownList) {
                                    editDropDownList.dataSource.add(newType);
                                    editDropDownList.value(newType.Id);
                                    editDropDownList.trigger("change");
                                }
                            }
                        }
                    }
                    $("#resolver-group-type-window").data("kendoWindow").close();
                },
                error: function (result) {
                    handleResponseTextError(result);
                }
            });
            stopSpinner();
        } else {
            clk.preventDefault();
        }
    });
}

function scaleSize(maxW, maxH, currW, currH) {

    var ratio = currH / currW;

    if (currW >= maxW && ratio <= 1) {
        currW = maxW;
        currH = currW * ratio;
    } else if (currH >= maxH) {
        currH = maxH;
        currW = currH / ratio;
    }

    return [currW, currH];
}

function addCustomerContributorsClickHandlers() {
    // Add More Rows button.
    $("#addMoreRowsToolbar").click(function (e) {
        e.preventDefault();
        var grid = $("#AddCustomerContributorGrid").data("kendoGrid");
        repeat(function () { grid.addRow(); }, 4);
    });
}

function removeGridHeightRestriction() {
    $('.k-grid-content-locked').css('height', '');
    $('.k-grid-content').css('height', '');
}


/*
* Menu Tree Functions
*/

function onSelectMenuItem(e) {
    var isExpanded = $(e.node).attr("data-expanded") === "true";
    if (isExpanded) {
        this.collapse(e.node);
    }
    else {
        this.expand(e.node);
    }
}

function highlightSelectedNode() {
    var selectedNode = $('.k-state-selected');
    hightlightNode(selectedNode);

    var parentNode = $(selectedNode).parent();
    var grandParentNode = $(parentNode).parent();
    hightlightNode(grandParentNode);
}

function hightlightNode(node) {
    $(node).css('background-color', 'white');
    $(node).css('border-color', 'transparent');
    $(node).css('color', '#3C3C35');
}

function onEdit(e) {
        var arg = e;
        arg.container.data('kendoWindow').bind('activate', function () {
            e.container.find("input:visible:first").focus();
        });
}

function onCreateOnly(e) {
    if (e.model.isNew()) {
        var arg = e;
        arg.container.data('kendoWindow').bind('activate', function () {
            e.container.find("input:visible:first").focus();
        });
    }
}

function onEditOnly(e) {
    if (!e.model.isNew()) {
        var arg = e;
        arg.container.data('kendoWindow').bind('activate', function () {
            e.container.find("input:visible:first").focus();
        });
    }
}

/*
 * Diagram functions
 */

function flatten(data, level) {
    var item;
    for (var i = 0; i < data.length; i++) {
        item = data[i];
        dataMap.push(item);
        item.flatIndex = dataMap.length - 1;
        item.itemLevel = level;
        if (item.Units && item.Units.length) {
            item.hasChildren = true;
            lastRoot = item;
            flatten(item.Units, level + 1);
        }
    }
}

// On click of the checkbox:
function selectRow() {

    // get checked value and row
    var checked = this.checked;
    var row = $(this).closest("tr");

    // add or removed the "selected" class
    if (checked) {
        //-select the row
        row.addClass("k-state-selected");
    } else {
        //-remove selection
        row.removeClass("k-state-selected");
    }
}

function wordwrap(str, intWidth, strBreak, cut) {
    // http://locutus.io/php/wordwrap/
    var m = ((arguments.length >= 2) ? arguments[1] : 75);
    var b = ((arguments.length >= 3) ? arguments[2] : '\r\n');
    var c = ((arguments.length >= 4) ? arguments[3] : false);

    var i, j, l, s, r;

    str += '';

    if (m < 1) {
        return str;
    }

    for (i = -1, l = (r = str.split(/\r\n|\n|\r/)).length; ++i < l; r[i] += s) {
        for (s = r[i], r[i] = '';
          s.length > m;
          r[i] += s.slice(0, j) + ((s = s.slice(j)).length ? b : '')) {
            j = c === 2 || (j = s.slice(0, m + 1).match(/\S*(\s)?$/))[1]
                ? m
                : j.input.length - j[0].length ||
                c === true && m ||
                j.input.length + (j = s.slice(m).match(/^\S*/))[0].length;
        }
    }

    return r.join('\r\n');
}

function dataURLtoBlob(dataurl) {
    var arr = dataurl.split(',');
    var mime = arr[0].match(/:(.*?);/)[1];
    var bstr = atob(arr[1]);
    var n = bstr.length;
    var u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new Blob([u8arr], { type: mime });
}

function onDiagram(diagramSelector) {
    var diagram = $(diagramSelector).getKendoDiagram();
    var bbox = diagram.boundingBox();
    var width = bbox.width + bbox.x + 50;
    var height = bbox.height + bbox.y + 100;

    diagram.wrapper.width(width);
    diagram.wrapper.height(height);

    diagram.resize();
}

function saveDiagram(url, dataUri, filename, level, notificationType) {
    var data = { dataUri: dataUri, filename: filename, level: level };

    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        dataType: 'json',
        success: function () {
            displayNotification("Diagram saved successfully.", notificationType);
        },
        error: function (result) {
            onErrorNoXhr(result);
        },
        complete: function() {
            stopSpinner();
        }
    });
}