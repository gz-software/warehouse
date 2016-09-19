using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ePacket.AddPackage;

namespace WHL.Models.Entity.Shipments.Epacket
{
    public class EpacketAddAPACPackageRequest : ePacket.AddPackage.AddAPACPackageRequest
    {
        public AddAPACPackageRequest GetBase() {
            return (AddAPACPackageRequest)this;
        }

        public int ID { get; set; }

        public new virtual EpacketAuth Auth { get; set; }
        
        public new virtual ICollection<EpacketPickUpAddressInfo> PickUpAddress { get; set; }
        public new virtual ICollection<EpacketReturnAddressInfo> ReturnAddress { get; set; }

        public new virtual ICollection<EpacketItem> ItemList { get; set; }

        public new virtual ICollection<EpacketShipFromAddressInfo> ShipFromAddress { get; set; }
        public new virtual ICollection<EpacketShipToAddressInfo> ShipToAddress { get; set; }
         
    }
}