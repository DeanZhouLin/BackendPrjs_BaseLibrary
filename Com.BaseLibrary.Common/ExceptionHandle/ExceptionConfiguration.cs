using System;
using System.Xml.Serialization;

using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Collection;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.ExceptionHandle
{
	/// <summary>
	/// �쳣��Ϣ����
	/// </summary>
	[XmlRoot("exceptionConfiguration")]
	public class ExceptionConfiguration
	{
		/// <summary>
		/// App.config/web.config�����ļ���ָ���쳣��Ϣ�����ļ���AppSetting�ļ�ֵ
		/// ����<add key="ExceptionConfigFile" value="bin/Configuration/ExceptionMessage.config" />
		/// </summary>
		private const string SECTION_NAME_Error_CONFIG = "ExceptionConfigFile";


		/// <summary>
		/// ��ȡ��ǰӦ�ó�����쳣������Ϣ
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
		/// ��ǰӦ�ó�������
		/// </summary>
		[XmlAttribute("Application")]
		public string Application { get; set; }

		/// <summary>
		/// �쳣��Ϣ�б�
		/// </summary>
		[XmlArray("exceptions")]
		[XmlArrayItem("exception", typeof(ExceptionSetup))]
		public KeyedList<string, ExceptionSetup> ExceptionSetupList { get; set; }

	}

	/// <summary>
	/// �쳣��Ϣ����
	/// </summary>
	public class ExceptionSetup : IKeyedItem<string>, ICloneable
	{
		/// <summary>
		/// �쳣��Ϣ�ļ�
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// �쳣��Ϣ����ģ��
		/// </summary>
		[XmlAttribute("module")]
		public string Module { get; set; }

		/// <summary>
		/// ��ʾ�쳣��Ϣ�Ƿ���Ҫ��Log
		/// </summary>
		[XmlAttribute("needLog")]
		public bool NeedLog { get; set; }

		/// <summary>
		/// �쳣��Ϣ��������
		/// </summary>
		public SeverityLevel Severity { get; set; }

		/// <summary>
		/// �쳣��Ϣ
		/// </summary>
		[XmlText]
		public string ErrorMessage { get; set; }

		/// <summary>
		/// �����ֶ�
		/// �쳣��Ϣ�ļ�
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
