var ibDeliveryList;                    // ib: the delivery list in js memory
var ibSelectedDelivery;                // ib: current selected delivery object in the ib delivery select box

var ibTableObj;                        // ib: data table
var ibTableSelectAllBoxObj;            // ib: items select all checkbox

var ibDeliverySelectObj;               // ib: delivery select box
var ibCarrierSelectObj;                // ib: carrier select box
var ibSrcStoreSelectObj;               // ib: source store select box 
var ibTarStoreSelectObj;               // ib: target store select box 

var ibSubmitDeliveryBtnObj;            // ib: save draft delivery btn
var ibDeleteDeliveryBtnObj;            // ib: delete the processing delivery obbound btn
var ibReceiveDeliveryBtnObj;           // ib: confirm the draft receive btn when no srcStore
var ibFinishDeliveryBtnObj;            // ib: finish
var ibAddItemBtnObj;                 // ib:add dataTables items
var ibDeleteItemBtnObj;              // ib:delete dataTables items

var ibQueryDelivery;                   // ib: the ib delivery query object
var ibNewDelivery;                     // ib: the new ib delivery object

var ibCarrierIDObj;                    //ib: obCarrierID   input
var ibSrcStoreIDObj;                   //ib: obSrcStoreID  input
var ibTarStoreIDObj;                   //ib: obTarStoreID  input
var ibTarStoreNameObj;                 //ib: obTarStoreName  span
var ibDeliveryTypeObj;                 //ib: deliveryType   input
var ibVersionObj;                      //ib: obVersion   input

var ibDeliveryStatus;               //ib: DeliveryStatus
var ibDeliverySelectValue      // ib: select value

var ibQueryStore = {                   // ib: search the stores belong to current user.clientID
    ClientID: currentEmployee.ClientID
}

var ibSrcStoreID;
var ibTarStoreID;  // come from main page source selection.
var ibTarStoreName;         // come from param

var srcStore = "";

// INTERFACE: call from main page to change ths source store.
// ex:  ibCallSetTarStore(1);
function ibCallConfig() {
    ibQueryDelivery = {              // the ib delivery query object
        TarStoreID: -1,    // from main page defined
        ClientID: currentEmployee.ClientID,
        QueryStatus1: DeliveryStatusEnum.Delivering,   // From New: New = 0, Submit = 1,  Processing = 2,BadOrder = 3,Cancel = 4,Delivering = 5,Received = 6, Error = 7, Success = 8 
        QueryStatus2: DeliveryStatusEnum.Success
    }
    ibNewDelivery = {
        ID: 0,
        DON: Delivery_Label_NewDelivery,
        SrcStoreID: 1,
        TarStoreID: -1,
        Status: DeliveryStatusEnum.New,
        StatusLayout: Delivery_Label_NewPurchaseInDelivery,
        Carrier: 1,
        ClientID: currentEmployee.ClientID,
        DeliveryItemList: new Array()
    }
   
}

// INTERFACE: call initial inbound form
function ibCallInit() {
    ibInitBtns();              // init all buttons status.
    ibInitCarrierSelect();     // init carrier select
    ibInitDeliverySelect();    // init delivery select
    ibInitBothStoreSelect();  // init source and target store select
    ibInitInput();
}

// INTERFACE: call update the current inbound store
function ibCallUpdateStore(ibParamTarStoreID, ibParamTarStoreName) {
    ibTarStoreID = ibParamTarStoreID;
    ibTarStoreName = ibParamTarStoreName;
    ibTarStoreNameObj = $("#ibTarStoreNameSpan")
    ibTarStoreNameObj.html(ibTarStoreName);

    ibQueryDelivery.TarStoreID = ibTarStoreID;
    ibNewDelivery.TarStoreID = ibTarStoreID;
    ibInitDeliverySelect();
    ibChangeSrcStoreSelect();
}


// INTERFACE: call from main page to change this datatables data.
// ex:  ibCallAddItems(ibParamInvList);
function ibCallAddItems(ibParamInvList) {

    $('.dataTables_scrollHeadInner').attr("style", "width:1108px!important");
    $('.dataTable').attr("style", "width:1108pximportant");
    if (ibParamInvList.length < 1) {// no data select 
        return;
    }

    if (ibDeliveryStatus == DeliveryStatusEnum.Received) {//status is Received
        return;
    }

    // get outbound modal table  data
    var ibDefTable = new Array();
    var ibDefaultTable = $('#ibTable').dataTable();
    var ibTrs = ibDefaultTable.fnGetData();

    if (ibTrs.length > 0) {
        for (var i = 0; i < ibTrs.length; i++) {
            ibDefTable.push(ibTrs[i]);
        }

        //compare ibDefTable and  ibParamInvList
        for (var j = 0; j < ibDefTable.length; j++) {
            for (var k = 0; k < ibParamInvList.length; k++) {
                if (ibDefTable[j].Product.ID == ibParamInvList[k].ID) {
                    ibParamInvList.splice(k, 1);
                }
            }
        }
    }
    //add to ibTable
    for (var i = 0; i < ibParamInvList.length; i++) {
        var a = ibGenOutItems(ibParamInvList[i].ID, ibParamInvList[i].SKU, ibParamInvList[i].ShortTitle, 1);
        ibSelectedDelivery.DeliveryItemList.unshift(a)
        ibTableObj.row.add(a).draw(true);

        var ckBoxObjID = "ibTableRowBox" + ibParamInvList[i].ID;
        var ckBoxObj = $("#" + ckBoxObjID);
        ckBoxObj.closest("tr").css("color", "#33CCFF");
        var inQtyObjID = "ibTableRowInQty" + ibParamInvList[i].ID;
        var outQtyObjID = "ibTableRowOutQty" + ibParamInvList[i].ID;
        var inQtyObj = $("#" + inQtyObjID);
        var outQtyObj = $("#" + outQtyObjID);
        inQtyObj.val(outQtyObj.val());
    }

    $('.dataTables_scrollHeadInner').attr("style", "width:1108px!important");
    $('.dataTable').attr("style", "width:1108pximportant");
}

//change the srcStore value
function ibChangeSrcStoreSelect() {
    ibSrcStoreSelectObj = $('#ibSrcStoreSelect');

    var url = './Store/GetStoreDataTableJson';
    var reqData = { queryStore: obQueryStore };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        ibSrcStoreSelectObj.empty();
        $.each(result.data, function (i, item) {
            if (ibSrcStoreID == -1 || ibDeliverySelectValue == 1 || ibDeliverySelectValue == 0) {
                $("<option></option>").val(item.ID).text(item.Name).appendTo(ibSrcStoreSelectObj);
            } else {
                if (ibSrcStoreID == item.ID) {
                    $("<option></option>").val(item.ID).text(item.Name).appendTo(ibSrcStoreSelectObj);
                }
            }
        });
        $("<option></option>").val("Null").text(Delivery_Label_PurchaseInNoSource).appendTo(ibSrcStoreSelectObj);
    }

}





function ibModalClose() {//close  ibModal
    $("#inboundModal").modal("hide");
}

function ibInitInput() {// init  input
    ibDeliveryTypeObj = $("#ibDeliveryType");
    ibVersionObj = $("#ibVersion");
    ibCarrierIDObj = $("#ibCarrierID");
    ibSrcStoreIDObj = $("#ibSrcStoreID");
    ibTarStoreIDObj = $("#ibTarStoreID");
}


// initial 5 post buttons
function ibInitBtns() {
    ibSubmitDeliveryBtnObj = $('#ibSubmitDeliveryBtn');
    ibReceiveDeliveryBtnObj = $('#ibReceiveDeliveryBtn');
    ibFinishDeliveryBtnObj = $("#ibFinishDeliveryBtn");
    ibDeleteDeliveryBtnObj = $("#ibDeleteDeliveryBtn");
    ibAddItemBtnObj = $("#ibAddItemBtn");
    ibDeleteItemBtnObj = $("#ibDeleteItemBtn");

}

// get the target and source data in select box from server
function ibInitBothStoreSelect() {
    ibSrcStoreSelectObj = $('#ibSrcStoreSelect');
    ibTarStoreSelectObj = $('#ibTarStoreSelect');

    var url = './Store/GetStoreDataTableJson';
    var reqData = { queryStore: ibQueryStore };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        ibSrcStoreSelectObj.empty();
        ibTarStoreSelectObj.empty();
        $.each(result.data, function (i, item) {
            $("<option></option>").val(item.ID).text(item.Name).appendTo(ibSrcStoreSelectObj);
            $("<option></option>").val(item.ID).text(item.Name).appendTo(ibTarStoreSelectObj);
        });
    }

    // set the change method  SrcStore
    ibSrcStoreSelectObj.change(function () {
        $("#ibTarStoreSelect option").css("display", "");
        ibSelectedDelivery.SrcStoreID = ibSrcStoreSelectObj.val();
        $("#ibTarStoreSelect").val("");
        $("#ibTarStoreSelect option[value='" + ibSrcStoreSelectObj.val() + "']").css("display", "none");
        ibSrcStoreIDObj.val(ibSrcStoreSelectObj.val());
        disabledFinishBtn();
    });
    //set the chage method TarStore
    ibTarStoreSelectObj.change(function () {
        $("#ibSrcStoreSelect option").css("display", "");
        ibSelectedDelivery.TarStoreID = ibTarStoreSelectObj.val();
        $("#ibSrcStoreSelect").val("");
        $("#ibSrcStoreSelect option[value='" + ibTarStoreSelectObj.val() + "']").css("display", "none");
        ibTarStoreIDObj.val(ibTarStoreSelectObj.val());
        disabledFinishBtn();
    });
}

// get the carriers in select box from server
function ibInitCarrierSelect() {
    ibCarrierSelectObj = $('#ibCarrierSelect');

    var url = './Carrier/GetCarrierDataTableJson';
    var reqData = null;
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        ibCarrierSelectObj.empty();
        $.each(result.data, function (i, item) {
            $("<option></option>").val(item.ID).text(item.Name).appendTo(ibCarrierSelectObj);
        });
    }
    // set the change method
    ibCarrierSelectObj.change(function () {
        ibSelectedDelivery.Carrier = ibCarrierSelectObj.val();
        ibCarrierIDObj.val(ibCarrierSelectObj.val());
    });

}

// get the delivery in select box from server
function ibInitDeliverySelect(deliveryID) {

    ibDeliverySelectObj = $('#ibDeliverySelect');

    var url = './Delivery/GetDeliveryDataTableJson';
    var reqData = { queryDelivery: ibQueryDelivery };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        ibDeliverySelectObj.empty();
        result.data.unshift(ibGetNewDelivery()); // add the new delivery option into the top of the data array~
        $.each(result.data, function (i, item) {
            var deliveryTypeLayout = item.DeliveryTypeLayout;
            if (deliveryTypeLayout != undefined && item.DeliveryType != 0) {
                var text = item.DON + " " + deliveryTypeLayout + " " + item.StatusLayout;
            } else {
                var text = item.DON + " " + item.StatusLayout;
            }
            var value = item.ID;
            $("<option></option>").val(value).text(text).appendTo(ibDeliverySelectObj);
        });
        ibDeliveryList = result.data;
        if ((deliveryID != undefined) && (deliveryID > 0)) {
            ibDeliverySelectObj.val(deliveryID); // select by the return id

        } else {
            ibDeliverySelectObj.val(0); // select the new
            ibSelectedDelivery = ibDeliveryList[0];
        }
        ibInitTable();

        ibDeliverySelectObj.trigger("change");
    }

    ibDeliverySelectObj.off('change');
    // set the change method
    ibDeliverySelectObj.change(function () {

        var selectedIndex = $(this).children('option:selected').index();
        ibDeliverySelectValue = $(this).children('option:selected').val();


        ibSelectedDelivery = ibDeliveryList[selectedIndex];       // update memory ib delivery

        if (ibDeliverySelectValue == 0) {//新建入库单
            ibCarrierSelectObj.val(1);       // update carrier select
            ibSrcStoreSelectObj.val(null);
            ibTarStoreSelectObj.val(1);
        } else {
            ibCarrierSelectObj.val(ibSelectedDelivery.CarrierID);       // update carrier select
            ibSrcStoreSelectObj.val(ibSelectedDelivery.SrcStoreID);   // update source select
            ibTarStoreSelectObj.val(ibSelectedDelivery.TarStoreID);   // update target select
        }

        ibDeliveryTypeObj.val(ibSelectedDelivery.DeliveryType);
        ibVersionObj.val(ibSelectedDelivery.Version);
        ibCarrierIDObj.val(ibSelectedDelivery.CarrierID);
        ibSrcStoreIDObj.val(ibSelectedDelivery.SrcStoreID);
        ibTarStoreIDObj.val(ibSelectedDelivery.TarStoreID);

        srcStore = $("#ibSrcStoreSelect").children('option:selected').text();

        ibTableObj.clear().draw(); // clear the table data first

        if (ibSelectedDelivery.DeliveryItemList != undefined) { // readd the item list data to table
            ibTableObj.rows.add(ibSelectedDelivery.DeliveryItemList).draw();
        }


        // handle buttons
        ibSubmitDeliveryBtnObj.fadeOut();
        ibReceiveDeliveryBtnObj.fadeOut();
        ibDeleteDeliveryBtnObj.fadeOut();
        ibFinishDeliveryBtnObj.fadeOut();
        ibAddItemBtnObj.fadeOut();
        cancelDisableIbSelected();

        ibDeliveryStatus = ibSelectedDelivery.Status;
        //log(ibSelectedDelivery.Status)
        if (ibSelectedDelivery.Status == DeliveryStatusEnum.New) {  // new
            deliveryStatusAdd();
        }

        if (ibSelectedDelivery.Status == DeliveryStatusEnum.Delivering) { // Delivering
            deliveryStatusDelivering();
            ibAddItemBtnObj.fadeIn();
            if (ibSelectedDelivery.DeliveryType == DeliveryTypeEnum.Transfer) {
                  disableIbSelected();
            }
          
        }

        if (ibSelectedDelivery.Status == DeliveryStatusEnum.Received) {  // Received
            deliveryStatusReceived();
            disableIbSelected();
        }

        if (ibSelectedDelivery.Status == DeliveryStatusEnum.Error) {  // Error

        }

        if (ibSelectedDelivery.Status == DeliveryStatusEnum.Success) {  // Success
            ibAddItemBtnObj.fadeOut();
            ibDeleteItemBtnObj.fadeOut();
            ibSubmitDeliveryBtnObj.fadeOut();
            ibReceiveDeliveryBtnObj.fadeOut();
            ibDeleteDeliveryBtnObj.fadeOut();
            ibFinishDeliveryBtnObj.fadeOut();
            disabledOutQtyObj();
            disabledInQtyObj();
        }

        if (ibDeliverySelectValue == 0) {//new add no srcStore
            $("#SrcStoreLabel").hide();
            $("#SrcStoreDiv").hide();
            $("#ibCarrierSelectLabel").hide();
            $("#ibCarrierSelectDiv").hide();
        } else {
            if (ibSelectedDelivery.DeliveryType != DeliveryTypeEnum.PurchaseIn) {
                $("#ibCarrierSelectLabel").show();
                $("#ibCarrierSelectDiv").show();
            }
            else {
                $("#ibCarrierSelectLabel").hide();
                $("#ibCarrierSelectDiv").hide();
            }
            if (srcStore != "") {//if srcStore is not null  just can change inQty ,can't delete the items
                $("#SrcStoreLabel").show();
                $("#SrcStoreDiv").show();
            } else {
                $("#SrcStoreLabel").hide();
                $("#SrcStoreDiv").hide();
            }
        }

        initQtyEquals();

    });
}

//禁用下拉框
function disableIbSelected() {
    ibCarrierSelectObj.attr("disabled", "disabled");
    ibSrcStoreSelectObj.attr("disabled", "disabled");
    ibTarStoreSelectObj.attr("disabled", "disabled");
}


//取消禁用
function cancelDisableIbSelected() {
    ibCarrierSelectObj.removeAttr("disabled");
    ibSrcStoreSelectObj.removeAttr("disabled");
    ibTarStoreSelectObj.removeAttr("disabled");
}

//在状态为New 下的相关事件
function deliveryStatusAdd() {
    ibSubmitDeliveryBtnObj.fadeIn();//can Submit
    ibDeleteItemBtnObj.fadeIn();//can Delete   DataTable   Item
    ibAddItemBtnObj.fadeIn();//can ADD   DataTable   Item

    var ibDefaultTable = $('#ibTable').dataTable();// inbound dataTable Obj
    var ibTrs = ibDefaultTable.fnGetData();//get inbound dataTable  data
    $("#ibSrcStoreSelect").val(null);
    if (ibTrs.length > 0) { //if inbound dataTable is not null

        for (var j = 0; j < ibTrs.length; j++) {
            var inQtyObjID = "ibTableRowInQty" + ibTrs[j].ProductID;
            var outQtyObjID = "ibTableRowOutQty" + ibTrs[j].ProductID;
            var inQtyObj = $("#" + inQtyObjID);
            var outQtyObj = $("#" + outQtyObjID);
            // remove disabled  
            inQtyObj.removeAttr("disabled");
            outQtyObj.removeAttr("disabled");
            inQtyObj.val(outQtyObj.val());// init   inQty defalut  value  equals  outQty value
        }

    }
}



//在状态为Delivering 下的相关事件
function deliveryStatusDelivering() {
    ibSubmitDeliveryBtnObj.fadeIn(); //can submit
    ibReceiveDeliveryBtnObj.fadeIn();//can change status to Received
    if (ibSelectedDelivery.DeliveryType != DeliveryTypeEnum.PurchaseIn) {//购入
        ibDeleteDeliveryBtnObj.fadeIn();//  can delete  when the srcStore is null
        $("#SrcStoreLabel").hide();
        $("#SrcStoreDiv").hide();
        $("#ibCarrierSelectLabel").hide();
        $("#ibCarrierSelectDiv").hide();
    } else {
        ibDeleteItemBtnObj.fadeOut();
        $("#SrcStoreLabel").show();
        $("#SrcStoreDiv").show();
        $("#ibCarrierSelectLabel").show();
        $("#ibCarrierSelectDiv").show();
    }
    disabledOutQtyObj();
}

//在状态为Received 下的相关事件
function deliveryStatusReceived() {
    ibFinishDeliveryBtnObj.fadeIn(); // can finish
    ibDeleteItemBtnObj.fadeOut();//can't deleteIn    dataTables   Item
    if (srcStore == null) {
        ibDeleteDeliveryBtnObj.fadeIn();//  can delete  when the srcStore is null
    } else {

    }
    disabledInQtyObj();
    checkQtyEquals();
    disabledFinishBtn();
}

//禁用 dataTables 所有 OutQty
function disabledOutQtyObj() {

    var ibDefaultTable = $('#ibTable').dataTable();
    var ibTrs = ibDefaultTable.fnGetData();

    if (ibTrs.length > 0) {

        for (var j = 0; j < ibTrs.length; j++) {
            var outQtyObjID = "ibTableRowOutQty" + ibTrs[j].ProductID;
            var outQtyObj = $("#" + outQtyObjID);
            outQtyObj.attr("disabled", "disabled");
        }

    }
}


//禁用 dataTables 所有 InQty  
function disabledInQtyObj() {

    var ibDefaultTable = $('#ibTable').dataTable();
    var ibTrs = ibDefaultTable.fnGetData();

    if (ibTrs.length > 0) {

        for (var j = 0; j < ibTrs.length; j++) {
            var inQtyObjID = "ibTableRowInQty" + ibTrs[j].ProductID;
            var inQtyObj = $("#" + inQtyObjID);
            inQtyObj.attr("disabled", "disabled");
        }

    }
}


// new delivery as an default option
function ibGetNewDelivery() {
    return ibNewDelivery;
}

// when the ib InQty input updated
function ibUpdateInQty(qtyInputObj) {
    var trObj = qtyInputObj.parents('tr');
    ibTableObj.row(trObj).data().InQty = qtyInputObj.context.value;
    
}


// when the ib OutQty input updated
function ibUpdateOutQty(qtyInputObj) {
    var trObj = qtyInputObj.parents('tr');
    ibTableObj.row(trObj).data().OutQty = qtyInputObj.context.value;
    var id = qtyInputObj.context.id;
    id = id.substring(16);
    var InQtyObj = "ibTableRowInQty" + id;
    if (ibSelectedDelivery.Status == DeliveryStatusEnum.Received) {  // Received
        checkQtyEquals();
    } else {
        $("#" + InQtyObj).val(qtyInputObj.val());
    }
    if (ibSelectedDelivery.Status == DeliveryStatusEnum.Received) {//Received状态下 判断是否能结单
          disabledFinishBtn();
    }
  
}

// 检查 InQty  OutQty 是否相等
function checkQtyEquals() {
    var ibDefTable = new Array();
    var ibDefaultTable = $('#ibTable').dataTable();
    var ibTrs = ibDefaultTable.fnGetData();
    if (ibTrs.length > 0) {

        for (var i = 0; i < ibTrs.length; i++) {
            ibDefTable.push(ibTrs[i]);
        }
        for (var j = 0; j < ibTrs.length; j++) {
            var inQtyObjID = "ibTableRowInQty" + ibTrs[j].ProductID;
            var outQtyObjID = "ibTableRowOutQty" + ibTrs[j].ProductID;

            var inQtyObj = $("#" + inQtyObjID);
            var outQtyObj = $("#" + outQtyObjID);
            var ckBoxObjID = "ibTableRowBox" + ibTrs[j].ProductID;
            var ckBoxObj = $("#" + ckBoxObjID);

            if (inQtyObj.val() != outQtyObj.val()) {
                ckBoxObj.closest("tr").css("color", "#FF3333");
            } else {
                ckBoxObj.closest("tr").css("color", "#033333");
            }
        }
    }
}

//当收货收多了的时候不能结单
function disabledFinishBtn() {
    var ibDefTable = new Array();
    var ibDefaultTable = $('#ibTable').dataTable();
    var ibTrs = ibDefaultTable.fnGetData();
    var len = 0;
    if (ibTrs.length > 0) {

        for (var i = 0; i < ibTrs.length; i++) {
            ibDefTable.push(ibTrs[i]);
        }
        for (var j = 0; j < ibTrs.length; j++) {
            var inQtyObjID = "ibTableRowInQty" + ibTrs[j].ProductID;
            var outQtyObjID = "ibTableRowOutQty" + ibTrs[j].ProductID;

            var inQtyObj = $("#" + inQtyObjID);
            var outQtyObj = $("#" + outQtyObjID);

            if (inQtyObj.val() > outQtyObj.val()) {
                len++;
            }

            if(len>0){
                ibFinishDeliveryBtnObj.fadeOut();
            } else {
                ibFinishDeliveryBtnObj.fadeIn();
            }
        }
    }

}

// init dataTable outQty =inQty
function initQtyEquals() {

    var ibDefTable = new Array();
    var ibDefaultTable = $('#ibTable').dataTable();
    var ibTrs = ibDefaultTable.fnGetData();

    if (ibTrs.length > 0) {

        for (var i = 0; i < ibTrs.length; i++) {
            ibDefTable.push(ibTrs[i]);
        }
        for (var j = 0; j < ibTrs.length; j++) {
            var inQtyObjID = "ibTableRowInQty" + ibTrs[j].ProductID;
            var outQtyObjID = "ibTableRowOutQty" + ibTrs[j].ProductID;

            var inQtyObj = $("#" + inQtyObjID);
            var outQtyObj = $("#" + outQtyObjID);
            if (inQtyObj.val() == 0) {
                inQtyObj.val(outQtyObj.val());
            }
            var trObj = inQtyObj.parents('tr');
            ibTableObj.row(trObj).data().InQty = inQtyObj.val();
        }
    }

}


// delete the selected ib items
function ibDeleteItems() {
    //ibShowData();
    // Iterate over all checkboxes in the table
    ibTableObj.$('input[type="checkbox"]').each(function () {
        // If checkbox doesn't exist in DOM
        if (this.checked) {
            var trObj = $(this).parents('tr');
            ibTableObj.row(trObj).remove().draw(true);
        }
    })
}

// test: add new ib items
function ibAddItems() {
    ibModalClose();
    $.toaster(Delivery_Label_Inbound_AddMsg, Common_Label_Info, 'info');
}



// test: generate test ibbound items.
function ibGenOutItems(productID, productSKU, productShortTitle, ibQty) {
    var deliveryItem = {
        "Product": {
            "ID": productID,
            "ShortTitle": productShortTitle,
            "SKU": productSKU
        },
        "DeliveryID": ibSelectedDelivery.ID,
        "ProductID": productID,
        "InQty": 0,
        "OutQty": ibQty,
        "UpdateEmployeeID": currentEmployee.ID
    }
    return deliveryItem;
}


// prepare the post data before submit
function ibPrepareData() {
    ibSelectedDelivery = getStructObject("ibForm"); // reget from current form
    ibSelectedDelivery.DeliveryItemList = ibTableObj.rows().data().toArray();
}

// post data and save the ibbound delivery draft~
function ibSubmitDelivery() {
    ibSubmitDeliveryBtnObj.attr("disabled", "disabled");
    ibPrepareData();
    var url = './Delivery/InboundSubmitDeliveryJson';
    var reqData = { submitDelivery: ibSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Warning, 'danger');
        }
        ibSubmitDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;

        ibInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
    }
}

// confirm ibbound the selected delivery
function ibReceiveDelivery() {
    ibReceiveDeliveryBtnObj.attr("disabled", "disabled");
    ibPrepareData();
    var url = './Delivery/InboundReceiveDeliveryJson';
    var reqData = { submitDelivery: ibSelectedDelivery };
    console.log(reqData);
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        ibReceiveDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        ibInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
        invTableObj.draw();                     // reload the main page inventory table to 
    }
}


// finish ibbound the selected delivery
function ibFinishDelivery() {
    ibFinishDeliveryBtnObj.attr("disabled", "disabled");
    ibPrepareData();

    var url = './Delivery/InboundFinishDeliveryJson';
    var reqData = { submitDelivery: ibSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        ibFinishDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        ibInitDeliverySelect(returnDeliveryID); // reload the sub page delivery select box.
        invTableObj.draw();                     // reload the main page inventory table to 
    }
}

// delete ibbound the selected delivery
function ibDeleteDelivery() {
    ibDeleteDeliveryBtnObj.attr("disabled", "disabled");
    ibPrepareData();
    var url = './Delivery/InboundDeleteDeliveryJson';
    var reqData = { submitDelivery: ibSelectedDelivery };
    jsonPost(url, reqData, callback, false, false);

    function callback(result) {
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        ibDeleteDeliveryBtnObj.removeAttr("disabled");
        var returnDeliveryID = result.data;
        ibInitDeliverySelect(); // reload the sub page delivery select box.
        invTableObj.draw();                     // reload the main page inventory table to 
    }
}

// test: debug and show data in js console
function ibShowData() {
    ibPrepareData();
    debug(ibSelectedDelivery);
}


// init ibbound datatable
function ibInitTable() {
    if (ibTableObj != undefined) {
        return;
    }

    ibTableSelectAllBoxObj = $('#ibTableSelectAllBox'); // select all box

    ibTableObj = $('#ibTable').DataTable({
        data: ibSelectedDelivery.DeliveryItemList,
        "bRetrieve": true,
        "language": dtLanguage,
        "columns": [
           { "data": "Product.ID" },
           { "data": "Product.SKU" },
           { "data": "Product.ShortTitle" },
            { "data": "InQty" },
           { "data": "OutQty" }
        ],
        "columnDefs": [
                {
                    "render": function (data, type, row) {
                        return '<input name="Product.ID" value="' + row.Product.ID + '" type="checkbox"  id="ibTableRowBox' + row.Product.ID + '"/>';
                    }, "targets": 0, 'searchable': false, 'orderable': false
                }, {
                    "render": function (data, type, row) {
                        //debug(this.parent)
                        var str = '<input  value="' + row.InQty + '" type="number" class="form-control" min="1" max="999" style="width:100px!important;"  onchange="ibUpdateInQty($(this))"  id="ibTableRowInQty' + row.Product.ID + '"/>';
                        return str
                    }, "targets": 3
                }, {
                    "render": function (data, type, row) {
                        //debug(this.parent)
                        var str = '<input  value="' + row.OutQty + '" type="number" class="form-control" min="1" max="999" style="width:100px!important;" onchange="ibUpdateOutQty($(this))" id="ibTableRowOutQty' + row.Product.ID + '"/>';
                        return str
                    }, "targets": 4
                }
        ],

        "dom": 'tip',
        "scrollY": 200,
        "order": [0, "asc"]

    });

    // Handle click on "Select all" control
    ibTableSelectAllBoxObj.on('click', function () {
        var rows = ibTableObj.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
    });

    // Handle click on checkbox to set state of "Select all" control
    $('#ibTable').find('tbody').on('change', 'input[type="checkbox"]', function () {
        if (!this.checked) { // If checkbox is not checked
            var el = ibTableSelectAllBoxObj.get(0);
            if (el && el.checked && ('indeterminate' in el)) {
                el.indeterminate = true;
            }
        }
    });
    checkQtyEquals();

}