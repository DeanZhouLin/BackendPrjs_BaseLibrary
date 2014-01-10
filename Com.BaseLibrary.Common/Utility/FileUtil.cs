using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Com.BaseLibrary.Utility
{
	public static class FileUtil
	{
		public static string GetWebLocalFileContent(string relativePath)
		{
			string file = HttpContext.Current.Server.MapPath(relativePath);
			if (File.Exists(file))
			{
				return File.ReadAllText(file);
			}
			return string.Empty;
		}
	}
}
