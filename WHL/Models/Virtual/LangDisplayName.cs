using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Threading;
using System.Reflection;
using WHL.Resources;
using WHL.Helpers;

namespace WHL.Models.Virtual
{  
    /// <summary>
    /// DisplayNameAttribute Extenstion:  we  extened the mvc display name attribute for support multilanguage
    /// Author: Pango
    /// </summary>
    public class LangDisplayName : DisplayNameAttribute
    {
        private string _defaultName = "";
        public Type ResourceType
        {
            get { return LangHelper.GetLangType(); }
        }
        public string ResourceName
        {
            get;
            set;
        }
        public LangDisplayName(string defaultName)
        {
            _defaultName = defaultName;
        }
        public override string DisplayName
        {
            get
            {
                PropertyInfo p = ResourceType.GetProperty(ResourceName);
                if (p != null)
                    return p.GetValue(null, null).ToString();
                else
                    return _defaultName;
            }
        }
    }
}