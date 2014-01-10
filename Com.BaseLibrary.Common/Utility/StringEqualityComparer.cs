using System;
using System.Collections.Generic;

namespace Com.BaseLibrary.Utility
{
    /// <summary>
    /// �Ƚ������ַ����Ƿ����IEqualityComparerʵ��
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