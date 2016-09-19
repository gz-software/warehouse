using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ePacket;
using ePacket.AddPackage;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Carriers;

namespace WHL.Models.Entity.Shipments.Epackets
{
    public class EpacketShipData : BaseEntity
    {
       
  
        [LangDisplayName("ShipData", ResourceName = "EpacketShipData_Entity_ShipData")]
        public string PackageStreamData { get; set; }


        public int ShipmentID { get; set; }

        public virtual Shipment Shipment { get; set; }

        [NotMapped]
        public virtual ePacket.AddPackage.AddAPACPackageRequest PackageData{
            get{

                if (!String.IsNullOrEmpty(PackageStreamData))
                {
                    return (AddAPACPackageRequest)JsonConvert.DeserializeObject(this.PackageStreamData, typeof(AddAPACPackageRequest));
                }
                else {

                    return null;
                }
                

            }set{
                PackageData = value;
            }
        }
    }
}