using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Stores;
using WHL.Models.Entity.Carriers;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Shipments.Epackets;

namespace WHL.Models.Entity.Shipments
{
    public class Shipment:BaseEntity
    {

        public int DeliveryID { get; set; }

        public virtual Delivery Delivery { get; set; }

        [StringLength(300)]
        public string TrackCode { get; set; }

        [JsonIgnore]
        public virtual ICollection<EpacketShipData> EpacketShipmentList { get; set; }


        public virtual dynamic ShipData
        {

            get
            {
                if (this.Delivery.Carrier != null)
                {
                    return GetShipData(this.Delivery.Carrier.Type);
                }
                else
                {
                    return null;
                }

            }
            set
            {
                ShipData = value;
            }
        }

        public dynamic GetShipData(int type)
        {

            switch (type)
            {
                case (int)CarrierTypeEnum.EPACKET:
                    return EpacketShipmentList.First();
                default:
                    return null;
            }
        }
    }
}