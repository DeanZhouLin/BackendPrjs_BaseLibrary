using System;
using System.Configuration;
using System.IO;

namespace Com.BaseLibrary.Utility
{
	public class PathUtil
	{
		/// <summary>
		/// 根据相对路径返回文件的完整路径
		/// </summary>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public static string GetFullFilePath(string relativePath)
		{
			string path = relativePath;
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			if (!relativePath.ToUpper().StartsWith(AppDomain.CurrentDomain.BaseDirectory.ToUpper()))
			{
				path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.Replace('/', '\\').TrimStart('\\'));
			}
			return path;
		}

		public static void MakeSurePathExist(string updatelogsFile)
		{
			string spliter = "\\";
			int index = updatelogsFile.IndexOf(spliter, 3);
			while (index > 0)
			{
				string directory = updatelogsFile.Substring(0, index);
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
				index = updatelogsFile.IndexOf(spliter, (index + 1));
			}
		}

		public static string GetImageFileName(string prefex)
		{
			if (StringUtil.IsNullOrEmpty(prefex))
			{
				prefex = string.Empty;
			}
			else
			{
				prefex += "-";
			}
			return prefex + DateTime.Now.ToString("yyyyMMddHHmmssfff");
		}
	}
}