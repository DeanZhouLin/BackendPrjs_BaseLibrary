using System;
using System.Collections.Generic;

namespace Com.BaseLibrary.Web
{
	/// <summary>
	/// Parses a given user-agent string and builds the corresponding BrowserInfo.
	/// </summary>
	/// <remarks>
	/// Note to the extenders:
	/// More todo: 
	///		I:
	///		there are some browsers that don't use specific identities, rather, generic agent
	///		strings are used. e.g. "Mozilla/2.02 [fr] (WinNT; I)" is actually Netscape.
	///		In these cases, we need to maintain a hashtable that matches each of these special
	///		agent strings to their real browsers.
	///		Currently we do not implement this algorithm as the above case is very rare - most 
	///		modern browsers which are used on Digvalue.com can be identified by a specific string.
	///		II:
	///		Some browsers have their application version that is different from the agent string,
	///		like Safari, version 2.0.3 has the agent string "Safari/417.8".
	///		If application version is vital, this feature should be added.
	/// </remarks>
	public static class BrowserInfoParser
	{

		#region look-up list for browser identification. List, rather than dictionary, here is used as ordering is important.
		private static List<string> m_IdentifierList;
		private static List<BrowserApplication> m_ApplicationList;
		private static List<BrowserEngine> m_BrowserEngineList;
		#endregion

		static BrowserInfoParser()
		{
			m_IdentifierList = new List<string>();
			m_ApplicationList = new List<BrowserApplication>();
			m_BrowserEngineList = new List<BrowserEngine>();

			// only browser that can be uniquely identified should add their identifications here.
			m_IdentifierList.Add("Firefox");
			m_ApplicationList.Add(BrowserApplication.Firefox);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

			m_IdentifierList.Add("Thunderbird");
			m_ApplicationList.Add(BrowserApplication.Thunderbird);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

			m_IdentifierList.Add("Minefield");
			m_ApplicationList.Add(BrowserApplication.Firefox);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

			m_IdentifierList.Add("Opera");
			m_ApplicationList.Add(BrowserApplication.Opera);
			m_BrowserEngineList.Add(BrowserEngine.Opera);

			m_IdentifierList.Add("Netscape");
			m_ApplicationList.Add(BrowserApplication.Netscape);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

			m_IdentifierList.Add("Firebird");
			m_ApplicationList.Add(BrowserApplication.Firebird);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

			// sometimes iCab uses Lynx as part of its user-agent string, so put iCab before Lynx
			m_IdentifierList.Add("iCab");
			m_ApplicationList.Add(BrowserApplication.iCab);
			m_BrowserEngineList.Add(BrowserEngine.iCab);

			m_IdentifierList.Add("Lynx");
			m_ApplicationList.Add(BrowserApplication.Lynx);
			m_BrowserEngineList.Add(BrowserEngine.Lynx);

			m_IdentifierList.Add("Konqueror");
			m_ApplicationList.Add(BrowserApplication.Konqueror);
			m_BrowserEngineList.Add(BrowserEngine.KHTML);

			// Note: Omniweb's agent string sometimes contains "Safari", so put it before Safari.
			m_IdentifierList.Add("OmniWeb");
			m_ApplicationList.Add(BrowserApplication.OmniWeb);
			m_BrowserEngineList.Add(BrowserEngine.KHTML);

			m_IdentifierList.Add("Safari");
			m_ApplicationList.Add(BrowserApplication.Safari);
			m_BrowserEngineList.Add(BrowserEngine.KHTML);

			m_IdentifierList.Add("SeaMonkey");
			m_ApplicationList.Add(BrowserApplication.Mozilla);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

			m_IdentifierList.Add("WebTV");
			m_ApplicationList.Add(BrowserApplication.WebTV);
			m_BrowserEngineList.Add(BrowserEngine.MSIE);

			// Note, add ie and Mozila to the end of the list as many other apps masquerade themselves as IE or Mozilla.
			m_IdentifierList.Add("MSIE");
			m_ApplicationList.Add(BrowserApplication.InternetExplorer);
			m_BrowserEngineList.Add(BrowserEngine.MSIE);

			m_IdentifierList.Add("Mozilla");
			m_ApplicationList.Add(BrowserApplication.Mozilla);
			m_BrowserEngineList.Add(BrowserEngine.Gecko);

		}

		public static BrowserInfo Parse(string rawIdentity)
		{
			BrowserApplication browserApplication = BrowserApplication.Unrecognized;
			BrowserEngine browserEngine = BrowserEngine.Unrecognized;
			string appVersion = string.Empty;

			if (rawIdentity == null)
			{
				return BrowserInfo.CreateUnrecognizedBrowserInfo();
			}

			string key = "";
			int startPos = -1;

			// recognize browser from the unique strings, if any
			for (int i = 0; i < m_IdentifierList.Count; i++)
			{
				key = m_IdentifierList[i];
				startPos = DoParseBrowserApplication(rawIdentity, key);
				if (startPos == -1)
				{
					continue;
				}
				browserApplication = m_ApplicationList[i];
				browserEngine = m_BrowserEngineList[i];
				break;
			}

			// get browser version
			if (startPos != -1)
			{
				appVersion = ParseAppVersion(rawIdentity, startPos + key.Length + 1);
			}
			return new BrowserInfo(rawIdentity, browserApplication, browserEngine, appVersion);
		}

		/// <summary>
		/// parses the browser version from the specified position of the raw identity.
		/// returns string.Empty if no version is detected.
		/// </summary>
		/// <param name="startPosition"> the position from where to start parsing the application version.</param>
		/// <returns></returns>
		private static string ParseAppVersion(string rawIdentity, int startPos)
		{
			// get the version of the browser
			// the version number start from the first non-blank and non-slash character 
			// after the browser identification string
			for (; startPos < rawIdentity.Length; startPos++)
			{
				if (rawIdentity[startPos] != ' ' &&
					rawIdentity[startPos] != '/')
					break;
			}

			// in very rare cases, the end of the string is reached, no version is found.
			if (startPos == rawIdentity.Length - 1)
			{
				return string.Empty;
			}

			// the version number ends when it encounters the following character, or the end of the string
			int endPos;
			for (endPos = startPos; endPos < rawIdentity.Length; endPos++)
			{
				if (rawIdentity[endPos] == ' ' ||
					rawIdentity[endPos] == ')' ||
					rawIdentity[endPos] == '(' ||
					rawIdentity[endPos] == ';' ||
					rawIdentity[endPos] == ',')
					break;
			}

			// also, in rare cases, endpos is the same as startPos, no version is found.
			if (endPos == startPos)
			{
				return string.Empty;
			}

			return rawIdentity.Substring(startPos, endPos - startPos);
		}


		/// <summary>
		/// parses the browser application type.
		/// returns the position of the magic string in the agent string.
		/// -1 if no application is recognized.
		/// </summary>
		/// <param name="browserID"></param>
		/// <returns></returns>
		private static int DoParseBrowserApplication(string rawIdentity, string browserID)
		{
			int startPos = rawIdentity.IndexOf(browserID, StringComparison.InvariantCultureIgnoreCase);
			return startPos;
		}
	}
}
