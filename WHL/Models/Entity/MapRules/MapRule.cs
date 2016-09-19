using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;

namespace WHL.Models.Entity.MapRules
{

    /// <summary>
    /// Author: Pango
    /// 导出入规则类：记录了导入/导出EXCEL的规则主类。
    /// </summary>
    public class MapRule:BaseEntity
    {

        [Required]
        [StringLength(200)]
        [LangDisplayName("Name", ResourceName = "MapRule_Entity_Name")]
        public string Name { get; set; }

        // import or export
        public int AssType { get; set; }

        // inventory or delivery
        public int DataType { get; set; }

        public int ClientID { get; set; }

        public virtual Client Client { get; set; }


        [StringLength(1500)]
        [LangDisplayName("Description", ResourceName = "MapRule_Entity_Description")]
        public string Description { get; set; }


        public virtual ICollection<MapRuleItem> RuleItemList { get; set; }


        // AssType : 处理类型
        [LangDisplayName("Ass Type", ResourceName = "MapRule_Entity_AssTypeLayout")]
        public virtual string AssTypeLayout
        {
            get
            {
                return GetAssTypeLayout(this.AssType );
            }
        }

        public static string GetAssTypeLayout(int AssType )
        {
            switch (AssType )
            {
                case (int)AssTypeEnum.Export:
                    return LangHelper.Get("MapRule_Entity_AssType_Enum_Export"); // 导出 Export
                case (int)AssTypeEnum.Import:
                    return LangHelper.Get("MapRule_Entity_AssType_Enum_Import"); // 导入 Import
                default:
                    return "undefined";
            }
        }


        [LangDisplayName("Data Type", ResourceName = "MapRule_Entity_DataTypeLayout")]
        public virtual string DataTypeLayout
        {
            get
            {
                return GetDataTypeLayout(this.DataType);
            }
        }

        // DataType: 数据类型
        public static string GetDataTypeLayout(int dataType)
        {
            switch (dataType)
            {
                case (int)DataTypeEnum.Inventory:
                    return LangHelper.Get("MapRule_Entity_DataType_Enum_Inventory"); // 库存记录 Inventory
                case (int)DataTypeEnum.Delivery:
                    return LangHelper.Get("MapRule_Entity_DataType_Enum_Delivery"); // 货运记录 Delivery
                default:
                    return "undefined";
            }
        }

    }


    /// <summary>
    /// Author: Pango
    /// 导入或导出的业务类型。
    /// </summary>
    public enum DataTypeEnum
    {
        Inventory = 1,
        Delivery = 2
    }

    /// <summary>
    /// Author: Pango
    /// 是导入还是导出。
    /// </summary>
    public enum AssTypeEnum
    {
        Export = 1,
        Import = 2
    }
}