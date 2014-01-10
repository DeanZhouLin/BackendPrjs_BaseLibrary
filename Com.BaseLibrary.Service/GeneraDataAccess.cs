using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Com.BaseLibrary.Contract;
using System.Reflection;
using System.Data.Linq.Mapping;
using Com.BaseLibrary.Utility;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using Com.BaseLibrary.Entity;

namespace Com.BaseLibrary.Service
{
	/// <summary>
	/// 所有数据访问对象的基类
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TDataContext"></typeparam>
	public class GeneraDataAccess<TDataContext>
		where TDataContext : DataContextBase, new()
	{
		/// <summary>
		/// 创建一条数据
		/// </summary>
		/// <param name="entity"></param>
		public virtual TKey Create<TEntity, TKey>(TEntity entity)
			where TEntity : DataContractBase<TKey>, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				entities.InsertOnSubmit(entity);
				dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
				return entity.Key;
			}
		}

		/// <summary>
		/// 删除某一条数据
		/// </summary>
		/// <param name="key"></param>
		public virtual void Delete<TEntity, TKey>(TKey key)
			where TEntity : DataContractBase<TKey>, new()
		{

			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				var v = from c in entities
						where c.Key.Equals(key)
						select c;
				entities.DeleteAllOnSubmit(v);
				dataContext.SubmitChanges();
			}

		}

		/// <summary>
		/// 删除某一条数据
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Delete<TEntity, TKey>(TEntity entity) where TEntity : DataContractBase<TKey>, new()
		{

			Delete<TEntity, TKey>(entity.Key);

		}

		/// <summary>
		/// 批量删除指定数据
		/// </summary>
		/// <param name="keyList"></param>
		public virtual void BatchDelete<TEntity, TKey>(List<TKey> keyList) where TEntity : DataContractBase<TKey>, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				var v = from c in entities
						where keyList.Contains(c.Key)
						select c;
				entities.DeleteAllOnSubmit(v);
				dataContext.SubmitChanges();
			}
		}

		/// <summary>
		/// 删除表所有数据
		/// </summary>
		public virtual void DeleteAll<TEntity>()
			where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();

				entities.DeleteAllOnSubmit(entities);
				dataContext.SubmitChanges();
			}
		}

		/// <summary>
		/// 根据主键返回一条数据
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual TEntity Get<TEntity, TKey>(TKey key)
			where TEntity : DataContractBase<TKey>, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				dataContext.ReadWithNoLock = true;
				return dataContext.GetTable<TEntity>().FirstOrDefault<TEntity>(c => c.Key.Equals(key));
			}
		}

		/// <summary>
		/// 返回表的所有数据
		/// </summary>
		/// <returns></returns>
		public virtual List<TEntity> GetAll<TEntity>()

				where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				return dataContext.GetTable<TEntity>().ToList<TEntity>();
			}
		}

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Update<TEntity, TKey>(TEntity entity)
			where TEntity : DataContractBase<TKey>, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				TEntity orgEntity = dataContext.GetTable<TEntity>().FirstOrDefault<TEntity>(c => c.Key.Equals(entity.Key));
				if (orgEntity != null)
				{
					AttachValue(orgEntity, entity);
					dataContext.SubmitChanges();
				}

			}
		}


		/// <summary>
		/// 分页查询的实现
		/// </summary>
		/// <param name="queryProperty"></param>
		/// <returns></returns>
		public virtual QueryResultInfo<TEntity> Query<TEntity>(QueryPagerInfo queryPager, Func<TEntity, bool> predicate)
			where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				dataContext.ReadWithNoLock = true;

				QueryResultInfo<TEntity> queryResult = new QueryResultInfo<TEntity>();
				Table<TEntity> entities = dataContext.GetTable<TEntity>();


				if (queryPager.AllowPaging)
				{
					queryResult.RecordCount = entities.Count(predicate);

					queryResult.RecordList = entities
						.Where(predicate)
						.Skip(queryPager.CurrentPageIndex * queryPager.PageSize)
						.Take(queryPager.PageSize)
						.ToList();

				}
				else
				{
					queryResult.RecordList = entities
						.Where(predicate)
						.ToList();
				}

				return queryResult;
			}
		}


		/// <summary>
		/// 由具体的子类实现，用于更新数据时的赋值
		/// </summary>
		/// <param name="orgEntity">对应数据库中真实数据的对象</param>
		/// <param name="entity"></param>
		protected virtual void AttachValue<TEntity>(TEntity orgEntity, TEntity entity)
		{
			Type type = typeof(TEntity);
			List<PropertyInfo> properties = DataContractTypeManager.GetTypeProperies(type);
			foreach (PropertyInfo property in properties)
			{
				object value = property.GetValue(entity, null);
				property.SetValue(orgEntity, value, null);
			}
		}


		public TEntity GetByCustomWhere<TEntity>(Func<TEntity, bool> predicate)
				where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.FirstOrDefault(predicate);
			}
		}

		public TResult GetByCustomWhere<TEntity, TResult>(Func<TEntity, bool> predicate, Func<TEntity, TResult> selector)
				where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.Where(predicate).Select(selector).FirstOrDefault();
			}
		}

		public List<TEntity> GetListByCustomWhere<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.Where(predicate).ToList();
			}
		}
		
		public List<TResult> GetListByCustomWhere<TEntity, TResult>(Func<TEntity, bool> predicate, Func<TEntity, TResult> selector) where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.Where(predicate).Select(selector).ToList();
			}
		}

		public void DeleteByCustomWhere<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, new()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				entities.DeleteAllOnSubmit(entities.Where(predicate));
				dataContext.SubmitChanges();
			}
		}
	}

	//public static class LinqToSqlExtension
	//{
	//    public static int DeleteBatch<T>(this Table<T> table, IQueryable<T> entities)
	//        where T : class
	//    {
	//        DbCommand delete = table.GetEnumerator<T>(entities);
	//        var parameters = from p in delete.Parameters.Cast<DbParameter>()
	//                         select p.Value;
	//        return table.Context.ExecuteCommand(delete.CommandText, parameters.ToArray());
	//    }
	//}



}