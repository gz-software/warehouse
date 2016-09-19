using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Stores;



namespace WHL.Models.Entity.Inventories
{
    /// <summary>
    /// Author: Pango
    /// 库存实体类： 放某个SKU的所有库存信息的主类。
    /// </summary>
    public class Inventory : BaseEntity
    {

        public int ProductID { get; set; }

        public int StoreID { get; set; }

        [LangDisplayName("Avail", ResourceName = "Inventory_Entity_Avail")]
        public int? Avail { get; set; }


        [LangDisplayName("SwAvail", ResourceName = "Inventory_Entity_SwAvail")]
        public int? SwAvail { get; set; }


        [LangDisplayName("OnIn", ResourceName = "Inventory_Entity_OnIn")]
        public int? OnIn { get; set; }


        [LangDisplayName("OnOut", ResourceName = "Inventory_Entity_OnOut")]
        public int? OnOut { get; set; }


        [LangDisplayName("InSum", ResourceName = "Inventory_Entity_InSum")]
        public int? InSum { get; set; }


        [LangDisplayName("OutSum", ResourceName = "Inventory_Entity_OutSum")]
        public int? OutSum { get; set; }

        [LangDisplayName("OnHold", ResourceName = "Inventory_Entity_OnHold")]
        public int? OnHold { get; set; }

        [LangDisplayName("DiffSum", ResourceName = "Inventory_Entity_DiffSum")]
        public int? DiffSum { get; set; }



        [LangDisplayName("ShipMiss", ResourceName = "Inventory_Entity_ShipMiss")]
        public int? ShipMiss { get; set; }



        public virtual Product Product { get; set; }

        public virtual Store Store { get; set; }

        [JsonIgnore]
        [NotMapped]
        public virtual int QueryAvail1 { get; set; }

        [JsonIgnore]
        [NotMapped]
        public virtual int QueryAvail2 { get; set; }

    }
}