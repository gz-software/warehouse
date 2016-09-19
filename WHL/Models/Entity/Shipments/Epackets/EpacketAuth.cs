using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Carriers;

namespace WHL.Models.Entity.Shipments.Epackets
{
    /// <summary>
    /// Author: Pango
    /// AuthEpacket：the auth of Epacket, AuthEpacket O2O Carrier
    /// </summary>
    public class EpacketAuth:BaseEntity
    {
        [StringLength(300)]
        [LangDisplayName("AppID", ResourceName = "EpacketAuth_Entity_AppID")]
        public string AppID { get; set; }


        [StringLength(300)]
        [LangDisplayName("AppCert", ResourceName = "EpacketAuth_Entity_AppCert")]
        public string AppCert { get; set; }


        [StringLength(300)]
        [LangDisplayName("APIDevUserID", ResourceName = "EpacketAuth_Entity_APIDevUserID")]
        public string APIDevUserID { get; set; }
        
        [StringLength(300)]
        [LangDisplayName("Version", ResourceName = "EpacketAuth_Entity_Version")]
        public string Version { get; set; }

        [StringLength(300)]
        [LangDisplayName("APISellerUserID", ResourceName = "EpacketAuth_Entity_APISellerUserID")]
        public string APISellerUserID { get; set; }

        [StringLength(2000)]
        [LangDisplayName("APISellerUserToken", ResourceName = "EpacketAuth_Entity_APISellerUserToken")]
        public string APISellerUserToken { get; set; }
        
        [StringLength(300)]
        [LangDisplayName("URL", ResourceName = "EpacketAuth_Entity_URL")]
        public string URL { get; set; }

        [StringLength(100)]
        [LangDisplayName("Carrier", ResourceName = "EpacketAuth_Entity_Carrier")]
        public string Carrier { get; set; }

        public int CarrierID { get; set; }

    }
}