using System;
using System.Collections.Generic;

namespace Com.BaseLibrary.Session
{
	[Serializable]
	public class SessionData
	{
		private Dictionary<string, string> DataList { get; set; }
		public SessionData()
		{
			DataList = new Dictionary<string, string>();
			IsNew = true;
		}

		public bool IsNew { get; set; }
		public bool HasChanged { get; set; }
		public string this[string key]
		{
			get
			{
				string defaultValue = null;
				DataList.TryGetValue(key, out defaultValue);
				return defaultValue;
			}
			set
			{
				if (value == null)
				{
					if (DataList.ContainsKey(key))
					{
						HasChanged = true;
						DataList.Remove(key);
					}
				}
				else
				{
					if (DataList.ContainsKey(key))
					{
						DataList[key] = value;
					}
					else
					{
						HasChanged = true;
						DataList.Add(key, value);
					}
				}
				
			}
		}
		public void Remove(string key)
		{
			if (DataList.ContainsKey(key))
			{
				HasChanged = true;
				DataList.Remove(key);
			}
		}
		public void Clear()
		{
			if (DataList.Count > 0)
			{
				HasChanged = true;
				DataList.Clear();
			}

		}
		public int Count { get { return DataList.Count; } }
	}
}
