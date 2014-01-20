using System;

namespace Com.BaseLibrary.Contract
{
	public class EnumDescriptionAttribute : Attribute
	{
		public string Text { get; set; }
		public string Value { get; set; }

		public EnumDescriptionAttribute(string text, string value)
		{
			Text = text;
			Value = value;
		}
        public EnumDescriptionAttribute(string text)
        {
            Text = text;
            Value = text;
        }
	}
}
