using System.Web;

using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Web
{
    public class QueryStringManager
    {
        #region fileds
		private HttpRequestBase m_HttpRequest;
        #endregion

		public QueryStringManager(HttpRequestBase request)
        {
            m_HttpRequest = request;
        }

        public string GetValue(string paramName)
        {
            return string.IsNullOrEmpty(m_HttpRequest.QueryString[paramName]) ? string.Empty : m_HttpRequest.QueryString[paramName];
        }
        public T GetValue<T>(string paramName, T defaultValue)
        {
            string value = string.IsNullOrEmpty(m_HttpRequest.QueryString[paramName]) ? string.Empty : m_HttpRequest.QueryString[paramName];

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            try
            {
                return StringUtil.ToType<T>(value);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
