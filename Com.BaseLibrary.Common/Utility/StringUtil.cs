using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System;

namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 生日的正则表达式
        /// </summary>
		public const string BIRTHDAY_REGEXPATTERN = @"^([1][12]|[0]?[1-9])[\/-]([3][01]|[12]\d|[0]?[1-9])[\/-](\d{4})$";

        /// <summary>
        /// Email的正则表达式
        /// </summary>
		public const string EmailRegexPattern = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";


        /// <summary>
        /// Url的正则表达
        /// </summary>
		public const string WEBSITEURL_REGEXPATTERN =
            "^(http|https|ftp)\\://([a-zA-Z0-9\\.\\-]+(\\:[a-zA-Z0-9\\.&%\\$\\-]+)*@)?((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|([a-zA-Z0-9\\-]+\\.)*[a-zA-Z0-9\\-]+)(\\:[0-9]+)?((/|/[^/][a-zA-Z0-9\\.\\,\\?\\\'\\\\/\\+&%\\$#\\=~_\\-@]*))*$";

        /// <summary>
        /// 帐号的正则表达
        /// </summary>
		public const string USERACCOUNT_REGEXPATTERN = @"^[a-zA-Z0-9\-_\.]{1,500}$";
        // @"^[a-zA-Z0-9\!\$\%\^\&\*\-_\{\}\|\~\?\.]{1,500}$"

        /// <summary>
        /// 判断一个字符串是Null或是空
        /// 注：即时只有空格的字符串也算是空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(object input)
        {
            if (input == null)
            {
                return true;
            }

            return input.ToString().Trim().Length == 0;
        }

        /// <summary>
        /// Encode XML 文本
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string XMLEncode(string strText)
        {
            if (string.IsNullOrEmpty(strText))
            {
                return string.Empty;
            }
            string strTextRet = strText;
            strTextRet = strTextRet.Replace("&", "&amp;");
            strTextRet = strTextRet.Replace("\"", "&quot;");
            strTextRet = strTextRet.Replace("'", "&apos;");
            strTextRet = strTextRet.Replace("<", "&lt;");
            strTextRet = strTextRet.Replace(">", "&gt;");
            return strTextRet;
        }

        /// <summary>
        /// 判断字符串是否是生日格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBirthday(string input)
        {
            if (StringUtil.IsNullOrEmpty(input))
            {
                return false;
            }
            try
            {
                DateTime.Parse(input);
                return true;
            }
            catch
            {

                return false;
            }
        }

        /// <summary>
        /// 判断字符串是否是Url格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsWebSiteUrl(string input)
        {
            if (!IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input.Trim(), WEBSITEURL_REGEXPATTERN);
            }
            return false;
        }

        /// <summary>
        /// 判断字符串是否是Email格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            if (!IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input.Trim(), EmailRegexPattern);
            }
            return false;
        }

        public static bool IsZipCode(string input)
        {
            if (!IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input.Trim(), @"^[0-9]{6}$");
            }
            return false;
        }

        /// <summary>
        /// 把一个Collection Join成一个字符串返回
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="delimiter">分隔符.</param>
        /// <returns></returns>
        public static string JoinString<T>(IEnumerable<T> collection, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            int pos = 0;
            foreach (T t in collection)
            {
                if (t == null || StringUtil.IsNullOrEmpty(t.ToString())) continue;

                if (pos != 0)
                {
                    sb.Append(delimiter);
                }


                sb.Append(t.ToString());
                pos++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Encode SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SqlEncode(string input)
        {
            if (IsNullOrEmpty(input))
            {
                return input;
            }
            return input.Replace("'", "''");
        }

        /// <summary>
        /// 判断是否是Integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInteger(object value)
        {
            if (value == null)
            {
                return false;
            }
            try
            {
                string val = value.ToString();
                if (val.IndexOf(".") >= 0)
                {
                    return false;
                }
                int.Parse(val);
                return true;
            }
            catch
            {
                return false;
            }


        }

        /// <summary>
        /// 判断是否是Integer，如果是，并返回转换结果
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsInteger(object value, out int result)
        {
            if (value == null)
            {
                result = 0;
                return false;
            }
            try
            {
                result = int.Parse(value.ToString());
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }

        }

        /// <summary>
        /// 判断是否是Decimal类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsNotDecimal(string p)
        {
            try
            {
                decimal.Parse(p);
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 如果数字的小数点后只有0，则去掉
        /// 如：8989.000,则返回8989；
        ///     343.3300，则返回343.33；
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string TrimNumberExtraZero(object number)
        {
            if (number == null)
            {
                return string.Empty;
            }
            string data = number.ToString();
            if (data.IndexOf('.') >= 0)
            {
                while (data.EndsWith("0"))
                {
                    data = data.TrimEnd('0');
                }
                if (data.EndsWith("."))
                {
                    data = data.TrimEnd('.');
                }
            }

            return data;
        }

        /// <summary>
        /// 判断两个字符串是否相等，忽略大小写和尾部的空格
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool EqualsIgnorCaseAndSpace(object str1, object str2)
        {
            if (str1 == null)
            {
                return IsNullOrEmpty(str2);
            }
            else if (str2 == null)
            {
                return IsNullOrEmpty(str1);
            }
            else
            {
                return str1.ToString().TrimEnd().ToUpper() == str2.ToString().TrimEnd().ToUpper();
            }
        }
        public static bool EqualsInStringFormat(object str1, object str2)
        {
            if (str1 == null)
            {
                return str2 == null;
            }
            else if (str2 == null)
            {
                return str1 == null;
            }
            else
            {
                return str1.ToString() == str2.ToString();
            }
        }

        public static string TrimSpace(string p)
        {
            if (p != null)
            {
                return p.Trim();
            }
            else
            {
                return p;
            }
        }

        public static string PadRight(string p, int totalWidth)
        {
            if (p == null)
            {
                return string.Empty.PadRight(totalWidth);
            }
            else
            {
                return p.PadRight(totalWidth);
            }
        }
        public static string PadLeft(string p, int totalWidth)
        {
            if (p == null)
            {
                return string.Empty.PadLeft(totalWidth);
            }
            else
            {
                return p.PadLeft(totalWidth);
            }
        }

        public static string GetStringValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }

        public static DateTime? GetDateTimeValue(string value)
        {
            if (IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return DateTime.Parse(value);
            }
            catch
            {

                return null;
            }

        }

        public static string GetDateTimeString(DateTime? value)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                return value.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {

                return null;
            }

        }

        public static string GetShortDateString(DateTime? value)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                return value.Value.ToShortDateString();
            }
            catch
            {

                return null;
            }

        }

        public static string GetShortTimeString(DateTime? value)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                return value.Value.ToShortTimeString();
            }
            catch
            {

                return null;
            }

        }

        public static Int32? GetIntValue(string value)
        {
            if (IsNullOrEmpty(value))
            {
                return null;
            }
            return Int32.Parse(value);
        }
        public static decimal? GetDecimalValue(string value)
        {
            if (IsNullOrEmpty(value))
            {
                return null;
            }
            return decimal.Parse(value);
        }

        public static T ConvertToT<T>(string item)
        {
            object obj = ConverToTType<T>(item);
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;

        }

        public static object ConverToTType<T>(string item)
        {
            if (IsNullOrEmpty(item))
            {
                return null;
            }
            Type type = typeof(T);
            return ConvertToType(item, type);
        }
        public static T ToType<T>(string item)
        {
            object obj = ConverToTType<T>(item);
            return obj == null ? default(T) : (T)obj;
        }
        public static object ConvertToType(string item, Type type)
        {
            if (string.IsNullOrEmpty(item))
            {
                return null;
            }

            string typeName = type.FullName.ToUpper();
            if (typeName.IndexOf("SYSTEM.INT32") >= 0)
            {
                return int.Parse(item);
            }
            if (typeName.IndexOf("SYSTEM.INT64") >= 0)
            {
                return long.Parse(item);
            }
            else if (typeName.IndexOf("SYSTEM.DECIMAL") >= 0)
            {
                return decimal.Parse(item);
            }
            else if (typeName.IndexOf("SYSTEM.FLOAT") >= 0)
            {
                return float.Parse(item);
            }
            else if (typeName.IndexOf("SYSTEM.DOUBLE") >= 0)
            {
                return double.Parse(item);
            }
            else if (typeName.IndexOf("SYSTEM.DATETIME") >= 0)
            {
                return DateTime.Parse(item);
            }
            else if (typeName.IndexOf("SYSTEM.SINGLE") >= 0)
            {
                return Single.Parse(item);
            }
            return item;
        }

        public static object ConvertToType1(object item, Type type)
        {
            if (item == null)
            {
                return null;
            }
            if (item.GetType().Name == type.Name)
            {
                return item;
            }

            string val = item == null ? string.Empty : item.ToString();
            return ConvertToType(val, type);
        }

        public static bool IsNullable(Type type)
        {
            string typeName = type.FullName.ToUpper();
            return typeName.IndexOf("SYSTEM.NULLABLE") >= 0;

        }

        public static bool IsDateTimeType(Type type)
        {
            string typeName = type.FullName.ToUpper();
            return typeName.IndexOf("SYSTEM.DATETIME") >= 0;
        }



        public static bool IsUserAccount(string input)
        {
            if (!IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input.Trim(), USERACCOUNT_REGEXPATTERN);
            }
            return false;
        }

        public static bool IsCellPhoneNumber(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            string req = @"^(13[0-9]|14[0-9]|15[0-9]|18[0-9])\d{8}$";
            return Regex.IsMatch(input.Trim(), req);
        }

        public static bool IsLengthedNumber(string input, int len)
        {
            if (IsNullOrEmpty(input)) return false;


            if (IsNumber(input) && (input.Length == len)) return true;
            return false;
        }
        public static bool IsLengthed(string input, string min, string max)
        {
            if (IsNullOrEmpty(input)) return false;
            if (min == "*" && max == "*")
            {
                return true;
            }
            else if (min == "*")
            {
                return input.Length <= Converter.ToInt32(max, int.MaxValue);
            }
            else if (max == "*")
            {
                return input.Length >= Converter.ToInt32(min, int.MinValue);
            }
            else
            {
                return input.Length <= Converter.ToInt32(max, int.MaxValue) && input.Length >= Converter.ToInt32(min, int.MinValue);
            }

        }

        public static bool IsAllCHNChars(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            var req = @"[\u4e00-\u9fa5]{1,}$";
            return Regex.IsMatch(input.Trim(), req);
        }

        /// <summary>
        /// 英文大小写字母和数字混合
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsENAndNumChars(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            var req = @"^[A-Za-z0-9]+$";
            return Regex.IsMatch(input.Trim(), req);
        }

        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPositiveInteger(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            var req = @"^[0-9]*[1-9][0-9]*$";
            return Regex.IsMatch(input.Trim(), req);
        }
        public static bool IsInteger(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            var req = @"^[0-9]*[0-9][0-9]*$";
            return Regex.IsMatch(input.Trim(), req);
        }
        /// <summary>
        /// 正整数+0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPositiveIntegerAll(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            var req = "^\\d+$";
            return Regex.IsMatch(input.Trim(), req);
        }

        public static bool IsNumber(string input)
        {
            if (IsNullOrEmpty(input)) return true;
            // var req = @"^[\-]*[0-9]*$";
            var req = @"^[\-]{0,1}[0-9]*$";

            if (Regex.IsMatch(input.Trim(), req)) return true;
            return false;
        }
        
        /// <summary>
        /// 带小数的数值验证
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision">精度</param>
        /// <returns></returns>
        public static bool IsDecimal(string input, string precision)
        {
            try
            {
                decimal d = Decimal.Parse(input);
                int p = Int32.Parse(precision);
                if ( input.IndexOf(".") < 1)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }



            return input.Substring(input.IndexOf(".")).Length-1 <= Int32.Parse(precision);
        }

        public static string GetShortString(object str, int len)
        {
            if (str == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(str.ToString()))
            {
                return string.Empty;
            }
            if (str.ToString().Length > len)
            {
                return str.ToString().Substring(0, len) + "...";
            }
            else
            {
                return str.ToString();
            }
        }

        /// <summary>   
        /// 得到字符串的长度，一个汉字算2个字符   
        /// </summary>   
        /// <param name="str">字符串</param>   
        /// <returns>返回字符串长度</returns>   
        public static int GetLength(string str)
        {
            if (str.Length == 0) return 0;

            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    // Oracle的JDBC驱动会将中文转换为2字节或3字节
                    tempLen += 3;
                }
                else
                {
                    tempLen += 1;
                }
            }

            return tempLen;
        }

        /// <summary>
        /// oracle中的字符串截取
        /// 一个汉字3个字节
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="len">oracle db中的字符长度</param>
        /// <returns></returns>
        public static string SubStringDB(string str, int len)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            string strReturn = string.Empty;

            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    // Oracle的JDBC驱动会将中文转换为2字节或3字节
                    tempLen += 3;
                }
                else
                {
                    tempLen += 1;
                }
                if (tempLen <= len)
                {
                    strReturn += str.Substring(i, 1);
                }
                else
                {
                    break;
                }
            }

            return strReturn;
        }

        public static string ToPriceFormmat(object obj)
        {
            decimal dec = Converter.ToDecimal(obj, 0m);
            return dec.ToString("###0.00");
        }

        public static string ToWinePriceFormmat(object obj)
        {
            decimal dec = Converter.ToDecimal(obj, 0m);
            return dec.ToString("###0.0");
        }

        public static string ReplaceEnd(string value, string oldStr, string newStr)
        {
            if (IsNullOrEmpty(value))
            {
                return value;
            }
            if (!value.EndsWith(oldStr))
            {
                return value;
            }
            return value.Substring(0, value.LastIndexOf(oldStr)) + newStr;
        }

        /// <summary> 
        /// 简体转繁体 
        ///Eddy     2010.11.11
        /// </summary> 
        /// <param name="strRows"></param> 
        /// <returns></returns> 
        public static string ToComplexChinese(string character)
        {
            string complex;
            try
            {
                complex = Microsoft.VisualBasic.Strings.StrConv(character, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, System.Globalization.CultureInfo.CreateSpecificCulture("zh-CN").LCID);
                complex = Encoding.GetEncoding("Big5").GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("Big5"), Encoding.UTF8.GetBytes(complex)));
            }
            catch (Exception e)
            {
                throw e;
            }
            return complex;
        }
        /// <summary>
        ///繁体转简体
        ///Eddy     2010.11.11
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string ToSimplifiedChinese(string character)
        {
            string simplified;
            try
            {
                simplified = Microsoft.VisualBasic.Strings.StrConv(character, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, System.Globalization.CultureInfo.CreateSpecificCulture("zh-CN").LCID);
                simplified = Encoding.GetEncoding("UTF-8").GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("UTF-8"), Encoding.UTF8.GetBytes(simplified)));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return simplified;
        }

        /// <summary>
        /// 部分隐藏邮箱手机等用户隐私信息
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static string HideUserPrivacy(string userPrivacy)
        {
            if (!string.IsNullOrWhiteSpace(userPrivacy))
            {
                string strUP = userPrivacy.Trim();
                if (IsCellPhoneNumber(strUP))
                {
                    string cellnumber = strUP.Remove(3, 4);
                    cellnumber = cellnumber.Insert(3, "****");

                    return cellnumber;
                }
                else
                {
                    int hidelength = strUP.Length / 2;
                    int length = strUP.Length - hidelength;
                    string temp = strUP.Substring(hidelength, length);

                    temp = strUP.Replace(temp, RepeatString("*", length));

                    return temp;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 部分隐藏用户名，只显示第一个和最后一个字母，其余为*
        /// </summary>
        /// <param name="UserName">要隐藏的用户名</param>
        /// <returns>用户名大于2个字符则隐藏，否则不做操作</returns>
        public static string HideUserName(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName) || UserName.Length <= 2)
            {
                return UserName;
            }
            StringBuilder result = new StringBuilder();
            result.Append(UserName.Substring(0, 1));
            int hideLength = UserName.Length - 2;
            StringBuilder hideStr = new StringBuilder();
            for (int i = 0; i < hideLength; i++)
            {
                hideStr.Append('*');
            }
            result.Append(hideStr).Append(UserName.Substring(hideLength, 1));
            return result.ToString();
        }

        public static string RepeatString(string str, int n)
        {

            char[] arr = str.ToCharArray();

            char[] arrDest = new char[arr.Length * n];

            for (int i = 0; i < n; i++)
            {

                Buffer.BlockCopy(arr, 0, arrDest, i * arr.Length * 2, arr.Length * 2);

            }

            return new string(arrDest);

        }

        public static string DisplayString(string eptStra, string replaceStr)
        {
            if (IsNullOrEmpty(eptStra))
            {
                return replaceStr;
            }
            else
            {
                return eptStra;
            }
        }

        /// <summary>
        /// 获得时间戳
        /// </summary>
        /// <returns></returns>
        public static int GetTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(DateTime.Now - startTime).TotalSeconds;
        }
    }
}