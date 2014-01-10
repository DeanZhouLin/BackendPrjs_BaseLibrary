using System.Reflection;
using System.Configuration;
using System;

namespace Com.BaseLibrary.Logging
{
    /// <summary>
    /// 日志管理器的标准接口
    /// </summary>
    public abstract class Logger
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="applicationName">应用程序名称</param>
        /// <param name="module">模块</param>
        /// <param name="logType">日志类型</param>
        /// <param name="title">日志标题</param>
        /// <param name="detail">日志详细信息</param>
        public abstract void DoWrite(string applicationName, string module, string logType, string title, string detail);


       
        public abstract void WriteLogToXmlFile(string applicationName, string module, string logType, string title, string detail);
        public abstract void WriteLogLine(string message);

        private const string LOGGER_TYPE_NAME = "ImplLogger";
        private static Logger m_SingleLogger = null;
        private static readonly object m_SynObj = new object();

        /// <summary>
        /// 获取当前日志管理器，
        /// 首先根据App.config文件中指定的实现类实例化，
        /// 如果没有配置或实例化失败，则创建CustomLogger，详情请见CustomLogger类
        /// </summary>
        public static Logger CurrentLogger
        {
            get
            {
                if (m_SingleLogger == null)
                {
                    lock (m_SynObj)
                    {
                        if (m_SingleLogger == null)
                        {
                            try
                            {
                                string[] loggerTypeName = ConfigurationManager.AppSettings[LOGGER_TYPE_NAME].Split(',');
                                string fullLoggerClassName = loggerTypeName[0];
                                string loggerAssembleName = loggerTypeName[1];
                                m_SingleLogger = (Logger)Assembly.Load(loggerAssembleName).CreateInstance(fullLoggerClassName);
                            }
                            catch
                            {
                                m_SingleLogger = new CustomLogger();
                            }
                        }
                    }
                }
                return m_SingleLogger;
            }
        }
    }
}