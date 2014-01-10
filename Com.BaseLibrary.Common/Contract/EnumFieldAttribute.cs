using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.BaseLibrary.Contract
{
	public class EnumDescriptionAttribute : Attribute
	{
		public string Text { get; set; }
		public string Value { get; set; }

		public EnumDescriptionAttribute(string text, string value)
		{
			this.Text = text;
			this.Value = value;
		}
        public EnumDescriptionAttribute(string text)
        {
            this.Text = text;
            this.Value = text;
        }
	}
}
