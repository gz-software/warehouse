using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;

namespace WHL.Models.Entity.Deliveries
{
    /// <summary>
    /// Author: Pango
    /// 运单子货品实体类：这里放货运单里面一出一入的子货品数据，包括子货品，出入数量等的类。
    public class DeliveryItem:BaseEntity
    {
        public int ProductID { get; set; }


        public int DeliveryID { get; set; }


        [LangDisplayName("Inbound Quantity", ResourceName = "DeliveryItem_Entity_InQty")]
        public int InQty { get; set; }

         [LangDisplayName("Outbound Quantity", ResourceName = "DeliveryItem_Entity_OutQty")]
        public int OutQty { get; set; }


        public virtual Product Product { get; set; }

        public virtual Delivery Delivery { get; set; }
    }
}