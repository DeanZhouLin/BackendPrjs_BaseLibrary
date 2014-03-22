using System;
using System.Web;
using System.Web.Caching;

using Com.BaseLibrary.Logging;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Caching;


namespace Com.BaseLibrary.Configuration
{
    /// <summary>
    /// 加载配置文件错误异常
    /// </summary>
    public class LoadFileException : ApplicationException
    {
        /// <summary>
        /// 文件的路径
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; private set; }


        public LoadFileException(string typeName, string fileName, Exception e)
            : base(string.Empty, e)
        {
            FileName = fileName;
            TypeName = typeName;


        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("Unable to load file {0} for type {1}", FileName, TypeName);
            }
        }
    }

    /// <summary>
    /// 配置文件管理工具
    /// </summary>
    public static class ConfigurationManager
    {
        #region const
        private const string LoadFileFailedMessage =
@"Load config file failed:
  File:{0}
  Type:{1}
  Exception
{2}";
        #endregion

        /// <summary>
        /// 缓存管理器
        /// </summary>
        private static ICacheManager CacheManager
        {
            get
            {
                return CacheManagerFactory.CreateCacheManager();
            }
        }

        private static readonly object m_SyncObject = new object();

        /// <summary>
        /// 根据配置文件内容反序列化成一个配置对象
        /// 用configFile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configFile">配置文件</param>
        /// <returns></returns>
        public static T LoadConfiguration<T>(string configFile) where T : class
        {
            return LoadConfiguration<T>(configFile, ConfigurationHelper.GetAppSetting(configFile));
        }

        /// <summary>
        /// 根据配置文件内容反序列化成一个配置对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的类型</typeparam>
        /// <param name="key">存在缓存里面的键值</param>
        /// <param name="configFile">配置文件</param>
        /// <returns>返回根据配置文件反序列化的对象</returns>
        public static T LoadConfiguration<T>(string key, string configFile) where T : class
        {
            if (configFile == null)
            {
                configFile = key;
            }
            return LoadConfiguration<T>(key, configFile, null);
        }

        /// <summary>
        /// 根据配置文件内容反序列化成一个配置对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的类型</typeparam>
        /// <param name="key">存在缓存里面的键值</param>
        /// <param name="configFile">配置文件</param>
        /// <param name="callBack">对象从缓存中移除时触发的事件</param>
        /// <returns>返回根据配置文件反序列化的对象</returns>
        public static T LoadConfiguration<T>(string key, string configFile, CacheObjectRemovedCallBack callBack)
            where T : class
        {
            T config = CacheManager[key] as T;
            if (config == null)
            {
                lock (m_SyncObject)
                {
                    {
                        string fullPath = PathUtil.GetFullFilePath(configFile);
                        try
                        {
                            config = ObjectXmlSerializer.LoadFromXml<T>(fullPath);
                            CacheManager.Add(key, config, CacheObjectPriority.Default, callBack, fullPath);
                        }
                        catch (Exception e)
                        {
                            throw new LoadFileException(typeof(T).Name, configFile, e);
                        }
                    }
                }
            }

            return config;
        }

        public static void RemoveConfigurationCache(string key)
        {
            if (!CacheManager.Contains(key)) return;
            lock (m_SyncObject)
            {
                try
                {
                    CacheManager.Remove(key);
                }
                catch
                {
                    throw new Exception("RemoveConfigurationCache error");
                }
            }
        }

        /// <summary>
        /// 从缓存中获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetFromCache<T>(string key) where T : class
        {
            return CacheManager[key] as T;
        }


        public static void SaveConfiguration<T>(string configFile, T t) where T : class
        {
            ObjectXmlSerializer.Save(configFile, t);
        }

        public static void SaveAs<T>(string configFile, T t) where T : class
        {
            ObjectXmlSerializer.SaveAs(configFile, t);
        }
    }
}