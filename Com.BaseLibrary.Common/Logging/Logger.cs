using System.Reflection;
using System.Configuration;

namespace Com.BaseLibrary.Logging
{
    /// <summary>
    /// ��־�������ı�׼�ӿ�
    /// </summary>
    public abstract class Logger
    {
        /// <summary>
        /// ��¼��־
        /// </summary>
        /// <param name="applicationName">Ӧ�ó�������</param>
        /// <param name="module">ģ��</param>
        /// <param name="logType">��־����</param>
        /// <param name="title">��־����</param>
        /// <param name="detail">��־��ϸ��Ϣ</param>
        public abstract void DoWrite(string applicationName, string module, string logType, string title, string detail);


       
        public abstract void WriteLogToXmlFile(string applicationName, string module, string logType, string title, string detail);
        public abstract void WriteLogLine(string message);

        private const string LOGGER_TYPE_NAME = "ImplLogger";
        private static Logger m_SingleLogger;
        private static readonly object m_SynObj = new object();

        /// <summary>
        /// ��ȡ��ǰ��־��������
        /// ���ȸ���App.config�ļ���ָ����ʵ����ʵ������
        /// ���û�����û�ʵ����ʧ�ܣ��򴴽�CustomLogger���������CustomLogger��
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