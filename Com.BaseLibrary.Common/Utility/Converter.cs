using System;
using System.Collections.Generic;
using System.Text;

namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// 数据类型转换工具类
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts the input value to an int32.
        /// If the input is null or cannot be converted to the target type, defaultValue is returned.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int ToInt32(object input, int defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            int output;
            bool res = int.TryParse(input.ToString(), out output);
            return res ? output : defaultValue;
        }

        public static long ToInt64(object input, long defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            long output;
            bool res = long.TryParse(input.ToString(), out output);
            return res ? output : defaultValue;
        }

        public static decimal ToDecimal(object input, decimal defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            decimal output;
            bool res = decimal.TryParse(input.ToString(), out output);
            return res ? output : defaultValue;
        }

        public static DateTime ToDateTime(object input, DateTime defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            DateTime output;
            bool res = DateTime.TryParse(input.ToString(), out output);
            return res ? output : defaultValue;
        }

        public static long ToLong(object input, long defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            long output;
            bool res = long.TryParse(input.ToString(), out output);
            return res ? output : defaultValue;
        }

        //public static List<T> ToList<T>(T[] array)
        //{
        //    List<T> listT = null;
        //    if (array != null && array.Length > 0)
        //    {
        //        listT = new List<T>(array.Length);
        //        for (int i = 0; i < array.Length; i++)
        //        {
        //            T obj = array[i];
        //            if (obj == null) continue;

        //            listT.Add(obj);
        //        }
        //    }
        //    return listT;
        //}

        public static bool ToBoolean(string value)
        {
            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }
            return false;

        }

        public static string ToString(byte[] p, char p_2)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < p.Length; i++)
            {
                sb.AppendFormat("{0}{1}", p[i], p_2);
            }
            return sb.ToString();
        }
    }
}