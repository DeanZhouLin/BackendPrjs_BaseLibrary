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
using Com.BaseLibrary.ExceptionHandle;
using System.Collections;
using Com.BaseLibrary.Entity;

namespace Com.BaseLibrary.Service
{
	/// <summary>
	/// 所有数据访问对象的基类
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TDataContext"></typeparam>
	public abstract class DataAccessBase<TEntity, TKey, TDataContext>
		where TEntity : DataContractBase<TKey>, new()
		where TDataContext : DataContextBase, new()
	{

		#region Caches for TEntity
		private static Type m_EntityType;
		private static Type EntityType
		{
			get
			{
				if (m_EntityType == null)
				{
					m_EntityType = typeof(TEntity);
				}
				return m_EntityType;
			}
		}


		private static string m_TableName;
		private static string TableName
		{
			get
			{
				if (m_TableName == null)
				{
					using (TDataContext dataContext = new TDataContext())
					{
						m_TableName = dataContext.Mapping.GetTable(typeof(TEntity)).TableName;
					}

				}
				return m_TableName;
			}
		}

		private static string m_PrimaryKeyName;
		private static string PrimaryKeyName
		{
			get
			{
				if (m_PrimaryKeyName == null)
				{
					List<PropertyInfo> properties = DataContractTypeManager.GetTypeProperies(EntityType);
					foreach (var item in properties)
					{
						ColumnAttribute column = item.GetCustomAttributes(typeof(ColumnAttribute), false)[0] as ColumnAttribute;
						if (column.IsPrimaryKey)
						{
							m_PrimaryKeyName = column.Name;
							break;
						}
					}

				}
				return m_PrimaryKeyName;
			}
		}
		private static Dictionary<string, string> CachedSQLs = new Dictionary<string, string>();

		#endregion



		/// <summary>
		/// 创建一条数据
		/// </summary>
		/// <param name="entity"></param>
		public virtual TKey Create(TEntity entity)
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
		public virtual void Delete(TKey key)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				if (!CachedSQLs.ContainsKey("DELETEBYPRIMARYKEYSQL"))
				{
					CachedSQLs.Add("DELETEBYPRIMARYKEYSQL", String.Format("DELETE FROM {0} WHERE {1} = @Key", TableName, PrimaryKeyName));
				}

				SqlConnection connection = dataContext.Connection as SqlConnection;
				DbCommand command = new SqlCommand(CachedSQLs["DELETEBYPRIMARYKEYSQL"], connection);
				command.Parameters.Add(new SqlParameter("@Key", key));

				if (dataContext.Transaction != null)
				{
					command.Transaction = dataContext.Transaction;
				}
				else
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();
				}
				command.ExecuteNonQuery();
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
				if (keyList == null || keyList.Count == 0)
				{
					return;
				}
				if (keyList.Count == 1)
				{
					Delete(keyList[0]);
					return;
				}

				if (!CachedSQLs.ContainsKey("BATCHDELETEBYPRIMARYKEYSQL"))
				{
					CachedSQLs.Add("BATCHDELETEBYPRIMARYKEYSQL", String.Format("DELETE FROM {0} WHERE {1} IN (#Keys#)", TableName, PrimaryKeyName));
				}

				string strcommand = CachedSQLs["BATCHDELETEBYPRIMARYKEYSQL"];
				string list = null;
				if (keyList[0] is string)
				{
					list = ListUtil.ConnectToString1<TKey>(keyList, ",");
				}
				else
				{
					list = ListUtil.ConnectToString<TKey>(keyList, ",");
				}

				strcommand = strcommand.Replace("#Keys#", list);

				SqlConnection connection = dataContext.Connection as SqlConnection;
				DbCommand command = new SqlCommand(strcommand, connection);

				if (dataContext.Transaction != null)
				{
					command.Transaction = dataContext.Transaction;
				}
				else
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();
				}

				command.ExecuteNonQuery();
				//dataContext.ExecuteCommand(command);
				//dataContext.SubmitChanges();
			}
		}

		/// <summary>
		/// 删除表所有数据
		/// </summary>
		public virtual void DeleteAll()
		{
			using (TDataContext dataContext = new TDataContext())
			{
				if (!CachedSQLs.ContainsKey("DELETEALLSQL"))
				{
					CachedSQLs.Add("DELETEALLSQL", String.Format("DELETE FROM {0}", TableName));
				}
				string command = CachedSQLs["DELETEALLSQL"];
				dataContext.ExecuteCommand(command);
				dataContext.SubmitChanges();
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
				//dataContext.ReadWithNoLock = true;
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
			TEntity orgEntity = default(TEntity);
			using (TDataContext dataContext = new TDataContext())
			{
				orgEntity = dataContext.GetTable<TEntity>().FirstOrDefault<TEntity>(c => c.Key.Equals(entity.Key));
				UpdateValue(entity, orgEntity);
				dataContext.SubmitChanges();
			}

		}

		private void UpdateValue(object entity, object orgEntity)
		{
			List<PropertyInfo> properties = DataContractTypeManager.GetTypeProperies(EntityType);
			foreach (var item in properties)
			{
				ColumnAttribute column = item.GetCustomAttributes(typeof(ColumnAttribute), false)[0] as ColumnAttribute;
				if (column.IsPrimaryKey)
				{
					continue;
				}
				if (column.IsDbGenerated)
				{
					continue;
				}

				object oValue = item.GetValue(orgEntity, null);
				object nValue = item.GetValue(entity, null);

				if (!StringUtil.EqualsInStringFormat(oValue, nValue))
				{
					item.SetValue(orgEntity, nValue, null);
				}
			}
		}
		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="oEntity"></param>
		public virtual void Update(TEntity entity, TEntity orgEntity)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				if (orgEntity == null)
				{
					orgEntity = entities.FirstOrDefault<TEntity>(c => c.Key.Equals(entity.Key));
					if (orgEntity == null)
					{
						return;
					}
				}
				else
				{
					entities.Attach(orgEntity);
				}
				UpdateValue(entity, orgEntity);
				dataContext.SubmitChanges();
			}
		}

		private void UpdateBySQL(TEntity entity, TEntity orgEntity)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				if (orgEntity != null)
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("UPDATE {0} SET ", TableName);
					Hashtable updatedValue = new Hashtable();
					List<PropertyInfo> properties = DataContractTypeManager.GetTypeProperies(EntityType);
					foreach (var item in properties)
					{
						ColumnAttribute column = item.GetCustomAttributes(typeof(ColumnAttribute), false)[0] as ColumnAttribute;
						if (column.IsPrimaryKey)
						{
							updatedValue.Add(PrimaryKeyName, item.GetValue(entity, null));
							continue;
						}
						if (column.IsDbGenerated)
						{
							continue;
						}
						object oValue = item.GetValue(orgEntity, null);
						object nValue = item.GetValue(entity, null);

						if (!StringUtil.EqualsInStringFormat(oValue, nValue))
						{
							string columName = string.IsNullOrEmpty(column.Name) ? item.Name : column.Name;
							sb.AppendFormat("{0}=@{0},", columName);
							updatedValue.Add(columName, nValue);
						}
					}

					if (updatedValue.Count == 1)
					{
						return;
					}

					string strcommand = sb.ToString();
					strcommand = strcommand.TrimEnd(',');
					strcommand = strcommand + string.Format(" WHERE {0}=@{0}", PrimaryKeyName);
					SqlConnection connection = dataContext.Connection as SqlConnection;
					DbCommand command = new SqlCommand(strcommand, connection);

					foreach (var item in updatedValue.Keys)
					{
						command.Parameters.Add(new SqlParameter("@" + item, updatedValue[item]));
					}

					if (dataContext.Transaction != null)
					{
						command.Transaction = dataContext.Transaction;
					}
					else
					{
						if (connection.State != ConnectionState.Open)
							connection.Open();
					}
					command.ExecuteNonQuery();
				}
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
				dataContext.ReadWithNoLock = true;

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


		public TEntity GetByCustomWhere(Func<TEntity, bool> predicate)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.FirstOrDefault(predicate);
			}
		}
		public TResult GetByCustomWhere<TResult>(Func<TEntity, bool> predicate, Func<TEntity, TResult> selector)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.Where(predicate).Select(selector).FirstOrDefault();
			}
		}
		public List<TEntity> GetListByCustomWhere(Func<TEntity, bool> predicate)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.Where(predicate).ToList();
			}
		}

		public List<TResult> GetListByCustomWhere<TResult>(Func<TEntity, bool> predicate, Func<TEntity, TResult> selector)
		{
			using (TDataContext dataContext = new TDataContext())
			{
				Table<TEntity> entities = dataContext.GetTable<TEntity>();
				return entities.Where(predicate).Select(selector).ToList();
			}
		}

		public void DeleteByCustomWhere(Func<TEntity, bool> predicate)
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