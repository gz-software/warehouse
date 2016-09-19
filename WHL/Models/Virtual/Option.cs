using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHL.Models.Virtual
{
    /// <summary>
    /// Simple Classs for option List in select of HTML
    /// </summary>
    public class Option
    {
        public Option(string value,string text)
        {
            this.Text = text;
            this.Value = value;
        }
        public string Value { get; set; }
        public string Text { get; set; }
    }
}