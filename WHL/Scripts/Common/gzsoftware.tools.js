function log(str) {
    console.log(str);
}

function debug(str) {
    console.debug(str);
}

function jsonPost(url, reqData, callback, async, isDebug) {
    //isDebug = true;

    var callerMethod = jsonPost.caller.toString();
    var re = /function\s*(\w*)/i;
    var callerMethodName = re.exec(callerMethod);

    var reqStr = JSON.stringify(reqData);
    var returnData;
    if (isDebug) {
        debug("[" + callerMethodName + "]" + "[Param] " + reqStr);
    }
    $.ajax({
        type: "POST",
        url: url,
        data: reqStr,
        async: async,
        contentType : 'application/json; charset=utf-8',
        success: function (result) {
            var resultStr = JSON.stringify(result);
            if (isDebug) {
                debug("[" + callerMethodName + "]" +  "[Return] " + result);
            }

            if (callback != undefined) {
                callback(result)
            } else {
                returnData = result;
                if (async) {
                    return returnData;
                }
            }
        }
    });
    if (!async) {
        return returnData;
    }
}

function jsonUpload(url, formData, callback, async, isDebug) {
    //isDebug = true;

    var callerMethod = jsonUpload.caller.toString();
    var re = /function\s*(\w*)/i;
    var callerMethodName = re.exec(callerMethod);

   
    var returnData;
    if (isDebug) {
        debug("[" + callerMethodName + "]" + "[FormData] " + formData);
    }
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        async: async,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var resultStr = JSON.stringify(result);
            if (isDebug) {
                debug("[" + callerMethodName + "]" + "[Return] " + result);
            }

            if (callback != undefined) {
                callback(result)
            } else {
                returnData = result;
                if (async) {
                    return returnData;
                }
            }
        }
    });
    if (!async) {
        return returnData;
    }
}


function jsonGet(url, reqData, callback, async, isDebug) {
    //isDebug = true;

    var callerMethod = jsonGet.caller.toString();
    var re = /function\s*(\w*)/i;
    var callerMethodName = re.exec(callerMethod);

    var reqStr = JSON.stringify(reqData);
    var returnData;
    if (isDebug) {
        debug("[" + callerMethodName + "]" + "[Param] " + reqStr);
    }
    $.ajax({
        type: "GET",
        url:  url,
        data: reqData,
        contentType: 'application/json; charset=utf-8',
        async: async,
        success: function (result) {
            var resultStr = JSON.stringify(result);
            if (isDebug) {
                debug("[" + callerMethodName + "]" + "[Return] " + result);
            }

            if (callback != undefined) {
                callback(result)
            } else {
                returnData = result;
                if (async) {
                    return returnData;
                }
            }
        }
    });
    if (!async) {
        return returnData;
    }
}

Date.prototype.yyyymmdd = function () {
    var yyyy = this.getFullYear();
    var mm = this.getMonth() < 9 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1); // getMonth() is zero-based
    var dd = this.getDate() < 10 ? "0" + this.getDate() : this.getDate();
    return "".concat(yyyy).concat(mm).concat(dd);
};

Date.prototype.yyyymmddhhmm = function () {
    var yyyy = this.getFullYear();
    var mm = this.getMonth() < 9 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1); // getMonth() is zero-based
    var dd = this.getDate() < 10 ? "0" + this.getDate() : this.getDate();
    var hh = this.getHours() < 10 ? "0" + this.getHours() : this.getHours();
    var min = this.getMinutes() < 10 ? "0" + this.getMinutes() : this.getMinutes();
    return "".concat(yyyy).concat(mm).concat(dd).concat(hh).concat(min);
};

Date.prototype.yyyymmddhhmmss = function () {
    var yyyy = this.getFullYear();
    var mm = this.getMonth() < 9 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1); // getMonth() is zero-based
    var dd = this.getDate() < 10 ? "0" + this.getDate() : this.getDate();
    var hh = this.getHours() < 10 ? "0" + this.getHours() : this.getHours();
    var min = this.getMinutes() < 10 ? "0" + this.getMinutes() : this.getMinutes();
    var ss = this.getSeconds() < 10 ? "0" + this.getSeconds() : this.getSeconds();
    return "".concat(yyyy).concat(mm).concat(dd).concat(hh).concat(min).concat(ss);
};



