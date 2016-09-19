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
    /// 用户实体类： 记录用户信息
    /// </summary>
    public class Employee:BaseEntity
    {
      

        [Required]
        [StringLength(200)]
        [LangDisplayName("Employee Name", ResourceName = "Employee_Entity_Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        [LangDisplayName("Employee Code", ResourceName = "Employee_Entity_Code")]
        public string Code { get; set; }

        public int Language { get; set; }

        public int ClientID { get; set; }

        [Required]
        [LangDisplayName("Version", ResourceName = "Employee_Entity_Version")]
        public int Version { get; set; }

        // lazy load 
        public Client Client { get; set; }

      
    }

    /// <summary>
    /// 语言类型枚举：英语，简体，繁体，泰文
    /// </summary>
    public enum LanguageEnum
    {
        en_US = 0,
        zh_CN = 1,
        zh_HK = 2,
        th_TH = 3
    }
}