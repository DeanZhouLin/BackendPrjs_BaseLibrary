using System;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

using System.Xml.Serialization;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Common.Cryptography;
using Com.BaseLibrary.Logging;
using Com.BaseLibrary.Contract;


namespace Com.BaseLibrary.Utility
{

    /// <summary> 
    /// 说明：在.net2.0以上版本中发送电子邮件的方法示例 
    /// 用到的类主要位于System.Net.Mail和System.Net命名空间下 
    /// </summary> 
    public class EmailUtil
    {

        public static void Send(string subject, string body)
        {

            Send(EmailConfiguration.Current.From,
                EmailConfiguration.Current.DisplayName,
                EmailConfiguration.Current.To,
                EmailConfiguration.Current.CcList,
                EmailConfiguration.Current.BccList,
                subject,
                null,
                body,
                EmailConfiguration.Current.SmptServer.UserName,
                EmailConfiguration.Current.SmptServer.ClearPassword,
                EmailConfiguration.Current.SmptServer.Server,
                EmailConfiguration.Current.SmptServer.Port,
                EmailConfiguration.Current.SmptServer.EnableSSL,
                EmailConfiguration.Current.Timeout,
                EmailConfiguration.Current.AsyncSend
                );
        }

        /// <summary>
        /// 通过调用WCF实现邮件发送
        /// </summary>
        /// <param name="receivers"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void Send(string receivers, string subject, string body)
        {
            Send(EmailConfiguration.Current.From,
                EmailConfiguration.Current.DisplayName,
                receivers,
                EmailConfiguration.Current.CcList,
                EmailConfiguration.Current.BccList,
                subject,
                null,
                body,
               EmailConfiguration.Current.SmptServer.UserName,
               EmailConfiguration.Current.SmptServer.ClearPassword,
               EmailConfiguration.Current.SmptServer.Server,
               EmailConfiguration.Current.SmptServer.Port,
               EmailConfiguration.Current.SmptServer.EnableSSL,
               EmailConfiguration.Current.Timeout,
               EmailConfiguration.Current.AsyncSend
               );
        }

        /// <summary>
        /// 发送邮件方法，主题中包含发送的机器名称
        /// 主要 用于异常邮件发送
        /// </summary>
        /// <param name="receivers"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendExceptionMail(string receivers, string subject, string body)
        {
            subject = System.Environment.MachineName + subject;
            Send2(receivers, subject, body);
        }


        /// <summary>
        /// 直接调用DoSendEmail实现邮件发送
        /// </summary>
        /// <param name="receivers"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void Send2(string receivers, string subject, string body)
        {
            if (string.IsNullOrEmpty(subject))
            {
                subject = " 未命名邮件";
            }
            try
            {
                DoSendEmail(EmailConfiguration.Current.From,
               EmailConfiguration.Current.DisplayName,
               receivers,
               EmailConfiguration.Current.CcList,
               EmailConfiguration.Current.BccList,
               subject,
               null,
               body,
              EmailConfiguration.Current.SmptServer.UserName,
              EmailConfiguration.Current.SmptServer.ClearPassword,
              EmailConfiguration.Current.SmptServer.Server,
              EmailConfiguration.Current.SmptServer.Port,
              EmailConfiguration.Current.SmptServer.EnableSSL,
              EmailConfiguration.Current.Timeout,
              EmailConfiguration.Current.AsyncSend
              );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void Send(
                string sender,
                string displayName,
                string receivers,
                string ccList,
                string bccList,
                 string subject,
                 List<string> attachments,
                string body,
                string userName,
                 string password,
                 string smtpHost,
                int? port,
                bool enableSSL,
                int? timeOut,
                bool asyncSend
                )
        {
            try
            {
                string title = subject;
                if (StringUtil.IsNullOrEmpty(title))
                {
                    title = "未命名邮件";
                }
                //title = string.Format("【{1}】{0}", title, Environment.MachineName);
                CommonService.Instance.SendEmail2(
                    sender,
                    displayName,
                    receivers,
                    ccList,
                    bccList,
                    title,
                    attachments,
                    body,
                    userName,
                    password,
                    smtpHost,
                    port,
                    enableSSL,
                    timeOut,
                    asyncSend);

            }
            catch (Exception ex)
            {
                Logger.CurrentLogger.DoWrite(
                       "邮件系统",
                       "发送邮件错误",
                       "错误",
                       subject,
                       ex.ToString() + "\r\n__Mail body:__\r\n" + body);
            }
        }


        public static void SendWithLog(string application, string module, string subject, string body)
        {
            try
            {
                Send(subject, body);
                Logger.CurrentLogger.DoWrite(
                       application,
                       module,
                       "错误",
                       subject,
                       body);
            }
            catch
            {

            }
        }

        internal static void DoSendEmail(
        string sender,
        string receivers,
        string ccList,
        string bccList,
        string subject,
        List<string> attachments,
        string body,
        string userName,
        string password,
        string smtpHost,
        int? port,
        bool enableSSL,
        int? timeOut,
        bool asyncSend

        )
        {
            DoSendEmail(
                sender,
                null,
                receivers,
                ccList,
                bccList,
                subject,
                attachments,
                body,
                userName,
                password,
                smtpHost, port,
                enableSSL,
                timeOut,
                asyncSend);
        }
        internal static void DoSendEmail(
            string sender,
            string displayName,
            string receivers,
            string ccList,
            string bccList,
            string subject,
            List<string> attachments,
            string body,
            string userName,
            string password,
            string smtpHost,
            int? port,
            bool enableSSL,
            int? timeOut,
            bool asyncSend

            )
        {
            MailMessage message = new MailMessage();
            if (string.IsNullOrEmpty(displayName))
            {
                message.From = new MailAddress(sender);
            }
            else
            {
                message.From = new MailAddress(sender, displayName);
            }
            message.To.Add(receivers);

            if (!string.IsNullOrEmpty(ccList))
            {
                message.CC.Add(ccList);
            }
            if (!string.IsNullOrEmpty(bccList))
            {
                message.Bcc.Add(bccList);
            }
            if (attachments != null && attachments.Count > 0)
            {
                attachments.ForEach(c => message.Attachments.Add(new Attachment(c)));

            }
            message.Subject = subject;
            message.IsBodyHtml = true;//设置邮件正文为html格式 
            message.Body = body;//设置邮件内容 



            SmtpClient client = new SmtpClient();
            client.Host = smtpHost;
            if (port != null && port.Value > 0)
            {
                client.Port = port.Value;
            }

            client.Credentials = new NetworkCredential(userName, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.EnableSsl = enableSSL;
            if (timeOut != null)
            {
                client.Timeout = timeOut.Value;
            }

            try
            {
                if (asyncSend)
                {

                    client.SendAsync(message, null);
                }
                else
                {
                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                Logger.CurrentLogger.DoWrite(
                       "邮件系统",
                       "发送邮件错误",
                       "错误",
                       subject,
                       ex.ToString() + "\r\n__Mail body:__\r\n" + body);
            }

        }


    }

    [XmlRoot("emailConfiguration")]
    public class EmailConfiguration
    {
        [XmlAttribute("serviceUrl")]
        public string ServiceUrl { get; set; }
        [XmlElement("from")]
        public string From { get; set; }
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        [XmlElement("to")]
        public string To { get; set; }
        [XmlElement("ccList")]
        public string CcList { get; set; }
        [XmlElement("bccList")]
        public string BccList { get; set; }
        [XmlElement("smptServer")]
        public SmptServerInfo SmptServer { get; set; }

        [XmlArray("templates")]
        [XmlArrayItem("template", typeof(EmailTemplateInfo))]
        public List<EmailTemplateInfo> EmailTemplateList { get; set; }

        [XmlElement("timeout")]
        public int Timeout { get; set; }

        [XmlElement("asyncSend")]
        public bool AsyncSend { get; set; }

        public class EmailTemplateInfo
        {
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlText]
            public string Content { get; set; }
        }

        public class SmptServerInfo
        {
            [XmlAttribute("Server")]
            public string Server { get; set; }

            [XmlAttribute("UserName")]
            public string UserName { get; set; }

            [XmlAttribute("Password")]
            public string Password { get; set; }

            [XmlAttribute("Port")]
            public int Port { get; set; }

            [XmlAttribute("EnableSSL")]
            public bool EnableSSL { get; set; }
            [XmlAttribute("PasswordEncrypted")]
            public bool PasswordEncrypted { get; set; }
            private string clearPassword;
            public string ClearPassword
            {
                get
                {
                    if (string.IsNullOrEmpty(clearPassword))
                    {
                        if (PasswordEncrypted)
                        {
                            clearPassword = Encryptor.Decrypt(Password);
                        }
                        else
                        {
                            clearPassword = Password;
                        }
                    }
                    return clearPassword;
                }
            }


        }

        public string this[string name]
        {
            get
            {
                EmailTemplateInfo emailTemplate = EmailTemplateList.Find(c => c.Name.ToUpper() == name.ToUpper());
                return emailTemplate == null ? string.Empty : emailTemplate.Content;
            }
        }


        public static EmailConfiguration Current
        {
            get
            {
                return ConfigurationManager.LoadConfiguration<EmailConfiguration>("EmailConfigFile");
            }
        }

    }
}