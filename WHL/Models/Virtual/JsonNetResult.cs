using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WHL.Models.Virtual
{



    public class JsonNetPackResult

    {

        public JsonNetPackResult(int flag, String message, Object data)
        {
            this.flag = flag;
            this.message = message;
            this.data = data;
        }

        public int flag { get; set; }

        public string message { get; set; }

        public Object data { get; set; }


    }
    /// <summary>
    /// jsonResult extension： the mvc json result not support reference loop handing. we extended it.
    /// Author: Pango
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };
        }

        public JsonNetResult(int flag, string message, Object data) {
            JsonNetPackResult resObject = new JsonNetPackResult(flag, message, data);
            Data = resObject;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
        }



        public JsonNetResult(Object data)
        {
            Data = data;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            var scriptSerializer = JsonSerializer.Create(this.Settings);

            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, this.Data);
                response.Write(sw.ToString());
            }
        }

        public string ToJsonString(){
         var scriptSerializer = JsonSerializer.Create(this.Settings);

            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, this.Data);
                return (sw.ToString());
            }
        }

        
    }
}