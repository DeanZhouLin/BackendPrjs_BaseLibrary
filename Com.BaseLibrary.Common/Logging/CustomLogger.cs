using System;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Configuration;

using Com.BaseLibrary.Common.Cryptography;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.Logging
{
	/// <summary>
	/// 日志管理器
	/// </summary>
	public class CustomLogger : Logger
	{
		private static string connectionString;
		public static string ConnectionString
		{
			get
			{
				if (string.IsNullOrEmpty(connectionString))
				{
					connectionString = ConfigurationHelper.GetConnectionString("SecurityConn");
					if (!connectionString.Contains(";"))
					{
						connectionString = Encryptor.Decrypt(connectionString);
					}
				}
				return connectionString;
			}
		}
		private bool dbErrorFlag = false;
		private static readonly object synObject = new object();
		public CustomLogger()
		{
			//RemoveFileToDatabase();
		}

		private static readonly string MachineName = System.Environment.MachineName;

		/// <summary>
		/// 写入日志
		/// 首先会尝试向数据库中记录日志，如果写入接数据库失败，则暂时写入XML文件中
		/// </summary>
		/// <param name="applicationName">应用程序名称</param>
		/// <param name="module">模块</param>
		/// <param name="logType">日志类型</param>
		/// <param name="title">日志标题</param>
		/// <param name="detail">日志详细信息</param>
		public override void DoWrite(string applicationName, string module, string logType, string title, string detail)
		{
			string newTitle = string.Format("【{0}】{1}", MachineName, title);
			DateTime logTime = DateTime.Now;
			try
			{

				if (ConfigurationManager.ConnectionStrings["SecurityConn"] == null)
				{
					WriteLogToXmlFile(applicationName, module, logType, newTitle, detail);
				}
				else
				{  //及日志到数据库
					WriteLogToDatabase1(applicationName, module, logType, newTitle, detail, logTime);
					//如果曾经有写入数据失败的日志，则把文件中的日志写入数据库中
					if (dbErrorFlag)
					{
						lock (synObject)
						{
							dbErrorFlag = false;
							RemoveFileToDatabase();
						}
					}
				}

			}
			catch
			{
				dbErrorFlag = true;//表示写入数据库失败
				//及日志到XML文件中
				WriteLogToXmlFile(applicationName, module, logType, newTitle, detail);
			}

		}

		/// <summary>
		/// 把日志从XML文件移到数据库中
		/// </summary>
		private void RemoveFileToDatabase()
		{
			lock (synObject)
			{
				string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempLog.XML");
				XmlDocument xml = new XmlDocument();
				if (!File.Exists(logFile))
				{
					xml.CreateComment("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
					xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "logs", ""));
				}
				else
				{
					xml.Load(logFile);
				}

				if (xml.FirstChild.ChildNodes == null || xml.FirstChild.ChildNodes.Count == 0)
				{
					return;
				}
				foreach (XmlNode item in xml.FirstChild.ChildNodes)
				{
					string applicationName = item.Attributes["application"].Value;
					string module = item.Attributes["module"].Value;
					string logType = item.Attributes["logType"].Value;
					DateTime logTime = DateTime.Parse(item.Attributes["logTime"].Value);
					string title = item.ChildNodes[0].ChildNodes[0].InnerText;
					string detail = item.ChildNodes[1].ChildNodes[0].InnerText;
					WriteLogToDatabase1(applicationName, module, logType, title, detail, logTime);
				}
				xml.FirstChild.RemoveAll();
				xml.Save(logFile);
			}
		}


		/// <summary>
		/// 把日志写入XML文件中
		/// 注：客户端程序因无法访问数据库，请直接把日志记入XML文件
		/// </summary>
		/// <param name="applicationName"></param>
		/// <param name="module"></param>
		/// <param name="logType"></param>
		/// <param name="title"></param>
		/// <param name="detail"></param>
		public override void WriteLogToXmlFile(
			string applicationName,
			string module,
			string logType,
			string title,
			string detail)
		{
			lock (synObject)
			{
				try
				{
					DoWriteLogToXML(applicationName, module, logType, title, detail);
				}
				catch (Exception ex)
				{
					EmailUtil.Send("记录错误日志到XML文件失败", ex.ToString());
				}
			}

		}

		private static void DoWriteLogToXML(string applicationName, string module, string logType, string title, string detail)
		{
			string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempLog.XML");
			XmlDocument xml = new XmlDocument();
			if (!File.Exists(logFile))
			{
				xml.CreateComment("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "logs", ""));

			}
			else
			{
				xml.Load(logFile);
			}

			XmlNode node = xml.CreateNode(XmlNodeType.Element, "log", string.Empty);
			XmlAttribute attr = xml.CreateAttribute("application");
			attr.Value = applicationName;
			node.Attributes.Append(attr);

			attr = xml.CreateAttribute("machine");
			attr.Value = Environment.MachineName;
			node.Attributes.Append(attr);

			attr = xml.CreateAttribute("module");
			attr.Value = module;
			node.Attributes.Append(attr);

			attr = xml.CreateAttribute("logType");
			attr.Value = logType;
			node.Attributes.Append(attr);

			attr = xml.CreateAttribute("logTime");
			attr.Value = DateTime.Now.ToString();
			node.Attributes.Append(attr);

			XmlElement elTitle = xml.CreateElement("title");
			elTitle.AppendChild(xml.CreateTextNode(title));
			node.AppendChild(elTitle);

			XmlElement elDetail = xml.CreateElement("detail");
			elDetail.AppendChild(xml.CreateTextNode(detail));
			node.AppendChild(elDetail);
			xml.FirstChild.AppendChild(node);
			xml.Save(logFile);
		}


		/// <summary>
		/// 把日志写入数据库中
		/// </summary>
		/// <param name="applicationName"></param>
		/// <param name="module"></param>
		/// <param name="logType"></param>
		/// <param name="title"></param>
		/// <param name="detail"></param>
		/// <param name="logTime"></param>
		private void WriteLogToDatabase1(string applicationName, string module, string logType, string title, string detail, DateTime logTime)
		{

			if (ConfigurationManager.ConnectionStrings["SecurityConn"] == null)
			{
				WriteLogToXmlFile(applicationName, module, logType, title, detail);
				return;
			}

			if (ConfigurationManager.ConnectionStrings["SecurityConn"].ProviderName == "System.Data.OracleClient")
			{
				WriteToOracle(applicationName, module, logType, title, detail, logTime);
			}
			else
			{
				logTime = WriteToSQLServer(applicationName, module, logType, title, detail, logTime);

			}
		}

		private void WriteToOracle(string applicationName, string module, string logType, string title, string detail, DateTime logTime)
		{
			//            string commandText = @"INSERT INTO MKT.LogCenter(ID,ApplicationName,Module,LogType,Title,Detail,LogTime,Machine)
			//								  VALUES(SEQ_LOGCENTER_ID.NextVal,:ApplicationName,:Module,:LogType,:Title,:Detail,:LogTime,:Machine) ";




			//            using (OracleConnection conn = new OracleConnection(ConnectionString))
			//            {
			//                OracleCommand command = new OracleCommand(commandText, conn);
			//                command.Parameters.Add(new OracleParameter(":ApplicationName", applicationName));
			//                command.Parameters.Add(new OracleParameter(":Module", module));
			//                command.Parameters.Add(new OracleParameter(":LogType", logType));
			//                command.Parameters.Add(new OracleParameter(":Title", title));
			//                command.Parameters.Add(new OracleParameter(":Detail", detail));
			//                command.Parameters.Add(new OracleParameter(":LogTime", logTime));
			//                command.Parameters.Add(new OracleParameter(":Machine", Environment.MachineName));
			//                conn.Open();
			//                command.ExecuteNonQuery();
			//                conn.Close();
			//            }
		}

		private static DateTime WriteToSQLServer(string applicationName, string module, string logType, string title, string detail, DateTime logTime)
		{
			string commandText = @"INSERT INTO RawData.dbo.LogCenter(ApplicationName,Module,LogType,Title,Detail,LogTime)
								  VALUES(@ApplicationName,@Module,@LogType,@Title,@Detail,@LogTime) ";

			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand command = new SqlCommand(commandText, conn);
				command.Parameters.Add(new SqlParameter("@ApplicationName", applicationName));
				command.Parameters.Add(new SqlParameter("@Module", module));
				command.Parameters.Add(new SqlParameter("@LogType", logType));
				command.Parameters.Add(new SqlParameter("@Title", title));
				command.Parameters.Add(new SqlParameter("@Detail", detail));
				command.Parameters.Add(new SqlParameter("@LogTime", logTime));
				conn.Open();
				command.ExecuteNonQuery();
			}
			return logTime;
		}

		private static readonly object synObject2 = new object();
		public override void WriteLogLine(string message)
		{
			StringBuilder sr = new StringBuilder();
			sr.AppendLine();
			sr.AppendFormat("{0}:{1}", DateTime.Now.ToString(), message);

			lock (synObject2)
			{
				string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("log-{0}.txt", DateTime.Now.ToString("yyMMdd")));
				File.AppendAllText(logFile, sr.ToString());
			}
		}
	}
}
