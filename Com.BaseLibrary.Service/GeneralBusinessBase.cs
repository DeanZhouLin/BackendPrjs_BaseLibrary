using System;
using System.Linq;
using System.Text;
using System.Data.Linq;

using System.Collections.Generic;
using Com.BaseLibrary.Contract;
using Com.BaseLibrary.Transaction;
using Com.BaseLibrary.Entity;

namespace Com.BaseLibrary.Service
{
	/// <summary>
	/// 所有业务逻辑处理类的基类
	/// </summary>
	/// <typeparam name="TDataAccess">相应的数据访问对象</typeparam>
	/// <typeparam name="TEntity">相应的实体</typeparam>
	/// <typeparam name="TKey">相应的实体主键的数据类型</typeparam>
	/// <typeparam name="TDataContext">对应的数据库DataContext对象</typeparam>
	public abstract class GeneralBusinessBase<TDataContext>
		where TDataContext : DataContextBase, new()
	{
		/// <summary>
		/// 实例化对应的数据访问对象
		/// </summary>
		protected static readonly GeneraDataAccess<TDataContext> DAO = new GeneraDataAccess<TDataContext>();

		/// <summary>
		/// 创建一个记录
		/// </summary>
		/// <param name="entity"></param>
		public static TKey Create<TEntity, TKey>(TEntity entity)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				DAO.Create<TEntity, TKey>(entity);
				return entity.Key;
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		/// <summary>
		/// 创建一批记录
		/// </summary>
		/// <param name="entityList"></param>
		public static List<TKey> BatchCreate<TEntity, TKey>(List<TEntity> entityList)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				List<TKey> keyList = new List<TKey>();
				using (DbTransactionScope ts = new DbTransactionScope())
				{
					foreach (TEntity entity in entityList)
					{
						DAO.Create<TEntity, TKey>(entity);
						keyList.Add(entity.Key);
					}
					ts.Complete();

				}
				return keyList;
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		/// <summary>
		/// 根据主键删除一条记录
		/// </summary>
		/// <param name="key"></param>
		public static void Delete<TEntity, TKey>(TKey key)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				DAO.Delete<TEntity, TKey>(key);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}

		}
		/// <summary>
		/// 清空整个表的记录
		/// </summary>
		public static void DeleteAll<TEntity>() where TEntity : class, new()
		{
			try
			{
				DAO.DeleteAll<TEntity>();
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}

		}
		/// <summary>
		/// 根据主键列表批量删除数据
		/// </summary>
		/// <param name="keyList">主键列表</param>
		public static void BatchDelete<TEntity, TKey>(List<TKey> keyList)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				DAO.BatchDelete<TEntity, TKey>(keyList);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}

		}
		/// <summary>
		/// 更新一条记录
		/// </summary>
		/// <param name="entity"></param>
		public static void Update<TEntity, TKey>(TEntity entity)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				DAO.Update<TEntity, TKey>(entity);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}

		}

		/// <summary>
		/// 批量更新
		/// </summary>
		/// <param name="entityList"></param>
		public static void BatchUpdate<TEntity, TKey>(List<TEntity> entityList)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				using (DbTransactionScope ts = new DbTransactionScope())
				{
					foreach (TEntity entity in entityList)
					{
						DAO.Update<TEntity, TKey>(entity);
					}

					ts.Complete();

				}
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		/// <summary>
		/// 根据主键获取一条记录
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static TEntity Get<TEntity, TKey>(TKey key)
			where TEntity : DataContractBase<TKey>, new()
		{
			try
			{
				return DAO.Get<TEntity, TKey>(key);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		/// <summary>
		/// 获取整个表的记录
		/// </summary>
		/// <returns></returns>
		public static List<TEntity> GetAll<TEntity>()
			where TEntity : class, new()
		{
			try
			{
				return DAO.GetAll<TEntity>();
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		/// <summary>
		/// 分页查询，返回结果集合满足条件的记录数
		/// </summary>
		/// <param name="queryProperty"></param>
		/// <returns></returns>
		public static QueryResultInfo<TEntity> Query<TEntity>(QueryPagerInfo queryProperty, Func<TEntity, bool> predicate)
				where TEntity : class, new()
		{
			try
			{
				return DAO.Query<TEntity>(queryProperty, predicate);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		public static TEntity GetByCustomWhere<TEntity>(Func<TEntity, bool> predicate)
				where TEntity : class, new()
		{
			try
			{
				return DAO.GetByCustomWhere<TEntity>(predicate);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		public static List<TEntity> GetListByCustomWhere<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, new()
		{
			try
			{
				return DAO.GetListByCustomWhere<TEntity>(predicate);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		public static void DeleteByCustomWhere<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, new()
		{
			try
			{
				DAO.DeleteByCustomWhere<TEntity>(predicate);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		public static TResult GetByCustomWhere<TEntity, TResult>(Func<TEntity, bool> predicate, Func<TEntity, TResult> selector) where TEntity : class, new()
		{
			try
			{
				return DAO.GetByCustomWhere<TEntity, TResult>(predicate, selector);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		public static List<TResult> GetListByCustomWhere<TEntity, TResult>(Func<TEntity, bool> predicate, Func<TEntity, TResult> selector) where TEntity : class, new()
		{
			try
			{
				return DAO.GetListByCustomWhere<TEntity, TResult>(predicate, selector);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}
	}
}
