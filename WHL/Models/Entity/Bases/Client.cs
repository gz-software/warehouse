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
    /// 客户公司类：届时由接口创建公司
    /// </summary>
    public class Client 
    {

        [LangDisplayName("ID", ResourceName = "Base_Entity_ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        [LangDisplayName("Client Name", ResourceName = "Client_Name")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

        [Required]
        [LangDisplayName("Version", ResourceName = "Client_Entity_Version")]
        public int Version { get; set; }

        [LangDisplayName("Update Date", ResourceName = "Base_Entity_UpdateDateLayout")]
        public virtual string UpdateDateLayout
        {
            get
            {
                return this.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}