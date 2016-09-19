using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Stores;
using WHL.Models.Entity.Carriers;
using WHL.Models.Entity.Shipments;

namespace WHL.Models.Entity.Deliveries
{

    /// <summary>
    /// Author: Pango
    /// 货运单实体类：这里放货运单信息的主类。
    /// </summary>
    public class Delivery : BaseEntity
    {
        [Required]
        [StringLength(200)]
        [LangDisplayName("Deliver Order Number", ResourceName = "Delivery_Entity_DON")]
        public string DON { get; set; }

        // status: draft,submit,processing,delivering,badorder,cancel,error,success
        public int Status { get; set; }

        // shipping carrier
        public int? CarrierID { get; set; }

        public int? SrcStoreID { get; set; }

        public int? TarStoreID { get; set; }


        public int ClientID { get; set; }

        public int Version { get; set; }

        // 1.Transfer 2.Purchase In 3.Selling Out
        public int DeliveryType { get; set; }

        public virtual Client Client { get; set; }

        public virtual Store SrcStore { get; set; }


        public virtual Store TarStore { get; set; }

        public virtual ICollection<DeliveryItem> DeliveryItemList { get; set; }

        // lazy load delivery log list
        public ICollection<DeliveryLog> DeliveryLogList { get; set; }

        [JsonIgnore]
        public virtual ICollection<Shipment> ShipmentList { get; set; }
      

        [NotMapped]
        public virtual int QueryStatus1 { get; set; }
        [NotMapped]
        public virtual int QueryStatus2 { get; set; }

        
        public virtual Carrier Carrier { get; set; }

       [NotMapped]
        public virtual Shipment Shipment
        {
            get
            {
                if ((this.ShipmentList!=null)&&(this.ShipmentList.Count > 0)) {
                    return ShipmentList.First();
                }
                return null;
                

            }
            set
            {
                Shipment = value;
            }
        }


        [LangDisplayName("Source Store Name", ResourceName = "Delivery_Entity_SrcStoreNameLayout")]
        public virtual string SrcStoreNameLayout
        {
            get
            {
                if (this.SrcStore != null)
                {
                    return this.SrcStore.Name;
                }
                else
                {
                    return "undefined";
                }
            }
        }


        [LangDisplayName("Target Store Name", ResourceName = "Delivery_Entity_TarStoreNameLayout")]
        public virtual string TarStoreNameLayout
        {
            get
            {
                if (this.TarStore != null)
                {
                    return this.TarStore.Name;
                }
                else
                {
                    return "undefined";
                }
            }
        }

        [LangDisplayName("Status", ResourceName = "Delivery_Entity_Status")]
        public virtual string StatusLayout
        {
            get
            {
                return GetStatusLayout(this.Status);
            }
        }

        public static string GetStatusLayout(int status)
        {
            switch (status)
            {

                case (int)DeliveryStatusEnum.New:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_New");          // 新建货运单
                case (int)DeliveryStatusEnum.Submit:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Submited");     // 提交
                case (int)DeliveryStatusEnum.Processing:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Processing");   // 处理中
                case (int)DeliveryStatusEnum.Delivering:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Delivering");   // 运输中
                case (int)DeliveryStatusEnum.BadOrder:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_BadOrder");     // 坏单
                case (int)DeliveryStatusEnum.Cancel:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Cancel");       // 取消
                case (int)DeliveryStatusEnum.Received:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Received");     // 已收货
                case (int)DeliveryStatusEnum.Error:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Error");        // 异常
                case (int)DeliveryStatusEnum.Success:
                    return LangHelper.Get("Delivery_Entity_Status_Enum_Success");      // 成功
                default:
                    return "undefined";
            }
        }


        [LangDisplayName("Delivery Type", ResourceName = "Delivery_Entity_DeliveryType")]
        public virtual string DeliveryTypeLayout
        {
            get
            {
                return GetDeliveryTypeLayout(this.DeliveryType);
            }
        }

        public static string GetDeliveryTypeLayout(int deliveryType)
        {
            switch (deliveryType)
            {

                case (int)DeliveryTypeEnum.Transfer:
                    return LangHelper.Get("Delivery_Entity_Type_Enum_Transfer");       // 调拨
                case (int)DeliveryTypeEnum.PurchaseIn:
                    return LangHelper.Get("Delivery_Entity_Type_Enum_PurchaseIn");     // 购货入库
                case (int)DeliveryTypeEnum.SellingOut:
                    return LangHelper.Get("Delivery_Entity_Type_Enum_SellingOut");     // 销售出库
             
                default:
                    return "undefined";
            }
        }




    }

    /// <summary>
    /// Author: Pango
    /// ENUM for delivery status
    /// </summary>
    public enum DeliveryStatusEnum
    {
        New = 0,
        Submit = 1,
        Processing = 2,
        BadOrder = 3,
        Cancel = 4,
        Delivering = 5,
        Received = 6,
        Error = 7,
        Success = 8
    }


    /// <summary>
    /// Author: Pango
    /// ENUM for delivery type
    /// </summary>
    public enum DeliveryTypeEnum
    {
        Transfer = 1,
        PurchaseIn = 2,
        SellingOut = 3
    }
}