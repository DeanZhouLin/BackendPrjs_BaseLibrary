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
	public abstract class BusinessBase<TDataAccess, TEntity, TKey, TDataContext>
		where TDataAccess : DataAccessBase<TEntity, TKey, TDataContext>, new()
		where TEntity : DataContractBase<TKey>, new()
		where TDataContext : DataContextBase, new()
	{
		/// <summary>
		/// 实例化对应的数据访问对象
		/// </summary>
		protected static readonly TDataAccess DAO = new TDataAccess();

		/// <summary>
		/// 创建一个记录
		/// </summary>
		/// <param name="entity"></param>
		public virtual TKey Create(TEntity entity)
		{
			try
			{
				ValidateRequiredField(entity);
				CheckForCreate(entity);
				DAO.Create(entity);
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
		public virtual List<TKey> BatchCreate(List<TEntity> entityList)
		{
			try
			{
				List<TKey> keyList = new List<TKey>();
				using (DbTransactionScope ts = new DbTransactionScope())
				{
					foreach (TEntity entity in entityList)
					{
						ValidateRequiredField(entity);
						CheckForCreate(entity);
						DAO.Create(entity);
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
		public virtual void Delete(TKey key)
		{
			try
			{
				DAO.Delete(key);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}

		}
		/// <summary>
		/// 清空整个表的记录
		/// </summary>
		public virtual void DeleteAll()
		{
			try
			{
				DAO.DeleteAll();
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
		public virtual void BatchDelete(List<TKey> keyList)
		{
			try
			{
				DAO.BatchDelete(keyList);
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
		public virtual void Update(TEntity entity)
		{
			try
			{
				ValidateRequiredField(entity);
				TEntity oEntity = CheckForUpdate(entity);
				DAO.Update(entity, oEntity);
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
		public virtual void BatchUpdate(List<TEntity> entityList)
		{
			try
			{
				using (DbTransactionScope ts = new DbTransactionScope())
				{
					foreach (TEntity entity in entityList)
					{
						ValidateRequiredField(entity);
						TEntity oEntity = CheckForUpdate(entity);
						DAO.Update(entity, oEntity);
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
		public virtual TEntity Get(TKey key)
		{
			try
			{
				return DAO.Get(key);
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
		public virtual List<TEntity> GetAll()
		{
			try
			{
				return DAO.GetAll();
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
		public virtual QueryResultInfo<TEntity> Query(QueryPropertyInfo<TEntity> queryProperty)
		{
			try
			{
				return DAO.Query(queryProperty);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		/// <summary>
		/// 一般查询
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual List<TEntity> QueryList(TEntity entity)
		{
			try
			{
				return DAO.QueryList(entity);
			}
			catch (Exception ex)
			{
				throw BizExceptionBuilder.BuildException(ex);
			}
		}

		public virtual void ChangeStatus(TKey key, string newStatus)
		{
			//do nothing
		}

		#region Tool Methods

		/// <summary>
		/// 检验不允许为空的字段是否为空，
		/// 由各实现的子类去实现该方法
		/// </summary>
		/// <param name="entity"></param>
		protected virtual void ValidateRequiredField(TEntity entity)
		{
		}

		/// <summary>
		/// 为创建数据而检验数据的完整性
		/// 由各实现的子类去实现该方法
		/// </summary>
		/// <param name="entity"></param>
		protected virtual void CheckForCreate(TEntity entity)
		{
		}

		/// <summary>
		/// 为更新数据而检验数据的完整性
		/// 由各实现的子类去实现该方法
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected virtual TEntity CheckForUpdate(TEntity entity)
		{
			return DAO.Get(entity.Key);
		}
		#endregion
	}
}
