using System;
using System.Collections.Generic;
using System.Text;
using Com.BaseLibrary.Collection;
using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Configuration;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using Com.BaseLibrary.Data;

namespace Com.BaseLibrary.Resources
{

	public class DBConnectionResource
	{

		static DBConnectionResource()
		{
			InitResourceData();
		}

		private static void InitResourceData()
		{
			instance = new DBConnectionResource();
			instance.DataConnectionInfoList = new KeyedList<string, DataConnectionInfo>();
			LoadConnectionFromDB();
		}

		private static void LoadConnectionFromDB()
		{
//            try
//            {
//                string sql = @"SELECT * FROM DataConnectionInfo A
//WHERE EXISTS(SELECT 1 FROM dbo.AppConnection B WHERE A.ConnectionName=B.ConnectionName AND B.ApplicationID={0})";
//                sql = string.Format(sql, ConfigurationHelper.GetAppSetting("ApplicationID"));
//                DataTable dt = DBHelper.GetTable(sql);
//                foreach (DataRow row in dt.Rows)
//                {
//                    DataConnectionInfo connecton = new DataConnectionInfo();
//                    connecton.ConnectionName = row["ConnectionName"].ToString();
//                    connecton.DatabaseName = row["DatabaseName"].ToString();
//                    connecton.DbServer = row["DbServer"].ToString();
//                    connecton.Timeout = Converter.ToInt32(row["Timeout"], 25);

//                    connecton.UsePooling = Converter.ToInt32(row["UsePooling"], 1);
//                    connecton.PoolSize = Converter.ToInt32(row["PoolSize"], 50);

//                    connecton.UserID = row["UserID"].ToString();
//                    connecton.Password = row["Password"].ToString();

//                    instance.DataConnectionInfoList.Add(connecton);
//                }
//            }
//            catch (Exception ex)
//            {
//                //todo...
//            }

		}

		public class DataConnectionInfo : IKeyedItem<string>
		{

			private String _Key;
			private String _DbServer;
			private String _DatabaseName;
			private String _UserID;
			private String _Password;
			private Int32 _Timeout;
			private Int32 _UsePooling;
			private Int32? _PoolSize;



			public String ConnectionName
			{
				get { return _Key; }
				set
				{
					if (_Key != value)
					{
						_Key = value;
					}
				}
			}


			public String DbServer
			{
				get { return _DbServer; }
				set
				{
					if (_DbServer != value)
					{
						_DbServer = value;
					}
				}
			}


			public String DatabaseName
			{
				get { return _DatabaseName; }
				set
				{
					if (_DatabaseName != value)
					{
						_DatabaseName = value;
					}
				}
			}


			public String UserID
			{
				get { return _UserID; }
				set
				{
					if (_UserID != value)
					{
						_UserID = value;
					}
				}
			}


			public String Password
			{
				get { return _Password; }
				set
				{
					if (_Password != value)
					{
						_Password = value;
					}
				}
			}


			public Int32 Timeout
			{
				get { return _Timeout; }
				set
				{
					if (_Timeout != value)
					{
						_Timeout = value;
					}
				}
			}


			public Int32 UsePooling
			{
				get { return _UsePooling; }
				set
				{
					if (_UsePooling != value)
					{
						_UsePooling = value;
					}
				}
			}


			public Int32? PoolSize
			{
				get { return _PoolSize; }
				set
				{
					if (_PoolSize != value)
					{
						_PoolSize = value;
					}
				}
			}

			private string m_ConnectionString;

			public string ConnectionString
			{
				get
				{
					if (m_ConnectionString == null)
					{
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat("server={0};database={1};Persist Security Info=True;User ID={2};Password={3}",
							DbServer,
							DatabaseName,
							UserID,
							Password);
						m_ConnectionString = sb.ToString();
					}
					return m_ConnectionString;
				}
				internal set { m_ConnectionString = value; }
			}


			#region IKeyedItem<string> ≥…‘±

			public string Key
			{
				get { return ConnectionName; }
			}

			#endregion
		}

		public KeyedList<string, DataConnectionInfo> DataConnectionInfoList { get; set; }

		private static DBConnectionResource instance;
		public static DBConnectionResource Current
		{
			get
			{
				return instance;
			}
		}

		public static string GetConnectionString(string name)
		{
			DataConnectionInfo connection = Current.DataConnectionInfoList.GetItemByKey(name);
			if (connection == null)
			{
				string connectionString = ConfigurationHelper.GetConnectionString(name);
				if (connectionString == null)
				{
					throw new Exception(string.Format("Cannot find the DbConnection :{0}", name));
				}
				connection = new DataConnectionInfo();
				connection.ConnectionName = name;
				connection.ConnectionString = connectionString;
				Current.DataConnectionInfoList.Add(connection);
			}
			return connection.ConnectionString;


		}
	}
}
