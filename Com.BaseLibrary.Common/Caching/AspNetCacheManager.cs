using System;
using System.Web;
using System.Text;
using System.Web.Caching;
using System.Collections.Generic;


namespace Com.BaseLibrary.Caching
{
    /// <summary>
    /// 基于Asp.NET Cache的缓存实现，只有在Web应用程序中方可使用，
    /// 使用Asp.NET里面的Cache缓存对象
    /// </summary>
    public class AspNetCacheManager : ICacheManager
    {
        private Cache AspNetCache
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }

        public AspNetCacheManager()
        {

        }

        #region ICacheManager Members


        public void Add(string key, object value, CacheObjectPriority priority, CacheObjectRemovedCallBack callBack, params object[] expirations)
        {
            CacheItemRemovedCallback onCallBack = null;
            CacheItemPriority itemPriority = GetPriority(priority);
            DateTime absExpiration = Cache.NoAbsoluteExpiration;
            TimeSpan slidingExpiration = Cache.NoSlidingExpiration;
            CacheDependency dependecy = null;
            if (callBack != null)
            {
                onCallBack =
                    delegate(string ikey, Object ivalue, CacheItemRemovedReason reason)
                    {
                        callBack(ikey, ivalue, GetReason(reason));
                    }
                    ;
            }

            if (expirations != null && expirations.Length > 0)
            {
                object obj = expirations[0];
                if (obj is DateTime)
                {
                    absExpiration = (DateTime)obj;
                }
                else if (obj is TimeSpan)
                {
                    slidingExpiration = (TimeSpan)obj;
                }
                if (obj is string)
                {
                    dependecy = new CacheDependency(obj.ToString());
                }
                else if (obj is SqlServerUtcDateTime)
                {
                    AggregateCacheDependency dependencies = new AggregateCacheDependency();
                    foreach (var item in expirations)
                    {
                        SqlServerUtcDateTime sqlUtcDateTime = obj as SqlServerUtcDateTime;
                        CustomSqlCacheDependence customSqlCacheDependence = new CustomSqlCacheDependence(sqlUtcDateTime.ConnectionString, sqlUtcDateTime.SQL, sqlUtcDateTime.Interval, sqlUtcDateTime.CommandType);
                        customSqlCacheDependence.Init();
                        dependencies.Add(customSqlCacheDependence);
                    }
                    dependecy = dependencies;
                }
                else
                {
                    AggregateCacheDependency dependencies = new AggregateCacheDependency();
                    foreach (var item in expirations)
                    {
                        if (item is CacheDependency)
                        {
                            dependencies.Add(item as CacheDependency);
                        }
                    }
                    dependecy = dependencies;
                }
            }

            AspNetCache.Add(key, value, dependecy, absExpiration, slidingExpiration, itemPriority, onCallBack);
        }



        public void Add(string key, object value)
        {
            Add(key, value, CacheObjectPriority.Default, null);
        }

        public bool Contains(string key)
        {
            return AspNetCache[key] != null;
        }

        public int Count
        {
            get { return AspNetCache.Count; }
        }

        public void Flush()
        {
            throw new NotSupportedException();
        }

        public object GetData(string key)
        {
            return AspNetCache.Get(key);
        }

        public void Remove(string key)
        {
            AspNetCache.Remove(key);
        }

        public object this[string key]
        {
            get { return AspNetCache[key]; }
        }

        #endregion

        private CacheObjectRemovedReason GetReason(CacheItemRemovedReason reason)
        {
            switch (reason)
            {
                case CacheItemRemovedReason.DependencyChanged:
                    return CacheObjectRemovedReason.DependencyChanged;
                case CacheItemRemovedReason.Expired:
                    return CacheObjectRemovedReason.Expired;
                case CacheItemRemovedReason.Removed:
                    return CacheObjectRemovedReason.Removed;
                case CacheItemRemovedReason.Underused:
                    return CacheObjectRemovedReason.Underused;
                default:
                    return CacheObjectRemovedReason.Removed;
            }
        }

        private CacheItemPriority GetPriority(CacheObjectPriority priority)
        {
            switch (priority)
            {
                case CacheObjectPriority.High:
                    return System.Web.Caching.CacheItemPriority.High;
                case CacheObjectPriority.Low:
                    return System.Web.Caching.CacheItemPriority.Low;
                case CacheObjectPriority.Default:
                    return System.Web.Caching.CacheItemPriority.Default;
                case CacheObjectPriority.NotRemovable:
                    return System.Web.Caching.CacheItemPriority.NotRemovable;
                default:
                    return System.Web.Caching.CacheItemPriority.Default;
            }
        }


        #region IEnumerable 成员

        public System.Collections.IEnumerator GetEnumerator()
        {
            return AspNetCache.GetEnumerator();
        }

        #endregion
    }
}
