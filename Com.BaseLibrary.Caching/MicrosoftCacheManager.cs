using System;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.EnterpriseLibrary.Caching.BackingStoreImplementations;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;

using EntCacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager;
namespace Com.BaseLibrary.Caching
{
	/// <summary>
	/// 基于企业库缓存的实现
	/// </summary>
	public class MicrosoftCacheManager : ICacheManager
	{
		private EntCacheManager s_CacheManager;

		private class CacheItemRefreshAction : ICacheItemRefreshAction
		{

			private CacheObjectRemovedCallBack m_RemovedCallBack;

			internal CacheItemRefreshAction(CacheObjectRemovedCallBack callBack)
			{
				m_RemovedCallBack = callBack;
			}

			public void Refresh(string removedKey, object expiredValue, CacheItemRemovedReason removalReason)
			{
				m_RemovedCallBack(removedKey, expiredValue, GetReason(removalReason));
			}

			private CacheObjectRemovedReason GetReason(CacheItemRemovedReason reason)
			{
				switch (reason)
				{
					case CacheItemRemovedReason.Removed:
						return CacheObjectRemovedReason.DependencyChanged;
					case CacheItemRemovedReason.Expired:
						return CacheObjectRemovedReason.Expired;
					case CacheItemRemovedReason.Unknown:
						return CacheObjectRemovedReason.Removed;
					case CacheItemRemovedReason.Scavenged:
						return CacheObjectRemovedReason.Underused;
					default:
						return CacheObjectRemovedReason.Removed;
				}
			}
		}


		public MicrosoftCacheManager()
		{
			CreateCacheManager();
		}

		/// <summary>
		/// First get default cache manager according to configuration
		/// if no cache configuration,constract a cachemanager
		/// </summary>
		private void CreateCacheManager()
		{
			EntCacheManager cacheManager = null;
			try
			{
				cacheManager = CacheFactory.GetCacheManager();
			}
			catch
			{
				//do nothing
			}
			if (cacheManager == null)
			{
				DictionaryConfigurationSource internalConfigurationSource = new DictionaryConfigurationSource();
				CacheManagerSettings settings = new CacheManagerSettings();
				internalConfigurationSource.Add(CacheManagerSettings.SectionName, settings);
				CacheStorageData storageConfig = new CacheStorageData("Null Storage", typeof(NullBackingStore));
				settings.BackingStores.Add(storageConfig);
				CacheManagerData cacheManagerConfig = new CacheManagerData("CommonLibraryCache", 5, 1000, 10, storageConfig.Name);
				settings.CacheManagers.Add(cacheManagerConfig);
				settings.DefaultCacheManager = cacheManagerConfig.Name;
				Microsoft.Practices.EnterpriseLibrary.Caching.CacheManagerFactory cacheFactory = new Microsoft.Practices.EnterpriseLibrary.Caching.CacheManagerFactory(internalConfigurationSource);
				cacheManager = cacheFactory.CreateDefault();
			}
			this.s_CacheManager = cacheManager;
		}

		public MicrosoftCacheManager(string cacheName)
		{
			s_CacheManager = CacheFactory.GetCacheManager(cacheName);
		}

		#region ICacheManager Members

		public int Count
		{
			get { return s_CacheManager.Count; }
		}

		public object this[string key]
		{
			get { return s_CacheManager[key]; }
		}

		public void Add(string key, object value)
		{
			s_CacheManager.Add(key, value);
		}

		public void Add(string key, object value, CacheObjectPriority priority, CacheObjectRemovedCallBack callBack, params object[] expirations)
		{
			CacheItemPriority itemPriority = GetPriority(priority);
			ICacheItemRefreshAction action = null;
			ICacheItemExpiration[] expires = null;
			if (callBack != null)
			{
				action = new CacheItemRefreshAction(callBack);
			}
			if (expirations != null && expirations.Length > 0)
			{
				expires = new ICacheItemExpiration[expirations.Length];
				for (int i = 0; i < expirations.Length; i++)
				{
					expires[i] = GetExpiration(expirations[i]);
				}
			}
			s_CacheManager.Add(key, value, itemPriority, action, expires);
		}



		public bool Contains(string key)
		{
			return s_CacheManager.Contains(key);
		}

		public void Flush()
		{
			s_CacheManager.Flush();
		}

		public object GetData(string key)
		{
			return s_CacheManager.GetData(key);
		}

		public void Remove(string key)
		{
			s_CacheManager.Remove(key);
		}

		#endregion

		private ICacheItemExpiration GetExpiration(object obj)
		{
			if (obj is DateTime)
			{
				return new AbsoluteTime((DateTime)obj);
			}

			if (obj is TimeSpan)
			{
				return new Microsoft.Practices.EnterpriseLibrary.Caching.Expirations.SlidingTime((TimeSpan)obj);
			}

			if (obj is string)
			{
				return new Microsoft.Practices.EnterpriseLibrary.Caching.Expirations.FileDependency(obj.ToString());
			}

			if (obj is SqlServerUtcDateTime)
			{
				SqlServerUtcDateTime ss = obj as SqlServerUtcDateTime;
				return new SqlDateTimeDependency(ss.ConnectionString, ss.SQL, ss.CommandType);
			}

			return new Microsoft.Practices.EnterpriseLibrary.Caching.Expirations.NeverExpired();
		}

		private CacheItemPriority GetPriority(CacheObjectPriority priority)
		{
			switch (priority)
			{
				case CacheObjectPriority.High:
					return CacheItemPriority.High;
				case CacheObjectPriority.Low:
					return CacheItemPriority.Low;
				case CacheObjectPriority.Default:
					return CacheItemPriority.Normal;
				case CacheObjectPriority.NotRemovable:
					return CacheItemPriority.NotRemovable;
				default:
					return CacheItemPriority.Normal;
			}
		}

		#region IEnumerable 成员

		public System.Collections.IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
