using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Collection;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.ExceptionHandle
{
	/// <summary>
	/// 异常信息配置
	/// </summary>
	[XmlRoot("exceptionConfiguration")]
	public class ExceptionConfiguration
	{
		/// <summary>
		/// App.config/web.config配置文件中指定异常信息配置文件的AppSetting的键值
		/// 例：<add key="ExceptionConfigFile" value="bin/Configuration/ExceptionMessage.config" />
		/// </summary>
		private const string SECTION_NAME_Error_CONFIG = "ExceptionConfigFile";


		/// <summary>
		/// 获取当前应用程序的异常配置信息
		/// </summary>
		public static ExceptionConfiguration Current
		{
			get
			{
				string setting = ConfigurationHelper.GetAppSetting(SECTION_NAME_Error_CONFIG);
				if (string.IsNullOrEmpty(setting))
				{
					return null;
				}
				return ConfigurationManager.LoadConfiguration<ExceptionConfiguration>(SECTION_NAME_Error_CONFIG, setting);
			}
		}

		public static string GetErrorMessage(string name)
		{
			ExceptionSetup exceptionSetup = ExceptionConfiguration.Current.ExceptionSetupList[name].Clone() as ExceptionSetup;

			return exceptionSetup.ErrorMessage;
		}
		/// <summary>
		/// 当前应用程序名称
		/// </summary>
		[XmlAttribute("Application")]
		public string Application { get; set; }

		/// <summary>
		/// 异常信息列表
		/// </summary>
		[XmlArray("exceptions")]
		[XmlArrayItem("exception", typeof(ExceptionSetup))]
		public KeyedList<string, ExceptionSetup> ExceptionSetupList { get; set; }

	}

	/// <summary>
	/// 异常信息定义
	/// </summary>
	public class ExceptionSetup : IKeyedItem<string>, ICloneable
	{
		/// <summary>
		/// 异常信息的键
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// 异常信息所属模块
		/// </summary>
		[XmlAttribute("module")]
		public string Module { get; set; }

		/// <summary>
		/// 表示异常信息是否需要记Log
		/// </summary>
		[XmlAttribute("needLog")]
		public bool NeedLog { get; set; }

		/// <summary>
		/// 异常信息的严重性
		/// </summary>
		public SeverityLevel Severity { get; set; }

		/// <summary>
		/// 异常信息
		/// </summary>
		[XmlText]
		public string ErrorMessage { get; set; }

		/// <summary>
		/// 辅助字段
		/// 异常信息的键
		/// </summary>
		[XmlIgnore]
		public string Key
		{
			get { return this.Name; }
		}

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}

	public enum SeverityLevel
	{
		General,
	}
}
