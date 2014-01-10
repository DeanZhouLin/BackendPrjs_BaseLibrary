

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Loquat.CommonLibrary.Contract;
using System.Reflection;
using System.Data.Linq.Mapping;
using Loquat.CommonLibrary.Utility;
using System.Data.Common;

namespace Loquat.CommonLibrary.Service
{
	/// <summary>
	/// 所有数据访问对象的基类
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TDataContext"></typeparam>
	public abstract class DataAccessBaseNew<TEntity, TKey, TDataContext>
		where TEntity : DataContractBase<TKey>, new()
		where TDataContext : DataContextBase, new()
	{
		/// <summary>
		/// 创建一条数据
		/// </summary>
		/// <param name="entity"></param>
		public virtual TKey Create(TEntity entity)
		{
			using (DataContextBase dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				entities.InsertOnSubmit(entity);
				dataContext.SubmitChanges();
				return entity.Key;
			}
		}

		/// <summary>
		/// 删除某一条数据
		/// </summary>
		/// <param name="key"></param>
		public virtual void Delete(TKey key)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entites = dataContext.GetTable<TEntity>();
				entites.DirectDelete(c => c.Key.Equals(key));
				dataContext.SubmitChanges();
			}
		}


		/// <summary>
		/// 删除某一条数据
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Delete(TEntity entity)
		{
			Delete(entity.Key);
		}

		/// <summary>
		/// 批量删除指定数据
		/// </summary>
		/// <param name="keyList"></param>
		public virtual void BatchDelete(List<TKey> keyList)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				//Table<TEntity> entities = dataContext.GetTable<TEntity>();

				//var q = from c in entities where keyList.Contains(c.Key) select c;
				//entities.DeleteAllOnSubmit(q);
				//dataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
				Table<TEntity> entites = dataContext.GetTable<TEntity>();
				entites.DirectDelete(c => keyList.Contains(c.Key));
				dataContext.SubmitChanges();
			}
		}

		/// <summary>
		/// 删除表所有数据
		/// </summary>
		public virtual void DeleteAll()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				entities.DeleteAllOnSubmit<TEntity>(from c in entities select c);
				dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
			}
		}

		/// <summary>
		/// 根据主键返回一条数据
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual TEntity Get(TKey key)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				TEntity entity = new TEntity();
				return dataContext.GetTable<TEntity>().FirstOrDefault<TEntity>(c => c.Key.Equals(key));
			}
		}

		/// <summary>
		/// 返回表的所有数据
		/// </summary>
		/// <returns></returns>
		public virtual List<TEntity> GetAll()
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
		public virtual void Update(TEntity entity)
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
		/// 更新实体
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="oEntity"></param>
		public virtual void Update(TEntity entity, TEntity oEntity)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				dataContext.GetTable<TEntity>().Attach(oEntity);
				AttachValue(oEntity, entity);
				dataContext.SubmitChanges();


			}
		}

		/// <summary>
		/// 分页查询的实现
		/// </summary>
		/// <param name="queryProperty"></param>
		/// <returns></returns>
		public virtual QueryResultInfo<TEntity> Query(QueryPropertyInfo<TEntity> queryProperty)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				QueryResultInfo<TEntity> queryResult = new QueryResultInfo<TEntity>();
				Table<TEntity> entities = dataContext.GetTable<TEntity>();

				var query = ConstructQuery(queryProperty, entities);
				if (queryProperty.AllowPaging)
				{
					queryResult.RecordCount = query.Count();
					query = query.Skip(queryProperty.CurrentPageIndex * queryProperty.PageSize);
					query = query.Take(queryProperty.PageSize);
				}

				queryResult.RecordList = query.ToList<TEntity>();
				return queryResult;
			}
		}





		/// <summary>
		/// 一般查询的实现
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual List<TEntity> QueryList(TEntity entity)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				var query = ConstructWhere(entity, entities);
				return query.ToList<TEntity>();
			}
		}


		/// <summary>
		/// 构建分页查询的Where条件
		/// 具体由各子类具体实现
		/// </summary>
		/// <param name="queryProperty"></param>
		/// <param name="entities"></param>
		/// <returns></returns>
		protected virtual IQueryable<TEntity> ConstructQuery(QueryPropertyInfo<TEntity> queryProperty, Table<TEntity> entities)
		{
			return from c in entities select c;
		}


		/// <summary>
		/// 构建分页查询的Where条件
		/// 具体由各子类具体实现
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="entities"></param>
		/// <returns></returns>
		protected virtual IQueryable<TEntity> ConstructWhere(TEntity entity, Table<TEntity> entities)
		{

			return from c in entities select c;
		}




		/// <summary>
		/// 由具体的子类实现，用于更新数据时的赋值
		/// </summary>
		/// <param name="orgEntity">对应数据库中真实数据的对象</param>
		/// <param name="entity"></param>
		protected virtual void AttachValue(TEntity orgEntity, TEntity entity)
		{
			Type type = typeof(TEntity);
			List<PropertyInfo> properties = DataContractTypeManager.GetTypeProperies(type);
			foreach (PropertyInfo property in properties)
			{
				object value = property.GetValue(entity, null);
				property.SetValue(orgEntity, value, null);
			}
		}

		private string getTableName()
		{
			throw new NotImplementedException();
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
