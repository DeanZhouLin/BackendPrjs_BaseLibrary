
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;


using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Web
{
	public class QueryStringBuilder
	{
		private Dictionary<string, string> m_QueryStringCollection;

		public QueryStringBuilder()
		{
			m_QueryStringCollection = new Dictionary<string, string>(new StringEqualityComparer());
		}

		public QueryStringBuilder(NameValueCollection queryString)
			: this()
		{
			if (queryString != null)
			{
				foreach (string name in queryString.AllKeys)
				{
					m_QueryStringCollection.Add(name, queryString[name]);
				}
			}
		}

		/// <summary>
		/// Adds the query string.
		/// If the specified name already exists, the previous value will be replaced.
		/// </summary>
		/// <param name="paramName">Name of the param.</param>
		/// <param name="paramValue">The param value.</param>
		public void AddQueryString(string paramName, string paramValue)
		{
			m_QueryStringCollection[paramName] = paramValue;
		}

		/// <summary>
		/// Removes the query string.
		/// </summary>
		/// <param name="paramName">Name of the param.</param>
		public void RemoveQueryString(string paramName)
		{
			m_QueryStringCollection.Remove(paramName);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// Note: the value has been url-encoded.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, string> pair in m_QueryStringCollection)
			{
				if (StringUtil.IsNullOrEmpty(pair.Value))
				{
					continue;
				}
				sb.Append(HttpUtility.UrlEncodeUnicode(pair.Key));
				sb.Append("=");
				sb.Append(HttpUtility.UrlEncodeUnicode(pair.Value));
				sb.Append("&");
			}
			return sb.ToString().TrimEnd('&');
		}

		public NameValueCollection ToQueryString()
		{
			NameValueCollection nv = new NameValueCollection();
			foreach (KeyValuePair<string, string> pair in m_QueryStringCollection)
			{
				if (StringUtil.IsNullOrEmpty(pair.Value))
				{
					continue;
				}
				nv.Add(pair.Key, pair.Value);
			}
			return nv;
		}

		public string[] ToArray()
		{
			int len = m_QueryStringCollection.Count * 2;
			string[] ss = new string[len];
			int i = 0;
			foreach (KeyValuePair<string, string> pair in m_QueryStringCollection)
			{

				ss[i] = pair.Key;
				ss[i + 1] = pair.Value;
				i = i + 2;
			}
			return ss;
		}

		/// <summary>
		/// Gets the number of parameters.
		/// </summary>
		/// <value>The parameter count.</value>
		public int ParameterCount
		{
			get
			{
				return m_QueryStringCollection.Count;
			}
		}

		public string this[string key]
		{
			get
			{
				if (m_QueryStringCollection.ContainsKey(key))
				{
					return m_QueryStringCollection[key];
				}
				return string.Empty;
			}
		}
	}
}