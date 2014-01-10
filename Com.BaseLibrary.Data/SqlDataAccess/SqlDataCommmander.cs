using System;

using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using Com.BaseLibrary.Entity;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Transaction;

namespace Com.BaseLibrary.DataAccess
{
	public class SqlDataCommmander : IDisposable
	{
		public SqlConnection Connection { get; set; }
		public SqlCommand command { get; set; }

		public SqlDataCommmander(string connnetionName, string cmdText)
		{

			command = new SqlCommand(cmdText);
			string connectionString = ConfigurationHelper.GetConnectionString(connnetionName);
			if (DbTransactionScope.Enabled)
			{

				if (DbTransactionScope.Transaction == null)
				{
					Connection = new SqlConnection(connectionString);
					Connection.Open();
					DbTransactionScope.Transaction = Connection.BeginTransaction();
				}
				Connection = DbTransactionScope.Transaction.Connection as SqlConnection;
			}
			else
			{
				Connection = new SqlConnection(connectionString);

			}
			command.Connection = Connection;
			command.Transaction = DbTransactionScope.Transaction as SqlTransaction;
		}
		public bool IsStoreProcedure
		{
			get
			{
				return command.CommandType == CommandType.StoredProcedure;
			}
			set
			{
				if (value)
				{
					command.CommandType = CommandType.StoredProcedure;
				}
				else
				{
					command.CommandType = CommandType.Text;
				}
			}
		}

		public int ExecuteNonQuery()
		{
			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
			return command.ExecuteNonQuery();
		}

		public DataTable ExecuteDataTable()
		{
			SqlDataAdapter da = new SqlDataAdapter(command);
			DataSet ds = new DataSet("ds");
			ds.Tables.Add(new DataTable("dt"));
			da.Fill(ds, "dt");
			return ds.Tables[0];

		}

		public T ExecuteEntity<T>()
			 where T : class, new()
		{
			DataTable dt = ExecuteDataTable();
			if (dt.Rows.Count > 0)
			{
				return EntityBuilder.BuildEntity<T>(dt.Rows[0]);
			}
			return default(T);
		}

		public List<T> ExecuteEntityList<T>()
					 where T : class, new()
		{
			DataTable dt = ExecuteDataTable();
			return EntityBuilder.BuildEntityList<T>(dt);
		}

		public void SetParameterValue(string parameterName, object value)
		{
			SetParameterValue(parameterName, value, ParameterDirection.Input);
		}

		public void SetParameterValue(string parameterName, object value, ParameterDirection parameterDirection)
		{
			if (value == null)
			{
				value = DBNull.Value;
			}

			if (!parameterName.StartsWith("@"))
			{
				parameterName = "@" + parameterName;
			}
			if (command.Parameters.Contains(parameterName))
			{
				command.Parameters[parameterName].Value = value;
			}
			else
			{

				SqlParameter parameter = new SqlParameter(parameterName, value);
				parameter.Direction = parameterDirection;
				command.Parameters.Add(parameter);
			}
		}
		public void SetParameterValue(string parameterName, object value, ParameterDirection parameterDirection, int size)
		{
			if (value == null)
			{
				value = DBNull.Value;
			}

			if (!parameterName.StartsWith("@"))
			{
				parameterName = "@" + parameterName;
			}
			if (command.Parameters.Contains(parameterName))
			{
				command.Parameters[parameterName].Value = value;
			}
			else
			{

				SqlParameter parameter = new SqlParameter(parameterName, value);
				parameter.Direction = parameterDirection;
				parameter.Size = size;

				command.Parameters.Add(parameter);
			}
		}

		public object GetParamterValue(string parameterName)
		{
			if (!parameterName.StartsWith("@"))
			{
				parameterName = "@" + parameterName;
			}
			if (command.Parameters.Contains(parameterName))
			{
				return command.Parameters[parameterName].Value;
			}
			return null;
		}

		

		public object ExecuteScalar()
		{
			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
			return command.ExecuteScalar();
		}
		public void Dispose()
		{
			command.Dispose();
			Connection.Dispose();
		}
	}

	public class RemoteServerNotFoundException : Exception
	{
		public string ServerIP { get; set; }
		public RemoteServerNotFoundException(string ip)
		{
			ServerIP = ip;
		}
	}
}
