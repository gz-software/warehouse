using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WHL.Models.Virtual;

namespace WHL.Models.Entity.Bases
{
    /// <summary>
    /// Author: Pango
    /// 产品实体类：记录了产品的数据，理论上由外部接口塞进产品数据的主类。
    /// </summary>
    public class Product:BaseEntity
    {
        [Required]
        [StringLength(100)]
        [LangDisplayName("SKU", ResourceName = "Product_Entity_SKU")]
        public string SKU { get; set; }


        [StringLength(100)]
        [LangDisplayName("MPN", ResourceName = "Product_Entity_MPN")]
        public string MPN { get; set; }




        [Required]
        [StringLength(400)]
        [LangDisplayName("Title", ResourceName = "Product_Entity_ShortTitle")]
        public string ShortTitle { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [LangDisplayName("Price", ResourceName = "Product_Entity_Price")]
        public decimal Price { get; set; }

        [StringLength(1500)]
        [LangDisplayName("Full Title", ResourceName = "Product_Entity_Title")]
        public string Title { get; set; }


        [StringLength(200)]
        [LangDisplayName("MainImage", ResourceName = "Product_Entity_MainImage")]
        public string MainImage { get; set; }

        [Required]
        [LangDisplayName("Version", ResourceName = "Product_Entity_Version")]
        public int Version{ get; set; }

        public int? ClientID { get; set; }

        // lazy load
        public Client Client { get; set; }

    }
}