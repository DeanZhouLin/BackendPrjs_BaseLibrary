using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;

using Com.BaseLibrary.Entity;
using Com.BaseLibrary.Logging;
using Com.BaseLibrary.XmlAccess.Configuration;
using System.Web;
using System.IO;

namespace Com.BaseLibrary.XmlAccess
{
	internal static class XmlLoader
	{
		/// <summary>
		/// Loads the data table.
		/// Returns null if failed to load the xml file.
		/// </summary>
		/// <param name="xmlFileName">Name of the XML file.</param>
		/// <returns></returns>
		private static DataTable LoadXmlDataTable(string path, string xmlFileName)
		{
			string xmlFileFullPath = GetFileFullPath(path, xmlFileName);
			
				DataSet ds = new DataSet();
				XmlReadMode mode = ds.ReadXml(xmlFileFullPath, XmlReadMode.ReadSchema);
				return ds.Tables[0];
			
		}

		/// <summary>
		/// Loads the data table.
		/// </summary>
		/// <param name="xmlFileName">Name of the XML file.</param>
		/// <returns></returns>
		public static DataTable LoadDataTable(string xmlFileName)
		{
			DataTable dt;

			// try default folder
			dt = LoadXmlDataTable(XmlAccessConfiguration.Current.DefaultXmlDataFolder, xmlFileName);
			if (dt != null)
			{
				return dt;
			}

			// retry for alternative folders
			if (XmlAccessConfiguration.Current.AlternateXmlDataFolders == null ||
				XmlAccessConfiguration.Current.AlternateXmlDataFolders.Folders == null ||
				XmlAccessConfiguration.Current.AlternateXmlDataFolders.Folders.Count == 0)
			{
				return null;
			}

			foreach (string path in XmlAccessConfiguration.Current.AlternateXmlDataFolders.Folders)
			{
				dt = LoadXmlDataTable(path, xmlFileName);
				if (dt != null)
				{
					return dt;
				}
			}

			return null;
		}

		#region private helper function
		private static string GetFileFullPath(string virtualPath, string relativePath)
		{
			string path = string.Empty;
			if (HttpContext.Current != null)
			{
				path = HttpContext.Current.Server.MapPath(virtualPath);
			}
			else
			{
				path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, virtualPath.Replace('/', '\\').TrimStart('~').TrimStart('\\'));
			}
			return Path.Combine(path, relativePath);
		}
		#endregion
		
	}
}