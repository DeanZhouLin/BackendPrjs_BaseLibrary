using System.Data.Linq;
using System.Data.Linq.Mapping;
using Com.BaseLibrary.Transaction;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Resources;

namespace Com.BaseLibrary.Service
{

	public class DataContextBase : DataContext, IDisposable
	{
		private static MappingSource mappingSource = new AttributeMappingSource();

		public DataContextBase(string connectionStringName)
			: this(connectionStringName, false)
		{
			//if (DbTransactionScope.Enabled
			//    && DbTransactionScope.Transaction != null)
			//{
			//    this.Transaction = DbTransactionScope.Transaction;
			//}

		}

		public DataContextBase(string connectionStringName, bool readWithNolock)
			: base(getConnnection(connectionStringName), mappingSource)
		{
			switchToCurrentDatabase();
			this.ReadWithNoLock = readWithNolock;

			if (DbTransactionScope.Enabled && DbTransactionScope.Transaction != null)
			{
				this.Transaction = DbTransactionScope.Transaction;
			}
			else if (ReadWithNoLock)
			{
				this.Connection.StateChange += new StateChangeEventHandler(OnConnectionStateChange);
			}
		}

		#region only runing for unlock query
		protected bool IsDisposed { get; private set; }
		/// <summary>
		/// Handles the Begin / End Transaction Logic
		/// 
		/// This should make the Context less eager to open and ONLY
		/// start a trans when needed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConnectionStateChange(object sender, StateChangeEventArgs e)
		{
			if (!ReadWithNoLock)
			{
				return;
			}

			if (e.CurrentState == ConnectionState.Open && e.OriginalState == ConnectionState.Closed)
			{
				BeginTransaction();

			}
			if (e.CurrentState == ConnectionState.Closed && e.OriginalState == ConnectionState.Open)
			{
				CommitTransaction();
			}
		}
		/// <summary>
		/// Closes the internal context connection
		/// </summary>
		private void CloseConnection()
		{
			if (!IsDisposed && this.Connection.State == ConnectionState.Open)
			{
				this.Connection.Close();
			}
		}
		/// <summary>
		/// Open the internal context connection
		/// </summary>
		private void OpenConnection()
		{
			if (!IsDisposed && this.Connection.State == ConnectionState.Closed)
			{
				this.Connection.Open();
			}

		}
		/// <summary>
		/// Starts a new transaction for the current DataContext.
		/// Ensures that the connection is open prior to creating an open
		/// connection.
		/// </summary>
		/// <param name="isolationLevel"><see cref="IsolationLevel"/></param>
		private void BeginTransaction()
		{
			OpenConnection();
			if (this.Transaction == null)
			{
				this.Transaction = this.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
			}
		}
		/// <summary>
		/// Commits the transaction
		/// </summary>
		/// <param name="trans"></param>
		private void CommitTransaction()
		{
			if (!IsDisposed && this.Transaction != null && this.Transaction.Connection != null)
			{
				try
				{
					this.Transaction.Commit();
				}
				catch
				{
					this.Transaction.Rollback();
					throw;
				}
				finally
				{
					this.Transaction.Dispose();
					this.Transaction = null;
				}
			}
		}


		private void EnsureTransaction()
		{
			if (this.Transaction == null)
			{
				BeginTransaction();
			}
		}

		#endregion

		public override void SubmitChanges(ConflictMode failureMode)
		{
			//switchToCurrentDatabase();
			base.SubmitChanges(failureMode);
		}

		/// <summary>
		/// 获取连接对象
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		private static DbConnection getConnnection(string connectionStringName)
		{
			DbConnection connection = null;
			if (DbTransactionScope.Transaction != null)
			{
				connection = DbTransactionScope.Transaction.Connection;
			}
			else
			{
				string connectionString = DBConnectionResource.GetConnectionString(connectionStringName);
				connection = new SqlConnection(connectionString);
				if (DbTransactionScope.Enabled)
				{
					connection.Open();
					DbTransactionScope.Transaction = connection.BeginTransaction();
				}
			}
			return connection;
		}

		/// <summary>
		/// 更改当前数据库为DataContext指定的数据。
		/// </summary>
		private void switchToCurrentDatabase()
		{
			if (this.Mapping != null && !string.IsNullOrEmpty(this.Mapping.DatabaseName))
			{
				if (this.Connection.Database.ToUpper() != this.Mapping.DatabaseName.ToUpper())
				{
					//检查连接是否已经打开？如没有打开，则一定要先打开
					if (this.Connection.State != System.Data.ConnectionState.Open)
					{
						this.Connection.Open();
					}
					this.Connection.ChangeDatabase(this.Mapping.DatabaseName);
				}
			}
		}

		private bool m_ReadWithNoLock;
		public bool ReadWithNoLock
		{
			get { return m_ReadWithNoLock; }
			set
			{
				if (value != m_ReadWithNoLock)
				{
					m_ReadWithNoLock = value;
					if (value)
					{
						this.Connection.StateChange += new StateChangeEventHandler(OnConnectionStateChange);
					}

				}
			}
		}




		#region IDisposable 成员

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing"></param>
		public new void Dispose(bool disposing)
		{
			if (!IsDisposed && disposing)
			{
				if (ReadWithNoLock && !DbTransactionScope.Enabled)
				{
					CommitTransaction();
					CloseConnection();

				}
				base.Dispose();
				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);

			// Suppress finalization of this disposed instance.
			GC.SuppressFinalize(this);
		}

		~DataContextBase()
		{
			Dispose(false);
		}


		#endregion
	}
}
