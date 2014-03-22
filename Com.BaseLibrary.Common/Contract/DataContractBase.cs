using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Contract
{
    /// <summary>
    /// 实体类的基类
    /// </summary>
    [DataContract]
    [Serializable]
    public class DataContractBase
    {
        public DataContractBase()
        { }

        /// <summary>
        /// 辅助字段
        /// 用于帮助实现DataGridView上某行选中或未选中 
        /// </summary>
        /// [DataMember]
        public bool Selected { get; set; }


        /// <summary>
        /// 辅助字段
        /// 用于帮助实现DataGridView上某行选中或未选中时的标示
        /// </summary>
        public string SelectedMark
        {
            get
            {
                return Selected ? "√" : string.Empty;
            }
            set
            {
                Selected = value == "√";
            }
        }

        /// <summary>
        /// 实体主键
        /// 表的主键对应该属性
        /// </summary>
        [DataMember]
        public virtual int ID { get; set; }


        /// <summary>
        /// 判断两个字符串在省略掉尾部的空格之后是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected bool StringEqualWithoutSpace(string a, string b)
        {
            if (a == null)
            {
                return b == null;
            }
            return b != null && a.TrimEnd() == b.TrimEnd();
        }

        /// <summary>
        /// 去掉字符串的两边的空格
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        protected string TrimSpace(string a)
        {
            return a == null ? null : a.Trim();
        }

    }

    /// <summary>
    /// 实体类扩展
    /// dean 添加
    /// </summary>
    public static class DataContractBaseExtension
    {

        public static object GetValue(this DataContractBase currObj, string propertyName)
        {
            return currObj.GetType().GetProperty(propertyName).GetValue(currObj, null);
        }

        public static object GetValue(this DataContractBase currObj, string propertyName, object nullValue)
        {
            return currObj.GetValue(propertyName) ?? nullValue;
        }

        public static T GetValue<T>(this DataContractBase currObj, string propertyName) where T : class ,new()
        {
            return currObj.GetValue(propertyName) as T;
        }

        public static void SetValue(this DataContractBase currObj, string propertyName, object value)
        {
            currObj.GetType().GetProperty(propertyName).SetValue(currObj, value, null);
        }

        public static void BatchSetValue(this DataContractBase currObj, Dictionary<string, object> propAndValues)
        {
            if (propAndValues == null) return;

            foreach (var pv in propAndValues)
            {
                currObj.SetValue(pv.Key, pv.Value);
            }
        }

        public static void SetCreateEditInfo(this DataContractBase currObj, string currentUserName, DateTime dtNow, Dictionary<string, object> propAndValues = null)
        {
            currObj.BatchSetValue(new Dictionary<string, object>
            {
                {"CreateUser",currentUserName},
                {"CreateDate",dtNow},
                {"EditUser",currentUserName},
                {"EditDate",dtNow}
            });
            currObj.BatchSetValue(propAndValues);
        }

        public static void SetEditInfo(this DataContractBase currObj, string currentUserName, DateTime dtNow, Dictionary<string, object> propAndValues = null)
        {
            currObj.BatchSetValue(new Dictionary<string, object>
            {
                {"EditUser",currentUserName},
                {"EditDate",dtNow}
            });
            currObj.BatchSetValue(propAndValues);
        }

        public static void AddItem<T, V>(this Dictionary<T, V> sourceDic, T key, V value)
        {
            if (sourceDic == null)
            {
                throw new NullReferenceException();
            }
            if (sourceDic.ContainsKey(key))
            {
                sourceDic[key] = value;
            }
            else
            {
                sourceDic.Add(key, value);
            }
        }
    }


}
