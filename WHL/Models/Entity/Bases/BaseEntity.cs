using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

using WHL.Models.Virtual;


namespace WHL.Models.Entity.Bases

{
    /// <summary>
    /// Author: Pango
    /// 底层类： 所有实体类的底层类，用于放每张表几乎公共的属性。
    /// </summary>
    public class BaseEntity
    {
        [LangDisplayName("ID", ResourceName = "Base_Entity_ID")]
        public int ID { get; set; }

        public int? UpdateEmployeeID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

        [LangDisplayName("Update Date", ResourceName = "Base_Entity_UpdateDateLayout")]
        public virtual string UpdateDateLayout
        {
            get
            {
                return this.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }


        public virtual Employee UpdateEmployee { get; set; }


        [NotMapped]
        [JsonIgnore]
        public virtual DateTime QueryUpdateDate1 { get; set; }

        [NotMapped]
        [JsonIgnore]
        public virtual DateTime QueryUpdateDate2 { get; set; }


        /// <summary>
        /// generate js enum resources in all entityies
        /// </summary>
        /// <returns></returns>
        public static string GenerateJsEnumResource()
        {
            String jsStr = "";
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type t in asm.GetTypes())
            {
                if ((t.Namespace!=null)&&(t.Namespace.ToString().IndexOf("WHL.Models.Entity")>-1))
                {
                    if (t.BaseType == typeof(Enum))
                    {
                        jsStr += "var " + t.Name + "={}; ";
                        foreach (int value in Enum.GetValues(t))
                        {
                            string strName = Enum.GetName(t, value);//获取名称
                            string strValue = value.ToString();//获取值
                            jsStr += "" + t.Name + "." + strName + "=" + strValue + ";  ";
                        }
                    }
                }
            }
            return jsStr;
        }


        /// <summary>
        /// Clone an entity object
        /// </summary>
        /// <returns></returns>
        public BaseEntity Clone()
        {
            var type = this.GetType().BaseType;
            var clone = Activator.CreateInstance(type);

            foreach (var property in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty))
            {
                if (property.CanWrite)
                {
                    property.SetValue(clone, property.GetValue(this, null), null);
                }
            }
            return (BaseEntity)clone;
        }

    }

}