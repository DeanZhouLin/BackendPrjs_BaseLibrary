using System;
using System.Collections.Generic;
using System.Text;
using Com.BaseLibrary.Logging;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;
using System.Data.SqlClient;

namespace Com.BaseLibrary.ExceptionHandle
{
    /// <summary>
    /// 自定义异常的基类
    /// </summary>
    public class BizException : Exception
    {
        /// <summary>
        /// 是否为业务异常
        /// </summary>
        public bool IsBizException { get; set; }

        /// <summary>
        /// 异常信息的键
        /// 通过它从配置文件中读取该异常的配置信息
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public override string Message
        {
            get
            {
                return this.ErrorMessage;
            }
        }

        /// <summary>
        /// 实例化一个对象，通过异常名称name从配置文件中读出异常信息，
        /// 并用传入的参数进行格式化而得到最终的ErrorMessage
        /// </summary>
        /// <param name="name">异常名称</param>
        /// <param name="parameters">对消息进行格式化的参数</param>
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
                //EmailUtil.Send(ApplicationName + " 发生错误。", content);
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
            : this("系统发生错误，请稍后再试", ex)
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
