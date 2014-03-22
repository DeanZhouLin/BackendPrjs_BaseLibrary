using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Com.BaseLibrary.Utility
{
	public class UrlManager
	{
		public Dictionary<string, string> QueryString { get; set; }
		public string BaseAddress { get; set; }
		public UrlManager(string url)
		{
			QueryString = new Dictionary<string, string>();
			string[] parts = url.Split('?');
			BaseAddress = parts[0];
			if (parts.Length > 0)
			{
				string queryString = parts[1];
				string[] queryStringKeyValue = queryString.Split('&');
				foreach (string KeyValue in queryStringKeyValue)
				{
					string[] ss = KeyValue.Split('=');
					if (ss.Length == 2)
					{
						string key = ss[0];
						string val = ss[1];
						Add(key, val);

					}
				}
			}
		}

		public UrlManager Add(string key, string val)
		{
			if (QueryString.ContainsKey(key))
			{
				QueryString[key] = val;
			}
			else
			{
				QueryString.Add(key, val);
			}
			return this;
		}

		public UrlManager Remove(string name)
		{
			QueryString.Remove(name);
			return this;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(BaseAddress);
			if (QueryString.Count > 0)
			{
				sb.Append("?");
				foreach (string key in QueryString.Keys)
				{
					sb.AppendFormat("{0}={1}&", key, QueryString[key]);
				}
			}
			return sb.ToString().TrimEnd('&');

		}
	}
}
