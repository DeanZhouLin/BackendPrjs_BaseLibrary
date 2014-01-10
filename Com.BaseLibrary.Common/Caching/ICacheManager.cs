using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Com.BaseLibrary.Caching
{
    /// <summary>
    /// 缓存管理器的接口，用于操作缓存对象
    /// </summary>
    public interface ICacheManager : IEnumerable
    {
        /// <summary>
        /// 缓存中的对象个数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 根据缓存键获取缓存对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存的对象</returns>
        object this[string key] { get; }

        /// <summary>
        /// 添加一个对象到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">要缓存的对象</param>
        void Add(string key, object value);

        /// <summary>
        /// 添加一个对象到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">要缓存的对象</param>
        /// <param name="priority">缓存对象的优先级</param>
        /// <param name="callBack">缓存对象过期后触发的事件</param>
        /// <param name="expirations">过期策略</param>
        void Add(string key, object value, CacheObjectPriority priority, CacheObjectRemovedCallBack callBack, params object[] expirations);

        /// <summary>
        /// 检查当前缓存中是否包含指定对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        /// <summary>
        /// 清空缓存里面的所有对象
        /// </summary>
        void Flush();

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetData(string key);

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
