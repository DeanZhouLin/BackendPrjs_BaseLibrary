
using System;
using System.IO;

namespace Com.BaseLibrary.Utility
{
	public class ConfigurationHelper
	{
		/// <summary>
		/// ����ApSection��ȡ�����ļ�������·��
		/// </summary>
		/// <param name="appSection"></param>
		/// <returns></returns>
		public static string GetConfigurationFile(string appSection)
		{
			string configFile = GetAppSetting(appSection);
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile.Replace('/', '\\').TrimStart('\\'));
		}

		/// <summary>
		/// ��ȡAppSetting��Ϣ
		/// </summary>
		/// <param name="appSection"></param>
		/// <returns></returns>
		public static string GetAppSetting(string appSection)
		{
			return System.Configuration.ConfigurationManager.AppSettings[appSection];
		}

        //public static string GetReflashedAppSetting(string appSection)
        //{
        //    System.Configuration.ConfigurationManager.RefreshSection(appSection);
        //    return GetAppSetting(appSection);
        //}
		/// <summary>
		/// ����ApSection��ȡ�����ļ�������·��
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
