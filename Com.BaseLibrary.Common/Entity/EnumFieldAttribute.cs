using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.BaseLibrary.Entity
{
	public class EnumFieldAttribute : Attribute
	{
		public string Text { get; set; }
		public string Value { get; set; }

		public EnumFieldAttribute(string text, string value)
		{
			this.Text = text;
			this.Value = value;
		}
        public EnumFieldAttribute(string text)
        {
            this.Text = text;
            this.Value = text;
        }
	}
}
