using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WHL.Services;
using WHL.Models.Virtual;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Carriers;

namespace WHL.Controllers
{
    /// <summary>
    /// CarrierController：we put all Carrier view controllers here
    /// </summary>
    public class CarrierController : BaseController
    {
        /// <summary>
        /// Get the carrier list result in jquery data table JSON format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Carrier Object</param>
        /// <returns>JSON: DataTable Result with paging and sorting</returns>
        public JsonNetResult GetCarrierDataTableJson(DTParams dtParams, Carrier queryCarrier)
        {
            DTResult<Carrier> carrierDataTableResult = carrierService.GetCarrierDataTableResult(dtParams, queryCarrier);
            return new JsonNetResult(carrierDataTableResult);
        }
    }
}