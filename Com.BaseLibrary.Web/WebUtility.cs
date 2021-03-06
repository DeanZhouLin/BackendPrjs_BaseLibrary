using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Web
{
	public static class WebUtility
	{
        public const String HtmlNewLine = "<br />";
        public const String _appSettingsPrefix = "AspNetForumsSettings.";
        public static Regex _pathComponentTextToEscape = new Regex(@"([^A-Za-z0-9\- ]+|\.| )", RegexOptions.Singleline | RegexOptions.Compiled);
        public static Regex _pathComponentTextToUnescape = new Regex(@"((?:_(?:[0-9a-f][0-9a-f][0-9a-f][0-9a-f])+_)|\+)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex _fileComponentTextToEscape = new Regex(@"([^A-Za-z0-9 ]+|\.| )", RegexOptions.Singleline | RegexOptions.Compiled);
        public static Regex _fileComponentTextToUnescape = new Regex(@"((?:_(?:[0-9a-f][0-9a-f][0-9a-f][0-9a-f])+_)|_|\-)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex _escapePeriod = new Regex(@"(?:\.config|\.ascx|\.asax|\.cs|\.vb)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

		public static bool IsSecureSite(string url)
		{
			if (StringUtil.IsNullOrEmpty(url))
			{
				return false;
			}
			else
			{
				if (url.ToUpper().IndexOf("HTTPS://") == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}


        #region Encode/Decode
        /// <summary>
        /// Converts a prepared subject line back into a raw text subject line.
        /// </summary>
        /// <param name="textToFormat">The prepared subject line.</param>
        /// <returns>A raw text subject line.</returns>
        /// <remarks>This function is only needed when editing an existing message or when replying to
        /// a message - it turns the HTML escaped characters back into their pre-escaped status.</remarks>
        public static string HtmlDecode(String textToFormat)
        {
            if (StringUtil.IsNullOrEmpty(textToFormat))
                return textToFormat;

            //ScottW: Removed Context dependency
            return System.Web.HttpUtility.HtmlDecode(textToFormat);
            // strip the HTML - i.e., turn < into &lt;, > into &gt;
            //return HttpContext.Current.Server.HtmlDecode(FormattedMessageSubject);
        }

        /// <summary>
        /// Converts a prepared subject line back into a raw text subject line.
        /// </summary>
        /// <param name="textToFormat">The prepared subject line.</param>
        /// <returns>A raw text subject line.</returns>
        /// <remarks>This function is only needed when editing an existing message or when replying to
        /// a message - it turns the HTML escaped characters back into their pre-escaped status.</remarks>
        public static string HtmlEncode(String textToFormat)
        {
            // strip the HTML - i.e., turn < into &lt;, > into &gt;

            if (StringUtil.IsNullOrEmpty(textToFormat))
                return textToFormat;

            //ScottW: Removed Context dependency
            return System.Web.HttpUtility.HtmlEncode(textToFormat);
            //return HttpContext.Current.Server.HtmlEncode(FormattedMessageSubject);
        }

        public static string UrlEncode(string urlToEncode)
        {
            if (StringUtil.IsNullOrEmpty(urlToEncode))
                return urlToEncode;

            return System.Web.HttpUtility.UrlEncode(urlToEncode).Replace("'", "%27");
            ;
        }

        public static string UrlDecode(string urlToDecode)
        {
            if (StringUtil.IsNullOrEmpty(urlToDecode))
                return urlToDecode;

            return System.Web.HttpUtility.UrlDecode(urlToDecode);
        }

        public static string UrlEncodePathComponent(string text)
        {
            return UrlEncode(text, _pathComponentTextToEscape, '+', '_');
        }

        public static string UrlDecodePathComponent(string text)
        {
            return UrlDecode(text, _pathComponentTextToUnescape);
        }

        public static string UrlEncodeFileComponent(string text)
        {
            return UrlEncode(text, _fileComponentTextToEscape, '-', '_');
        }

        public static string UrlDecodeFileComponent(string text)
        {
            return UrlDecode(text, _fileComponentTextToUnescape);
        }

        private static string UrlEncode(string text, Regex pattern, char spaceReplacement, char escapePrefix)
        {
            if (StringUtil.IsNullOrEmpty(text))
                return text;

            Match match = pattern.Match(text);
            StringBuilder encText = new StringBuilder();
            int lastEndIndex = 0;
            bool escapeAllPeriods = _escapePeriod.IsMatch(text);
            while (match.Value != string.Empty)
            {
                if (lastEndIndex != match.Index)
                    encText.Append(text.Substring(lastEndIndex, match.Index - lastEndIndex));

                if (match.Value == " ")
                    encText.Append(spaceReplacement);
                else if (match.Value == "." && match.Index != text.Length - 1 && !escapeAllPeriods)
                    encText.Append("."); // . at the end of text causes a 404... only encode . at the end of text
                else
                {
                    encText.Append(escapePrefix);
                    byte[] bytes = Encoding.Unicode.GetBytes(match.Value);
                    if (bytes != null)
                    {
                        foreach (byte b in bytes)
                        {
                            string hexByte = b.ToString("X");

                            if (hexByte.Length == 1)
                                encText.Append("0");

                            encText.Append(hexByte);
                        }
                    }
                    encText.Append(escapePrefix);
                }

                lastEndIndex = match.Index + match.Length;
                match = pattern.Match(text, lastEndIndex);
            }

            if (lastEndIndex < text.Length)
                encText.Append(text.Substring(lastEndIndex));

            return encText.ToString();
        }

        private static string UrlDecode(string text, Regex pattern)
        {
            if (StringUtil.IsNullOrEmpty(text))
                return text;

            Match match = pattern.Match(text);
            StringBuilder decText = new StringBuilder();
            int lastEndIndex = 0;
            while (match.Value != string.Empty)
            {
                if (lastEndIndex != match.Index)
                    decText.Append(text.Substring(lastEndIndex, match.Index - lastEndIndex));

                if (match.Value.Length == 1)
                    decText.Append(" ");
                else
                {
                    byte[] bytes = new byte[(match.Value.Length - 2) / 2];

                    for (int i = 1; i < match.Value.Length - 1; i += 2)
                        bytes[(i - 1) / 2] = byte.Parse(match.Value.Substring(i, 2), NumberStyles.AllowHexSpecifier);

                    decText.Append(Encoding.Unicode.GetString(bytes));
                }

                lastEndIndex = match.Index + match.Length;
                match = pattern.Match(text, lastEndIndex);
            }

            if (lastEndIndex < text.Length)
                decText.Append(text.Substring(lastEndIndex));

            return decText.ToString();
        }

        #endregion

	}
}
