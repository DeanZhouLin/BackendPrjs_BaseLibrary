using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;

using Com.BaseLibrary.Utility;
using Com.BaseLibrary.Logging;
using Com.BaseLibrary.Data;
using Com.BaseLibrary.DataAccess.Configuration;

namespace Com.BaseLibrary.DataAccess
{

	using DataCommandHashtable = Dictionary<string, Command>;



	public static class DataCommandFactory
	{

		#region fields
		private const string EventCategory = "DataCommandManager";

		private const int FILE_CHANGE_NOTIFICATION_INTERVAL = 500;
		private static FileChangedEventHandler s_FileChangeHandler;
		private static Object s_CommandSyncObject;
		private static Object s_CommandFileListSyncObject;
		private static FileSystemWatcher s_Watcher;

		private static DataCommandHashtable m_DataCommands;
		private static DataCommandHashtable DataCommands
		{
			get
			{
				if (m_DataCommands == null || m_DataCommands.Count == 0)
				{
					//InitFromDB();
					InitFromConfigFile();
				}
				return m_DataCommands;
			}
		}

		//        private static void InitFromDB()
		//        {
		//            DataCommandHashtable newCommands = new Dictionary<string, Command>();
		//            string sql1 =
		//@"SELECT [CommandID]
		//      ,[ConnectionName]
		//      ,[CommandName]
		//      ,[CommandText]
		//      ,[CommandType]
		//      ,[Timeout]
		//  FROM [dbo].[DataCommandInfo]";
		//            string sql2 =
		//@"SELECT [ParameterID]
		//      ,[CommandID]
		//      ,[ParameterName]
		//      ,[DataType]
		//      ,[Size]
		//      ,[Direction]
		//FROM [BaseData].[dbo].[CommandParameterInfo]";
		//            DataTable dtCommands = DBHelper.GetTable(sql1);
		//            DataTable dtParameters = DBHelper.GetTable(sql2);
		//            foreach (DataRow row in dtCommands.Rows)
		//            {
		//                Command commandSetup = new Command();
		//                commandSetup.Database = row["ConnectionName"].ToString();
		//                commandSetup.Name = row["CommandName"].ToString();
		//                commandSetup.CommandText = GetCleanCommandText(row);
		//                commandSetup.CommandType = (CommandType)row["CommandType"];
		//                commandSetup.TimeOut = (int)row["Timeout"];
		//                DataRow[] rows = dtParameters.Select(string.Format("CommandID={0}", row["CommandID"]));
		//                if (rows.Length > 0)
		//                {
		//                    commandSetup.Parameters = new List<CommandParameter>();
		//                    List<CommandParameter> parameterList = new List<CommandParameter>();
		//                    foreach (DataRow parameterRow in rows)
		//                    {
		//                        CommandParameter paramter = new CommandParameter();
		//                        paramter.Name = parameterRow["ParameterName"].ToString();
		//                        paramter.DbType = (DbType)parameterRow["DataType"];
		//                        paramter.Size = (int)parameterRow["Size"];
		//                        paramter.Direction = (ParameterDirection)parameterRow["Direction"];
		//                        parameterList.Add(paramter);
		//                    }
		//                    commandSetup.Parameters = parameterList;
		//                }
		//                newCommands.Add(commandSetup.Name, commandSetup);
		//            }

		//            m_DataCommands = newCommands;
		//        }

		private const string pattern = @"(\t|\r|\n)";
		private const string pattern1 = @"\s{2,}";

		private static string GetCleanCommandText(DataRow row)
		{
			String commandText = row["CommandText"].ToString();
			commandText = Regex.Replace(commandText, pattern, " ");
			commandText = Regex.Replace(commandText, pattern1, " ");
			return commandText;
		}
		private static string s_DataAccessFolder;
		private static Dictionary<string, List<string>> s_FileCommands;

		#endregion

		static DataCommandFactory()
		{

		}
		private static void InitFromConfigFile()
		{
			try
			{
				s_FileChangeHandler = new FileChangedEventHandler(FILE_CHANGE_NOTIFICATION_INTERVAL);
				s_FileChangeHandler.ActualHandler += new FileSystemEventHandler(Watcher_Changed);
				s_DataAccessFolder = Path.Combine(Path.GetDirectoryName(DataAccessConfiguration.DataAccessFile), DataAccessConfiguration.Current.CommandFolder);

				s_CommandSyncObject = new object();
				s_CommandFileListSyncObject = new object();
				s_Watcher = new FileSystemWatcher(s_DataAccessFolder);
				s_Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
				s_Watcher.Changed += new FileSystemEventHandler(s_FileChangeHandler.ChangeEventHandler);
				s_Watcher.EnableRaisingEvents = true;
				UpdateAllCommandFiles();
			}
			catch (Exception ex)
			{
				Logger.CurrentLogger.DoWrite("CommonLibrary", "DataAccess", "InitDataCommandFailed", "Êý¾Ý·ÃÎÊ´íÎó", ex.ToString());
			}
		}

		private static void Watcher_Changed(object sender, FileSystemEventArgs e)
		{

			lock (s_CommandFileListSyncObject)
			{
				foreach (string file in s_FileCommands.Keys)
				{
					if (string.Compare(file, e.FullPath, true) == 0)
					{
						UpdateCommandFile(file);
						break;
					}
				}
			}
			//ClearAllCommands();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m_FileNamePattern"></param>
		/// <exception cref="DataCommandFileLoadException">m_FileNamePattern does not exist or contains invalid information</exception>
		private static void UpdateCommandFile(string fileName)
		{
			lock (s_CommandSyncObject)
			{
				DataCommandHashtable newCommands = m_DataCommands == null ? new DataCommandHashtable() : new DataCommandHashtable(m_DataCommands);
				List<string> commandNames = null;
				if (s_FileCommands.ContainsKey(fileName))
				{
					commandNames = s_FileCommands[fileName];
					foreach (string commandName in commandNames)
					{
						newCommands.Remove(commandName);
					}
				}
				commandNames = new List<string>();
				CommandConfiguration commands = ObjectXmlSerializer.LoadFromXml<CommandConfiguration>(fileName);
				foreach (Command cmd in commands.CommandList)
				{
					cmd.BuildParameterDictionary();
					//SaveCommandToDb(cmd);
					newCommands.Add(cmd.Name, cmd);
					commandNames.Add(cmd.Name);
				}
				m_DataCommands = newCommands;
				s_FileCommands[fileName] = commandNames;
			}
		}

		private static void SaveCommandToDb(Command cmd)
		{
			//string commandText = @"UP_CreateDateCommand";
			//Dictionary<string, object> paramters = new Dictionary<string, object>();
			//paramters.Add("@ConnectionName", "DollMall");
			//paramters.Add("@CommandName", cmd.Name);
			//paramters.Add("@CommandText", cmd.CommandText);
			//paramters.Add("@CommandType", (int)cmd.CommandType);
			//int id = DBHelper.ExecuteScalar<int>(commandText, paramters, CommandType.StoredProcedure);
			//if (cmd.Parameters != null)
			//{
			//    foreach (var item in cmd.Parameters)
			//    {
			//        commandText = @"UP_CreateCommandParameter";
			//        paramters = new Dictionary<string, object>();
			//        paramters.Add("@CommandID", id);
			//        paramters.Add("@ParameterName", item.Name);
			//        paramters.Add("@DataType", (int)item.DbType);
			//        paramters.Add("@Size", item.Size);
			//        paramters.Add("@Direction", (int)item.Direction);
			//        DBHelper.ExecuteScalar<int>(commandText, paramters, CommandType.StoredProcedure);
			//    }
			//}

		}


		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="DataCommandFileNotSpecifiedException"> if the datacommand file list 
		/// configuration file does not contain any valid file name.
		/// </exception>
		internal static void UpdateAllCommandFiles()
		{
			lock (s_CommandFileListSyncObject)
			{
				List<DataCommandFile> fileList = DataAccessConfiguration.Current.CommandFileList;
				if (fileList == null || fileList.Count == 0)
				{
					throw new DataCommandFileNotSpecifiedException();
				}
				s_FileCommands = new Dictionary<string, List<string>>();
				m_DataCommands = new DataCommandHashtable();


				// update each datacommand file
				foreach (DataCommandFile commandFile in fileList)
				{
					string fileName = Path.Combine(s_DataAccessFolder, commandFile.FileName);
					UpdateCommandFile(fileName);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal static void ClearAllCommands()
		{
			m_DataCommands = null;
		}
		/// <summary>
		/// Get DataCommand corresponding to the given command name.
		/// </summary>
		/// <param name="name">Name of the DataCommand </param>
		/// <returns>DataCommand</returns>  
		/// <exception cref="KeyNotFoundException">the specified DataCommand does not exist.</exception>
		public static DataCommand CreateDataCommand(string name)
		{
			return new DataCommand(DataCommands[name]);
		}
		public static List<T> GetEntityList<T>(string commandName)
			where T : class, new()
		{
			DataCommand command = CreateDataCommand(commandName);
			return command.ExecuteEntityList<T>();
		}
	}
}
