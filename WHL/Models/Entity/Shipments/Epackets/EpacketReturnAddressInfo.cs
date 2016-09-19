using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Carriers;

using ePacket;

namespace WHL.Models.Entity.Shipments.Epacket
{
    public class EpacketReturnAddressInfo : ePacket.ReturnAddressInfo
    {

        public int ID { get; set; }


        public int? EpacketAddAPACPackageRequestID { get; set; }


        public virtual EpacketAddAPACPackageRequest EpacketAddAPACPackageRequest { get; set; }
    }
}