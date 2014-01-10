using System;

namespace Com.BaseLibrary.Web
{
	public enum StringStartedSymbol
	{
		SingleQuotes,
		DoubleQuotes,

	}
	public static class JavascriptUtility
	{
		public static string StringEncoding(string value, StringStartedSymbol symbol)
		{
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Replace("\\", "\\\\");
				value = value.Replace("&", "\\&");

				value = value.Replace("\n", "\\\n");
				value = value.Replace("\r", "\\\r");
				value = value.Replace("\t", "\\\t");
				value = value.Replace("\b", "\\\b");
				value = value.Replace("\f", "\\\f");
				switch (symbol)
				{
					case StringStartedSymbol.SingleQuotes:
						value = value.Replace("'", "\\'");
						break;
					case StringStartedSymbol.DoubleQuotes:
						value = value.Replace("\"", "\\\"");
						break;
				}
			}
			return value;
		}
	}
}
