using System;
using System.Web;
using System.Collections.Generic;


namespace Com.BaseLibrary.Web
{
	/// <summary>
	/// Specifies what application the web browser is.
	/// </summary>
	public enum BrowserApplication
	{
		/// <summary>
		/// Internet explorer
		/// </summary>
		InternetExplorer,

		/// <summary>
		/// Firefox
		/// </summary>
		Firefox,

		/// <summary>
		/// Thunderbird, actually an email client, sometimes has its own identity
		/// </summary>
		Thunderbird,

		/// <summary>
		/// Mozilla, now SeaMonkey
		/// </summary>
		Mozilla,

		/// <summary>
		/// Opera
		/// </summary>
		Opera,

		/// <summary>
		/// Netscape
		/// </summary>
		Netscape,

		/// <summary>
		/// Firebird, predecessor of Firefox
		/// </summary>
		Firebird,

		/// <summary>
		/// Safari
		/// </summary>
		Safari,

		/// <summary>
		/// Lynx, a text-based http agent
		/// </summary>
		Lynx,

		/// <summary>
		/// Konqueror
		/// </summary>
		Konqueror,

		/// <summary>
		/// OmniWeb
		/// </summary>
		OmniWeb,

		/// <summary>
		/// Web TV
		/// </summary>
		WebTV,

		/// <summary>
		/// iCab
		/// </summary>
		iCab,

		/// <summary>
		/// Not recognized.
		/// </summary>
		Unrecognized
	}

	/// <summary>
	/// Specifies what engine the browser is using.
	/// </summary>
	public enum BrowserEngine
	{
		/// <summary>
		/// Internet Explorer family, IE, WebTV
		/// </summary>
		MSIE,

		/// <summary>
		/// Gecko family, netscape, mozilla, firefox, etc
		/// </summary>
		Gecko,

		/// <summary>
		/// Opera. Opera's proprietary rendering engine.
		/// </summary>
		Opera,

		/// <summary>
		/// KHTML family. konqueror, safari, OmniWeb
		/// </summary>
		KHTML,

		/// <summary>
		/// Lynx.
		/// </summary>
		Lynx,

		/// <summary>
		/// iCab
		/// </summary>
		iCab,

		/// <summary>
		/// not recognized.
		/// </summary>
		Unrecognized
	}

	/// <summary>
	/// Represents the browser infomation, to be more exact, the HTTP agent the user is using. 
	/// </summary>
	public sealed class BrowserInfo
	{

		private BrowserApplication m_BrowserApplication = BrowserApplication.Unrecognized;
		private BrowserEngine m_BrowserEngine = BrowserEngine.Unrecognized;
		private string m_RawIdentity;
		private string m_BrowserAppVersion;

		internal BrowserInfo(string identity, BrowserApplication app, BrowserEngine engine, string version)
		{
			m_RawIdentity = identity;
			m_BrowserApplication = app;
			m_BrowserEngine = engine;
			m_BrowserAppVersion = version;
		}

		private BrowserInfo()
		{
		}

		/// <summary>
		/// Gets the application type of the browser. 
		/// </summary>
		public BrowserApplication BrowserApplication
		{
			get
			{
				return m_BrowserApplication;
			}
		}

		/// <summary>
		/// Gets the browser engine.
		/// </summary>
		public BrowserEngine BrowserEngine
		{
			get
			{
				return m_BrowserEngine;
			}
		}


		/// <summary>
		/// Gets the version of the browser application.<br/>
		/// returns null if no browser info is recognized.
		/// </summary>
		public string BrowserAppVersion
		{
			get { return m_BrowserAppVersion; }
		}

		/// <summary>
		/// Gets the raw user-agent string sent by the browser. 
		/// this is the same string that is used to contruct a BrowserInfo instance.
		/// </summary>
		public string RawIdentity
		{
			get { return m_RawIdentity; }
		}

		/// <summary>
		/// Returns a string indicating the browser application, engine, and version information.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "BrowserApplication=[" + BrowserApplication.ToString() + "]|" +
				   "BrowserEngine=[" + BrowserEngine.ToString() + "]|" +
				   "BrowserAppVersion=[" + BrowserAppVersion.ToString() + "]";
		}

		/// <summary>
		/// Gets a value indicating whether this instance is IE5.5 or above.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is I e55 or above; otherwise, <c>false</c>.
		/// </value>
		public bool IsIE55OrAbove
		{
			get 
			{
				if (BrowserApplication == BrowserApplication.InternetExplorer)
				{
					double ver;
					if (double.TryParse(BrowserAppVersion, out ver) && ver >= 5.5)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is gomez agent.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is gomez agent; otherwise, <c>false</c>.
		/// </value>
		public bool IsGomezAgent
		{
			get 
			{
				return (m_RawIdentity.IndexOf("GomezAgent") != -1);
			}
		}

		internal static BrowserInfo CreateUnrecognizedBrowserInfo()
		{
			BrowserInfo info = new BrowserInfo();
			info.m_BrowserApplication = BrowserApplication.Unrecognized;
			info.m_BrowserEngine = BrowserEngine.Unrecognized;
			return info;
		}
	}
}
