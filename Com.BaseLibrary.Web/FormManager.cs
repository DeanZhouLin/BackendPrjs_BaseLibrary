using System.Web;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Web
{
    public class FormManager
    {
        #region fileds
		private HttpRequestBase m_HttpRequest;
        #endregion

		public FormManager(HttpRequestBase request)
        {
            m_HttpRequest = request;
        }

        public string GetValue(string paramName)
        {
            string tmp = string.IsNullOrEmpty(m_HttpRequest.Form.Get(paramName)) ? string.Empty : m_HttpRequest.Form.Get(paramName);
            return System.Web.HttpUtility.HtmlEncode(tmp).Trim();
        }
        public T GetValue<T>(string paramName, T defaultValue)
        {
            string value = string.IsNullOrEmpty(m_HttpRequest.Form.Get(paramName)) ? string.Empty : m_HttpRequest.Form.Get(paramName);

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
