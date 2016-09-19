var obDeliveryList;                    // ob: the delivery list in js memory
var obSelectedDelivery;                // ob: current selected delivery object in the ob delivery select box

var obTableObj;                        // ob: data table
var obTableSelectAllBoxObj;            // ob: items select all checkbox

var obDeliverySelectObj;                // ob: delivery select box
var obCarrierSelectObj;                 // ob: carrier select box
var obSrcStoreSelectObj;                // ob: source store select box 
var obTarStoreSelectObj;                // ob: target store select box 


var obCarrierIDObj;                    //ob: obCarrierID  input
var obSrcStoreIDObj;                   //ob: obSrcStoreID   input
var obSrcStoreNameObj;                 //ob: store name span
var obTarStoreIDObj;                   //ob: obTarStoreID  input
var obDeliveryTypeObj;                 //ob: deliveryType   input
var obVersionObj;                      //ob: obVersion   input

var obSubmitDeliveryBtnObj;            // ob: save draft delivery btn
var obProcessDeliveryBtnObj;           // ob: confirm the draft delivery btn
var obShipDeliveryBtnObj;              // ob: ship the processing delivery obbound btn
var obCancelDeliveryBtnObj;            // ob: cancel the processing delivery obbound btn
var obDeleteDeliveryBtnObj;            // ob: delete the processing delivery obbound btn
var obAddOutItemBtnObj;                //ob : add table item
var obDeleteOutItemBtnObj;             //ob: delete table item
var obShipmentBtnObj                   //ob: shipment button

var obQueryDelivery;                   // ob: the ob delivery query object
var obNewDelivery;                     // ob: the new ob delivery object

var obDeliverySelectValue              // ob: Delivery  select value

var obQueryStore = {                   // ob: search the stores belong to current user.clientID
    ClientID: currentEmployee.ClientID
}

var obSrcStoreID;           // come from param
var obSrcStoreName;         // come from param

// INTERFACE: call from main page to change ths source store.
// ex:  obCallSetSrcStore(1);
function obCallConfig() {
    obQueryDelivery = {              // the ob delivery query object
        SrcStoreID: -1,    // from main page defined
        ClientID: currentEmployee.ClientID,
        QueryStatus1: DeliveryStatusEnum.New,   // From New: New = 0, Submit = 1,  Processing = 2,BadOrder = 3,Cancel = 4,Delivering = 5,Received = 6, Error = 7, Success = 8  new=0,submit=1,processing=2,delivering=3 
        QueryStatus2: DeliveryStatusEnum.Delivering
    }
    obNewDelivery = {
        ID: 0,
        DON: Delivery_Label_NewDelivery,
        SrcStoreID: -1,
        TarStoreID: 1,
        Status: DeliveryStatusEnum.New,
        StatusLayout: Delivery_Label_Outbound_Select_New,
        Carrier: 1,
        ClientID: currentEmployee.ClientID,
        DeliveryItemList: new Array()
    }
}


// INTERFACE: call initial outbound form
function obCallInit() {
    obInitBtns();              // init all buttons status.
    obInitCarrierSelect();     // init carrier select
    obInitBothStoreSelect();  // init source and target store select
    obInitDeliverySelect();    // init delivery select
    obInitInput();//init  input 
}


// INTERFACE: call update the current outbound store
function obCallUpdateStore(obParamSrcStoreID, obParamSrcStoreName) {
    obSrcStoreID = obParamSrcStoreID;
    obSrcStoreName = obParamSrcStoreName;
    obSrcStoreNameObj = $("#obSrcStoreNameSpan")
    obSrcStoreNameObj.html(obSrcStoreName);
    obQueryDelivery.SrcStoreID = obSrcStoreID;
    obNewDelivery.SrcStoreID = obSrcStoreID;
    obInitDeliverySelect();
    obChangeSrcStoreSelect();
}


function obModalClose() {//close  obModal
    $("#outboundModal").modal("hide");
}

function obInitInput() {// init  input
    obDeliveryTypeObj = $("#obDeliveryType");
    obVersionObj = $("#obVersion");
    obCarrierIDObj = $("#obCarrierID");
    obSrcStoreIDObj = $("#obSrcStoreID");
    obTarStoreIDObj = $("#obTarStoreID");
}


// initial 5 post buttons
function obInitBtns() {
    obSubmitDeliveryBtnObj = $('#obSubmitDeliveryBtn');
    obProcessDeliveryBtnObj = $('#obProcessDeliveryBtn');
    obShipDeliveryBtnObj = $('#obShipDeliveryBtn');
    obCancelDeliveryBtnObj = $('#obCancelDeliveryBtn');
    obDeleteDeliveryBtnObj = $('#obDeleteDeliveryBtn');
    obShipmentBtnObj = $('#obShipmentBtn');
    obAddOutItemBtnObj = $('#obAddOutItemBtn');
    obDeleteOutItemBtnObj = $('#obDeleteOutItemBtn');


}

// get the target and source data in select box from server
function obInitBothStoreSelect() {
    obSrcStoreSelectObj = $('#obSrcStoreSelect');
    obTarStoreSelectObj = $('#obTarStoreSelect');

    var url = './Store/GetStoreDataTableJson';
    var reqData = { queryStore: obQueryStore };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        obSrcStoreSelectObj.empty();
        obTarStoreSelectObj.empty();
        $.each(result.data, function (i, item) {
            $("<option></option>").val(item.ID).text(item.Name).appendTo(obSrcStoreSelectObj);
            $("<option></option>").val(item.ID).text(item.Name).appendTo(obTarStoreSelectObj);
        });
      
    }
    $('#obSrcStoreSelect option:eq(0)').attr('selected', 'selected');
    $('#obTarStoreSelect option:eq(1)').attr('selected', 'selected');

    // set the change method
    obSrcStoreSelectObj.change(function () {//当发出仓改变的时候 目的仓不能出现相同的仓库
        obSelectedDelivery.SrcStoreID = obSrcStoreSelectObj.val();
        $("#obTarStoreSelect option").css("display", "");
    
        obSrcStoreIDObj.val(obSrcStoreSelectObj.val());
        obCompareStore();
    })
    obTarStoreSelectObj.change(function () {
        obSelectedDelivery.TarStoreID = obTarStoreSelectObj.val();
        obTarStoreIDObj.val(obTarStoreSelectObj.val());
    })
}


function obCompareStore() {
    obSrcStoreSelectObj = $("#obSrcStoreSelect");
    obTarStoreSelectObj = $("#obTarStoreSelect");

    //比较发出仓和目的仓是否一致
    if (obSrcStoreSelectObj[0].options[0].selected == true) {//发出仓第一项被选中
        if (obSrcStoreSelectObj.val() == obTarStoreSelectObj.val()) {//发出仓与目的仓的参数相同
            obTarStoreSelectObj[0].options[1].selected = true;
        } else {
            obTarStoreSelectObj[0].options[0].selected = true;
        }
       
    } else {
        if (obSrcStoreSelectObj.val() == obTarStoreSelectObj.val()) {//发出仓与目的仓的参数相同
            obTarStoreSelectObj[0].options[0].selected = true;
        } else {
            obTarStoreSelectObj[0].options[0].selected = true;
        }
    }
    $("#obTarStoreSelect option[value='" + obSrcStoreSelectObj.val() + "']").css("display", "none");
    obTarStoreIDObj.val($("#obTarStoreSelect  option:selected").val());

}


//change the srcStore value
// 选择左侧仓库时 筛选发货仓   非所有时只显示单个仓库
function obChangeSrcStoreSelect() {
    obSrcStoreSelectObj = $('#obSrcStoreSelect');
    var url = './Store/GetStoreDataTableJson';
    var reqData = { queryStore: obQueryStore };
    jsonPost(url, reqData, callback, true, false);
    function callback(result) {
        obSrcStoreSelectObj.empty();
        obTarStoreSelectObj.empty();
        $.each(result.data, function (i, item) {
            if (obSrcStoreID <= 0 && obDeliverySelectValue == 0) {//当选择所有仓库并且是新增调拨单的时候  发出仓显示所有仓库
                $("<option></option>").val(item.ID).text(item.Name).appendTo(obSrcStoreSelectObj);
                $("<option></option>").val(item.ID).text(item.Name).appendTo(obTarStoreSelectObj);

            } else {
                if (obSrcStoreID != 0 && obSrcStoreID == item.ID) {//如果选择了某个仓库名称,发出仓只显示该仓库
                    $("<option></option>").val(item.ID).text(item.Name).appendTo(obSrcStoreSelectObj);
                }
                if (item.ID != obSrcStoreSelectObj.val()) {
                    $("<option></option>").val(item.ID).text(item.Name).appendTo(obTarStoreSelectObj);
                }
            }
        });
        obCompareStore();
    }
}

// get the carriers in select box from server
function obInitCarrierSelect() {
    obCarrierSelectObj = $('#obCarrierSelect');
    var url = './Carrier/GetCarrierDataTableJson';
    var reqData = null;
    jsonPost(url, reqData, callback, true, false);
    function callback(result) {
        obCarrierSelectObj.empty();
        $.each(result.data, function (i, item) {
            $("<option></option>").val(item.ID).text(item.Name).appendTo(obCarrierSelectObj);
        });
    }

    // set the change method
    obCarrierSelectObj.change(function () {
        obSelectedDelivery.Carrier = obCarrierSelectObj.val();
        obCarrierIDObj.val(obCarrierSelectObj.val());//赋值给input
    })

}

// get the delivery in select box from server
function obInitDeliverySelect(deliveryID) {

    obDeliverySelectObj = $('#obDeliverySelect');

    var url = './Delivery/GetDeliveryDataTableJson';
    var reqData = { queryDelivery: obQueryDelivery };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        obDeliverySelectObj.empty();
        result.data.unshift(obGetNewDelivery()); // add the new delivery option into the top of the data array~
        $.each(result.data, function (i, item) {
            var deliveryTypeLayout = item.DeliveryTypeLayout;
            var text = "";

            if (deliveryTypeLayout != undefined && item.DeliveryType != 0) {
                text = item.DON + " " + deliveryTypeLayout + " " + item.StatusLayout;
            } else {
                text = item.DON + " " + item.StatusLayout;
            }

            var value = item.ID;
            $("<option></option>").val(value).text(text).appendTo(obDeliverySelectObj);
        });
        obDeliveryList = result.data;
        if ((deliveryID != undefined) && (deliveryID > 0)) {
            obDeliverySelectObj.val(deliveryID); // select by the return id

        } else {
            obDeliverySelectObj.val(0); // select the new
            obSelectedDelivery = obDeliveryList[0];
        }
        obInitTable();
        obDeliverySelectObj.trigger("change");
    }

    obDeliverySelectObj.off('change');
    // set the change method
    obDeliverySelectObj.change(function () {//状态改变的时候

        var selectedIndex = $(this).children('option:selected').index();
        obDeliverySelectValue = $(this).children('option:selected').val();

        obSelectedDelivery = obDeliveryList[selectedIndex];       // update memory ob delivery
        showTarStore();//show TarStore or not

        if (obDeliverySelectValue == 0) {// 新增的时候
            obVersionObj.val(0);//版本号默认为0
            obDeliveryTypeObj.val(DeliveryTypeEnum.Transfer);//调拨

        } else {//非新增时         
            obDeliveryTypeObj.val(obSelectedDelivery.DeliveryType);// update target select DeliveryType
            obSrcStoreSelectObj.val(obSelectedDelivery.SrcStoreID);
            obVersionObj.val(obSelectedDelivery.Version);//版本号
            // update source select

            obTarStoreSelectObj.val(obSelectedDelivery.TarStoreID);

        }

        obChangeCarrierSelect(obSelectedDelivery.CarrierID);//根据条件处理运输选择框的内容
        //把相关的值放到INPUT中

        obCarrierIDObj.val(obCarrierSelectObj.val());
        obSrcStoreIDObj.val(obSrcStoreSelectObj.val());
        obTarStoreIDObj.val(obTarStoreSelectObj.val());


        obTableObj.clear().draw(); // clear the table data first

        if (obSelectedDelivery.DeliveryItemList != undefined) { // readd the item list data to table
            obTableObj.rows.add(obSelectedDelivery.DeliveryItemList).draw();
        }

        if (obSelectedDelivery.DeliveryType == DeliveryTypeEnum.Transfer && obDeliverySelectValue != 0) {
            obAddOutItemBtnObj.fadeOut();
            obDeleteOutItemBtnObj.fadeOut();

        } else {
            cancelDisableObSelected();
            obAddOutItemBtnObj.fadeIn();
            obDeleteOutItemBtnObj.fadeIn();
        }

        // handle buttons
        obSubmitDeliveryBtnObj.fadeOut();
        obProcessDeliveryBtnObj.fadeOut();
        obShipDeliveryBtnObj.fadeOut();
        obCancelDeliveryBtnObj.fadeOut();
        obDeleteDeliveryBtnObj.fadeOut();
        obShipmentBtnObj.fadeOut();

        obTarStoreSelectHandle();

        if (obSelectedDelivery.Status == DeliveryStatusEnum.New) {  // new
            obSubmitDeliveryBtnObj.fadeIn(); //can submit

        }
        if (obSelectedDelivery.Status == DeliveryStatusEnum.Submit) {  // submited
            cancelDisableObSelected();
            obSubmitDeliveryBtnObj.fadeIn();   // can submit
            obProcessDeliveryBtnObj.fadeIn();  // can process
            obDeleteDeliveryBtnObj.fadeIn();   // can delete
        }
        if (obSelectedDelivery.Status == DeliveryStatusEnum.Processing) {  // processing
            if (obSelectedDelivery.Shipment != undefined) { // 
                obShipmentBtnObj.fadeIn()
            }
            obShipDeliveryBtnObj.fadeIn();     // can ship
            obCancelDeliveryBtnObj.fadeIn();   // can cancel
            obAddOutItemBtnObj.fadeOut();
            obDeleteOutItemBtnObj.fadeOut();
            disableObSelected();// can't change 
        }
        if (obSelectedDelivery.Status == DeliveryStatusEnum.BadOrder) {  // BadOrder

        }
        if (obSelectedDelivery.Status == DeliveryStatusEnum.Cancel) {  // Cancel

        }
        if (obSelectedDelivery.Status == DeliveryStatusEnum.Delivering) {  // Delivering
            disabledObOutQtyObj();
            disableObSelected();// can't change       
        }

    })
}


// 根据条件处理运输选择框的内容
function obChangeCarrierSelect(id) {

    var url = './Carrier/GetCarrierDataTableJson';
    var reqData = null;

    jsonPost(url, reqData, callback, true, false);
    function callback(result) {
        obCarrierSelectObj.empty();
        $.each(result.data, function (i, item) {
            if (obSelectedDelivery.DeliveryType != DeliveryTypeEnum.SellingOut) {//除非销售出库 否则不显示CanTransfer为false 
                if (item.CanTransfer != false) {
                    $("<option></option>").val(item.ID).text(item.Name).appendTo(obCarrierSelectObj);
                }
            } else {
                $("<option></option>").val(item.ID).text(item.Name).appendTo(obCarrierSelectObj);
            }
        });
        if (id != undefined) {
            $("#obCarrierSelect option[value='" + id + "']").attr("selected", "selected");
            obCarrierIDObj.val(id);       // update carrier select
        } else {
            $("#obCarrierSelect option:first").prop("selected", 'selected');
            obCarrierIDObj.val($("#obCarrierSelect  option:selected").val());       // update carrier select
        }

    }

    // set the change method
    obCarrierSelectObj.change(function () {
        obSelectedDelivery.Carrier = obCarrierSelectObj.val();
        obCarrierIDObj.val(obCarrierSelectObj.val());//赋值给input
    })

}



// deal with different deliveryType value
function obTarStoreSelectHandle() {
    var obDeliveryTypeVal = obDeliveryTypeObj.val(); //deliveryType value
    if (obDeliveryTypeVal == DeliveryTypeEnum.SellingOut) {// SellingOut
        $("#obTarStoreSelect option").each(function () {//when  deliveryType is  sellingOut  only  show  selling
            $(this).css("display", "block");

            if ($(this).val() != "Null" || $(this).val() == obSrcStoreIDObj.val()) {
                $(this).css("display", "none");
            }
        });
        obTarStoreIDObj.val($("#obTarStoreSelect  option:selected").val());       // update carrier select
    } else {//when  deliveryType is not  sellingOut   don't show  selling item
        $("#obTarStoreSelect option").each(function () {
            $(this).css("display", "block");
            if ($(this).val() == "Null" || obSrcStoreSelectObj.val() == $(this).val()) {
                $(this).css("display", "none");
            }
            if ($("#obTarStoreSelect  option:eq(0)").val() == obSrcStoreSelectObj.val()) {
                $("#obTarStoreSelect  option:eq(1)").attr("selected", "selected");
            } else {
                $("#obTarStoreSelect  option:eq(0)").attr("selected", "selected");
            }
            obTarStoreIDObj.val($("#obTarStoreSelect  option:selected").val());
        });


    }
}

function showTarStore() {
    if (obSelectedDelivery.DeliveryType == DeliveryTypeEnum.SellingOut) {//销出 
        $("#tarStoreLbl").hide();
        $("#tarStoreDiv").hide();
    } else {
        $("#tarStoreLbl").show();
        $("#tarStoreDiv").show();
    }
}


function disableObSelected() {
    obCarrierSelectObj.attr("disabled", "disabled");
    obSrcStoreSelectObj.attr("disabled", "disabled");
    obTarStoreSelectObj.attr("disabled", "disabled");
}

function cancelDisableObSelected() {
    obCarrierSelectObj.removeAttr("disabled");
    obSrcStoreSelectObj.removeAttr("disabled");
    obTarStoreSelectObj.removeAttr("disabled");
}

// new delivery as an default option
function obGetNewDelivery() {
    return obNewDelivery;
}

//禁用 dataTables 所有 OutQty
function disabledObOutQtyObj() {
    var obDefaultTable = $('#obTable').dataTable();
    var obTrs = obDefaultTable.fnGetData();

    if (obTrs.length > 0) {
        for (var j = 0; j < obTrs.length; j++) {
            var outQtyObjID = "obTableRowOutQty" + obTrs[j].ProductID;
            var outQtyObj = $("#" + outQtyObjID);
            outQtyObj.attr("disabled", "disabled");
        }
    }
}

// when the ob qty input updated
function obUpdateQty(qtyInputObj) {
    var trObj = qtyInputObj.parents('tr');
    obTableObj.row(trObj).data().OutQty = qtyInputObj.context.value;
    //obShowData();
}

// delete the selected ob items
function obDeleteItems() {
    //obShowData();
    // Iterate over all checkboxes in the table
    obTableObj.$('input[type="checkbox"]').each(function () {
        // If checkbox doesn't exist in DOM
        if (this.checked) {
            var trObj = $(this).parents('tr');
            obTableObj.row(trObj).remove().draw(true);
        }
    })
}

// test: add new ob items
function obAddItems() {
    obModalClose();
    $.toaster(Delivery_Label_Outbound_AddMsg, Common_Label_Info, 'info');
}

function obCallAddItems(obParamInvList) {
    $('.dataTables_scrollHeadInner').attr("style", "width:1108px!important");
    $('.dataTable').attr("style", "width:1108pximportant");

    if (obParamInvList == undefined || obParamInvList.length < 1) {// no data select 
        return;
    }

    // get outbound modal table  data
    var obDefTable = new Array();
    var oTableLocal = $('#obTable').dataTable();
    var obTrs = oTableLocal.fnGetData();

    if (obTrs.length > 0) {
        for (var i = 0; i < obTrs.length; i++) {
            obDefTable.push(obTrs[i]);
        }

        //compare obDefTable and  obParamInvList
        for (var j = 0; j < obDefTable.length; j++) {
            for (var k = 0; k < obParamInvList.length; k++) {
                if (obDefTable[j].Product.ID == obParamInvList[k].ID) {
                    obParamInvList.splice(k, 1);
                }
            }
        }
    }
    //add to obTable
    for (var i = 0; i < obParamInvList.length; i++) {
        var a = obGenOutItems(obParamInvList[i].ID, obParamInvList[i].SKU, obParamInvList[i].ShortTitle, 1);
        obSelectedDelivery.DeliveryItemList.unshift(a)
        obTableObj.row.add(a).draw(true);

        var ckBoxObjID = "obTableRowBox" + obParamInvList[i].ID;
        var ckBoxObj = $("#" + ckBoxObjID);
        ckBoxObj.closest("tr").css("color", "#33CCFF");
    }

    $('.dataTables_scrollHeadInner').attr("style", "width:1108px!important");
    $('.dataTable').attr("style", "width:1108pximportant");
}

// test: generate test obbound items.
function obGenOutItems(productID, productSKU, productShortTitle, obQty) {
    var deliveryItem = {
        "Product": {
            "ID": productID,
            "ShortTitle": productShortTitle,
            "SKU": productSKU
        },
        "DeliveryID": obSelectedDelivery.ID,
        "ProductID": productID,
        "InQty": 0,
        "OutQty": obQty,
        "UpdateEmployeeID": currentEmployee.ID
    }
    return deliveryItem;
}


// prepare the post data before submit
function obPrepareData() {
    obSelectedDelivery = getStructObject("obForm"); // reget from current form
    obSelectedDelivery.DeliveryItemList = obTableObj.rows().data().toArray();
}

// post data and save the obbound delivery draft~
function obSubmitDelivery() {
    obSubmitDeliveryBtnObj.attr("disabled", "disabled");
    obPrepareData();
    var url = './Delivery/OutboundSubmitDeliveryJson';
    var reqData = { submitDelivery: obSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        obSubmitDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;

        obInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
    }
}

// confirm obbound the selected delivery
function obProcessDelivery() {
    obProcessDeliveryBtnObj.attr("disabled", "disabled");
    obPrepareData();

    var url = './Delivery/OutboundProcessDeliveryJson';
    var reqData = { submitDelivery: obSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        obProcessDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        obInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
        invTableObj.draw();                     // reload the main page inventory table to refresh inv counts~
    }
}


// ship obbound the selected delivery
function obShipDelivery() {
    obShipDeliveryBtnObj.attr("disabled", "disabled");
    obPrepareData();

    var url = './Delivery/OutboundShipDeliveryJson';
    var reqData = { submitDelivery: obSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        obShipDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        obInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
        ibInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
        invTableObj.draw(); // reload the main page inventory table to refresh inv counts~
    }
}


// cancel obbound the selected delivery
function obCancelDelivery() {
    obCancelDeliveryBtnObj.attr("disabled", "disabled");
    obPrepareData();

    var url = './Delivery/OutboundCancelDeliveryJson';
    var reqData = { submitDelivery: obSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        obCancelDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        obInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
        invTableObj.draw(); // reload the main page inventory table to refresh inv counts~
    }
}

// delete obbound the selected delivery
function obDeleteDelivery() {
    obDeleteDeliveryBtnObj.attr("disabled", "disabled");
    obPrepareData();

    var url = './Delivery/OutboundDeleteDeliveryJson';
    var reqData = { submitDelivery: obSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        obDeleteDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        obInitDeliverySelect(); // reload the sub page delivery select box.
        invTableObj.draw(); // reload the main page inventory table to refresh inv counts~
    }
}


// ob shipment button click
function obShipment() {
    //log(obSelectedDelivery)
    if (obSelectedDelivery.Carrier.Type == CarrierTypeEnum.EPACKET) {
        ekCallConfig(obSelectedDelivery);
        ekCallInit();
        $("#epacketModal").modal("show");
    }

}

// test: debug and show data in js console
function obShowData() {
    obPrepareData();
    debug(obSelectedDelivery);
}

// init obbound datatable
function obInitTable() {
    if (obTableObj != undefined) {
        return;
    }

    obTableSelectAllBoxObj = $('#obTableSelectAllBox'); // select all box

    obTableObj = $('#obTable').DataTable({
        data: obSelectedDelivery.DeliveryItemList,
        "bRetrieve": true,
        "language": dtLanguage,
        "columns": [
           { "data": "Product.ID" },
           { "data": "Product.SKU" },
           { "data": "Product.ShortTitle" },
           { "data": "OutQty" }
        ],
        "columnDefs": [
                {
                    "render": function (data, type, row) {
                        return '<input name="Product.ID" value="' + row.Product.ID + '" type="checkbox"  id="obTableRowBox' + row.Product.ID + '"/>';
                    }, "targets": 0, 'searchable': false, 'orderable': false
                }, {
                    "render": function (data, type, row) {
                        //debug(this.parent)
                        var str = '<input  value="' + row.OutQty + '" type="number" class="form-control" min="1" max="999" style="width:100px!important;" onchange="obUpdateQty($(this))"  id="obTableRowOutQty' + row.Product.ID + '"/>';
                        return str
                    }, "targets": 3
                }
        ],

        "dom": 'tip',
        "scrollY": 200,
        "order": [0, "asc"]

    });
    // Handle click on "Select all" control
    obTableSelectAllBoxObj.on('click', function () {
        var rows = obTableObj.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
    });

    // Handle click on checkbox to set state of "Select all" control
    $('#obTable').find('tbody').on('change', 'input[type="checkbox"]', function () {
        if (!this.checked) { // If checkbox is not checked
            var el = obTableSelectAllBoxObj.get(0);
            if (el && el.checked && ('indeterminate' in el)) {
                el.indeterminate = true;
            }
        }
    });

}





