using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.IO;

using WHL.DAL;
using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Stores;

namespace WHL.Services
{

    /// <summary>
    /// Store Service：we put all store biz logic handing here~
    /// Author: Pango
    /// </summary>
    public class StoreService : BaseService
    {
        /// <summary>
        /// Get Store LINQ Query List
        /// Notice: it will retun a LINQ Query only, not the real data, you should call "toList()" to get the real data.
        /// </summary>
        /// <param name="queryInventory">Query Store Object</param>
        /// <returns>LINQ Query List，you should call "toList()" to get the real data.</returns>
        public IQueryable<Store> GetStoreQueryList(Store queryStore)
        {

            var storeQueryList = from s in db.StoresDB select s;

            if (queryStore != null)
            {
                if (queryStore.ClientID > 0)
                {
                    storeQueryList = storeQueryList.Where(i => i.ClientID == queryStore.ClientID);
                }
            }
            return storeQueryList;
        }

        /// <summary>
        /// Get the store result in jquery data table format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Store Object</param>
        /// <returns>the DataTable Result with paging and sorting</returns>
        public DTResult<Store> GetStoreDataTableResult(DTParams dtParams, Store queryStore)
        {
            var queryList = GetStoreQueryList(queryStore);

            int count = queryList.Count();

            var data = new List<Store>();


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

            DTResult<Store> result = new DTResult<Store>
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