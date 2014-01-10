using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Com.BaseLibrary.Contract
{
    /// <summary>
    /// 查询返回结果集。
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[DataContract]
	[Serializable]
	public class ResultInfo<T>
	{
        /// <summary>
        /// 返回结果列表。
        /// </summary>
		[DataMember]
		public List<T> RecordList { get; set; }

        /// <summary>
        /// 返回结果数量。
        /// </summary>
		[DataMember]
		public int RecordCount { get; set; }
	}
}
