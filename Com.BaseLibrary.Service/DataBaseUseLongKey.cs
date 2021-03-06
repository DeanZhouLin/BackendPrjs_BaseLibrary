﻿using System;
using System.Linq;
using System.Collections.Generic;
using Com.BaseLibrary.Entity;
using System.Data.Objects;
using Com.BaseLibrary.Contract;

namespace Com.BaseLibrary.Service
{
	public class DataBaseUseLongKey<TEntity, EF>
		where TEntity : DataContractBaseLongKey, new()
		where EF : ObjectContext, new()
	{
		public virtual void Create(TEntity user)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				objectSet.AddObject(user);
				entities.SaveChanges();
			}
		}
		public virtual TEntity Get(long id)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				return objectSet.FirstOrDefault(c => c.ID == id);
			}
		}
		public virtual int Delete(TEntity entity)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				//TEntity old = objectSet.FirstOrDefault(c => c.ID == entity.ID);
				//if (old != null)
				//{
				//    entities.DeleteObject(old);
				//}
				//entities.SaveChanges();

				//使用批量删除技术
				int result = objectSet.Delete(c => c.ID == entity.ID);
				entities.SaveChanges();
				return result;
			}
		}
		public virtual int Delete(long Id)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				//TEntity old = objectSet.FirstOrDefault(c => c.ID == Id);
				//if (old != null)
				//{
				//    entities.DeleteObject(old);
				//}
				//entities.SaveChanges();

				//使用批量删除技术
				int result = objectSet.Delete(c => c.ID == Id);
				entities.SaveChanges();
				return result;
			}
		}
		public virtual int BatchDelete(List<long> idList)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				//List<TEntity> entityList = objectSet.Where(c => idList.Contains(c.ID)).ToList();
				//foreach (var item in entityList)
				//{
				//    entities.DeleteObject(item);
				//}
				//entities.SaveChanges();

				//使用批量删除技术
				int result = objectSet.Delete(c => idList.Contains(c.ID));
				entities.SaveChanges();
				return result;
			}
		}
		public virtual List<TEntity> GetAll()
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				return objectSet.ToList();
			}
		}
		public virtual void Update(TEntity newEntity)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				TEntity oldEntity = objectSet.FirstOrDefault(c => c.ID == newEntity.ID);
				if (oldEntity == null)
				{
					return;
				}
				AttachValue(newEntity, oldEntity);
				entities.SaveChanges();
			}
		}

		public virtual QueryResultInfo<TEntity> Query(QueryConditionInfo<TEntity> queryCondition)
		{
			QueryResultInfo<TEntity> result = new QueryResultInfo<TEntity>();
			using (EF entities = new EF())
			{
				var query = GetSelectLinq(entities, queryCondition);
				query = SetWhereClause(queryCondition, query);
				if (queryCondition.ReturnAllData)
				{
					query = SetOrder(queryCondition, query);
					result.RecordList = query.ToList();
					result.RecordCount = result.RecordList.Count;
				}
				else
				{
					result.RecordCount = query.Count();
					if (result.RecordCount == 0)
					{
						result.RecordList = new List<TEntity>();
						return result;
					}

					query = SetOrder(queryCondition, query);

					result.CurrentPageIndex = queryCondition.PageIndex;
					int startRowIndex = (result.CurrentPageIndex - 1) * queryCondition.PageSize;
					while (startRowIndex >= result.RecordCount)
					{
						result.CurrentPageIndex = result.CurrentPageIndex - 1;
						startRowIndex = (result.CurrentPageIndex - 1) * queryCondition.PageSize;
					}
					int pageSize = Math.Min((result.RecordCount - startRowIndex), queryCondition.PageSize);
					result.RecordList = query.Skip(startRowIndex).Take(pageSize).ToList();
				}

			}
			return result;
		}

		protected virtual IQueryable<TEntity> GetSelectLinq(EF entities, QueryConditionInfo<TEntity> queryCondition)
		{
			ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
			var query = from dd in objectSet
						select dd;
			return query;
		}



		protected virtual IQueryable<TEntity> SetOrder(QueryConditionInfo<TEntity> queryCondition, IQueryable<TEntity> query)
		{
			return query;
		}
		protected virtual IQueryable<TEntity> SetWhereClause(QueryConditionInfo<TEntity> queryCondition, IQueryable<TEntity> query)
		{
			return query;
		}
		protected virtual void AttachValue(TEntity newEntity, TEntity oldEntity)
		{

		}

        public  ObjectParameter BuildParameter(string parameterName, object value)
        {
            return new ObjectParameter(parameterName, value == null ? DBNull.Value : value);
        }
	}
}