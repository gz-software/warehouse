using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;


namespace WHL.Models.Entity.MapRules
{
    /// <summary>
    /// Author: Pango
    /// 导出入规则子元素类：这里放每一个出入规则的列名，界面名，排序等等。
    /// </summary>
    public class MapRuleItem:BaseEntity
    {
        public int MapRuleID { get; set; }

        public int MapRuleOptionID { get; set; }


        // not map this field,just for UI request and response
        [NotMapped]
        [LangDisplayName("Item Column", ResourceName = "MapRuleItem_Entity_Column")]
        public string Column { get; set; }
     
        [Required]
        [StringLength(200)]
        [LangDisplayName("Item Name", ResourceName = "MapRuleItem_Entity_Name")]
        public string Name { get; set; }

        [LangDisplayName("Sorting", ResourceName = "MapRuleItem_Entity_Sorting")]
        public int Sorting { get; set; }

      
        public virtual MapRuleOption MapRuleOption { get; set; }


        public virtual MapRule MapRule { get; set; }

    }
}