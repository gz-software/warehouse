using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WHL.Services;
using WHL.Models.Virtual;
using WHL.Models.Entity.Stores;

namespace WHL.Controllers
{
    /// <summary>
    /// StoreController：we put all store view controllers here
    /// </summary>
    public class StoreController : BaseController
    {
        /// <summary>
        /// Get the store list result in jquery data table JSON format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Store Object</param>
        /// <returns>JSON: DataTable Result with paging and sorting</returns>
        public JsonNetResult GetStoreDataTableJson(DTParams dtParams, Store queryStore)
        {
            DTResult<Store> storeDataTableResult = storeService.GetStoreDataTableResult(dtParams, queryStore);
            return new JsonNetResult(storeDataTableResult);
        }
    }
}