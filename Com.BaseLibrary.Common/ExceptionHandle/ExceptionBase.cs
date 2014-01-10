using System;
using Com.BaseLibrary.Logging;
using Com.BaseLibrary.Utility;
using System.Data.SqlClient;

namespace Com.BaseLibrary.ExceptionHandle
{
    /// <summary>
    /// �Զ����쳣�Ļ���
    /// </summary>
    public class BizException : Exception
    {
        /// <summary>
        /// �Ƿ�Ϊҵ���쳣
        /// </summary>
        public bool IsBizException { get; set; }

        /// <summary>
        /// �쳣��Ϣ�ļ�
        /// ͨ�����������ļ��ж�ȡ���쳣��������Ϣ
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public override string Message
        {
            get
            {
                return this.ErrorMessage;
            }
        }

        /// <summary>
        /// ʵ����һ������ͨ���쳣����name�������ļ��ж����쳣��Ϣ��
        /// ���ô���Ĳ������и�ʽ�����õ����յ�ErrorMessage
        /// </summary>
        /// <param name="name">�쳣����</param>
        /// <param name="parameters">����Ϣ���и�ʽ���Ĳ���</param>
        public BizException(object name, params object[] parameters)
            : base(name as string)
        {

            if (ExceptionConfiguration.Current != null
                && ExceptionConfiguration.Current.ExceptionSetupList.Contains(name.ToString()))
            {
                ExceptionSetup error = ExceptionConfiguration.Current.ExceptionSetupList[name.ToString()].Clone() as ExceptionSetup;
                this.Name = name.ToString();
                if (parameters != null && parameters.Length > 0)
                {
                    error.ErrorMessage = string.Format(error.ErrorMessage, parameters);
                }
                this.ErrorMessage = error.ErrorMessage;

                if (error.NeedLog)
                {
                    Logger.CurrentLogger.DoWrite(ExceptionConfiguration.Current.Application, error.Module, "BizException", this.ErrorMessage, this.ErrorMessage);
                }
            }
            else
            {
                this.ErrorMessage = name.ToString();
                if (parameters != null && parameters.Length > 0)
                {
                    this.ErrorMessage = string.Format(this.ErrorMessage, parameters);
                }
            }
        }
        private static string ApplicationName;
        static BizException()
        {
            ApplicationName = ConfigurationHelper.GetAppSetting("ApplicationName");
            if (string.IsNullOrEmpty(ApplicationName))
            {
                ApplicationName = AppDomain.CurrentDomain.FriendlyName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ex"></param>
        public BizException(string name, Exception innerException)
            : base(name, innerException)
        {
            if (innerException is BizException)
            {
                BizException bizEx = innerException as BizException;
                this.ErrorMessage = bizEx.ErrorMessage;
            }
            else if (ExceptionConfiguration.Current == null)
            {
                this.ErrorMessage = name;
                Logger.CurrentLogger.DoWrite(
                        ApplicationName,
                        "UnExceptedException",
                        "UnExceptedException",
                        innerException.Message,
                        innerException.ToString()
                        );

                sendEmail(ApplicationName, "UnExceptedException", "UnExceptedException", this.ErrorMessage, innerException.ToString());
            }
            else if (ExceptionConfiguration.Current.ExceptionSetupList.Contains(name))
            {
                ExceptionSetup error = ExceptionConfiguration.Current.ExceptionSetupList[name].Clone() as ExceptionSetup;
                this.ErrorMessage = error.ErrorMessage;

                if (error.NeedLog)
                {
                    Logger.CurrentLogger.DoWrite(
                        ExceptionConfiguration.Current.Application,
                        error.Module,
                        "BizException",
                        this.ErrorMessage,
                        innerException.ToString());
                }
                this.Name = name.ToString();
            }
            else
            {
                this.ErrorMessage = this.Name = name;
                //this.ErrorMessage = innerException.Message;
                Logger.CurrentLogger.DoWrite(
                        ApplicationName,
                        "UnExceptedException",
                        "UnExceptedException",
                        innerException.Message,
                        innerException.ToString());
            }
        }

        private void sendEmail(string ApplicationName, string module, string logType, string title, string detail)
        {
            try
            {
                //string content = string.Format(EmailConfiguration.Current["Error"], DateTime.Now, module, logType, title, detail, Environment.MachineName);
                //EmailUtil.Send(ApplicationName + " ��������", content);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ex"></param>
        public BizException(Exception ex)
            : this("ϵͳ�����������Ժ�����", ex)
        {

        }
    }

    public static class ExceptionFactory
    {
        public static Exception BuildException(Exception ex)
        {
            Exception returnEx = null;
            if (ex is BizException)
            {
                (ex as BizException).IsBizException = true;
                returnEx = ex;
            }
            else if (ex is SqlException)
            {
                SqlException sqlException = ex as SqlException;
                if (sqlException.Class == 16)
                {
                    returnEx = new BizException(ex.Message, ex);
                }
                else
                {
                    returnEx = new BizException(ex);
                }
            }
            else
            {
                returnEx = new BizException(ex);
            }
            return returnEx;
        }
    }
}
