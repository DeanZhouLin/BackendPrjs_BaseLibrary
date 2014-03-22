using System;
using System.Collections.Generic;

using System.Text;
using System.Data.Common;

namespace Com.BaseLibrary.Transaction
{
	/// <summary>
	/// 
	/// </summary>
	public class DbTransactionScope : IDisposable
	{
		[ThreadStatic]
		public static DbTransaction Transaction;

		[ThreadStatic]
		public static bool Enabled = false;

		~DbTransactionScope()
		{
			Dispose(true);
		}

		public DbTransactionScope()
		{
			Enabled = true;
		}

		public void Dispose()
		{
			Enabled = false;
			Dispose(true);
		}

		public void Complete()
		{
			Enabled = false;

			if (Transaction != null)
			{
				try
				{
					Transaction.Commit();
					Transaction.Dispose();
					Transaction = null;
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					Transaction = null;
				}
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Transaction != null)
				{
					try
					{
						Transaction.Rollback();
						Transaction.Dispose();
					}
					catch (Exception)
					{
						throw;
					}
					finally
					{
						Transaction = null;
					}
				}
			}
		}

	}
}
