using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Inventories;


namespace WHL.Models.Entity.Stores
{

    /// <summary>
    /// 仓库实体类：无论智能仓还是实体仓，记录仓库基本信息。
    /// </summary>
    public class Store:BaseEntity
    {
        [Required]
        [StringLength(200)]
        [LangDisplayName("Store Name", ResourceName = "Store_Entity_Name")]
        public string Name { get; set; }

        // type: solid store or smart warehouse
        public int Type { get; set; }


        [StringLength(1500)]
        [LangDisplayName("Description", ResourceName = "Store_Entity_Description")]
        public string Description { get; set; }

        public int ClientID { get; set; }

        // lazy load 
        public Client Client { get; set; }

        // lazy load inventory list
        public ICollection<Inventory> InventoryList { get; set; }


        [LangDisplayName("Store Type", ResourceName = "Store_Entity_Type")]
        public virtual string TypeLayout
        {
            get
            {
                return GetTypeLayout(this.Type);
            }
        }


        public static string GetTypeLayout(int storeType)
        {
            switch (storeType)
            {
                case (int)StoreTypeEnum.SolidStore:
                    return LangHelper.Get("Store_Entity_Type_Enum_SolidStore");
                case (int)StoreTypeEnum.SmartWarehouse:
                    return LangHelper.Get("Store_Entity_Type_Enum_SmartWarehouse");
                default:
                    return "undefined";
            }
        }

    }

    /// <summary>
    /// 库存类型枚举：一种是普通仓，一种是智能仓
    /// </summary>
    public enum StoreTypeEnum
    {
        SolidStore = 1,
        SmartWarehouse = 2
    }
}