using System.ServiceModel;
using System.Collections.Generic;

namespace Com.BaseLibrary.Contract
{
	/// <summary>
	/// Service的基类，定义了增，删，改，查
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	[ServiceContract]
	public interface IServiceContract<TEntity> : IServiceBase
		where TEntity : DataContractBase, new()
	{
		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="entity"></param>
		[OperationContract]
		int Create(TEntity entity);

		/// <summary>
		/// 批量的创建
		/// </summary>
		/// <param name="entityList"></param>
		[OperationContract]
		List<int> BatchCreate(List<TEntity> entityList);

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="key"></param>
		[OperationContract]
		void Delete(int key);

		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name="keyList"></param>
		[OperationContract]
		void BatchDelete(List<int> keyList);

		/// <summary>
		/// 删除所有数据，慎用
		/// </summary>
		[OperationContract]
		void DeleteAll();

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="entity"></param>
		[OperationContract]
		void Update(TEntity entity);

		/// <summary>
		/// 批量的更新
		/// </summary>
		/// <param name="entityList"></param>
		[OperationContract]
		void BatchUpdate(List<TEntity> entityList);

		/// <summary>
		/// 根据实体主键获取实体对象
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[OperationContract]
		TEntity Get(int key);

        [OperationContract]
        TEntity ItemGet(int key);
		/// <summary>
		/// 返回实体对应的表的所有数据
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<TEntity> GetAll();

		/// <summary>
		/// 标准的分页查询
		/// </summary>
		/// <param name="queryProperty"></param>
		/// <returns></returns>
		[OperationContract]
		ResultInfo<TEntity> Query(QueryPropertyInfo<TEntity> queryProperty);
        
        [OperationContract]
        ResultInfo<TEntity> ItemQuery(QueryPropertyInfo<TEntity> queryProperty);

		/// <summary>
		/// 普通的查询
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		[OperationContract]
		List<TEntity> QueryList(TEntity entity);

		void ChangeStatus(int key, string newStatus);
	}

	public interface IServiceBase
	{

	}
}
