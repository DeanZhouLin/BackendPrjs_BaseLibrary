using System;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

using System.Xml.Serialization;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Common.Cryptography;
using Com.BaseLibrary.Logging;
using System.ServiceModel;
using Com.BaseLibrary.Contract;
using System.Web;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.SqlClient;
using Com.BaseLibrary.Data;
using System.Data;



namespace Com.BaseLibrary.Utility
{

	public class PageViewUtil
	{
		/**
   Copyright 2009 Google Inc. All Rights Reserved.
 **/

		// Tracker version.
		private const string Version = "4.4sa";

		private const string CookieName = "__utmmobile";

		// The path the cookie will be available to, edit this to use a different
		// cookie path.
		private const string CookiePath = "/";

		// Two years in seconds.
		private static readonly TimeSpan CookieUserPersistence = TimeSpan.FromSeconds(63072000);



		private static readonly Regex IpAddressMatcher =
			new Regex(@"^([^.]+\.[^.]+\.[^.]+\.).*");

		// A string is empty in our terms, if it is null, empty or a dash.
		private static bool IsEmpty(string input)
		{
			return input == null || "-" == input || "" == input;
		}

		// The last octect of the IP address is removed to anonymize the user.
		private static string GetIP(string remoteAddress)
		{
			if (IsEmpty(remoteAddress))
			{
				return string.Empty;
			}
			// Capture the first three octects of the IP address and replace the forth
			// with 0, e.g. 124.455.3.123 becomes 124.455.3.0
			Match m = IpAddressMatcher.Match(remoteAddress);
			if (m.Success)
			{
				return m.Groups[1] + "0";
			}
			else
			{
				return string.Empty;
			}
		}

		// Generate a visitor id for this hit.
		// If there is a visitor id in the cookie, use that, otherwise
		// use the guid if we have one, otherwise use a random number.
		private static string GetVisitorId(
			string guid, string account, string userAgent, string sessionId, HttpCookie cookie)
		{

			// If there is a value in the cookie, don't change it.
			if (cookie != null && cookie.Value != null)
			{
				return cookie.Value;
			}

			String message;
			if (!IsEmpty(guid))
			{
				// Create the visitor id using the guid.
				message = guid + account;
			}
			else
			{
				// otherwise this is a new user, create a new random id.
				message = userAgent + sessionId;
			}

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] messageBytes = Encoding.UTF8.GetBytes(message);
			byte[] sum = md5.ComputeHash(messageBytes);

			string md5String = BitConverter.ToString(sum);
			md5String = md5String.Replace("-", "");

			md5String = md5String.PadLeft(32, '0');

			return "0x" + md5String.Substring(0, 16);
		}

		// Get a random number string.
		private static String GetRandomNumber()
		{
			Random RandomClass = new Random();
			return RandomClass.Next(0x7fffffff).ToString();
		}

		public static void TrackPageView(string sessionId, string userId, string account)
		{
			try
			{
				TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime());
				string timeStamp = timeSpan.TotalSeconds.ToString();

				string domainName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

				if (IsEmpty(domainName))
				{
					domainName = "";
				}

				string refererPage = GetRefererPage();

				if (IsEmpty(refererPage))
				{
					refererPage = "-";
				}

				string currentPage = HttpContext.Current.Request.Url.PathAndQuery;
				string userAgent = HttpContext.Current.Request.UserAgent;


				// Try and get visitor cookie from the request.
				HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(CookieName);

				string guidHeader = HttpContext.Current.Request.Headers.Get("X-DCMGUID");
				if (IsEmpty(guidHeader))
				{
					guidHeader = HttpContext.Current.Request.Headers.Get("X-UP-SUBNO");
				}
				if (IsEmpty(guidHeader))
				{
					guidHeader = HttpContext.Current.Request.Headers.Get("X-JPHONE-UID");
				}
				if (IsEmpty(guidHeader))
				{
					guidHeader = HttpContext.Current.Request.Headers.Get("X-EM-UID");
				}

				string visitorId = GetVisitorId(guidHeader, account, userAgent, sessionId, cookie);

				// Always try and add the cookie to the response.
				HttpCookie newCookie = new HttpCookie(CookieName);
				newCookie.Value = visitorId;
				newCookie.Expires = DateTime.Now + CookieUserPersistence;
				newCookie.Path = CookiePath;
				HttpContext.Current.Response.Cookies.Add(newCookie);

				string httpMethod = HttpContext.Current.Request.HttpMethod;

				string remoteIP = RequestUtil.GetClientIP();

				string language = GetLanguage();
				string browser = HttpContext.Current.Request.Browser.Browser;

				CommonService.Instance.TrackPageView(
				currentPage,
				refererPage,
				sessionId,
				userId,
				account,
				visitorId,
				userAgent,
				browser,
				remoteIP,
				domainName,
				language,
				httpMethod);


			}
			catch (Exception ex)
			{
				EmailUtil.SendWithLog(
					ConfigurationHelper.GetAppSetting("ApplicationName"),
					"Web Analytics",
					"Record Page View Failed",
					ex.ToString());
			}


		}

		public static string GetRefererPage()
		{
			//有可能报错，所以catch起来，不往外抛
			try
			{
				string refererPage = HttpContext.Current.Request.UrlReferrer == null ?
				string.Empty : HttpContext.Current.Request.UrlReferrer.PathAndQuery;
				return refererPage;
			}
			catch
			{
				return string.Empty;
			}

		}

		public static string GetLanguage()
		{
			string language = "zh-cn";
			if (HttpContext.Current.Request.UserLanguages != null
				&& HttpContext.Current.Request.UserLanguages.Length > 0)
			{
				language = HttpContext.Current.Request.UserLanguages[0];
			}
			return language;
		}

		private static readonly bool IgnoreLANVisitor = false;
		private static readonly bool HasIgnoreIPList = false;
		private static readonly List<string> IgnoreIPList = null;

		static PageViewUtil()
		{
			IgnoreLANVisitor = ConfigurationHelper.GetAppSetting("IgnoreLANVisitor") == "1";
			string ipList = ConfigurationHelper.GetAppSetting("IgnoreIPList");
			if (!StringUtil.IsNullOrEmpty(ipList))
			{
				IgnoreIPList = ListUtil.SplitToList<string>(ipList, ';');
				HasIgnoreIPList = IgnoreIPList.Count > 0;

			}

		}

		/// <summary>
		/// 不提供外部程序访问。
		/// </summary>
		/// <param name="currentPage"></param>
		/// <param name="refererPage"></param>
		/// <param name="sessionId"></param>
		/// <param name="userId"></param>
		/// <param name="account"></param>
		/// <param name="visitorId"></param>
		/// <param name="userAgent"></param>
		/// <param name="remoteIP"></param>
		/// <param name="domainName"></param>
		internal static void DoTrackPageView(
			string currentPage,
			string refererPage,
			string sessionId,
			string userId,
			string account,
			string visitorId,
			string userAgent,
			string browser,
			string remoteIP,
			string domainName,
			string language,
			string httpMethod
			)
		{
			if (IgnoreLANVisitor)
			{
				if (RequestUtil.IsInnerIP(remoteIP))
				{
					return;
				}
			}

			if (HasIgnoreIPList && IgnoreIPList.Contains(remoteIP))
			{
				return;
			}

			bool isRefresh = WriteToDatabase(currentPage, refererPage, sessionId, userId, account, userAgent, browser, remoteIP, domainName, language, httpMethod);


			if (!isRefresh)
			{
				SendRequestToGoogle(currentPage, refererPage, account, visitorId, userAgent, GetIP(remoteIP), domainName, language);
			}
		}



		private static void SendRequestToGoogle(
			string currentPage,
			string refererPage,
			string account,
			string visitorId,
			string userAgent,
			string remoteIP,
			string domainName,
			string language)
		{
			string utmGifLocation = "http://www.google-analytics.com/__utm.gif";
			string utmUrl = utmGifLocation + "?" +
				"utmwv=" + Version +
				"&utmn=" + GetRandomNumber() +
				"&utmhn=" + HttpUtility.UrlEncode(domainName) +
				"&utmr=" + HttpUtility.UrlEncode(refererPage) +
				"&utmp=" + HttpUtility.UrlEncode(currentPage) +
				"&utmac=" + account +
				"&utmcc=__utma%3D999.999.999.999.999.1%3B" +
				"&utmvid=" + visitorId +
				"&utmip=" + remoteIP;

			try
			{
				WebRequest connection = WebRequest.Create(utmUrl);
				((HttpWebRequest)connection).UserAgent = userAgent;
				connection.Headers.Add("Accepts-Language", language);

				using (WebResponse resp = connection.GetResponse())
				{
					// Ignore response
				}
			}
			catch (Exception ex)
			{
				//do nothing
			}
		}

		private static string connectionString;
		public static string ConnectionString
		{
			get
			{
				if (string.IsNullOrEmpty(connectionString))
				{
					connectionString = ConfigurationHelper.GetConnectionString("RowDataConn");
					if (!connectionString.Contains(";"))
					{
						connectionString = Encryptor.Decrypt(connectionString);
					}
				}
				return connectionString;
			}
		}

		/// <summary>
		/// 记录到我们自己的数据库中
		/// </summary>
		/// <param name="currentPage"></param>
		/// <param name="refererPage"></param>
		/// <param name="account"></param>
		/// <param name="visitorId"></param>
		/// <param name="userAgent"></param>
		/// <param name="remoteIP"></param>
		/// <param name="domainName"></param>
		/// <param name="language"></param>
		private static bool WriteToDatabase(
			string currentPage,
			string refererPage,
			string sessionId,
			string userId,
			string account,
			string userAgent,
			string browser,
			string remoteIP,
			string domainName,
			string language,
			string httpMethod
			)
		{
			try
			{
				#region SQL
				string commandText = @" MKT.PK_MKT_COMMON.p_Add_PageView";
				#endregion

				//string OS = string.Empty;
				//userId = userId == null ? string.Empty : userId;
				//refererPage = refererPage == null ? string.Empty : refererPage;
				//userAgent = userAgent == null ? string.Empty : userAgent;
				//remoteIP = remoteIP == null ? string.Empty : remoteIP;
				//browser = browser == null ? string.Empty : browser;
				//httpMethod = httpMethod == null ? string.Empty : httpMethod;
				//language = language == null ? string.Empty : language;

				bool IsRefresh = false;
				//using (OracleConnection conn = new OracleConnection(ConnectionString))
				//{
				//    OracleCommand command = new OracleCommand(commandText, conn);
				//    command.CommandType = System.Data.CommandType.StoredProcedure;
				//    command.Parameters.Add(new OracleParameter("p_DOMAINNAME", domainName));
				//    command.Parameters.Add(new OracleParameter("p_ACCOUNT", account));
				//    command.Parameters.Add(new OracleParameter("p_SESSION_ID", sessionId));
				//    command.Parameters.Add(new OracleParameter("p_USER_ID", userId));
				//    command.Parameters.Add(new OracleParameter("p_PAGE_URL", currentPage));
				//    command.Parameters.Add(new OracleParameter("p_REFER_PAGE_URL", refererPage));
				//    command.Parameters.Add(new OracleParameter("p_USER_AGENT", userAgent));
				//    command.Parameters.Add(new OracleParameter("p_REMOTE_IP", remoteIP));
				//    command.Parameters.Add(new OracleParameter("p_OS", OS));
				//    command.Parameters.Add(new OracleParameter("p_BROWSER", browser));
				//    command.Parameters.Add(new OracleParameter("p_ACCEPTS_LANGUAGE", language));
				//    command.Parameters.Add(new OracleParameter("p_HTTP_METHOD", httpMethod));
				//    OracleParameter p = new OracleParameter();
				//    p.ParameterName = "P_IsRefresh";
				//    p.Direction = System.Data.ParameterDirection.Output;
				//    p.DbType = System.Data.DbType.Decimal;
				//    command.Parameters.Add(p);
				//    conn.Open();
				//    command.ExecuteNonQuery();
				//    IsRefresh = ((decimal)p.Value) > 0;
				//    conn.Close();
				//}
				return IsRefresh;
			}
			catch (Exception ex)//截住异常不往外抛
			{
				EmailUtil.SendWithLog(
					"Common Service",
					"Web Analytics",
					"Record Page View Failed",
					ex.ToString());
				return false;
			}



		}


		[Obsolete("不再使用")]
		public static void RecordPageView(
			string sid,
			string cid,
			string customerName,
			string visitedUrl,
			DateTime visitedTime,
			string urlReferrer,
			string userAgent,
			string browser,
				string browserVersion,
				string os,
			string userIP,
			string promotionNO,
			string unionTrackID,
			string httpMothed
			)
		{

			#region SQL
			string commandText = "MKT.dbo.UP_PV_RecordPageView";
			#endregion

			SqlParameter[] parameters = new[]{
					    new SqlParameter("@SID",sid),
						new SqlParameter("@CID",cid),
						new SqlParameter("@CustomerName",customerName),
						new SqlParameter("@VisitedUrl",visitedUrl),
						new SqlParameter("@VisitedTime",visitedTime),
						new SqlParameter("@UrlReferrer",urlReferrer),
						new SqlParameter("@UserAgent",userAgent),
						new SqlParameter("@Browser",browser),
						new SqlParameter("@BrowserVersion",browserVersion),
						new SqlParameter("@OS",os),
						new SqlParameter("@UserIP",userIP),
						new SqlParameter("@PromotionNO",promotionNO),
						new SqlParameter("@UnionTrackID",unionTrackID),
						new SqlParameter("@HttpMothed",httpMothed)
				};

			SqlHelper.ExecuteNonQuery(
				ConnectionString,
				CommandType.StoredProcedure,
				commandText,
				parameters
				);

		}

		
		/// <summary>
		/// 记录PageView的存储过程
		/// </summary>
		private static readonly string RecordPageViewSP = ConfigurationHelper.GetAppSetting("RecordPageViewSP");

		public static void RecordPageView(
			string sid,
			string cid,
			string customerName,
			string visitedUrl,
			DateTime visitedTime,
			string urlReferrer,
			string userAgent,
			string browser,
			string browserVersion,
			string os,
			string userIP,
			string promotionNO,
			string unionTrackID,
			string httpMothed,
			string pageName,
			int? categoryId,
			int? brandId,
			int? merchantId,
			int? campaignId,
			string itemNo,
			string keyword,
			int? tuanId)
		{
			#region SQL
			//string commandText = "MKT.dbo.UP_PV_RecordPageView_V1";
			#endregion

			SqlParameter[] parameters = new[]{
					    new SqlParameter("@SID",sid),
						new SqlParameter("@CID",cid),
						new SqlParameter("@CustomerName",customerName),
						new SqlParameter("@VisitedUrl",visitedUrl),
						new SqlParameter("@VisitedTime",visitedTime),
						new SqlParameter("@UrlReferrer",urlReferrer),
						new SqlParameter("@UserAgent",userAgent),
						new SqlParameter("@Browser",browser),
						new SqlParameter("@BrowserVersion",browserVersion),
						new SqlParameter("@OS",os),
						new SqlParameter("@UserIP",userIP),
						new SqlParameter("@PromotionNO",promotionNO),
						new SqlParameter("@UnionTrackID",unionTrackID),
						new SqlParameter("@HttpMothed",httpMothed),

						new SqlParameter("@PageName",pageName),
						new SqlParameter("@CategoryID",categoryId),
						new SqlParameter("@BrandID",brandId),
						new SqlParameter("@MerchantID",merchantId),
						new SqlParameter("@CampaignID",campaignId),
						new SqlParameter("@TuanID",tuanId),
						new SqlParameter("@Keyword",keyword),
						new SqlParameter("@ItemNO",itemNo)
				};

			SqlHelper.ExecuteNonQuery(
				ConnectionString,
				CommandType.StoredProcedure,
				RecordPageViewSP,
				parameters
				);
		}
	}
}