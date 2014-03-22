using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.PrintCenter
{
	[Serializable]
	public class PrintDataInfo
	{
		public PrintDataInfo()
		{ }

		public string Template { get; set; }
		public String JobID { get; set; }
		public string ByDatabase { get; set; }
		public string Printer { get; set; }
		public int PrintCount { get; set; }
		public List<DataItemInfo> DataList { get; set; }

		public void Add(string name, object value)
		{
			DataItemInfo dataItemInfo = new DataItemInfo(name, value);
			if (DataList == null)
			{
				DataList = new List<DataItemInfo>();
			}
			else
			{
				DataItemInfo existItem = DataList.Find(c => c.Name == dataItemInfo.Name);
				if (existItem != null)
				{
					existItem.Value = dataItemInfo.Value;
				}
				else
				{
					DataList.Add(dataItemInfo);
				}
			}

		}
	}
	[Serializable]
	public class DataItemInfo
	{
		public DataItemInfo()
		{ }
		public DataItemInfo(string name, object value)
		{
			this.Name = name;

			if (StringUtil.IsNullOrEmpty(value))
			{
				this.Value = "　";
			}
			else if (value is DateTime)
			{
				this.Value = DateTime.Parse(value.ToString()).ToString("yyyy-MM-dd");
			}
			else if (value is decimal)
			{
				this.Value = StringUtil.TrimNumberExtraZero(value);
			}
			else
			{
				this.Value = value.ToString();
			}

		}
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
