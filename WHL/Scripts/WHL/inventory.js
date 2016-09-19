var invTableObj;
var invQueryStoreDivObj;
var invQueryInventory;
var invOutboundBtnObj;
var invInboundBtnObj;
var invImportBtnObj;
var invExportBtnObj;
var invTableSelectAllBoxObj;
var invStoreID;
var invStoreIDObj;
var invStoreName;


var invQueryStore = { // pango
    ClientID: currentEmployee.ClientID
}


// ready
$(document).ready(function () {

    invInitTable();
    invInitQueryStoreDiv();

    $("#tdStoreName").text(Store_Label_All);

    initLaydate();
    
    obCallConfig();                                                                     // INTERFACE: initial the outbound modal
    obCallInit();                                                                       // INTERFACE: initial the outbound modal

    ibCallConfig();                                                                     // INTERFACE: initial the inbound modal
    ibCallInit();                                                                       // INTERFACE: initial the inbound modal

    ipCallConfig('./Inventory/ImportJson', AssTypeEnum.Import, DataTypeEnum.Inventory);      // INTERFACE: initial the import modal  2=import,1=inventory
    ipCallInit()

    epCallConfig('./Inventory/ExportJson', AssTypeEnum.Export, DataTypeEnum.Inventory);      // INTERFACE: initial the import modal  1=export,1=inventory
    epCallInit();

    invInitBtns();


});

function initLaydate() {
    laydate({
        elem: '#QueryUpdateTime1'
    });
    laydate({
        elem: '#QueryUpdateTime2'
    });
}

function invInitBtns() {
    invOutboundBtnObj = $("#invOutboundBtn");
    invInboundBtnObj = $("#invInboundBtn");
    invImportBtnObj = $("#invImportBtn");
    invExportBtnObj = $("#invExportBtn");

    initInvOutboundBtnClick();
    initInvInboundBtnClick();
    initInvImportBtnClick();
    initInvExportBtnClick();
}

// click the invSearch btn
function invSearch() {
    invUpdateQuery();
    invTableObj.draw();
}

//bind  invOutboundBtn click event
function initInvOutboundBtnClick() {
    invOutboundBtnObj.bind("click", function () {
        obCallAddItems();
        $("#outboundModal").modal("show");
        var obParamInvList = [];
        invTableObj.$('input[type="checkbox"]').each(function () {
            // If checkbox doesn't exist in DOM
            if (this.checked) {
                var trObj = $(this).parents('tr');
                var checkedInv = invTableObj.row(trObj).data();
                //log(checkedInv.Product)
                obParamInvList.push(checkedInv.Product);
            }
        });
        obCallAddItems(obParamInvList);
        
    });
}


//bind  InvInboundBtnObj click event
function initInvInboundBtnClick() {

    invInboundBtnObj.bind("click", function () {
        $("#inboundModal").modal("show");
        var ibParamInvList = [];
        invTableObj.$('input[type="checkbox"]').each(function () {
            // If checkbox doesn't exist in DOM
            if (this.checked) {
                var trObj = $(this).parents('tr');
                var checkedInv = invTableObj.row(trObj).data();
                //log(checkedInv.Product)
                ibParamInvList.push(checkedInv.Product);
            }
        });
        ibCallAddItems(ibParamInvList);
    });
}

//bind  invImportBtnObj click event
function initInvImportBtnClick() {
  

    invImportBtnObj.bind("click", function () {

        ipCallUpdateEff(invStoreID, invStoreName);                // INTERFACE: call update the effName and Eff

        $("#importModal").modal("show");
    });
}

//bind  invExportBtnObj click event
function initInvExportBtnClick() {

    invExportBtnObj.bind("click", function () {

        epCallUpdateEff(invQueryInventory, invStoreName);

        $("#exportModal").modal("show");
      
    });
}



// click the reset btn
function invReset() {
    $("[name='Product.shortTitle']").val("");
    $("[name='Product.SKU']").val("");
    $("[name='QueryAvail1']").val("");
    $("[name='QueryAvail2']").val("");
    $("[name='QueryUpdateTime1']").val("");
    $("[name='QueryUpdateTime2']").val("");
    invUpdateQuery();
    invTableObj.draw();
}

// click one store,update the hidden StoreID value
function invSelectStore(storeID, storeName) {
    $("[name='StoreID']").val(storeID);
    if (storeName == "Common_Label_All") {
        storeName = Common_Label_All;
    }
    $("#tdStoreName").text(storeName);
    invStoreID = storeID;
    invStoreName = storeName;
    obCallUpdateStore(invStoreID,invStoreName); // let the outbound know the change
    ibCallUpdateStore(invStoreID,invStoreName); // let the outbound know the change

    invStoreIDObj.val(invStoreID)  // let query form know the change
    invSearch();

}

// get the query inventory param object
function invUpdateQuery() {
    invQueryInventory = getStructObject("invQueryForm"); // convent form data to structure object.
}

// get the query stores data in select box from server
function invInitQueryStoreDiv() {
    invQueryStoreDivObj = $('#invQueryStoreDiv');
    var url = './Store/GetStoreDataTableJson';
    var reqData = { queryStore: invQueryStore };
    jsonPost(url, reqData, callback, true, false);

    var storeStr = "<a href='javascript:invSelectStore(-1,\"" + Store_Label_All + "\")'>" + Store_Label_All + "</a><br>";

    function callback(result) {
        $.each(result.data, function (i, item) {
            var id = item.ID;
            var name = item.Name;
            storeStr += "<a href='javascript:invSelectStore(" + id + ",\"" + name + "\")'>" + name + "</a><br>";
        });
        invQueryStoreDivObj.html(storeStr);
    }
    invStoreID = -1;
    invStoreName = Store_Label_All;
    invStoreIDObj = $('#invStoreID');
    invStoreIDObj.val(invStoreID)
}

function invInitTable() {
    // init dataobject table here
    invTableSelectAllBoxObj = $('#invTableSelectAllBox'); // select all box
    
    invTableObj = $('#invTable').DataTable({
        "serverSide": true,
        "processing": true,

        "ajax": {
            "type": "POST",
            "url": './Inventory/GetInventoryDataTableJson',
            "contentType": 'application/json; charset=utf-8',
            'async': true,
            'data': function (dtParams) {
                //log(dtParams);
                var reqData = JSON.stringify({ dtParams: dtParams, queryInventory: invQueryInventory });
                //log(reqData);
                return reqData;
            }
        },
        "language": dtLanguage,
        "bFilter": false,
        "processing": true,
        "paging": true,
        "scrollY": 400,
        "scrollX": true,
        "pagingType": "full_numbers",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "dom": 'rt<"bottom"flp i><"clear">',
        "deferRender": true,
        "columns": [
           { "data": "ID" },
           { "data": "Product.SKU" },
           { "data": "Product.ShortTitle" },
           { "data": "Avail" },
           { "data": "OnHold" },
           { "data": "SwAvail" },
           { "data": "OnIn" },
           { "data": "OnOut" },
           { "data": "InSum" },
           { "data": "OutSum" },
           { "data": "DiffSum" },
           { "data": "ShipMiss" },
           { "data": "Store.Name" },
           { "data": "Store.TypeLayout" },
           { "data": "UpdateDateLayout" }
        ],
        "columnDefs": [
               {
                   "render": function (data, type, row) {
                       return '<input name="Product.ID" value="' + row.Product.ID + '" type="checkbox"  id="invTableRowBox' + row.Product.ID + '" />';
                   }, "targets": 0, 'searchable': false, 'orderable': false
               }
        ],
        "order": [0, "asc"],
        "initComplete": function (settings, json) {
            // do nothing
        }
    });

    //分页选项位置调整
    $("#invTable_length").removeClass("dataTables_length");
    $("#invTable_length").addClass("dataTables_length_right");
    $("#invTable_info").css("marginTop", "-37px");

    // Handle click on "Select all" control
    invTableSelectAllBoxObj.on('click', function () {
        var rows = invTableObj.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
    });

    // Handle click on checkbox to set state of "Select all" control
    $('#obTable').find('tbody').on('change', 'input[type="checkbox"]', function () {
        if (!this.checked) { // If checkbox is not checked
            var el = invTableSelectAllBoxObj.get(0);
            if (el && el.checked && ('indeterminate' in el)) {
                el.indeterminate = true;
            }
        }
    });

}