using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Threading;
using Com.BaseLibrary.Data;
using System.Data;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Caching
{
    /// <summary>
    /// 基于SQL查询结果的依赖
    /// </summary>
    public class SqlServerUtcDateTime
    {
        public string ConnectionString { get; set; }
        public string SQL { get; set; }
        public CommandType CommandType { get; set; }
        public int Interval { get; set; }
        private const int DefaultTimerInterval = 5000;//default
        public SqlServerUtcDateTime()
        {

        }
        public SqlServerUtcDateTime(string connectionString, string sql)
            : this(connectionString, sql, DefaultTimerInterval, CommandType.Text)
        {

        }
        public SqlServerUtcDateTime(string connectionString, string sql, CommandType commandType)
            : this(connectionString, sql, DefaultTimerInterval, commandType)
        {

        }
        public SqlServerUtcDateTime(string connectionName, string sql, int interval, CommandType commandType)
            : this()
        {
            this.CommandType = commandType;
            this.Interval = interval;
            this.ConnectionString = ConfigurationHelper.GetConnectionString(connectionName);
            this.SQL = sql;
        }
    }
}
