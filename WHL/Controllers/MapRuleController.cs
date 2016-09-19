using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WHL.Services;
using WHL.Models.Virtual;
using WHL.Models.Entity.FileDatas;
using WHL.Models.Entity.MapRules;


namespace WHL.Controllers
{
    /// <summary>
    /// MapRuleController：we put all mapping rule view controllers here
    /// </summary>
    public class MapRuleController : BaseController
    {

        /// <summary>
        /// Get the mapRule list result in jquery data table JSON format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Inventory Object</param>
        /// <returns>JSON: DataTable Result with paging and sorting</returns>
        public JsonNetResult GetMapRuleDataTableJson(DTParams dtParams, MapRule queryMapRule)
        {
            DTResult<MapRule> mapRuleDataTableResult = mapRuleService.GetMapRuleDataTableResult(dtParams, queryMapRule);
            return new JsonNetResult(mapRuleDataTableResult);
        }


        /// <summary>
        /// get all mapping rules options.
        /// </summary>
        /// <returns>Json: map rule options List</returns>
        public JsonNetResult GetMapRuleOptionListJson(MapRuleOption queryMapRuleOption)
        {
            List<MapRuleOption> mapRuleOptionDataList = mapRuleService.GetMapRuleOptionDataList(queryMapRuleOption);
            return new JsonNetResult(mapRuleOptionDataList);
        }

      
    }
}