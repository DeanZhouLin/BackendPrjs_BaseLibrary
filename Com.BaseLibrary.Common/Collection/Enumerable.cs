using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.BaseLibrary.Utility;

using System.Linq.Expressions;
using System.Web.UI.WebControls;

namespace System.Linq
{
	public static class Enumerable
	{
		//public static List<string> ConnectToStringList<TSource>(this IEnumerable<TSource> source, Func<TSource, List<string>> selector)
		//{
		//    List<string> list = new List<string>();
		//    var v = source.Select<TSource, List<string>>(selector);
		//    foreach (var item in v)
		//    {
		//        foreach (var item1 in item)
		//        {
		//            if (!list.Contains(item1))
		//            {
		//                list.Add(item1);
		//            }
		//        }

		//    }
		//    return list;
		//}

		public static string ConnectToStringList<TSource>(this IEnumerable<TSource> source, Func<TSource, List<string>> selector, char delimeter)
		{
			List<string> list = new List<string>();
			var v = source.Select<TSource, List<string>>(selector);
			foreach (var item in v)
			{
				foreach (var item1 in item)
				{
					if (!list.Contains(item1))
					{
						list.Add(item1);
					}
				}

			}

			return ListUtil.ConnectToString(list, delimeter);
		}

		public static IOrderedQueryable<TSource> SetSortOrder<TSource, TKey>(
			this IQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector,

			SortDirection sortDirection)
		{
			if (sortDirection == SortDirection.Ascending)
			{
				return source.OrderBy(keySelector);
			}
			else
			{
				return source.OrderByDescending(keySelector);
			}
		}

		
	}
}
