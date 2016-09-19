using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WHL.Models.Virtual;


namespace WHL.Models.Entity.MapRules
{
    /// <summary>
    /// Author: Pango
    /// 导出入规则的可用列类
    /// </summary>
    public class MapRuleOption
    {
        [LangDisplayName("ID", ResourceName = "Base_Entity_ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        [LangDisplayName("Option Column", ResourceName = "MapRuleOption_Entity_Column")]
        public string Column { get; set; }

        [Required]
        [StringLength(200)]
        [LangDisplayName("Option Name", ResourceName = "MapRuleOption_Entity_Name")]
        public string Name { get; set; }

        [Required]
        [LangDisplayName("Is Grouping Column", ResourceName = "MapRuleOption_Entity_Grouping")]
        public bool Grouping{ get; set; }

        [Required]
        // inventory or delivery
        public int DataType { get; set; }
    }


}