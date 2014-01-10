using System;
using System.Runtime.Serialization;

namespace Com.BaseLibrary.Contract
{
    /// <summary>
    /// 分页查询条件设置类。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
	[Serializable]
    public class QueryPagerInfo
    {
		public QueryPagerInfo()
        {
            AllowPaging = true;
          

        }
     

        /// <summary>
        /// 排序条件设置。
        /// </summary>
        [DataMember]
        public SortOrderInfo SortOrder { get; set; }

        /// <summary>
        /// 当前页码。
        /// </summary>
        [DataMember]
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// 每页记录数。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 是否分页，如果不支持分页，则返回满足条件的所有记录
        /// </summary>
        public bool AllowPaging { get; set; }
    }
}
