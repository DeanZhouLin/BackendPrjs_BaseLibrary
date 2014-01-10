using System;
using System.Collections.Generic;
using System.Text;

namespace Com.BaseLibrary.Entity
{
	[Serializable]
	public class QueryConditionInfo<T>
		where T : class, new()
	{
		public QueryConditionInfo()
		{
			OrderFileds = new List<OrderFiledInfo>();
			this.Condtion = new T();
		}
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public T Condtion { get; set; }
		/// <summary>
		/// 是否返回所有数据
		/// </summary>
		public bool ReturnAllData { get; set; }
		public List<OrderFiledInfo> OrderFileds { get; set; }
        /// <summary>
        /// 非管理员只能读取有权限商家的数据
        /// </summary>
        public List<int> MerchantList { get; set; }
        public List<int> ProductList { get; set; }
        public bool IsAdmin { get; set; }

		private string m_OrderFiledString;
		public string OrderFiledString
		{
			get
			{
				if (string.IsNullOrEmpty(m_OrderFiledString))
				{
					if (OrderFileds.Count == 0)
					{
						m_OrderFiledString = string.Empty;
					}
					else
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("ORDER BY ");
						foreach (var item in OrderFileds)
						{
							if (item.FieldName.Contains("[CN]"))
							{
								sb.AppendFormat("nlssort({0},'NLS_SORT=SCHINESE_PINYIN_M') {1},", item.FieldName.Replace("[CN]", ""), item.OrderDirection);
							}
							else if (item.FieldName.Contains("[EN]"))
							{
								sb.AppendFormat("nlssort({0},'NLS_SORT=BINARY_CI') {1},", item.FieldName.Replace("[EN]", ""), item.OrderDirection);
							}
							else
							{
								sb.AppendFormat("{0} {1},", item.FieldName, item.OrderDirection);
							}

						}
						m_OrderFiledString = sb.ToString().TrimEnd(',') + " ";
					}

				}
				return m_OrderFiledString;

			}

		}

		public void AddOrder(string fieldName, OrderDirection orderDirection)
		{
			OrderFiledInfo orderField = new OrderFiledInfo();
			orderField.FieldName = fieldName;
			orderField.OrderDirection = orderDirection;
			AddToOrderField(orderField);
		}



		public void AddOrder(string orderFieldFormat)
		{
			string[] orderFields = orderFieldFormat.Split('&');
			OrderFiledInfo orderField = new OrderFiledInfo();
			orderField.FieldName = orderFields[0];
			orderField.OrderDirection = orderFields[1].ToUpper() == "ASC" ? OrderDirection.ASC : OrderDirection.DESC; ;
			AddToOrderField(orderField);
		}
		private void AddToOrderField(OrderFiledInfo orderField)
		{

			OrderFileds.RemoveAll(c => c.FieldName == orderField.FieldName);
			m_OrderFiledString = null;
			OrderFileds.Insert(0, orderField);

			//最多3个排序字段
			if (OrderFileds.Count > 3)
			{
				OrderFileds.RemoveRange(3, OrderFileds.Count - 3);
			}

		}

		
	}
	[Serializable]
	public class OrderFiledInfo
	{
		public string FieldName { get; set; }
		public OrderDirection OrderDirection { get; set; }
	}
	public enum OrderDirection
	{
		ASC,
		DESC
	}


	[Serializable]
	public class QueryResult
	{
		public int m_PageSize;
		public int PageSize
		{
			get { return m_PageSize; }
			set
			{
				m_PageSize = value;
				m_PageCount = null;

			}
		}
		public int CurrentPageIndex { get; set; }


		public int m_RecordCount;
		public int RecordCount
		{
			get { return m_RecordCount; }
			set
			{
				m_RecordCount = value;
				m_PageCount = null;

			}
		}


		private int? m_PageCount;
		public int PageCount
		{
			get
			{
				if (m_PageCount == null)
				{
					if (PageSize > 0)
					{
						int mod = RecordCount % PageSize;
						m_PageCount = RecordCount / PageSize + (mod == 0 ? 0 : 1);
					}
					else
					{
						m_PageCount = 0;
					}
					
				}
				return m_PageCount.Value;

			}
		}
	}

	[Serializable]
	public class QueryResultInfo<T> : QueryResult
		where T : class
	{
		public List<T> RecordList { get; set; }

	}
}