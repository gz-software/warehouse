using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.OleDb;

using WHL.Helpers;
using WHL.Services;

using WHL.Models.Virtual;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.FileDatas;

namespace WHL.Controllers
{
    /// <summary>
    /// InventoryController：we put all inventory view controllers here
    /// </summary>
    public class InventoryController : BaseController
    {


        /// <summary>
        /// Query partial view page
        /// </summary>
        /// <returns></returns>
        public ActionResult Query()
        {
            
            return PartialView();
        }

        /// <summary>
        /// List partial view page
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return PartialView();
        }


        /// <summary>
        /// Get the inventory list result in jquery data table JSON format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Inventory Object</param>
        /// <returns>JSON: DataTable Result with paging and sorting</returns>
        public JsonNetResult GetInventoryDataTableJson(DTParams dtParams, Inventory queryInventory)
        {
            DTResult<Inventory> inventoryDataTableResult = inventoryService.GetInventoryDataTableResult(dtParams, queryInventory);
            return new JsonNetResult(inventoryDataTableResult);
        }


        // EXPORT: Export Inventory List
        public JsonNetResult ExportJson(FileData submitFileData,Inventory eff)
        {
            JsonNetPackResult packResult = null;
            packResult = inventoryService.Export(submitFileData, eff);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Json upload an excel file and import to db
        /// </summary>
        /// <param name="uploadFile">the excel upload file</param>
        /// <param name="mapRule">the map rule of import</param>
        /// <returns></returns>
        public JsonNetResult ImportJson(FileData submitFileData, int eff)
        {
            JsonNetPackResult packResult = inventoryService.Import(submitFileData,eff);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// close the database
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inventoryService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
