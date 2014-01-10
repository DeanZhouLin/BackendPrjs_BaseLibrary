//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Caching;
//using System.Threading;
//using Com.BaseLibrary.Data;
//using System.Data;
//using Com.BaseLibrary.Configuration;
//using Com.BaseLibrary.Utility;

//namespace Com.BaseLibrary.Caching
//{
//    /// <summary>
//    /// 基于SQL查询结果的依赖
//    /// </summary>
//    public class SqlCacheDependence : CacheDependency
//    {
//        public string ConnectionString { get; set; }
//        public string SQL { get; set; }
//        public CommandType CommandType { get; set; }
//        public int Interval { get; set; }

//        private const int DefaultTimerInterval = 5000;//default
//        private readonly Timer m_timer;
//        private readonly T m_readedContent;
//        private readonly DataTable m_readedTable;
//        private static readonly object m_SynObj = new object();

//        public SqlCacheDependence()
//        {
//            if (!this.HasChanged)
//            {
//                SetUtcLastModified(DateTime.MinValue);
//                NotifyDependencyChanged(this, EventArgs.Empty);
//            }


//        }
//        public SqlCacheDependence(string connectionString, string sql)
//            : this(connectionString, sql, DefaultTimerInterval, CommandType.Text)
//        {

//        }
//        public SqlCacheDependence(string connectionString, string sql, CommandType commandType)
//            : this(connectionString, sql, DefaultTimerInterval, commandType)
//        {

//        }
//        public SqlCacheDependence(string connectionName, string sql, int interval, CommandType commandType)
//            : this()
//        {
//            this.CommandType = commandType;
//            this.Interval = interval;
//            this.ConnectionString = ConfigurationHelper.GetConnectionString(connectionName);
//            this.SQL = sql;
//            if (typeof(T).Name == "Table")
//            {
//                m_readedTable = ReadTableFromDB();
//                m_timer = new Timer(CheckTableDependencyCallback, this, this.Interval, DefaultTimerInterval);
//            }
//            else
//            {
//                m_readedContent = ReadContentFromDB();
//                m_timer = new Timer(CheckContentDependencyCallback, this, this.Interval, DefaultTimerInterval);
//            }

//        }
//        private void CheckContentDependencyCallback(object sender)
//        {
//            lock (m_SynObj)
//            {
//                T content = ReadContentFromDB();
//                bool hasChanged = !content.Equals(m_readedContent);
//                if (hasChanged)
//                {
//                    SetUtcLastModified(DateTime.MinValue);
//                    SqlCacheDependence<T> cacheDependence = sender as SqlCacheDependence<T>;
//                    cacheDependence.NotifyDependencyChanged(sender, EventArgs.Empty);
//                    cacheDependence.m_timer.Dispose();
//                    cacheDependence.DependencyDispose();
//                    cacheDependence.Dispose();
//                }

//            }
//        }

//        private T ReadContentFromDB()
//        {
//            try
//            {
//                T t = (T)SqlHelper.ExecuteScalar(ConnectionString, CommandType, SQL);
//                return t;
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }


//        #region Table
//        private void CheckTableDependencyCallback(object sender)
//        {
//            lock (m_SynObj)
//            {
//                DataTable dt = ReadTableFromDB();
//                bool hasChanged = CheckTableContent(dt);
//                if (hasChanged)
//                {
//                    SqlCacheDependence<T> cacheDependence = sender as SqlCacheDependence<T>;
//                    cacheDependence.NotifyDependencyChanged(sender, EventArgs.Empty);
//                }
//            }
//        }

//        private bool CheckTableContent(DataTable dt)
//        {
//            bool hasChanged = dt.Rows.Count != m_readedTable.Rows.Count;
//            if (hasChanged)
//            {
//                return hasChanged;
//            }
//            int count = dt.Rows.Count;
//            for (int i = 0; i < count; i++)
//            {
//                hasChanged = dt.Rows[i][0].ToString() != m_readedTable.Rows[i][0].ToString();
//                if (hasChanged)
//                {
//                    return hasChanged;
//                }
//            }
//            return hasChanged;
//        }

//        private DataTable ReadTableFromDB()
//        {
//            try
//            {
//                DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, System.Data.CommandType.Text, SQL);
//                return ds.Tables[0];
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        #endregion

//        protected override void DependencyDispose()
//        {
//            if (m_timer != null)
//            {
//                m_timer.Dispose();
//            }
//            base.DependencyDispose();
//        }



//    }
//}
