using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Com.BaseLibrary.Collection;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace Com.BaseLibrary.DataAccess.Configuration
{
	[XmlRoot("database")]
	public class DatabaseServer : IKeyedItem<string>
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlIgnore]
		public string Key
		{
			get { return this.Name; }
		}

		[XmlAttribute("DbName")]
		public string DbName { get; set; }

		[XmlAttribute("Provider")]
		public DataProvider DataProvider { get; set; }

		[XmlAttribute]
		public string ConnectionString { get; set; }


		internal DbConnection CreateConnection()
		{
			return new SqlConnection(this.ConnectionString);
		}

		internal DbDataAdapter CreateDataAdapter()
		{
			return new SqlDataAdapter();
		}

		internal DbCommand CreateCommand(Command commandSetup)
		{
			SqlCommand sqlCommand = new SqlCommand(commandSetup.CommandText);
			sqlCommand.CommandTimeout = commandSetup.TimeOut;
			sqlCommand.CommandType = commandSetup.CommandType;
			return sqlCommand;
		}
		internal DbCommand CreateCommand(string commandText)
		{
			SqlCommand sqlCommand = new SqlCommand(commandText);
			return sqlCommand;
		}

		internal DbParameter CreateParameter(
			string name,
			DbType dbType,
			ParameterDirection direction,
			int size,
			object value)
		{
			DbParameter param = param = new SqlParameter();
			param.ParameterName = GetSqlParameterName(name);
			param.DbType = dbType;
			param.Direction = direction;
			if (size > 0)
			{
				param.Size = size;
			}
			if (direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput)
			{
				param.Value = value;
			}
			return param;
		}

		internal DbParameter CreateDbParameter(CommandParameter commandParameter)
		{

			return CreateParameter(commandParameter, DBNull.Value);
		}

		internal DbParameter CreateParameter(CommandParameter commandParameter, object val)
		{
			return CreateParameter(commandParameter.Name,
				commandParameter.DbType,
				commandParameter.Direction,
				commandParameter.Size, val);
		}

		public string GetSqlParameterName(string name)
		{
			if (name.StartsWith("@"))
			{
				return name;
			}
			return "@" + name.Trim();
		}
	}
	public enum DataProvider
	{
		SqlServer,
		Oracle,
		MySql,
	}
}
