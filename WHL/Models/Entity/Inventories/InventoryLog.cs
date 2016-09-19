using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Deliveries;



namespace WHL.Models.Entity.Inventories
{
    /// <summary>
    /// Author: Pango
    /// 库存变更日志实体类：记录库存每次变化的发生来源，以及变化前后之数量。
    /// </summary>
    public class InventoryLog:BaseEntity
    {

        /// <summary>
        /// the constructor which easy to set the inv1 and inv2 data
        /// </summary>
        /// <param name="inv1"></param>
        /// <param name="inv2"></param>
        public InventoryLog(Inventory inv1,Inventory inv2) {
            
            this.Avail1 = inv1.Avail;
            this.Avail2 = inv2.Avail;

            this.OnHold1 = inv1.OnHold;
            this.OnHold2 = inv2.OnHold;

            this.InSum1 = inv1.InSum;
            this.InSum2 = inv2.InSum;

            this.OutSum1 = inv1.OutSum;
            this.OutSum2 = inv2.OutSum;

            this.OnIn1 = inv1.OnIn;
            this.OnIn2 = inv2.OnIn;

            this.OnOut1 = inv1.OnOut;
            this.OnOut2 = inv2.OnOut;

            this.swAvail1 = inv1.SwAvail;
            this.SwAvail2 = inv2.SwAvail;

            this.ShipMiss1 = inv1.ShipMiss;
            this.ShipMiss2 = inv2.ShipMiss;

            this.DiffSum1 = inv1.DiffSum;
            this.DiffSum2 = inv2.DiffSum;
        }


        // import or delivery or adjustment 
        public int ChangeType { get; set; }


        public int InventoryID { get; set; }

        public int? DeliveryLogID { get; set; }

        // 下面记录了改变前和改变后各样数值

        [LangDisplayName("From Avail", ResourceName = "Inventory_Entity_Log_Avail1")]
        public int? Avail1 { get; set; }

        [LangDisplayName("To Avail", ResourceName = "Inventory_Entity_Log_Avail2")]
        public int? Avail2 { get; set; }

        [LangDisplayName("From OnHold", ResourceName = "Inventory_Entity_Log_OnHold")]
        public int? OnHold1 { get; set; }

        [LangDisplayName("To OnHold", ResourceName = "Inventory_Entity_Log_OnHold2")]
        public int? OnHold2 { get; set; }


        [LangDisplayName("From SwAvail", ResourceName = "Inventory_Entity_Log_SwAvail1")]
        public int? swAvail1 { get; set; }

        [LangDisplayName("To SwAvail", ResourceName = "Inventory_Entity_Log_SwAvail2")]
        public int? SwAvail2 { get; set; }


        [LangDisplayName("From OnIn", ResourceName = "Inventory_Entity_Log_OnIn1")]
        public int? OnIn1 { get; set; }

        [LangDisplayName("To OnIn", ResourceName = "Inventory_Entity_Log_OnIn2")]
        public int? OnIn2 { get; set; }


        [LangDisplayName("From OnOut", ResourceName = "Inventory_Entity_Log_OnOut1")]
        public int? OnOut1 { get; set; }

        [LangDisplayName("To OnOut", ResourceName = "Inventory_Entity_Log_OnOut2")]
        public int? OnOut2 { get; set; }


        [LangDisplayName("From InSum", ResourceName = "Inventory_Entity_Log_InSum1")]
        public int? InSum1 { get; set; }

        [LangDisplayName("To InSum", ResourceName = "Inventory_Entity_Log_InSum2")]
        public int? InSum2 { get; set; }


        [LangDisplayName("From OutSum", ResourceName = "Inventory_Entity_Log_OutSum1")]
        public int? OutSum1 { get; set; }

        [LangDisplayName("To OutSum", ResourceName = "Inventory_Entity_Log_OutSum2")]
        public int? OutSum2 { get; set; }



        [LangDisplayName("From diffSum", ResourceName = "Inventory_Entity_Log_DiffSum1")]
        public int? DiffSum1 { get; set; }


        [LangDisplayName("To DiffSum", ResourceName = "Inventory_Entity_Log_DiffSum2")]
        public int? DiffSum2 { get; set; }



        [LangDisplayName("From ShipMiss", ResourceName = "Inventory_Entity_Log_ShipMiss1")]
        public int? ShipMiss1 { get; set; }

        [LangDisplayName("To ShipMiss", ResourceName = "Inventory_Entity_Log_ShipMiss2")]
        public int? ShipMiss2 { get; set; }


        public virtual Inventory Inventory { get; set; }

        public virtual DeliveryLog DeliveryLog{ get; set; }


        [LangDisplayName("Type", ResourceName = "Inventory_Entity_Log_ChangeType")]
        public string ChangeTypeLayout
        {
            get
            {
                return GetChangeTypeLayout(this.ChangeType);
            }
        }

        public static string GetChangeTypeLayout(int changeType)
        {
            switch (changeType)
            {
                case (int)InventoryLogTypeEnum.Delivery:
                    return LangHelper.Get("InventoryLog_Entity_Type_Enum_Delivery");          // 由货运单影响： Updated by Delivery
                case (int)InventoryLogTypeEnum.Adjustment:
                    return LangHelper.Get("InventoryLog_Entity_Type_Enum_Adjustment");        // 由库存调整影响:Updated by Adjustment
                case (int)InventoryLogTypeEnum.Import:
                    return LangHelper.Get("InventoryLog_Entity_Type_Enum_Import");            // 由导入影响：Updated by Import 
                default:
                    return "undefined";
            }
        }
    }



    /// <summary>
    /// Author: Pango
    /// 日志类型，记录造成变化的来源，包括： 货运，库存调整，导入
    /// </summary>
    public enum InventoryLogTypeEnum
    {
        Delivery = 1,
        Adjustment = 2,
        Import = 3
    }
}