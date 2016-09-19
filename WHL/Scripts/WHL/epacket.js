var ekDelivery;
var ekFormObj;
var ekConfirmBtnObj;
var ekGetLabelBtnObj;

// INTERFACE: call from main page to init the epacket modal.
// ex:  ekCallConfig(ekParamDelivery);
function ekCallConfig(ekParamDelivery) {
    ekDelivery = ekParamDelivery;
    //log(ekDelivery.Shipment);
    //log(ekDelivery.Shipment.ShipData)
}


// INTERFACE: call initial ek form
function ekCallInit() {
    ekInitForm();
    setStructForm(ekFormObj[0],ekDelivery.Shipment)
}


// initial ek form
function ekInitForm() {
    ekFormObj = $("#ekForm");
    ekInitBtns();
}

// initial 5 post buttons
function ekInitBtns() {
    ekConfirmBtnObj = $('#ekConfirmBtn');
    ekGetLabelBtnObj = $('#ekGetLabelBtn');
}


// confirm an epackage package
function ekConfirm() {
    ekConfirmBtnObj.attr("disabled", "disabled");

    var url = './Epacket/Epacket/ConfirmPackageJson?carrierID=1&trackCode=123456&debug=true';
    var reqData = { carrierID: ekDelivery.CarrierID,trackCode:ekDelivery.Shipment.TrackCode,debug:true };
    jsonPost(url, reqData, callback, true, false);

    function callback(result) {
        log(result)
        if (result.flag) {
            $.toaster(result.message, Common_Label_Success, 'success');
        } else {
            $.toaster(result.message, Common_Label_Danger, 'danger');
        }
        ekConfirmBtnObj.removeAttr("disabled");
      
    }
}

// close  ekModal
function ekModalClose() {
    $("#epacketModal").modal("hide");
}