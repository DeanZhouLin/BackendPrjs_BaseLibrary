using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// XML工具类
    /// </summary>
	public static class XmlUtil
	{
		#region construct an xml document from xml format string

        /// <summary>
        /// 把字符串转换成一个XmlDocument对象返回
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
		public static XmlDocument LoadXmlFromString(string xmlString)
		{
			try
			{
				XmlDocument xmlBody = new XmlDocument();
				xmlBody.LoadXml(xmlString);
				return xmlBody;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region decode & encode

        /// <summary>
        /// Decode XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public static string XmlDecode(string data)
		{
			return
				Regex.Replace(
					Regex.Replace(
						Regex.Replace(
							Regex.Replace(Regex.Replace(data, "&apos;", "'", RegexOptions.IgnoreCase), "&quot;", "\"",
							              RegexOptions.IgnoreCase), "&lt;", "<", RegexOptions.IgnoreCase), "&gt;", ">",
						RegexOptions.IgnoreCase), "&amp;", "&", RegexOptions.IgnoreCase);
		}

        /// <summary>
        /// Encode XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public static string XmlEncode(string data)
		{
			return
				data.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;");
		}

		#endregion
	}
}