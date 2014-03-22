using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace Com.BaseLibrary.Entity
{
	internal class DataRowEntitySource : IEntityDataSource
	{
		private class RowColumnNameEnumerator : IEnumerator<string>
		{
			private IEnumerator m_InternalEnumeator;

			public RowColumnNameEnumerator(DataRow dr)
			{
				m_InternalEnumeator = dr.Table.Columns.GetEnumerator();
			}

			public string Current
			{
				get 
				{
					DataColumn column = m_InternalEnumeator.Current as DataColumn;
					return column.ColumnName;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				return m_InternalEnumeator.MoveNext();
			}

			public void Reset()
			{
				m_InternalEnumeator.Reset();
			}

			public void Dispose()
			{
				return;
			}
		}


		private DataRow m_DataRow;

		#region constructors
		public DataRowEntitySource(DataRow dr)
		{
			m_DataRow = dr;
		}
		#endregion

		public object this[string columnName]
		{
			get { return m_DataRow[columnName]; }
		}

		public object this[int index]
		{
			get { return m_DataRow[index];}
		}

		public IEnumerator<string> GetEnumerator()
		{
			return new RowColumnNameEnumerator(m_DataRow);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<string>)this).GetEnumerator();
		}

		public bool ContainsColumn(string columnName)
		{
			return m_DataRow.Table.Columns.Contains(columnName);
		}

		public void Dispose()
		{
			// datarow does not need to close.
			return;
		}
	}
}
