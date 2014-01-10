using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


using Com.BaseLibrary.Entity;
using Com.BaseLibrary.Logging;


using Com.BaseLibrary.DataAccess.Configuration;

using Com.BaseLibrary.Transaction;
using Com.BaseLibrary.Utility;



namespace Com.BaseLibrary.DataAccess
{
	/// <summary>
	/// 
	/// </summary>
	public class DataCommand : IDisposable
	{
		private DbConnection connection;
		private int currentRetryCount = 0;
		#region transaction operation
		protected DbTransaction Transaction
		{
			get
			{
				if (DbTransactionScope.Enabled)
				{
					if (DbTransactionScope.Transaction == null)
					{
						PrepareTransaction();
					}
					return DbTransactionScope.Transaction;
				}
				return null;
			}
		}
		private void PrepareTransaction()
		{
			if (DbTransactionScope.Enabled && DbTransactionScope.Transaction == null)
			{
				connection = DbServer.CreateConnection();
				try
				{
					connection.Open();
				}
				catch
				{
					connection = null;
					return;
				}

				try
				{
					DbTransactionScope.Transaction = connection.BeginTransaction();
				}
				catch
				{
					connection.Close();
					connection = null;
					DbTransactionScope.Transaction = null;
				}
			}
		}
		#endregion
		protected DatabaseServer DbServer { get; private set; }
		protected Command CommandSetup { get; private set; }
		public DbCommand DbCommand { get; protected set; }

		//protected DbCommand command
		//{
		//    get
		//    {
		//        return command;
		//    }
		//}

		#region constructors



		private DataCommand()
		{
		}

		internal DataCommand(string database)
		{
			if (CommandSetup != null)
			{
				DbServer = DataAccessConfiguration.Current.DatabaseInstances[CommandSetup.Database];
			}
			else
			{
				DbServer = DataAccessConfiguration.Current.DatabaseInstances[database];
			}
		}
		internal DataCommand(Command commandSetup)
		{
			CommandSetup = commandSetup;
			DbServer = DataAccessConfiguration.Current.DatabaseInstances[CommandSetup.Database];
			DbCommand = DbServer.CreateCommand(CommandSetup);
			SetupConnecton();

		}

		private void SetupConnecton()
		{
			if (Transaction == null)
			{
				connection = DbServer.CreateConnection();
			}
			else
			{
				connection = Transaction.Connection;
				DbCommand.Transaction = Transaction;
				if (connection.Database != DbServer.DbName)
				{
					connection.ChangeDatabase(DbServer.DbName);
				}
			}
            DbCommand.Connection = connection;
		}
		#endregion

		#region parameters
		/// <summary>
		/// get a parameter value
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public Object GetParameterValue(string parameterName)
		{
			return DbCommand.Parameters[parameterName].Value;
		}

		/// <summary>
		/// set a parameter value 
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		public void SetParameterValue(string parameterName, Object value)
		{
			object val = value == null ? DBNull.Value : value;
			//DbCommand.Parameters[parameterName].Value = value ?? DBNull.Value;
			CommandParameter cp = this.CommandSetup.ParameterDictionary[parameterName];
			DbParameter parameter;
			if (cp.DbType == DbType.Currency)
			{
				parameter = new SqlParameter(cp.Name, val);
			}
			else
			{
				parameter = DbServer.CreateParameter(cp, val);
			}

			DbCommand.Parameters.Add(parameter);
		}

		/// <summary>
		/// set a parameter value 
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		public void SetParameterValue<T>(string parameterName, Object value)
		{
			Type enumType = typeof(T);
			if (enumType.IsEnum)
			{
				string strValue = EnumUtil.GetValueByEnum(enumType, value);
				SetParameterValue(parameterName, strValue);
			}
			else
			{
				SetParameterValue(parameterName, value);
			}
		}

		#endregion


		#region db retry
		/// <summary>
		/// Moves to next db.
		/// returns false if no more db exists.
		/// </summary>
		/// <returns></returns>
		private bool RecordException(Exception ex)
		{
			//if ex == null, initialize retry count
			if (ex == null)
			{
				currentRetryCount = 0;
			}
			else
			{
				Logger.CurrentLogger.DoWrite(
					"CommonLibrary",
					"DataAccess",
					"ExecuteDataCommandFailed",
					this.CommandSetup.Name,
					this.DbCommand.CommandText + ex.ToString());
				currentRetryCount++;
				if (currentRetryCount < 3)
				{
					return true;
				}
			}
			return false;

		}
		#endregion

		#region execution
		public T ExecuteScalar<T>()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteScalar<T>();
				}
				catch (Exception iex)
				{
					ex = iex;
				}
			}
			while (RecordException(ex));
			throw ex;

		}
		public Object ExecuteScalar()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteScalar();
				}
				catch (Exception iex)
				{
					ex = iex;
				}
			} while (RecordException(ex));


			throw ex;

		}
		public int ExecuteNonQuery()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteNonQuery();
				}
				catch (Exception iex)
				{
					ex = iex;
				}
			} while (RecordException(ex));
			throw ex;

		}
		public T ExecuteEntity<T>()
			where T : class, new()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteEntity<T>();
				}
				catch (Exception iex)
				{
					ex = iex;
				}

			} while (RecordException(ex));

			throw ex;

		}

		public List<T> ExecuteEntityList<T>() where T : class, new()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteEntityList<T>();
				}
				catch (Exception iex)
				{
					ex = iex;
				}
			} while (RecordException(ex));


			throw ex;

		}
		public IDataReader ExecuteDataReader()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteDataReader();
				}
				catch (Exception iex)
				{
					ex = iex;
				}
			}
			while (RecordException(ex));

			throw ex;

		}
		public DataSet ExecuteDataSet()
		{
			Exception ex = null;
			do
			{
				try
				{
					return DoExecuteDataSet();
				}
				catch (Exception iex)
				{
					ex = iex;
				}
			} while (RecordException(ex));


			throw ex;
		}

		private T DoExecuteScalar<T>()
		{
			return (T)DoExecuteScalar();
		}

		private object DoExecuteScalar()
		{
			MakeSureConnOpen();
			return DbCommand.ExecuteScalar();
		}
		private int DoExecuteNonQuery()
		{
			MakeSureConnOpen();
			return DbCommand.ExecuteNonQuery();
		}




		private T DoExecuteEntity<T>() where T : class, new()
		{

			using (IDataReader reader = DoExecuteDataReader())
			{
				if (reader.Read())
				{
					return EntityBuilder.BuildEntity<T>(reader);
				}
				else
				{
					return null;
				}
			}

		}
		private IDataReader DoExecuteDataReader()
		{
			MakeSureConnOpen();
			return DbCommand.ExecuteReader();
		}
		private List<T> DoExecuteEntityList<T>() where T : class, new()
		{
			using (IDataReader reader = DoExecuteDataReader())
			{
				List<T> list = new List<T>();
				while (reader.Read())
				{
					T entity = EntityBuilder.BuildEntity<T>(reader);
					list.Add(entity);
				}
				return list;
			}
		}


		private DataSet DoExecuteDataSet()
		{
			try
			{
				DbDataAdapter da = DbServer.CreateDataAdapter();
				da.SelectCommand = DbCommand;
				DataSet ds = new DataSet();
				da.Fill(ds);
				return ds;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// 确保连接打开。
		/// </summary>
		private void MakeSureConnOpen()
		{
			if (DbCommand.Connection.State == ConnectionState.Closed)
			{
				DbCommand.Connection.Open();
			}
		}

		#endregion

		#region IDispose
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			//如果不存在连接，或者命令处于事物中，不需要关闭连接。
			if (DbCommand.Connection == null || DbTransactionScope.Enabled)
			{
				return;
			}

			if (DbCommand.Connection.State == ConnectionState.Open)
			{
				DbCommand.Connection.Dispose();
			}
		}
		#endregion



		public List<T> BuildEntityList<T>(DataTable table) where T : class, new()
		{
			List<T> list = new List<T>();
			foreach (DataRow row in table.Rows)
			{
				T entity = BuildEntity<T>(row);
				list.Add(entity);
			}
			return list;
		}

		public T BuildEntity<T>(DataRow row) where T : class, new()
		{
			return EntityBuilder.BuildEntity<T>(row);

		}
	}
}
