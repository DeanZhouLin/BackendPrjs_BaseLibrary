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
    public class CustomSqlCacheDependence : CacheDependency
    {
        public string ConnectionString { get; set; }
        public string SQL { get; set; }
        public CommandType CommandType { get; set; }
        public int Interval { get; set; }

        private const int DefaultTimerInterval = 5000;//default
        private Timer m_timer;
        private static readonly object m_SynObj = new object();

        public CustomSqlCacheDependence()
        {

        }
        public CustomSqlCacheDependence(string connectionString, string sql)
            : this(connectionString, sql, DefaultTimerInterval, CommandType.Text)
        {

        }
        public CustomSqlCacheDependence(string connectionString, string sql, CommandType commandType)
            : this(connectionString, sql, DefaultTimerInterval, commandType)
        {

        }
        public CustomSqlCacheDependence(string connectionString, string sql, int interval, CommandType commandType)
            : this()
        {
            this.CommandType = commandType;
            this.Interval = interval;
            this.ConnectionString = connectionString;
            this.SQL = sql;
            DateTime m_readedContent = ReadContentFromDB();
            SetUtcLastModified(m_readedContent);
           // m_timer = new Timer(CheckContentDependencyCallback, this, this.Interval, DefaultTimerInterval);
        }
        internal void Init()
        {
            m_timer = new Timer(CheckContentDependencyCallback, this, this.Interval, DefaultTimerInterval);
        }
        private void CheckContentDependencyCallback(object sender)
        {
            lock (m_SynObj)
            {
                DateTime content = ReadContentFromDB();
                bool hasChanged = !this.UtcLastModified.Equals(content);
                if (hasChanged)
                {
                    NotifyDependencyChanged(sender, EventArgs.Empty);
                }
            }
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
        
      
        protected override void DependencyDispose()
        {
            if (m_timer != null)
            {
                m_timer.Dispose();
            }
            base.DependencyDispose();
        }



    }
}
