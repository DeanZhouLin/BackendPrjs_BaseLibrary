using System;
using System.Collections.Generic;


namespace Com.BaseLibrary.Entity
{
	internal interface IEntityDataSource : IEnumerable<string>, IDisposable
	{
		Object this[string columnName]
		{
			get;
		}

		Object this[int iIndex]
		{
			get;
		}
		
		bool ContainsColumn(string columnName);

	}
}
