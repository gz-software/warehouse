using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHL.Models.Virtual
{
    /// <summary>
    /// 这个类用于对外部接口返回的一种常态结构
    /// </summary>
    public class JsonApiReturnData
    {

        public JsonApiReturnData(dynamic apiRequest, dynamic apiResponse, dynamic bizData) {
            this.ApiRequest = apiRequest;
            this.ApiResponse = apiResponse;
            this.BizData = bizData;
        }

        // 接口请求的数据
        public dynamic ApiRequest { get; set; }

        // 接口返回的数据
        public dynamic ApiResponse { get; set; }

        // 返回的业务数据
        public dynamic BizData { get; set; }
    }
}