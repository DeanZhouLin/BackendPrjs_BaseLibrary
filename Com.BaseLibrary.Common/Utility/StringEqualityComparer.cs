using System;
using System.Collections.Generic;

namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// 比较两个字符串是否相等IEqualityComparer实现
    /// </summary>
	public class StringEqualityComparer : IEqualityComparer<string>
	{

		public bool Equals(string x, string y)
		{
			return (string.Compare(x, y, true) == 0);
		}

		public int GetHashCode(string obj)
		{
			return obj.ToUpper().GetHashCode();
		}	
	}
}