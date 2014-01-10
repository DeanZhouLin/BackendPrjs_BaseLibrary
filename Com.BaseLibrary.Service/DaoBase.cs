using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Com.BaseLibrary.Data;

using Com.BaseLibrary.Entity;
using System.Data.Objects;
using Com.BaseLibrary.Contract;

namespace Com.BaseLibrary.Service
{
	public class DaoBase<TEntity, EF>
		where TEntity : DataContractBase, new()
		where EF : ObjectContext, new()
	{
		public static void Create(TEntity user)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				objectSet.AddObject(user);
				entities.SaveChanges();
			}
		}

		public static TEntity Get(int id)
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				return objectSet.FirstOrDefault(c => c.ID == id);
			}
		}

		public static int Delete(TEntity entity)
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
		public static int Delete(int Id)
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
		public static int BatchDelete(List<int> idList)
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

		public static List<TEntity> GetAll()
		{
			using (EF entities = new EF())
			{
				ObjectSet<TEntity> objectSet = entities.CreateObjectSet<TEntity>();
				return objectSet.ToList();
			}
		}
	}
}