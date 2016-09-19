var ipFormObj;                  // ip: import map rule form object
var ipMapRuleSelectObj;         // ip: import map rule select object
var ipMapRuleList;              // ip: server data in js memory
var ipSelectedMapRule;          // ip: current selected map rule object in js memory
var ipFileInputObj;             // ip: the import file input object
var ipBrowseBtnObj;             // ip: the import browse file button object
var ipFileCoverInputObj;        // ip: the file cover of upload input
var ipMapRuleDescDivObj;        // ip: map rule desc
var ipMapRuleItemListDivObj;    // ip:  div of rule items
var ipMapRuleOptionListDivObj;  // ip:  div of rule options
var ipEffInputObj;              // ip: the hidden id in page for import Effect entity
var ipAssTypeInputObj;          // ip: the hidden id in page for ass type
var ipDataTypeInputObj;         // ip: the hidden id in page for data type
var ipEffNameSpanObj;           // ip: the name span of Eff
var ipProcessTypeSelectObj      // ip: process type selection
var ipFileTypeSelectObj         // ip: file type selection

var ipQueryMapRule;              // ip: json in memory of ip query map rule

var ipUrl;                      // ip: import Url param
var ipAssType;                  // ip: param import ass type
var ipDataType;                 // ip: param import data type
var ipEff;                      // ip: the Effect data param
var ipEffName;                  // ip: the Effect data name

var ipMapRuleItemTableObj;      // ip: the map rule item table
var ipMapRuleOptionTableObj;    // ip the map rule option table

var ipDefaultOptionList;        // ip: the not changed default option list

// INTERFACE: call from main page to init the import modal.
// ex:  ipCallConfig('./Inventory/ImportJson', 2, 1);
function ipCallConfig(ipParamUrl, ipParamAssType, ipParamDataType) {

    ipUrl = ipParamUrl;
    ipAssType = ipParamAssType;
    ipDataType = ipParamDataType

    ipQueryMapRule = { // the ob delivery query object
        ClientID: currentEmployee.ClientID,
        AssType: ipAssType,       // 1.Export 2.Import
        DataType: ipDataType      // 1.Inventory 2.Delivery
    }
   
}

// INTERFACE: call initial import form
function ipCallInit() {

    ipInitDefaultOptionList(ipDataType);            // init the default option list

    ipAssTypeInputObj = $('#ipAssTypeInput');
    ipDataTypeInputObj = $('#ipDataTypeInput');

    ipAssTypeInputObj.val(ipAssType)
    ipDataTypeInputObj.val(ipDataType)

    ipInitForm();
    ipInitMapRuleSelect();                          // init delivery select
   
}

// INTERFACE: set the import Effect entity id and name
// ex: ipCallUpdateEff(1,"Guangzhou Store")
function ipCallUpdateEff(ipParamEff, ipParamEffName) {
    ipEff = ipParamEff;
    ipEffName = ipParamEffName;
    ipEffInputObj = $("#ipEffInput")
    ipEffNameSpanObj = $("#ipEffNameSpan")
    ipEffInputObj.val(ipEff);
    ipEffNameSpanObj.html(ipEffName);

}




function ipInitForm() {
    ipFormObj = $('#ipForm');
    ipBrowseBtnObj = $('#ipBrowseBtn');
    ipFileInputObj = $('#ipFileInput');
    ipFileCoverInputObj = $('#ipFileCoverInput');
    ipMapRuleDescDivObj = $('#ipMapRuleDescDiv');
    ipMapRuleItemListDivObj = $('#ipMapRuleItemListDiv');
    ipMapRuleOptionListDivObj = $('#ipMapRuleOptionListDiv');


    ipBrowseBtnObj.click(function () {
        ipFileInputObj.click();
    });

    ipFileInputObj.change(function () {
        var fileName = $(this).val();
        ipFileCoverInputObj.val(fileName);
        var arr = fileName.split(".");

        if (arr[1] != undefined) {
            if (arr[1] == "xls") {
                $("#ipFileTypeSelect option:eq(0)").attr("selected", 'selected');
            } else if (arr[1] == "xlsx") {
                $("#ipFileTypeSelect option:eq(1)").attr("selected", 'selected');
            } else if (arr[1] == "cvs") {
                $("#ipFileTypeSelect option:eq(2)").attr("selected", "selected");
            }
        }

    });

    ipInitProcessTypeSelect();
    ipInitFileTypeSelect();
}


// get the mapRule in select box from server
function ipInitMapRuleSelect() {
    ipMapRuleSelectObj = $('#ipMapRuleSelect');

    var url = './MapRule/GetMapRuleDataTableJson';
    var reqData = { queryMapRule: ipQueryMapRule };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        ipMapRuleSelectObj.empty();

        $.each(result.data, function (i, item) {
            var text = item.Name
            var value = item.ID;
            $("<option></option>").val(value).text(text).appendTo(ipMapRuleSelectObj);
        });
        ipMapRuleList = result.data;

        ipMapRuleSelectObj.trigger("change");
        ipSelectedMapRule = ipMapRuleList[0];

    }


    // set the change method
    ipMapRuleSelectObj.change(function () {
        var selectedIndex = $(this).children('option:selected').index();
        ipSelectedMapRule = ipMapRuleList[selectedIndex];       // update memory ip map rule
        ipMapRuleDescDivObj.html(ipSelectedMapRule.Description);

        var itemListStr = "<table border=0 width='100%' id='ipMapRuleItemTable'>";
        itemListStr += "<th>" + Common_Label_Remove + "</th><th>" + Common_Label_No + "</th><th>" + MapRuleItem_Entity_DisplayName + "</th><th>" + MapRuleItem_Entity_Column + "</th><th>" + MapRuleItem_Entity_Sorting + "</th>";
        itemListStr += "<tbody>"
        for (var i in ipSelectedMapRule.RuleItemList) {
            var item = ipSelectedMapRule.RuleItemList[i];
            itemListStr += ipGenItemTr(i, item.MapRuleOption.Column, item.Name)
        }
        itemListStr += "</tbody>"
        itemListStr += "</table>&nbsp;";
        ipMapRuleItemListDivObj.html(itemListStr)
        ipMapRuleItemTableObj = $("#ipMapRuleItemTable");


        var optListStr = "<table border=0 width='100%' id='ipMapRuleOptionTable'>"
        optListStr += "<th>" + MapRuleItem_Entity_DisplayName + "</th><th>" + MapRuleItem_Entity_Column + "</th><th>" + Common_Label_Add + "</th>";
        optListStr += "<tbody>"
        $.each(ipDefaultOptionList, function (i, option) {

            var col = option.Column
            var name = option.Name;
            var needAdd = true;

            for (var i in ipSelectedMapRule.RuleItemList) {
                var item = ipSelectedMapRule.RuleItemList[i];
                if (item.Column == col) {
                    needAdd = false
                }
            }

            if (needAdd) {
                optListStr += ipGenOptionTr(col, name)
            }
        });
        optListStr += "</tbody>"
        optListStr += "</table>&nbsp;";
        ipMapRuleOptionListDivObj.html(optListStr);
        ipMapRuleOptionTableObj = $("#ipMapRuleOptionTable");

    })
}

function ipGenItemTr(i, col, name) {
    var sortingInput = "<input type='text' name='submitFileData.MapRule.RuleItemList[" + i + "].Sorting' value='" + i + "'    class='form-control' style='width:40px' readonly/>";
    var columnInput = "<input type='text' name='submitFileData.MapRule.RuleItemList[" + i + "].Column' value='" + col + "'    class='form-control' style='width:120px' readonly/>";
    var nameInput = "<input type='text' name='submitFileData.MapRule.RuleItemList[" + i + "].Name' value='" + name + "'    class='form-control' style='width:120px'/>";
    var trStr = "";
    trStr += "<tr>";
    trStr += "<td>" + "<button type='button' class='btn btn-danger' onclick='ipItemRemove(this)'> " + Common_Label_Remove + " </button>" + "</td>";
    trStr += "<td>" + sortingInput + "</td>";
    trStr += "<td>" + nameInput + "</td>";
    trStr += "<td>" + columnInput + "</td>";
    trStr += "<td>" + "<button type='button' class='btn' onclick='ipItemUp(this)'>" + Common_Button_Up + "</button>" + "&nbsp;<button type='button' class='btn' onclick='ipItemDown(this)' >" + Common_Button_Down + "</button>" + "</td>";
    trStr += "</tr>";
    return trStr;
}


function ipInitDefaultOptionList(dataType) {
    var queryMapRuleOption = {};
    queryMapRuleOption.DataType = dataType;
    var url = './MapRule/GetMapRuleOptionListJson';
    var reqData = queryMapRuleOption;
    jsonPost(url, reqData, callback, true, false);
    function callback(result) {
        ipDefaultOptionList = result;
    }
}

function ipGenOptionTr(col, name) {
    var trStr = "";
    trStr += "<tr>";
    trStr += "<td>" + name + "</td>";
    trStr += "<td>" + col + "</td>";
    trStr += "<td>" + "<button type='button' class='btn btn-success' onclick='ipItemAdd(this)'>" + Common_Label_Add + "</button>" + "</td>";
    trStr += "</tr>";
    return trStr;
}


function ipSubmit() {
    var url = ipUrl;
    var formData = new FormData(ipFormObj[0]);
    //log(formData);
    jsonUpload(url, formData, callback, false, true);

    function callback(result) {
        //console.log(result);
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
            invTableObj.draw();                     // reload the main page inventory table to refresh inv counts~
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
    }
}

// make up item
function ipItemUp(btn) {
    var tr = $(btn).parent().parent();
    var prevTr = tr.prev();
    //log(tr.index())
    if ((prevTr.length > 0) && (tr.index() > 0)) {
        prevTr.insertAfter(tr);
        ipItemUpdate(ipMapRuleItemTableObj)
    }
}

// make down item
function ipItemDown(btn) {
    var tr = $(btn).parent().parent();
    var nextTr = tr.next();
    if (nextTr.length > 0) {
        nextTr.insertBefore(tr);
        ipItemUpdate(ipMapRuleItemTableObj)
    }
}

// add item from left to right
function ipItemAdd(btn) {
    var tr = $(btn).parent().parent();
    var nameTd = $(tr).find("td")[0];
    var colTd = $(tr).find("td")[1];
    var name = $(nameTd).html();
    var col = $(colTd).html();
    var newTrStr = ipGenItemTr(0, col, name);
    ipMapRuleItemTableObj.find('tbody:first-child').append(newTrStr);
    ipItemUpdate(ipMapRuleItemTableObj)
    tr.remove();
}

// remove item from right to left
function ipItemRemove(btn) {
    var tr = $(btn).parent().parent();
    var nameInput = $(tr).find("td input")[0];
    var colInput = $(tr).find("td input")[1];
    var name = $(nameInput).val();
    var col = $(colInput).val();
    tr.remove();
    var newTrStr = ipGenOptionTr(col, name);
    ipMapRuleOptionTableObj.find('tbody:first-child').append(newTrStr);
    ipItemUpdate(ipMapRuleItemTableObj)
}

// update the map rule item list input's index for the submiting
function ipItemUpdate(tbl) {
    tbl.find("tbody tr").each(function (i, tr) {
        var sortingInput = $(tr).find("td input")[0];
        var nameInput = $(tr).find("td input")[1];
        var colInput = $(tr).find("td input")[2];

        if (nameInput != undefined) {
            $(sortingInput).attr("name", "submitFileData.MapRule.RuleItemList[" + (i - 1) + "].Sorting")
            $(sortingInput).val(i - 1);
            $(nameInput).attr("name", "submitFileData.MapRule.RuleItemList[" + (i - 1) + "].Name")
            $(colInput).attr("name", "submitFileData.MapRule.RuleItemList[" + (i - 1) + "].Column")
            //log(nameInput) //debug
        }
    });
}


// initial the process Type select
function ipInitProcessTypeSelect() {
    ipProcessTypeSelectObj = $("#ipProcessTypeSelect");
    var url = "./FileData/GetProcessTypeListJson";
    jsonPost(url, null, callback, true, false);
    function callback(result) {
        $.each(result, function (i, item) {
            var text = item.Text
            var value = item.Value;
            $("<option></option>").val(value).text(text).appendTo(ipProcessTypeSelectObj);
        });
    }
}


// initial the file Type select
function ipInitFileTypeSelect() {
    ipFileTypeSelectObj = $("#ipFileTypeSelect");
    var url = "./FileData/GetFileTypeListJson";
    jsonPost(url, null, callback, true, false);
    function callback(result) {
        $.each(result, function (i, item) {
            var text = item.Text
            var value = item.Value;
            $("<option></option>").val(value).text(text).appendTo(ipFileTypeSelectObj);
        });
    }
}
