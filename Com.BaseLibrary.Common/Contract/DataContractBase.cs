using System;
using System.Runtime.Serialization;

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

        /// <summary>
        /// 未知类的 属性值的获取
        /// dean（测试）
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetValue(string propertyName)
        {
            return GetType().GetProperty(propertyName).GetValue(this, null);
        }

        public T GetValue<T>(string propertyName) where T : class ,new()
        {
            return GetValue(propertyName) as T;
        }

        public object GetValue(string propertyName,object defaultValue)
        {
            var res = GetValue(propertyName);
            return res ?? defaultValue;
        }
    }
}
