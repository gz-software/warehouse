var epFormObj;                  // ep: export map rule form object
var epQueryMapRule;             // ep: json in memory of ep query map rule
var epMapRuleList;              // ep: server data in js memory
var epSelectedMapRule;          // ep: current selected map rule object in js memory
var epMapRuleDescDivObj;        // ep: map rule desc
var epMapRuleItemListDivObj;    // ep:  div of rule items
var epMapRuleOptionListDivObj;  // ep:  div of rule options
var epAssTypeInputObj;          // ep: the hidden id in page for ass type
var epDataTypeInputObj;         // ep: the hidden id in page for data type
var epMapRuleSelectObj;         // ep: export  map rule select object
var epFileNameInputObj;         // ep: the export file name

var epMapRuleItemTableObj;      // ep: the map rule item table
var epMapRuleOptionTableObj;    // ep the map rule option table

var epUrl;                      // ep: export Url param
var epAssType;                  // ep: param export ass type
var epDataType;                 // ep: param export data type
var epEff;                      // ep: the Effect data param
var epEffName;                  // ep: the Effect data name

var epDefaultOptionList;        // ep: the not changed default option list

// INTERFACE: call from main page to init the export modal.
// ex:  epCallConfig('./Inventory/ExportJson', 1, 1);
function epCallConfig(epParamUrl, epParamAssType, epParamDataType) {

    epUrl = epParamUrl;
    epAssType = epParamAssType;
    epDataType = epParamDataType

    epQueryMapRule = { // the ob delivery query object
        ClientID: currentEmployee.ClientID,
        AssType : epAssType,        // 1.Export 2.Import
        DataType: epDataType        // 1.Inventory 2.Delivery
    }
   
}

// INTERFACE: call initial export form
function epCallInit() {
    epInitDefaultOptionList(epDataType);            // init the default option list

    epAssTypeInputObj = $('#epAssTypeInput');
    epDataTypeInputObj = $('#epDataTypeInput');

    epAssTypeInputObj.val(epAssType)
    epDataTypeInputObj.val(epDataType)


    epInitForm();
    epInitMapRuleSelect();                          // init delivery select

  
}

// INTERFACE: set the export query condition
// ex: epCallUpdateReqData(jsonReqData)
function epCallUpdateEff(epParamEff, epParamEffName) {
    epEff = epParamEff;
    epEffName = epParamEffName;
    epEffNameSpanObj = $("#epEffNameSpan")
    epEffNameSpanObj.html(epEffName);
    epUpdateFileName();     // update the export file name
}




function epInitForm() {
    epFormObj = $('#epForm');
    epMapRuleDescDivObj = $('#epMapRuleDescDiv');
    epMapRuleItemListDivObj = $('#epMapRuleItemListDiv');
    epMapRuleOptionListDivObj = $('#epMapRuleOptionListDiv');
    epFileNameInputObj = $('#epFileNameInput');

    epUpdateFileName();         // update the export file name
    epInitFileTypeSelect();     // initil the file type selection

}



// get the mapRule in select box from server
function epInitMapRuleSelect() {
    epMapRuleSelectObj = $('#epMapRuleSelect');

    var url = './MapRule/GetMapRuleDataTableJson';
    var reqData = { queryMapRule: epQueryMapRule };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        epMapRuleSelectObj.empty();

        $.each(result.data, function (i, item) {
            var text = item.Name
            var value = item.ID;
            $("<option></option>").val(value).text(text).appendTo(epMapRuleSelectObj);
        });
        epMapRuleList = result.data;

        epMapRuleSelectObj.trigger("change");
        epSelectedMapRule = epMapRuleList[0];
        //log(epSelectedMapRule)

    }


    // set the change method
    epMapRuleSelectObj.change(function () {
        var selectedIndex = $(this).children('option:selected').index();
        epSelectedMapRule = epMapRuleList[selectedIndex];       // update memory ep map rule
        epMapRuleDescDivObj.html(epSelectedMapRule.Description);

        var itemListStr = "<table border=0 width='100%' id='epMapRuleItemTable'>";
        itemListStr += "<th>"+Common_Label_Remove+"</th><th>"+Common_Label_No+"</th><th>"+MapRuleItem_Entity_DisplayName+"</th><th>"+MapRuleItem_Entity_Column+"</th><th>"+MapRuleItem_Entity_Sorting+"</th>"
        itemListStr += "<tbody>"
        for (var i in epSelectedMapRule.RuleItemList) {
            var item = epSelectedMapRule.RuleItemList[i];
            itemListStr += epGenItemTr(i, item.MapRuleOption.Column, item.Name)
        }
        itemListStr += "</tbody>"
        itemListStr += "</table>&nbsp;";
        epMapRuleItemListDivObj.html(itemListStr)
        epMapRuleItemTableObj = $("#epMapRuleItemTable");


        var optListStr = "<table border=0 width='100%' id='epMapRuleOptionTable'>"
        optListStr += "<th>"+MapRuleItem_Entity_DisplayName+"</th><th>"+MapRuleItem_Entity_Column+"</th><th>"+Common_Label_Add+"</th>";
        optListStr += "<tbody>"
        $.each(epDefaultOptionList, function (i, option) {

            var col = option.Column
            var name = option.Name;
            var needAdd = true;

            for (var i in epSelectedMapRule.RuleItemList) {
                var item = epSelectedMapRule.RuleItemList[i];
                if (item.Column == col) {
                    needAdd = false
                }
            }

            if (needAdd) {
                optListStr += epGenOptionTr(col, name)
            }
        });
        optListStr += "</tbody>"
        optListStr += "</table>&nbsp;";
        epMapRuleOptionListDivObj.html(optListStr);
        epMapRuleOptionTableObj = $("#epMapRuleOptionTable");

    })
}

function epGenItemTr(i, col, name) {
    var sortingInput = "<input type='text' name='submitFileData.MapRule.RuleItemList[" + i + "].Sorting' value='" + i + "'    class='form-control' style='width:40px' readonly/>";
    var columnInput = "<input type='text' name='submitFileData.MapRule.RuleItemList[" + i + "].Column' value='" + col + "'    class='form-control' style='width:120px' readonly/>";
    var nameInput = "<input type='text' name='submitFileData.MapRule.RuleItemList[" + i + "].Name' value='" + name + "'    class='form-control' style='width:120px'/>";
    var trStr = "";
    trStr += "<tr>";
    trStr += "<td>" + "<button type='button' class='btn btn-danger' onclick='epItemRemove(this)'>"+ Common_Button_Remove +"</button>" + "</td>";
    trStr += "<td>" + sortingInput + "</td>";
    trStr += "<td>" + nameInput + "</td>";
    trStr += "<td>" + columnInput + "</td>";
    trStr += "<td>" + "<button type='button' class='btn' onclick='epItemUp(this)'>"+ Common_Button_Up + "</button>" + "&nbsp;<button type='button' class='btn' onclick='epItemDown(this)' >" + Common_Button_Down +"</button>" + "</td>";
    trStr += "</tr>";
    return trStr;
}


function epInitDefaultOptionList(dataType) {
    var queryMapRuleOption = {};
    queryMapRuleOption.DataType = dataType;
    var url = './MapRule/GetMapRuleOptionListJson';
    var reqData = queryMapRuleOption;
    jsonPost(url, reqData, callback, true, false);
    function callback(result) {
        epDefaultOptionList = result;
    }
}

function epGenOptionTr(col, name) {
    var trStr = "";
    trStr += "<tr>";
    trStr += "<td>" + name + "</td>";
    trStr += "<td>" + col + "</td>";
    trStr += "<td>" + "<button type='button' class='btn btn-success' onclick='epItemAdd(this)'>" + Common_Button_Add + "</button>" + "</td>";
    trStr += "</tr>";
    return trStr;
}




// export it~
function epSubmit() {


    //var formData = new FormData(epFormObj[0]);

    var formData = getStructObject("epForm"); // convent form data to structure object.
    formData["eff"] = epEff;
    jsonPost(epUrl, formData, callback, true, false);

    function callback(result) {
       // alert(result); // get the download path from here and show on page.
        if (result.flag) {
            epUpdateFileName(); // 更新导出文件名称
    
            //log(result)
            var downloadPath = FileInfo_Result_Success_PleaseDownload + "<b>&nbsp;:&nbsp;<a href='" + result.data.FileDownloadPath + "'>" + result.data.FileName + "</a></b>";
            $.toaster(result.message, Common_Label_Success, 'success');
            $.toaster(downloadPath, Common_Label_Warning, 'warning');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }

    }

}


// make up item
function epItemUp(btn) {
    var tr = $(btn).parent().parent();
    var prevTr = tr.prev();
    //log(tr.index())
    if ((prevTr.length > 0) && (tr.index() > 0)) {
        prevTr.insertAfter(tr);
        epItemUpdate(epMapRuleItemTableObj)
    }
}

// make down item
function epItemDown(btn) {
    var tr = $(btn).parent().parent();
    var nextTr = tr.next();
    if (nextTr.length > 0) {
        nextTr.insertBefore(tr);
        epItemUpdate(epMapRuleItemTableObj)
    }
}

// add item from left to right
function epItemAdd(btn) {
    var tr = $(btn).parent().parent();
    var nameTd = $(tr).find("td")[0];
    var colTd = $(tr).find("td")[1];
    var name = $(nameTd).html();
    var col = $(colTd).html();
    var newTrStr = epGenItemTr(0, col, name);
    epMapRuleItemTableObj.find('tbody:first-child').append(newTrStr);
    epItemUpdate(epMapRuleItemTableObj)
    tr.remove();
}

// remove item from right to left
function epItemRemove(btn) {
    var tr = $(btn).parent().parent();
    var nameInput = $(tr).find("td input")[0];
    var colInput = $(tr).find("td input")[1];
    var name = $(nameInput).val();
    var col = $(colInput).val();
    tr.remove();
    var newTrStr = epGenOptionTr(col, name);
    epMapRuleOptionTableObj.find('tbody:first-child').append(newTrStr);
    epItemUpdate(epMapRuleItemTableObj)
}

// update the map rule item list input's index for the submiting
function epItemUpdate(tbl) {
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



// initial the file Type select
function epInitFileTypeSelect() {
    epFileTypeSelectObj = $("#epFileTypeSelect");
    var url = "./FileData/GetFileTypeListJson";
    jsonPost(url, null, callback, true, false);
    function callback(result) {
        $.each(result, function (i, item) {
            var text = item.Text
            var value = item.Value;
            $("<option></option>").val(value).text(text).appendTo(epFileTypeSelectObj);
        });
    }
}

// get the export file name
function epUpdateFileName() {
    var d = new Date();
    var newFileName = "export" + "_" + d.yyyymmddhhmmss();
    epFileNameInputObj.val(newFileName);
   
}


