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
    /// 货运单日志实体类：这里放货运单发生状态变更时，处理人和状态变化和时间的前后记录运单日志类。
    public class DeliveryLog : BaseEntity
    {

        public int DeliveryID { get; set; }

        // change from status
        public int Status1{ get; set; }

        // change to status
        public int Status2{ get; set; }

        [StringLength(1500)]
        [LangDisplayName("Description", ResourceName = "Delivery_Entity_Log_Description")]
        public string Description { get; set; }

        public virtual Delivery Delivery { get; set; }



        [LangDisplayName("From Status", ResourceName = "Deliver_Entity_Log_Status1")]
        public virtual string Status1Layout
        {
            get
            {
                return Delivery.GetStatusLayout(this.Status1);
            }
        }



        [LangDisplayName("To Status", ResourceName = "Deliver_Entity_Log_Status2")]
        public virtual string Status2Layout
        {
            get
            {
                return Delivery.GetStatusLayout(this.Status2);
            }
        }


     

    }
}