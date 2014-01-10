using System;

namespace Com.BaseLibrary.Caching
{
	
    /// <summary>
    /// 缓存对象被移除后触发的事件签名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="reason"></param>
	public delegate void CacheObjectRemovedCallBack(string key, Object value, CacheObjectRemovedReason reason);

    /// <summary>
    /// 缓存对象被移除的原因
    /// </summary>
	public enum CacheObjectRemovedReason
	{
        /// <summary>
        /// 被直接移除
        /// </summary>
		Removed,
        /// <summary>
        /// 过期而被系统自动移除
        /// </summary>
		Expired,

        /// <summary>
        /// 很久没有使用而被移除
        /// </summary>
		Underused,

        /// <summary>
        /// 因为依赖变化而被移除
        /// </summary>
		DependencyChanged,
	}
}
