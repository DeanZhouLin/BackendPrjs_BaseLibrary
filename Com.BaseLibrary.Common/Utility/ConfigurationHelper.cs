
using System;
using System.IO;

namespace Com.BaseLibrary.Utility
{
	public class ConfigurationHelper
	{
		/// <summary>
		/// 根据ApSection获取配置文件的完整路径
		/// </summary>
		/// <param name="appSection"></param>
		/// <returns></returns>
		public static string GetConfigurationFile(string appSection)
		{
			string configFile = GetAppSetting(appSection);
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile.Replace('/', '\\').TrimStart('\\'));
		}

		/// <summary>
		/// 获取AppSetting信息
		/// </summary>
		/// <param name="appSection"></param>
		/// <returns></returns>
		public static string GetAppSetting(string appSection)
		{
			return System.Configuration.ConfigurationManager.AppSettings[appSection];
		}

		/// <summary>
		/// 根据ApSection获取配置文件的完整路径
		/// </summary>
		/// <param name="appSection"></param>
		/// <returns></returns>
		public static string GetFullFilePath(string relativePath)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.Replace('/', '\\').TrimStart('\\'));
		}

		public static string GetConnectionString(string connnetionName)
		{
			return System.Configuration.ConfigurationManager.ConnectionStrings[connnetionName].ConnectionString;
		}
	}
}
