using System;
using System.Xml.Serialization;
using Com.BaseLibrary.Collection;
using Com.BaseLibrary.Caching;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Configuration;



namespace Com.BaseLibrary.Web.Coockie
{
	[XmlRoot("cookieConfiguration")]
	public class CookieConfiguration
	{

		[XmlElement("cookie")]
		public KeyedList<string, CookieEntryInfo> CookieList { get; set; }
		public static readonly ICacheManager CacheManager = CacheManagerFactory.CreateAspNetCacheManager();
		private const string CookieConfigKey = "COOKIECONFIG";
		private static readonly string CookieConfigFile = ConfigurationHelper.GetConfigurationFile("CookieConfig");

		public static CookieConfiguration Instance
		{
			get
			{
				return ConfigurationManager.LoadConfiguration<CookieConfiguration>(CookieConfigKey, CookieConfigFile);
			}
		}
	}

	public class CookieEntryInfo : IKeyedItem<string>
	{

		//private CookieManager CookieManager;

		private bool hasChanged = false;

		public CookieEntryInfo()
		{
			//m_CookieManager = new CookieManager();
		}

		public CookieEntryInfo(string cookieId)
			: this()
		{
			this.CookieID = cookieId;
		}

		public string Key
		{
			get { return CookieID; }
		}

		[XmlAttribute("CookieID")]
		public string CookieID { get; set; }


		[XmlAttribute("CookieName")]
		public string CookieName { get; set; }


		[XmlAttribute("DomainName")]
		public string DomainName { get; set; }

		[XmlAttribute("Path")]
		public string Path { get; set; }

		private TimeSpan m_ExpiresAfter;

		[XmlIgnore]
		public TimeSpan ExpiresAfter
		{
			get { return m_ExpiresAfter; }
			set
			{
				m_ExpiresAfter = value;
				//SetCookieExpires();
			}
		}

		private string m_ExpiresTime;
		[XmlAttribute("ExpiresAfter")]
		public string ExpiresTime
		{
			get { return m_ExpiresTime; }
			set
			{
				m_ExpiresTime = value;
				if (!string.IsNullOrEmpty(value))
				{
					int day = int.Parse(value);
					m_ExpiresAfter = new TimeSpan(day, 0, 0, 0);
				}
			}
		}

		[XmlAttribute("SecureOnly")]
		public bool SecureOnly { get; set; }


		[XmlIgnore]
		public bool EnableExpiration
		{
			get { return ExpiresAfter.Ticks != 0; }
		}

		/// <summary>
		/// return GetCookieValue(),SetCookieValue(value)
		/// </summary>
		public string Value
		{
			get
			{
				return CookieManager.GetCookieValue(CookieName, hasChanged);
			}
			set
			{

				SetCookieValue(value);
			}
		}


		public string this[string subKey]
		{
			get
			{
				return CookieManager.GetCookieValue(CookieName, subKey, hasChanged);
			}
			set
			{
				SetCookieValue(subKey, value);
			}
		}

		#region Action
		protected void SetCookieValue(string val)
		{
			SetCookieProperty();
			CookieManager.SetCookie(CookieName, val);
		}

		protected void SetCookieValue(string index, string val)
		{
			SetCookieProperty();
			CookieManager.SetCookie(CookieName, index, val);
		}
		private void SetCookieProperty()
		{
			if (!hasChanged)
			{
				CookieManager.CopyCookieToResponse(CookieName);
				CookieManager.SetCookieSecurity(CookieName, SecureOnly);
				CookieManager.SetDomain(CookieName, DomainName);
				CookieManager.SetCookieExpires(CookieName, ExpiresAfter);
				CookieManager.SetCookiePath(CookieName, Path);
				hasChanged = true;
			}
		}
		public void Delete()
		{
			CookieManager.DeleteCookie(CookieName,this.DomainName);
		}
		#endregion


		#region ICloneable Members

		public CookieEntryInfo Clone()
		{
			CookieEntryInfo newCookie = new CookieEntryInfo();
			newCookie.CookieID = this.CookieID;
			newCookie.CookieName = this.CookieName;
			newCookie.DomainName = this.DomainName;
			newCookie.ExpiresTime = this.ExpiresTime;
			newCookie.Path = this.Path;
			newCookie.SecureOnly = this.SecureOnly;

			//newCookie.SetDomain();
			//newCookie.SetCookieSecurity();
			//newCookie.SetCookieExpires();
			//newCookie.SetCookiePath();

			return newCookie;
		}

		#endregion
	}
}

