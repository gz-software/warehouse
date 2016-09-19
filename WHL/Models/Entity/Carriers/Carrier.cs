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
using WHL.Models.Entity.Shipments.Epackets;

namespace WHL.Models.Entity.Carriers
{
    /// <summary>
    /// Author: Pango
    /// Carrier：entitis of shipping carriers. client O2N carrier.
    /// </summary>
    public class Carrier : BaseEntity
    {
        [Required]
        [StringLength(200)]
        [LangDisplayName("Carrier Name", ResourceName = "Carrier_Entity_Name")]
        public string Name { get; set; }


        [StringLength(1000)]
        [LangDisplayName("Description", ResourceName = "Carrier_Entity_Description")]
        public string Description { get; set; }


        public int ClientID { get; set; }

        public virtual Client Client { get; set; }

        [JsonIgnore]
        public virtual ICollection<EpacketAuth> EpacketAuthList { get; set; }


        public int Type { get; set; }

        public virtual dynamic Auth
        {

            get
            {
                return GetAuth(this.Type);
            }
            set
            {
                Auth = value;
            }
        }


        public dynamic GetAuth(int type)
        {

            switch (type)
            {
                case (int)CarrierTypeEnum.EPACKET:
                    return EpacketAuthList.First();
                default:
                    return null;
            }
        }

        // 这个虚拟字段决定这个Carrier是否可以用于Transfer, 就目前，只有Epacket不可用于Transfer, 它只能SellingOut.
        [LangDisplayName("Can Transfer?", ResourceName = "Carrier_Entity_CanTransfer")]
        [NotMapped]
        public virtual Boolean CanTransfer
        {
            get
            {
                if (this.Type == (int)CarrierTypeEnum.EPACKET)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set {

                CanTransfer = value;
            }
        }


        [LangDisplayName("Carrier Type", ResourceName = "Carrier_Entity_Type")]
        public virtual string TypeLayout
        {
            get
            {
                return GetTypeLayout(this.Type);
            }
        }

        public static string GetTypeLayout(int type)
        {
            switch (type)
            {
                case (int)CarrierTypeEnum.EPACKET:
                    return "China Ebay International";
                case (int)CarrierTypeEnum.DGH:
                    return "DHL global mail";
                case (int)CarrierTypeEnum.USPS:
                    return "The United States Postal Service";
                case (int)CarrierTypeEnum.FEDEX:
                    return "Fedex";
                case (int)CarrierTypeEnum.HKEXPRESS:
                    return "HONGKONG POST -EC-SHIP";
                default:
                    return "undefined";
            }
        }
    }


    /// <summary>
    /// Author: Pango
    /// ENUM for carriers Type
    /// </summary>
    public enum CarrierTypeEnum
    {
        EPACKET = 1,
        DGH = 2,
        USPS = 3,
        FEDEX = 4,
        HKEXPRESS = 5

    }
}