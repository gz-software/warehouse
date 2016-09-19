using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;


using WHL.DAL;
using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Carriers;

namespace WHL.Services
{
    /// <summary>
    /// Carrier Service：we put all delivery carrier biz logic here~
    /// Author: Pango
    /// </summary>
    public class CarrierService : BaseService
    {
        /// <summary>
        /// Get Carrier LINQ Query List
        /// Notice: it will retun a LINQ Query only, not the real data, you should call "toList()" to get the real data.
        /// </summary>
        /// <param name="queryInventory">Query Carrier Object</param>
        /// <returns>LINQ Query List，you should call "toList()" to get the real data.</returns>
        public IQueryable<Carrier> GetCarrierQueryList(Carrier queryCarrier)
        {

            var carrierQueryList = from s in db.CarriersDB select s;

            if (queryCarrier != null)
            {
                if (queryCarrier.ClientID > 0)
                {
                    carrierQueryList = carrierQueryList.Where(i => i.ClientID == queryCarrier.ClientID);
                }
            }
            return carrierQueryList;
        }


        /// <summary>
        /// Get the carriers result in jquery data table format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Carrier Object</param>
        /// <returns>the DataTable Result with paging and sorting</returns>
        public DTResult<Carrier> GetCarrierDataTableResult(DTParams dtParams, Carrier queryCarrier)
        {
            var queryList = GetCarrierQueryList(queryCarrier);

            int count = queryList.Count();

            var data = new List<Carrier>();


            string sortOrder = "";

            if ((dtParams == null) || (dtParams.SortOrder == null))
            {   // 如果不是从界面进来的，是接口来的，就没有dtParams
                dtParams.Start = 0;
                dtParams.Length = count;
                dtParams.Order = null;
                sortOrder = "ID";
            }
            else
            {
                for (int i = 0; i < dtParams.Order.Length; i++)
                {
                    var order = dtParams.Order[i].Column;
                    var sort = dtParams.Order[i].Dir;
                    var thenByStr = dtParams.Columns[order].Data.Replace("Layout", "");
                    sortOrder += thenByStr + " " + sort + ",";
                }

                sortOrder = sortOrder.Substring(0, sortOrder.Length - 1);

            }

            data = queryList.OrderBy(sortOrder).Skip(dtParams.Start).Take(dtParams.Length).ToList();

            DTResult<Carrier> result = new DTResult<Carrier>
            {
                flag = SUCCESS,             // return call flag
                message = "Call Success",   // return call message
                draw = dtParams.Draw,       // how many time it have been call for this method
                data = data,                // the data of datatable
                recordsFiltered = count,    // records filter count
                recordsTotal = count        // total records count
            };

            return result;
        }

    }
}