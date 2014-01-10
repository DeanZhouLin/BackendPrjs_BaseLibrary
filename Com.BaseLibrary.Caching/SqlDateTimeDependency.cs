using System;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using System.Data;
using Com.BaseLibrary.Data;

namespace Com.BaseLibrary.Caching
{
    [Serializable]
    public class SqlDateTimeDependency : ICacheItemExpiration
    {
        // Fields
        private DateTime lastModifiedTime;

        public string ConnectionString { get; set; }
        public string SQL { get; set; }
        public CommandType CommandType { get; set; }
        public int Interval { get; set; }

        private const int DefaultTimerInterval = 5000;//default
        public SqlDateTimeDependency()
        {

        }
        public SqlDateTimeDependency(string connectionString, string sql)
            : this(connectionString, sql, DefaultTimerInterval, CommandType.Text)
        {

        }
        public SqlDateTimeDependency(string connectionString, string sql, CommandType commandType)
            : this(connectionString, sql, DefaultTimerInterval, commandType)
        {
            
        }
        public SqlDateTimeDependency(string connectionString, string sql, int interval, CommandType commandType)
            : this()
        {
            this.CommandType = commandType;
            this.Interval = interval;
            this.ConnectionString = connectionString;
            this.SQL = sql;
            lastModifiedTime = ReadContentFromDB();
        }
        private DateTime ReadContentFromDB()
        {
            try
            {
                DateTime t = (DateTime)SqlHelper.ExecuteScalar(ConnectionString, CommandType, SQL);
                return t;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool HasExpired()
        {
            DateTime lastEditDate = ReadContentFromDB();
            return (DateTime.Compare(this.lastModifiedTime, lastEditDate) != 0);
        }

        public void Initialize(CacheItem owningCacheItem)
        {
        }

        public void Notify()
        {
        }

        public DateTime LastModifiedTime
        {
            get
            {
                return this.lastModifiedTime;
            }
        }
    }


}
