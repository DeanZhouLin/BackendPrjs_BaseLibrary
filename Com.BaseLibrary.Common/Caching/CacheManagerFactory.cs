using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Configuration;
using System.Reflection;

using Com.BaseLibrary.Utility;
using System.Web;

namespace Com.BaseLibrary.Caching
{
    /// <summary>
    /// Cache管理器创建工厂
    /// </summary>
    public class CacheManagerFactory
    {
        private const string CACHE_TYPE_NAME = "ImplCacheManager";

        private static ICacheManager m_SingleCacheManager;
        private static readonly object m_SynObj = new object();
        private static ICacheManager m_AspNetCacheManager;
        private static readonly object m_AspNetSynObj = new object();
        private static ICacheManager m_MicrosoftCacheManager;
        private static readonly object m_MiscrosoftSynObj = new object();

        /// <summary>
        /// 1，首先根据配置文件中指定的实现类创建缓存管理器
        /// 2，如果配置文件中未指定或指定错误，
        ///   1）Asp.NET应用程序，则创建AspNetCacheManager
        ///   2）非Web应用程序，则创建GeneralCacheManager（微软的企业库缓存模块）
        /// </summary>
        /// <returns></returns>
        public static ICacheManager CreateCacheManager()
        {
            if (m_SingleCacheManager == null)
            {
                lock (m_SynObj)
                {
                    if (m_SingleCacheManager == null)
                    {
                        try
                        {
                            string[] cacheTypeName = ConfigurationHelper.GetAppSetting(CACHE_TYPE_NAME).Split(',');
                            string fullCacheManagerClassName = cacheTypeName[0];
                            string cacheAssembleName = cacheTypeName[1];
                            m_SingleCacheManager = (ICacheManager)Assembly.Load(cacheAssembleName).CreateInstance(fullCacheManagerClassName);
                        }
                        catch
                        {
                            if (HttpRuntime.Cache == null)
                            {
                                m_SingleCacheManager = CreateGeneralCacheManager();
                              
                            }
                            else
                            {
                                m_SingleCacheManager = CreateAspNetCacheManager();
                            }
                        }
                    }
                }
            }
            return m_SingleCacheManager;
        }

        /// <summary>
        /// 创建GeneralCacheManager（微软的企业库缓存模块）
        /// </summary>
        /// <returns></returns>
        public static ICacheManager CreateGeneralCacheManager()
        {
            if (m_MicrosoftCacheManager == null)
            {
                lock (m_MiscrosoftSynObj)
                {
                    if (m_MicrosoftCacheManager == null)
                    {
                        try
                        {
                            m_MicrosoftCacheManager = (ICacheManager)Assembly.Load("Com.BaseLibrary.Caching").CreateInstance("Com.BaseLibrary.Caching.MicrosoftCacheManager");
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            return m_MicrosoftCacheManager;
        }

        /// <summary>
        /// 创建AspNetCacheManager
        /// </summary>
        /// <returns></returns>
        public static ICacheManager CreateAspNetCacheManager()
        {
            if (m_AspNetCacheManager == null)
            {
                lock (m_AspNetSynObj)
                {
                    if (m_AspNetCacheManager == null)
                    {
                        try
                        {
                            m_AspNetCacheManager = (ICacheManager)Assembly.GetExecutingAssembly().CreateInstance("Com.BaseLibrary.Caching.AspNetCacheManager");
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            return m_AspNetCacheManager;
        }

    }
}
