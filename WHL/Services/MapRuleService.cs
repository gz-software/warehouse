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
using WHL.Models.Entity.FileDatas;
using WHL.Models.Entity.MapRules;

namespace WHL.Services
{
    public class MapRuleService: BaseService
    {
        /// <summary>
        /// Get Mapping Rule LINQ Query List
        /// Notice: it will retun a LINQ Query only, not the real data, you should call "toList()" to get the real data.
        /// </summary>
        /// <param name="queryMapRule">Map Rule Object</param>
        /// <returns>LINQ Query List，you should call "toList()" to get the real data.</returns>
        public IQueryable<MapRule> GetMapRuleQueryList(MapRule queryMapRule)
        {
            var mapRuleQueryList = from s in db.MapRulesDB select s;
            if (queryMapRule != null)
            {
                if (queryMapRule.ClientID > 0)
                {
                    mapRuleQueryList = mapRuleQueryList.Where(i => i.ClientID == queryMapRule.ClientID);
                }
                if (queryMapRule.DataType > 0)
                {
                    mapRuleQueryList = mapRuleQueryList.Where(i => i.DataType == queryMapRule.DataType);
                }
                if (queryMapRule.AssType > 0)
                {
                    mapRuleQueryList = mapRuleQueryList.Where(i => i.AssType == queryMapRule.AssType);
                }
            }
            return mapRuleQueryList;
        }


        /// <summary>
        /// Get the map rule result in jquery data table format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query MapRule Object</param>
        /// <returns>the DataTable Result with paging and sorting</returns>
        public DTResult<MapRule> GetMapRuleDataTableResult(DTParams dtParams, MapRule queryMapRule)
        {
            var queryList = GetMapRuleQueryList(queryMapRule);

            int count = queryList.Count();

            var data = new List<MapRule>();


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

            DTResult<MapRule> result = new DTResult<MapRule>
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


        /// <summary>
        /// Get the map rule option data list
        /// </summary>
        public List<MapRuleOption> GetMapRuleOptionDataList(MapRuleOption queryMapRuleOption) {
            var mapRuleOptionQueryList = from s in db.MapRuleOptionsDB select s;
            if (queryMapRuleOption != null)
            {
                if (queryMapRuleOption.DataType > 0)
                {
                    mapRuleOptionQueryList = mapRuleOptionQueryList.Where(i => i.DataType == queryMapRuleOption.DataType);
                }
            }

            return mapRuleOptionQueryList.ToList();
        }
    }
}