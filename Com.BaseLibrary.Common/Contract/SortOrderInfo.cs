using System;
using System.Runtime.Serialization;

namespace Com.BaseLibrary.Contract
{
    /// <summary>
    /// 排序配置类。
    /// </summary>
	[DataContract]
	[Serializable]
	public class SortOrderInfo
	{
        /// <summary>
        /// 排序字段。
        /// </summary>
		[DataMember]
		public string SortField { get; set; }

        /// <summary>
        /// 升降次序设置。
        /// </summary>
		[DataMember]
		public SortDirection SortDirection { get; set; }
	}

    /// <summary>
    /// 升降序枚举。
    /// </summary>
	public enum SortDirection
	{
		[EnumMember]
		ASC,
		[EnumMember]
		DESC,
	}

}
