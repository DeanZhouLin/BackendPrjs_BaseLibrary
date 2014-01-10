using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Reflection;


namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// List工具类
    /// </summary>
    public static class ListUtil
    {
        /// <summary>
        /// 从一个List克隆出一个新的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(IList<T> from)
        {
            List<T> to = new List<T>();
            if (from == null)
            {
                return to;
            }
            foreach (T t in from)
            {
                to.Add(t);
            }
            return to;
        }

        /// <summary>
        /// 把一个List复制到另一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopyTo<T>(IList<T> from, IList<T> to)
        {
            if (from == null || to == null)
                return;

            foreach (T t in from)
            {
                to.Add(t);
            }
        }

        /// <summary>
        /// 把一个数组转换成一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromList"></param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(T[] fromList)
        {
            List<T> tList = new List<T>();

            if (fromList == null || fromList.Length == 0)
                return tList;

            foreach (T t in fromList)
            {
                tList.Add(t);
            }
            return tList;
        }

        /// <summary>
        /// 把一个数组转换成一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromList"></param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(ICollection<T> collectons)
        {
            List<T> tList = new List<T>();

            if (collectons == null)
                return tList;

            IEnumerator<T> em = collectons.GetEnumerator();
            int i = 0;
            foreach (T t in collectons)
            {
                tList.Add(t);
                i++;
            }
            return tList;
        }


        /// <summary>
        /// 把一个Collection转换成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <returns></returns>
        public static T[] ConvertToArray<T>(ICollection<T> tlist)
        {
            if (tlist == null)
            {
                return new T[0];
            }
            T[] arrayT = new T[tlist.Count];
            int i = 0;
            foreach (T t in tlist)
            {
                arrayT[i] = t;
                i++;
            }
            return arrayT;

        }

        /// <summary>
        /// 把两个数组合并成一个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static T[] CombainArray<T>(T[] array1, T[] array2)
        {
            if (array1 == null)
            {
                return array2;
            }
            else if (array2 == null)
            {
                return array1;
            }
            else
            {
                T[] ss = new T[array1.Length + array2.Length];
                for (int i = 0; i < array1.Length; i++)
                {
                    ss[i] = array1[i];
                }
                for (int i = 0; i < array2.Length; i++)
                {
                    ss[i + array1.Length] = array2[i];
                }
                return ss;
            }
        }

        /// <summary>
        /// 把两个List合并成一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> ConbainList<T>(List<T> list1, List<T> list2)
        {
            List<T> ss = new List<T>();
            if (list1 != null)
            {
                foreach (T t in list1)
                {
                    ss.Add(t);
                }
            }
            if (list2 != null)
            {
                foreach (T t in list2)
                {
                    ss.Add(t);
                }
            }
            return ss;
        }

        public static string ConnectToString(List<string> list, char delimiter)
        {
            if (list == null || list.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (object item in list)
            {
                if (StringUtil.IsNullOrEmpty(item))
                {
                    continue;
                }
                sb.Append(string.Format("{0}{1}", item, delimiter));
            }
            string str = sb.ToString();
            if (str.EndsWith(delimiter.ToString()))
            {
                str = str.TrimEnd(delimiter);
            }
            return str;
        }

        public static string ConnectToString<TType>(List<TType> list, string delimiter)
        {
            if (list == null || list.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (TType item in list)
            {
                if (StringUtil.IsNullOrEmpty(item))
                {
                    continue;
                }
                sb.Append(string.Format("{0}{1}", item, delimiter));
            }
            string str = sb.ToString();
            if (str.EndsWith(delimiter.ToString()))
            {
                str = str.Substring(0, str.Length - delimiter.Length);
            }
            return str;
        }
        public static string ConnectToString1<TType>(List<TType> list, string delimiter)
        {
            if (list == null || list.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (TType item in list)
            {
                if (StringUtil.IsNullOrEmpty(item))
                {
                    continue;
                }
                sb.Append(string.Format("\'{0}\'{1}", item, delimiter));
            }
            string str = sb.ToString();
            if (str.EndsWith(delimiter.ToString()))
            {
                str = str.Substring(0, str.Length - delimiter.Length);
            }
            return str;
        }
        public static List<T> SplitToList<T>(string ObjectIDList, char delimiter)
        {
            List<T> listT = new List<T>();
            if (StringUtil.IsNullOrEmpty(ObjectIDList))
            {
                return listT;
            }
            string[] idArray = ObjectIDList.Split(delimiter);

            foreach (string item in idArray)
            {
                if (StringUtil.IsNullOrEmpty(item))
                {
                    continue;
                }
                listT.Add((T)StringUtil.ConvertToT<T>(item));
            }
            return listT;
        }



        public static List<T> Clone<T>(ICollection<T> collection)
        {
            List<T> to = new List<T>();
            if (collection == null)
            {
                return to;
            }
            foreach (T t in collection)
            {
                to.Add(t);
            }
            return to;
        }


        public static DataTable ConvertToTable<T>(List<T> list)
        {
            DataTable dt = new DataTable("dt");
            Type type = typeof(T);
            List<PropertyInfo> propertyList = TypeUtil.GetValuePropertyList(type);
            foreach (PropertyInfo property in propertyList)
            {
                Type realPropertyType = TypeUtil.GetRealPropertyType(property.PropertyType);
                DataColumn column = new DataColumn(property.Name, realPropertyType);
                dt.Columns.Add(column);
            }

            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo property in propertyList)
                {
                    object val = property.GetValue(t, null);
                    if (val == null)
                    {
                        row[property.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[property.Name] = val;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}