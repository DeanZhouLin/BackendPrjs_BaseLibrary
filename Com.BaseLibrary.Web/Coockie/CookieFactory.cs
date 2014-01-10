using System;
using System.Web;

namespace Com.BaseLibrary.Web.Coockie
{
	public static class CookieFactory
	{
		private const string PREFIX_COOKIENAME = "COOKIE_";

		public static CookieEntryInfo CreateCookie(string cookieId)
		{
			string cookieName = PREFIX_COOKIENAME + cookieId.ToString().Trim().ToUpper();
			CookieEntryInfo entity = HttpContext.Current.Items[cookieName] as CookieEntryInfo;
			if (entity == null)
			{
				entity = new CookieEntryInfo(cookieId);
				entity.CookieName = cookieName;
				HttpContext.Current.Items[cookieName] = entity;
			}
			return entity;
		}
        /// <summary>
        /// 由cookieId克隆一个实例CookieEntryInfo
        /// </summary>
        /// <param name="cookieId"></param>
        /// <returns></returns>
		public static CookieEntryInfo CreateCookie2(string cookieId)
		{
            return CookieConfiguration.Instance.CookieList[cookieId].Clone();//克隆一个CookieEntryInfo实例
		}
	}
}
