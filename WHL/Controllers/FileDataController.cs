using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

using WHL.Models.Virtual;
using WHL.Models.Entity.FileDatas;


namespace WHL.Controllers
{
    public class FileDataController : BaseController
    {
        /// <summary>
        /// Get the process type enum  
        /// </summary>
        /// <returns></returns>
        public JsonNetResult GetProcessTypeListJson()
        {

            List<Option> optList = new List<Option>();
            foreach (int value in Enum.GetValues(typeof(FileProcessTypeEnum)))
            {
                string strName = FileData.GetProcessTypeLayout(value);
                string strValue = value.ToString();
                Option opt = new Option(strValue, strName);
                optList.Add(opt);
            }
            return new JsonNetResult(optList);
        }



        /// <summary>
        /// Get the file type enum  
        /// </summary>
        /// <returns></returns>
        public JsonNetResult GetFileTypeListJson()
        {

            List<Option> optList = new List<Option>();
            foreach (int value in Enum.GetValues(typeof(FileTypeEnum)))
            {
                string strName = FileData.GetFileTypeLayout(value);
                string strValue = value.ToString();
                Option opt = new Option(strValue, strName);
                optList.Add(opt);
            }
            return new JsonNetResult(optList);
        }


    }
}